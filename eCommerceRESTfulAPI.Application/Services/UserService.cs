using AutoMapper;
using eCommerceRESTfulAPI.Application.DTOs;
using eCommerceRESTfulAPI.Application.Interfaces.Services;
using eCommerceRESTfulAPI.Application.Validators;
using eCommerceRESTfulAPI.Domain.Entities;
using eCommerceRESTfulAPI.Domain.Interfaces.Repositories;
using eCommerceRESTfulAPI.Application.Interfaces.Hashing;

namespace eCommerceRESTfulAPI.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICustomerRepository _customerRepository; 
        private readonly IMapper _mapper;
        private readonly EmailValidator _emailValidator;
        private readonly EmailUniquenessValidator _emailUniquenessValidator;
        private readonly IPasswordHasherService _passwordHasher;

        public UserService(
            IUserRepository userRepository,
            ICustomerRepository customerRepository, 
            IMapper mapper,
            EmailValidator emailValidator,
            EmailUniquenessValidator emailUniquenessValidator,
            IPasswordHasherService passwordHasher)
        {
            _userRepository = userRepository;
            _customerRepository = customerRepository;
            _mapper = mapper;
            _emailValidator = emailValidator;
            _emailUniquenessValidator = emailUniquenessValidator;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserDto> CreateUserAsync(UserCreateDto userCreateDto)
        {
            if (!_emailValidator.IsValid(userCreateDto.Email))
            {
                throw new ArgumentException("Invalid email format.");
            }

            await _emailUniquenessValidator.ValidateAsync(userCreateDto.Email);

            var user = _mapper.Map<User>(userCreateDto);
            user.PasswordHash = _passwordHasher.HashPassword(user, userCreateDto.Password);

            await _userRepository.CreateAsync(user);

            var customer = new Customer
            {
                UserId = user.Id, 
                User = user 
            };

            await _customerRepository.CreateAsync(customer); 

            return _mapper.Map<UserDto>(user);
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }
            return _mapper.Map<UserDto>(user);
        }

        public async Task UpdateUserAsync(int id, UserUpdateDto userUpdateDto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            if (!string.IsNullOrWhiteSpace(userUpdateDto.Email) && !_emailValidator.IsValid(userUpdateDto.Email))
            {
                throw new ArgumentException("Invalid email format.");
            }

            if (!string.IsNullOrWhiteSpace(userUpdateDto.Email))
            {
                await _emailUniquenessValidator.ValidateAsync(userUpdateDto.Email);
            }

            _mapper.Map(userUpdateDto, user);
            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            await _userRepository.DeleteAsync(id);
        }
    }
}
