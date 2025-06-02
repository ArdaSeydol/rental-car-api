using ardaRetake.Models.DTOs;

namespace ardaRetake.Services;

public interface IDbService
{
    public Task<ClientRequestDto> GetClientByIdAsync(int id);
    public Task CreateNewClientAsync(CreateClientDto dto);
}