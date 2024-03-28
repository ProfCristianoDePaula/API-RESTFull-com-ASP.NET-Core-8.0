using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MinhaPrimeiraAPI.Data;

namespace MinhaPrimeiraAPI.Controllers
{
    public class UsersController : Controller
    {
        private readonly MyContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        // IUserStore e IUserEmailStore são interfaces que definem os métodos que
        // devem ser implementados para armazenar e recuperar usuários e e-mails.
        private IUserStore<IdentityUser> _userStore;
        private IUserEmailStore<IdentityUser> _emailStore;

        public UsersController(MyContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: api/roles
        [HttpGet("api/roles")]
        public async Task<ActionResult<IEnumerable<IdentityRole>>> GetRoles()
        {
            try
            {
                var roles = await _context.IdentityRole.ToListAsync();
                if (roles.Count == 0)
                {
                    return NoContent(); // 204
                }
                return Ok(roles); // 200
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // 400
            }
        }

        // GET: api/role/id
        [HttpGet("api/role/getById/{id}")]
        public async Task<ActionResult<IdentityRole>> GetRole(string id)
        {
            try
            {
                var role = await _context.IdentityRole.FindAsync(id);
                if (role == null)
                {
                    return NotFound(); // 404
                }
                return Ok(role); // 200
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // 400
            }
        }


        // GET: api/role/name
        [HttpGet("api/role/getByName/{name}")]
        public async Task<ActionResult<IEnumerable<IdentityRole>>> GetRoleByName(string name)
        {
            try
            {
                var roles = _context.IdentityRole.Where(r => r.NormalizedName.Contains(name.Trim().ToUpper())).ToList();
                if (roles.IsNullOrEmpty())
                {
                    return NotFound(); // 404
                }
                return Ok(roles); // 200
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // 400
            }
        }

        // POST: api/role
        [HttpPost("api/role")]
        public async Task<ActionResult<IdentityRole>> CreateRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName.Trim()))
            {
                return BadRequest("Role name is required"); // 400
            }
            try
            {
                var roleExists = await _context.IdentityRole.AnyAsync(r => r.Name == roleName.Trim());
                if (roleExists)
                {
                    return BadRequest("Role Exists");
                }

                var role = new IdentityRole
                {
                    Name = roleName.Trim(),
                    NormalizedName = roleName.Trim().ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                };

                var result = await _context.IdentityRole.AddAsync(role);
                if (result == null)
                {
                    return BadRequest("Role not created");
                }

                await _context.SaveChangesAsync();
                return Created("/api/role", role); // 201
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // 400
            }
        }

        // PUT: api/role/id
        [HttpPut("api/role/{id}")]
        public async Task<ActionResult<IdentityRole>> UpdateRole(string id, string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return BadRequest("Role Name is required"); // 400
            }

            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Role Id is required"); // 400
            }

            try
            {
                var role = await _context.IdentityRole.FindAsync(id);
                if (role == null)
                {
                    return NotFound(); // 404
                }
                role.Name = roleName.Trim();
                role.NormalizedName = roleName.Trim().ToUpper();
                role.ConcurrencyStamp = Guid.NewGuid().ToString();

                _context.Entry(role).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(role); // 200
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // 400
            }
        }

        // DELETE: api/role/id
        [HttpDelete("api/role/{id}")]
        public async Task<ActionResult<IdentityRole>> DeleteRole(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Role Id is required"); // 400
            }
            try
            {
                var role = await _context.IdentityRole.FindAsync(id);
                if (role == null)
                {
                    return NotFound(); // 404
                }
                _context.IdentityRole.Remove(role);
                await _context.SaveChangesAsync();
                return NoContent(); // 204
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // 400
            }
        }

        // GET: api/users
        [HttpGet("api/users")]
        public async Task<ActionResult<IEnumerable<IdentityUser>>> GetUsers()
        {
            try
            {
                var users = await _context.IdentityUser.ToListAsync();
                if (users.Count == 0)
                {
                    return NoContent(); // 204
                }
                return Ok(users); // 200
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // 400
            }
        }

        // PUT: api/register
        [HttpPost("api/register")]
        public async Task<IActionResult> RegisterUserAsync(string userName, string email, string password, string role)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
                {
                    return BadRequest("All fields are required");
                }

                if (await _userManager.FindByEmailAsync(email) != null)
                {
                    return BadRequest("Email already exists");
                }

                if (await _userManager.FindByNameAsync(userName) != null)
                {
                    return BadRequest("Username already exists");
                }

                if (string.IsNullOrEmpty(password) || password.Length < 6)
                {
                    return BadRequest("Password must be at least 6 characters");
                }
                if (string.IsNullOrEmpty(role))
                {
                    return BadRequest("Role is required");
                }
                var roleExists = await _context.IdentityRole.AnyAsync(r => r.NormalizedName == role.Trim().ToUpper());
                if (!roleExists)
                {
                    return BadRequest("Role does not exist");
                }

                // Validar dados do usuário (implemente sua lógica de validação aqui)
                var user = new IdentityUser();
                user.UserName = userName.Trim();
                user.Email = email.Trim();
                await _userManager.AddToRoleAsync(user, role);
                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    // Adicionar o usuário ao papel
                    return Ok(user);
                }
                if (result.Errors != null)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return BadRequest(ModelState.ToList());
                }
            }
            return NotFound();
        }
    }
}








