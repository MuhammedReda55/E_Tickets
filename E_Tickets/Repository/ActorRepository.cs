using E_Tickets.Data;
using E_Tickets.Models;
using E_Tickets.Repository.IRepository;

namespace E_Tickets.Repository
{
    public class ActorRepository : Repository<Actor>, IActorRepository
    {
        public ActorRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
