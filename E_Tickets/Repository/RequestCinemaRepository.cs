﻿using E_Tickets.Data;
using E_Tickets.Models;
using E_Tickets.Repository.IRepository;

namespace E_Tickets.Repository
{
    public class RequestCinemaRepository : Repository<CinemaRequest>, IRequestCinemaRepository
    {
        public RequestCinemaRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }



}