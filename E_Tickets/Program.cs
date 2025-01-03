using E_Tickets.Data;
using E_Tickets.Repository.IRepository;
using E_Tickets.Repository;
using Microsoft.EntityFrameworkCore;
using E_Tickets.Models;
using Microsoft.AspNetCore.Identity;
using Stripe;
using E_Tickets.Utility;
using E_Tickets.DbInitionlizer;
//using E_Tickets.DbInitionlizer;




namespace E_Tickets
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

           builder.Services.AddAutoMapper(typeof(Program));
           // Add services to the container.
           builder.Services.AddControllersWithViews();

            builder.Services.AddControllersWithViews();

            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];


            builder.Services.AddDbContext<ApplicationDbContext>(
                option => option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
                );

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Lockout.AllowedForNewUsers = true; 
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); 
                options.Lockout.MaxFailedAccessAttempts = 5; 
            })
           .AddEntityFrameworkStores<ApplicationDbContext>()
           .AddDefaultTokenProviders();

            builder.Services.AddScoped<IMovieRepository, MovieRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ICinemaRepository, CinemaRepository>();
            builder.Services.AddScoped<IActorRepository, ActorRepository>();
            builder.Services.AddScoped<IActorMovieRepository, ActorMovieRepository>();
            builder.Services.AddScoped<IRequestCinemaRepository, RequestCinemaRepository>();
            builder.Services.AddScoped<ICartRepository, CartRepository>();
            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.AddScoped<IDbInitionlizer, DbInitioniser>();
         
            

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

          
            app.UseAuthorization();

            var scope = app.Services.CreateScope();
            var service = scope.ServiceProvider.GetService<IDbInitionlizer>();
            service.Initionlize();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
