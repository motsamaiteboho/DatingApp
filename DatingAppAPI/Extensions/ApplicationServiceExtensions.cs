using DatingAppAPI.Data;
using DatingAppAPI.Interfaces;
using DatingAppAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace DatingAppAPI.Extensions
{
  public static class ApplicationServiceExtensions
  {
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {

      services.AddDbContext<DataContext>( opt =>
      {
        opt.UseSqlite( config.GetConnectionString( "DefaultConnection" ) );
      } );

      services.AddCors();
      services.AddScoped<ITokenService, TokenService>();
      services.AddScoped<IUserRepository, UserRepository>();
      services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies() );

      return services;
    }
  }
}
