using Microsoft.AspNetCore.Mvc;
using BookingService.Services.Auth;
using BookingService.Models;
using Microsoft.AspNetCore.Authorization;
using BookingService.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BookingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;
        private readonly BookingDbContext _context;
        private readonly IEmailService _emailservice;

        //Constructor
        public AuthController(IUserService userService, IJwtService jwtService, BookingDbContext context, IEmailService emailService)
        {
            _userService = userService;
            _jwtService = jwtService;
            _context = context;
            _emailservice = emailService;
        }


        //Post: /api/auth/register
        //Registreer nieuwe gebruiker + retoureer jwttoken
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { errors });
            }

            var user = await _userService.RegisterAsync(request.Username, request.Email, request.Password);

            if (user == null)
            {
                return BadRequest("Gebruiker bestaat al.");
            }

            //Genereer token na registratie
            var token = _jwtService.GenerateToken(user);
            return Ok(new { token });
        }

        //Post: /api/auth/login
        //inlogen gebruikers + jwt retourneren
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.LoginAsync(request.Username, request.Password);

            if (user == null)
            {
                return Unauthorized("Ongeldige inloggegevens.");
            }

            var token = _jwtService.GenerateToken(user);
            return Ok(new { token });
        }

        //ingelogde gebruikers kunnen eigen account zien
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var username = User.Identity?.Name;

            if (username == null)
                return Unauthorized();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return NotFound("Gebruiker niet gevonden.");

            return Ok(new { username = user.Username, role = user.Role });
        }

        //Gebruikers promoveren naar admin
        [Authorize(Roles = "admin")]
        [HttpPost("promote")]
        public async Task<IActionResult> PromoteToAdmin([FromBody] PromoteUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { errors });
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == request.Username);

            if (user == null)
            {
                return NotFound("Gebruiker niet gevonden.");
            }

            user.Role = "admin";
            await _context.SaveChangesAsync();
            //Loging
            var log = new AuditLog
            {
                Action = "Promotie naar admin",
                PerformedBy = User.Identity?.Name ?? "onbekend",
                TargetUser = user.Username,
                Timestamp = DateTime.UtcNow
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();



            return Ok($"{user.Username} is ingesteld op ADMIN");

        }

        [Authorize(Roles = "admin")] // alleen admin mag testmail versturen
        [HttpPost("testmail")]
        public async Task<IActionResult> SendTestMail()
        {
            try
            {
                await _emailservice.SendEmailAsync(
                    "staycloudmail@gmail.com",
                    "Testmail van StayCloud",
                    "Dit is een testmail om te controleren of e-mailfunctionaliteit werkt.");

                return Ok("Testmail verzonden.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Fout bij verzenden testmail: {ex.Message}");
            }
        }


        // logging controleren door admin
        [Authorize(Roles = "admin")]
        [HttpGet("auditlogs")]
        public async Task<IActionResult> GetAuditLogs()
        {
            var logs = await _context.AuditLogs
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();

            return Ok(logs);
        }


    }
}