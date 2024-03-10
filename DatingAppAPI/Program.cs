
using DatingAppAPI.Data;
using DatingAppAPI.Extensions;
using DatingAppAPI.Interfaces;
using DatingAppAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DatingAppAPI
{
  public class Program
  {
    public static void Main( string[] args )
    {
      var builder = WebApplication.CreateBuilder( args );

      // Add services to the container.

      builder.Services.AddControllers();

      builder.Services.AddApplicationServices( builder.Configuration );

      builder.Services.AddIdentityServices( builder.Configuration );

      var app = builder.Build();

      // Configure the HTTP request pipeline.
      app.UseCors( builder => builder.AllowAnyHeader().AllowAnyMethod().WithOrigins( "https://localhost:4200" ) );

      app.UseAuthentication();
      app.UseAuthorization();

      app.MapControllers();

      app.Run();
    }
  }
}
