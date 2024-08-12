using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerceRESTfulAPI.Domain.Interfaces.Repositories
{
    public interface IAuthService
    {
        Task<string> AuthenticateAsync(string email, string password);
    }

}
