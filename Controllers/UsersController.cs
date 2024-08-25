using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UserApi.Data;
using UserApi.Models;

[Route("users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UserRepository _userRepository;

    public UsersController(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> Signup([FromBody] UserDto userDto)
    {
        var user = new User
        {
            Username = userDto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            Device = userDto.Device,
            IpAddress = userDto.IpAddress
        };

        await _userRepository.CreateUser(user);
        return Ok();
    }
}
