using eCommerceRESTfulAPI.Application.DTOs;

namespace eCommerceRESTfulAPI.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByIdAsync(int id);
        Task<UserDto> CreateUserAsync(UserCreateDto userCreateDto);
        Task UpdateUserAsync(int id, UserUpdateDto userUpdateDto);
        Task DeleteUserAsync(int id);
    }
}
