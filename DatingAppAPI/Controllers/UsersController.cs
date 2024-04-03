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

namespace DatingAppAPI.Controllers
{
  [Authorize]
  public class UsersController : BaseApiController
  {
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UsersController( IUserRepository userRepository, IMapper mapper )
    {
      _userRepository = userRepository;
      _mapper = mapper;
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
      var username =  User.FindFirst(ClaimTypes.NameIdentifier).Value;
      var user = await _userRepository.GetByUsernameAsync(username);

      if(user == null) { return NotFound(); }

      _mapper.Map( memberUpdateDto, user );

      //_userRepository.Update( user );

      if (await _userRepository.SaveAllAsync()) return NoContent();

      return BadRequest( "Failed to update user" );
    }
  }
}
