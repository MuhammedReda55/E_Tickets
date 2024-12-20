using E_Tickets.Models;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace E_Tickets.Repository.IRepository
{
    public interface IMovieRepository : IRepository<Movie>
    {
        
    }
}
