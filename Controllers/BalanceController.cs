using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
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
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.ReadToken(tokenDto.Token) as JwtSecurityToken;
        var userIdClaim = token?.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
        if (long.TryParse(userIdClaim, out var userId))
        {
            var balance = await _balanceRepository.GetBalance(userId);
            return Ok(new { balance });
        }

        return Unauthorized();
    }
}
