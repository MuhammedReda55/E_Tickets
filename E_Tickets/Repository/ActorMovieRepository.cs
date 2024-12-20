using E_Tickets.Data;
using E_Tickets.Models;
using E_Tickets.Repository.IRepository;

namespace E_Tickets.Repository
{
    public class ActorMovieRepository : Repository<ActorMovie>, IActorMovieRepository
    {
        public ActorMovieRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
