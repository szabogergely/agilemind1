using System.Threading.Tasks;
using PicBook.Domain;

namespace PicBook.Repository.EntityFramework
{
    public interface IUserRepository
    {
        Task Create(User entity);
        Task<User> FindByIdentifier(string userIdentifier);
    }
}