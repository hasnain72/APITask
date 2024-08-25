using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UserApi.Data;  // Ensure this using directive is included
using UserApi.Models;
using UserApi.Utilities;

namespace UserApi.Controllers
{
    [Route("users")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        private readonly BalanceRepository _balanceRepository;

        public AuthController(UserRepository userRepository, BalanceRepository balanceRepository)
        {
            _userRepository = userRepository;
            _balanceRepository = balanceRepository;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] LoginDto loginDto)
        {
            var user = await _userRepository.GetUserByUsername(loginDto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized();
            }

            var token = JwtUtils.GenerateJwtToken(user.Username, user.Id);

            // Log login activity
            await _userRepository.LogLoginActivity(user.Id, loginDto.IpAddress, loginDto.Device, loginDto.Browser);

            // Add balance if it's the first login
            if (await _balanceRepository.GetBalance(user.Id) == 0)
            {
                await _balanceRepository.AddBalance(user.Id, 5.0m);
            }

            return Ok(new { user.FirstName, user.LastName, token });
        }
    }
}
