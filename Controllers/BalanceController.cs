using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

[Route("users/auth")]
[ApiController]
public class BalanceController : ControllerBase
{
    private readonly BalanceRepository _balanceRepository;

    public BalanceController(BalanceRepository balanceRepository)
    {
        _balanceRepository = balanceRepository;
    }

    [HttpPost("balance")]
    public async Task<IActionResult> GetBalance([FromBody] TokenDto tokenDto)
    {
        if (string.IsNullOrWhiteSpace(tokenDto.Token))
        {
            return BadRequest("Token is required.");
        }

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadToken(tokenDto.Token) as JwtSecurityToken;

            if (token == null)
            {
                return Unauthorized("Invalid token.");
            }

            // Use the claim type that matches your token
            var userIdClaim = token.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid user ID.");
            }

            var balance = await _balanceRepository.GetBalance(userId);

            if (balance == null)
            {
                return NotFound("Balance not found.");
            }

            return Ok(new { balance });
        }
        catch
        {
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

}
