using System.Linq;
using System.Threading.Tasks;
using PicBook.Domain;

namespace PicBook.Repository.EntityFramework
{
    public class UserRepository : GenericCrudRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext)
        {
            Context = dbContext;
        }

        public async Task<User> FindByIdentifier(string userIdentifier)
        {
            var users = await FindAll(u => u.UserIdentifier == userIdentifier);


            return users.FirstOrDefault();
        }
    }
}