using Controllers;
using HotelAPI.Models;
using HotelFull.Server.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Org.BouncyCastle.Asn1.Ocsp;

namespace HotelAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly ILogger<HotelInfoController> _logger;
        private readonly HotelContext _context;
        private readonly IConfiguration _configuration;
        private readonly MemberService _memberService;

        public MemberController(HotelContext context, IConfiguration configuration, ILogger<HotelInfoController> logger)
        {
            _context = context;
            _configuration = configuration;
            _memberService = new MemberService(configuration);
            _logger = logger;
        }

        [HttpGet("secure")]
        [ServiceFilter(typeof(MemberJwtAuthFilter))]
        public IActionResult SecureEndpoint()
        {
            return Ok("This is a secure endpoint.");
        }

        [HttpGet]
        [ServiceFilter(typeof(MemberJwtAuthFilter))]
        public async Task<IActionResult> Get()
        {
            var members = await _context.Member
                .Select(m => new
                {
                    m.MemberID,
                    m.Name,
                    m.Title,
                    m.Email,
                    m.Phone
                })
                .ToListAsync();

            if (!members.Any())
            {
                return NoContent();
            }

            return Ok(members);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var member = await _context.Member
                .Where(m => m.MemberID == id)
                .Select(m => new
                {
                    m.MemberID,
                    m.Name,
                    m.Title,
                    m.Email,
                    m.Phone
                })
                .FirstOrDefaultAsync();

            if (member == null)
            {
                return NotFound(new { Message = $"Member with ID {id} not found." });
            }

            return Ok(member);
        }

        [HttpPost("register")]
        //[Consumes("multipart/form-data")]
        public async Task<IActionResult> Post([FromBody] MemberModel model)
        {
            if (model == null)
            {
                return BadRequest("Member model cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(model.Name) ||
                string.IsNullOrWhiteSpace(model.Email) ||
                string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest("Name, Email, and Password are required.");
            }

            var existingMember = await _context.Member
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.Email == model.Email);
            if (existingMember != null)
            {
                return Conflict("A member with this email already exists.");
            }

            var newMember = new Member
            {
                Name = model.Name,
                Phone = model.Phone,
                Email = model.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Title = model.Title
            };

            try
            {
                _context.Member.Add(newMember);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(Get), new { id = newMember.MemberID }, newMember);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the member.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Put(int id, [FromForm] MemberModel model)
        {
            var member = await _context.Member.FindAsync(id);

            if (member == null)
            {
                return NotFound(new { Message = $"Member with ID {id} not found." });
            }

            member.Name = model.Name ?? member.Name;
            member.Phone = model.Phone ?? member.Phone;
            member.Email = model.Email ?? member.Email;
            member.Title = model.Title ?? member.Title;

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                member.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
            }

            try
            {
                await _context.SaveChangesAsync();
                return Ok("Member updated successfully.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the member.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var member = await _context.Member.FindAsync(id);

            if (member == null)
            {
                return NotFound(new { Message = $"Member with ID {id} not found." });
            }

            _context.Member.Remove(member);

            try
            {
                await _context.SaveChangesAsync();
                return Ok("Member deleted successfully.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the member.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> MemberLogin([FromBody] Login model)
        {
            var member = await _context.Member.SingleOrDefaultAsync(m => m.Email == model.Email);

            if (member == null || !BCrypt.Net.BCrypt.Verify(model.Password, member.Password))
            {
                return Unauthorized("Invalid email or password.");
            }

            var accessToken = _memberService.GenerateTokens(member);
            var refreshToken = _memberService.GenerateRefreshToken(member);

            member.RefreshToken = refreshToken;
            member.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody]GoogleLoginRequest request)
        {
            var payload = await _memberService.VerifyGoogleToken(request.IdToken); // 注意是 IdToken
            if (payload == null)
            {
                return Unauthorized("Invalid Google token.");
            }

            // 檢查是否有 Email
            if (string.IsNullOrEmpty(payload.Email))
            {
                return BadRequest("Email is required.");
            }

            var member = _context.Member.FirstOrDefault(m => m.Email == payload.Email);
            var (accessToken, refreshToken) = (string.Empty, string.Empty);

            if (member == null)
            {
                // 新增成員
                member = new Member
                {
                    Name = payload.Name ?? string.Empty,
                    Phone = "012345678", // 預設電話號碼
                    Email = payload.Email ?? string.Empty,
                    Title = "Male",      // 預設 Title
                    Password = string.Empty // 預設密碼
                };

                accessToken = _memberService.GenerateTokens(member) ?? string.Empty;
                refreshToken = _memberService.GenerateRefreshToken(member) ?? string.Empty;
                member.RefreshToken = refreshToken;
                member.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

                _context.Member.Add(member);
                await _context.SaveChangesAsync();
            }
            else
            {
                accessToken = _memberService.GenerateTokens(member) ?? string.Empty;
                refreshToken = _memberService.GenerateRefreshToken(member) ?? string.Empty;
                member.RefreshToken = refreshToken;
                member.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

                await _context.SaveChangesAsync();
            }

            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.RefreshToken))
            {
                return BadRequest("Invalid refresh token.");
            }

            var principal = _memberService.GetPrincipalFromExpiredToken(request.RefreshToken);
            if (principal == null)
            {
                return BadRequest("Invalid refresh token.");
            }

            var email = principal.FindFirst(ClaimTypes.Email)?.Value;
            var member = await _context.Member.SingleOrDefaultAsync(m => m.Email == email);

            DateTime refreshTokenTime = (DateTime)member.RefreshTokenExpiryTime;
            if (member == null || refreshTokenTime <= DateTime.UtcNow)
            {
                return BadRequest("Invalid refresh token");
            }

            var accessToken = _memberService.GenerateTokens(member);
            return Ok(new
            {
                AccessToken = accessToken
            });
        }

        public class MemberModel
        {
            public string? Name { get; set; }
            public string? Phone { get; set; }
            public string? Email { get; set; }
            public string? Password { get; set; }
            public string? Title { get; set; }
        }

        public class Login
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class GoogleLoginRequest
        {
            public string? IdToken { get; set; }
        }

        public class RefreshTokenRequest
        {
            public string RefreshToken { get; set; }
        }
    }
}
