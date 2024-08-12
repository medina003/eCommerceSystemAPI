using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using eCommerceRESTfulAPI.Application.DTOs;
using eCommerceRESTfulAPI.Application.Interfaces.Services;

namespace eCommerceRESTfulAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IStringLocalizer<UserController> _localizer;

        public UserController(IUserService userService, IStringLocalizer<UserController> localizer)
        {
            _userService = userService;
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

        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] UserCreateDto userCreateDto, [FromHeader(Name = "Accept-Language")] string language = null)
        {
            SetCulture(language);
            var userDto = await _userService.CreateUserAsync(userCreateDto);
            return CreatedAtAction(nameof(GetUserById), new { id = userDto.Id }, userDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers([FromHeader(Name = "Accept-Language")] string language = null)
        {
            SetCulture(language);
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("by-id")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> GetUserById([FromHeader] int id, [FromHeader(Name = "Accept-Language")] string language = null)
        {
            SetCulture(language);
            var userDto = await _userService.GetUserByIdAsync(id);
            if (userDto == null)
            {
                return NotFound(_localizer["UserNotFound"]);
            }
            return Ok(userDto);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromHeader] int id, [FromBody] UserUpdateDto userUpdateDto, [FromHeader(Name = "Accept-Language")] string language = null)
        {
            SetCulture(language);
            await _userService.UpdateUserAsync(id, userUpdateDto);
            return NoContent();
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser([FromHeader] int id, [FromHeader(Name = "Accept-Language")] string language = null)
        {
            SetCulture(language);
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }
    }
}
