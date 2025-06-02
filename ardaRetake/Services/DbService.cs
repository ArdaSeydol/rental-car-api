using System.Data.Common;
using Microsoft.Data.SqlClient;
using ardaRetake.Models.DTOs;

namespace ardaRetake.Services;

public class DbService : IDbService
{
    private readonly string _connectionString;

    public DbService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Default") ?? string.Empty;
    }

    public async Task<ClientRequestDto> GetClientByIdAsync(int id)
{
    var query = @"
        SELECT 
            c.ID,
            c.FirstName,
            c.LastName,
            c.Address,
            car.VIN,
            col.Name AS Color,
            m.Name AS Model,
            cr.DateFrom,
            cr.DateTo,
            cr.TotalPrice,
            cr.Discount
        FROM clients c
        LEFT JOIN car_rentals cr ON c.ID = cr.ClientID
        LEFT JOIN cars car ON cr.CarID = car.ID
        LEFT JOIN models m ON car.ModelID = m.ID
        LEFT JOIN colors col ON car.ColorID = col.ID
        WHERE c.ID = @id";

    await using SqlConnection conn = new(_connectionString);
    await using SqlCommand cmd = new(query, conn);
    await conn.OpenAsync();
    cmd.Parameters.AddWithValue("@id", id);

    var reader = await cmd.ExecuteReaderAsync();

    ClientRequestDto? dto = null;

    while (await reader.ReadAsync())
    {
        if (dto is null)
        {
            dto = new ClientRequestDto
            {
                Id = reader.GetInt32(0),
                FirstName = reader.GetString(1),
                LastName = reader.GetString(2),
                Adress = reader.GetString(3),
                rentals = new List<Rental>()
            };
        }

        if (!reader.IsDBNull(4))
        {
            var price = reader.GetInt32(9);
            var discount = reader.IsDBNull(10) ? 0 : reader.GetInt32(10);

            dto.rentals.Add(new Rental
            {
                Vin = reader.GetString(4),
                Color = reader.IsDBNull(5) ? "Unknown" : reader.GetString(5),
                Model = reader.IsDBNull(6) ? "Unknown" : reader.GetString(6),
                DateFrom = reader.GetDateTime(7),
                DateTo = reader.GetDateTime(8),
                TotalPrice = price - discount
            });
        }
    }

    if (dto is null)
    {
        throw new Exception("Client with the given ID was not found.");
    }

    return dto;
}

     public async Task CreateNewClientAsync(CreateClientDto dto)
    {
        await using SqlConnection conn = new(_connectionString);
        await conn.OpenAsync();

        DbTransaction transaction = await conn.BeginTransactionAsync();
        await using SqlCommand cmd = new();
        cmd.Connection = conn;
        cmd.Transaction = (SqlTransaction)transaction;

        try
        {
            cmd.Parameters.Clear();
            cmd.CommandText = "SELECT COUNT(1) FROM cars WHERE ID = @carId";
            cmd.Parameters.AddWithValue("@carId", dto.CarId);
            var carExists = (int)await cmd.ExecuteScalarAsync() > 0;

            if (!carExists)
                throw new Exception("Car with the given ID does not exist.");

            cmd.Parameters.Clear();
            cmd.CommandText = @"
                INSERT INTO clients (FirstName, LastName, Address)
                OUTPUT INSERTED.ID
                VALUES (@firstName, @lastName, @address)";
            cmd.Parameters.AddWithValue("@firstName", dto.Client.FirstName);
            cmd.Parameters.AddWithValue("@lastName", dto.Client.LastName);
            cmd.Parameters.AddWithValue("@address", dto.Client.Address);

            var newClientId = (int)await cmd.ExecuteScalarAsync();

            var duration = (dto.DateTo - dto.DateFrom).Days;
            if (duration <= 0)
                throw new Exception("DateTo must be after DateFrom.");

            cmd.Parameters.Clear();
            cmd.CommandText = "SELECT PricePerDay FROM cars WHERE ID = @carId";
            cmd.Parameters.AddWithValue("@carId", dto.CarId);
            var pricePerDay = (int)(await cmd.ExecuteScalarAsync()
                ?? throw new Exception("Car price not found."));

            int totalPrice = duration * pricePerDay;

            cmd.Parameters.Clear();
            cmd.CommandText = @"
                INSERT INTO car_rentals (ClientID, CarID, DateFrom, DateTo, TotalPrice, Discount)
                VALUES (@clientId, @carId, @dateFrom, @dateTo, @totalPrice, NULL)";
            cmd.Parameters.AddWithValue("@clientId", newClientId);
            cmd.Parameters.AddWithValue("@carId", dto.CarId);
            cmd.Parameters.AddWithValue("@dateFrom", dto.DateFrom);
            cmd.Parameters.AddWithValue("@dateTo", dto.DateTo);
            cmd.Parameters.AddWithValue("@totalPrice", totalPrice);

            await cmd.ExecuteNonQueryAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
