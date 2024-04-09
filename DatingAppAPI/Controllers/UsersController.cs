using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DatingAppAPI.Data;
using DatingAppAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using DatingAppAPI.Interfaces;
using AutoMapper;
using DatingAppAPI.DTOs;
using System.Security.Claims;
using DatingAppAPI.Extensions;
using System.Security.Cryptography.Xml;

namespace DatingAppAPI.Controllers
{
  [Authorize]
  public class UsersController : BaseApiController
  {
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IPhotoService _photoService;

    public UsersController( IUserRepository userRepository, IMapper mapper, IPhotoService photoService )
    {
      _userRepository = userRepository;
      _mapper = mapper;
      _photoService = photoService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
       var users = await _userRepository.GetMembersAsync();

      return Ok(users);
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
      return await _userRepository.GetMemberAsync(username);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser( MemberUpdateDto memberUpdateDto )
    {
      var user = await _userRepository.GetByUsernameAsync( User.GetUsername());

      if(user == null) { return NotFound(); }

      _mapper.Map( memberUpdateDto, user );

      //_userRepository.Update( user );

      if (await _userRepository.SaveAllAsync()) return NoContent();

      return BadRequest( "Failed to update user" );
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
      var user = await _userRepository.GetByUsernameAsync( User.GetUsername() );

      if (user == null) { return NotFound(); };

      var result = await _photoService.AddPhotoAsync( file );

      if (result.Error != null)
      {
        return BadRequest($"Error: {result.Error}");
      }

      var photo = new Photo
      {
        Url = result.SecureUri.AbsoluteUri,
        PublicId = result.PublicId,
      };

      if(user.Photos.Count == 0)  photo.IsMain = true;

      user.Photos.Add(photo);

      if( await _userRepository.SaveAllAsync())
      {
        return CreatedAtAction( nameof( GetUser ), new { username = user.UserName },
          _mapper.Map<PhotoDto>( photo ) );
      }

      return BadRequest( ("Problem adding photo") );
    }

    [HttpDelete( "delete-photo/{photoId}" )]
    public async Task<ActionResult> DeletePhoto( int photoId )
    {
      var user = await _userRepository.GetByUsernameAsync( User.GetUsername() );
      if (user == null) { return NotFound(); }

      var photo = user.Photos.FirstOrDefault( x => x.Id == photoId );

      if (photo == null) { return NotFound(); }

      if (photo.IsMain) return BadRequest("You cannot delete the main photo");

      if(photo.PublicId !=  null)
      {
        var result = await _photoService.DeletePhotoAsync(photo.PublicId);
        if( result.Error != null ) return BadRequest( result.Error.Message );
      }

      user.Photos.Remove( photo );

      if (await _userRepository.SaveAllAsync()) return Ok();

      return BadRequest( "unable to delete the photo" );
    }
  }
}
