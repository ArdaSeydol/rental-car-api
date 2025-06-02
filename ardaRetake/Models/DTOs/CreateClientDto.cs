namespace ardaRetake.Models.DTOs;

public class CreateClientDto
{
    public ClientDto Client { get; set; } = null!;
    public int CarId { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
}

public class ClientDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Address { get; set; } = null!;
}