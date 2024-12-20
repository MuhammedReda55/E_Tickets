using E_Tickets.Data;
using E_Tickets.Models;
using E_Tickets.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace E_Tickets.Repository
{
    public class MovieRepository : Repository<Movie>, IMovieRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public MovieRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
        
        

    }
}
