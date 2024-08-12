using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using eCommerceRESTfulAPI.Application.DTOs;
using eCommerceRESTfulAPI.Application.Interfaces.Services;
using eCommerceRESTfulAPI.Domain.Interfaces.Repositories;

namespace eCommerceRESTfulAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IStringLocalizer<AuthController> _localizer;

        public AuthController(IAuthService authService, IStringLocalizer<AuthController> localizer)
        {
            _authService = authService;
            _localizer = localizer;
        }

        private void SetCulture(string language)
        {
            if (!string.IsNullOrEmpty(language))
            {
                System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo(language);
                System.Globalization.CultureInfo.CurrentUICulture = new System.Globalization.CultureInfo(language);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto, [FromHeader(Name = "Accept-Language")] string language = null)
        {
            SetCulture(language);

            if (loginDto == null || string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
            {
                return BadRequest(new { Message = _localizer["EmailAndPasswordRequired"] });
            }

            var token = await _authService.AuthenticateAsync(loginDto.Email, loginDto.Password);
            if (token == null)
            {
                return Unauthorized(new { Message = _localizer["InvalidEmailOrPassword"] });
            }

            return Ok(new { Token = token });
        }
    }
}
