using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph.Models;
using MinhaPrimeiraAPI.Models;
using MinhaPrimeiraAPI.Services;

namespace MinhaPrimeiraAPI.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly TokenService _tokenService;

        public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, TokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        /*
        // POST: /api/Login
        [HttpPost("/api/login")]
        public async Task<ActionResult> Login(Login model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);

            // Verficar se o Login foi bem sucedido
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest(result.ToString());
        }
        */

        // POST: /api/Login
        [HttpPost("/api/login")]
        public async Task<ActionResult> Login(Login model)
        {
            // Verificar se o modelo é válido
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // Verificar se o Username Existe
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                return BadRequest("Username not Found");
            }
            // Verificar se o Username está bloqueado
            if (await _userManager.IsLockedOutAsync(user))
            {
                return BadRequest(StatusCodes.Status423Locked);
            }
            // Verificar se a senha está correta
            if (await _userManager.CheckPasswordAsync(user, model.Password) == false)
            {
                return BadRequest("Invalid Password");
            }

            // Fazer o Login
            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);
            // Se o Login foi bem sucedido
            if (result.Succeeded)
            {
                var token = await _tokenService.GenerateToken(user);
                return Ok(new { token });
            }
            // Se o usuário está bloqueado
            if (result.IsLockedOut)
            {
                return BadRequest(StatusCodes.Status423Locked);
            }
            // Se o usuário não está autorizado
            if (result.IsNotAllowed)
            {
                return Forbid();
            }
            // Se o usuário não está confirmado
            return NotFound();
        }

        // POST: /api/Logout
        [HttpPost("/api/logout")]
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }
    }
}
