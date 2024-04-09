using AutoMapper;
using DatingAppAPI.Data;
using DatingAppAPI.DTOs;
using DatingAppAPI.Entities;
using DatingAppAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Security.Cryptography;
using System.Text;

namespace DatingAppAPI.Controllers
{
  namespace DatingAppAPI.Controllers
  {
    public class AccountController : BaseApiController
    {
      private readonly DataContext _context;
      private readonly ITokenService _tokenService;
      private readonly IMapper _mapper;

      public AccountController( DataContext context, ITokenService tokenService, IMapper mapper )
      {
        _context = context;
        _tokenService = tokenService;
        _mapper = mapper;
      }

      [HttpPost( "register" )]
      public async Task<ActionResult<UserDto>> Register( RegisterDto registerDto )
      {

        if( await UserExists(registerDto.Username))
        {
          return BadRequest("Username is taken");
        }

        var user = _mapper.Map<AppUser>( registerDto ); 

        using var hmac = new HMACSHA512();

        user.UserName = registerDto.Username.ToLower();
        user.PasswordHash = hmac.ComputeHash( Encoding.UTF8.GetBytes( registerDto.Password ) );
        user.PasswordSalt = hmac.Key;

        _context.Users.Add( user );
        await _context.SaveChangesAsync();

        return Ok( new UserDto
        {
          Username = user.UserName,
          Token = _tokenService.CreateToken( user ),
          KnownAs = user.UserName
        } ); ;
      }

      [HttpPost("login")]
      public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
      {
        var user = await _context.Users.FirstOrDefaultAsync(o => o.UserName == loginDto.Username);
        if( user == null ) 
        {
          return Unauthorized("Invalid username");
        }

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash( Encoding.UTF8.GetBytes( loginDto.Password ) );

        for( int i = 0; i < computedHash.Length; i++ )
        {
          if(computedHash[i] != user.PasswordHash[i] ) 
          { 
            return Unauthorized("Invalid password");
          }
        }

        return Ok( new UserDto
        {
          Username = user.UserName,
          Token = _tokenService.CreateToken( user ),
          PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
          KnownAs = user.knownAs
        } ); ;
      }
      private async Task<bool> UserExists(string username)
      {
        return await _context.Users.AnyAsync(o => o.UserName == username.ToLower());
      }
    }
  }

}
