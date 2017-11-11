using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PicBook.ApplicationService
{
    public interface IUserService
    {
        Task EnsureUser(IReadOnlyCollection<Claim> claims);
    }
}