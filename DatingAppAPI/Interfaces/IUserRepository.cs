using DatingAppAPI.DTOs;
using DatingAppAPI.Entities;

namespace DatingAppAPI.Interfaces
{
  public interface IUserRepository
  {
    void Update( AppUser user );
    Task<bool> SaveAllAsync();
    Task<IEnumerable<AppUser>> GetUsersAsync();
    Task<AppUser> GetByIdAsync( int id );
    Task<AppUser> GetByUsernameAsync( string username );
    Task<IEnumerable<MemberDto>> GetMembersAsync();
    Task<MemberDto> GetMemberAsync( string username );
  }
}
