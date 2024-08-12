using System;
using System.Threading.Tasks;
using eCommerceRESTfulAPI.Domain.Interfaces.Repositories;

namespace eCommerceRESTfulAPI.Application.Validators
{
    public class EmailUniquenessValidator
    {
        private readonly IUserRepository _userRepository;

        public EmailUniquenessValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ValidateAsync(string email)
        {
            if (await _userRepository.EmailExistsAsync(email))
            {
                throw new ArgumentException("Email уже существует.");
            }
        }
    }
}
