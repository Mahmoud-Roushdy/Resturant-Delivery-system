using FinalProject.Data;
using FinalProject.DTO.Authentication;
using FinalProject.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FinalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<RegistrationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        //private readonly SignOutResult _signOutResult;  
        private readonly IConfiguration _configuration;
        SignInManager<RegistrationUser> _signInManager;

        public AuthenticationController(ApplicationDbContext context, UserManager<RegistrationUser> userManager, SignInManager<RegistrationUser> signInManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            //_signOutResult =signOutResult;
            _signInManager = signInManager;
            _configuration = configuration;

        }
        //TODO:: logout
        [HttpGet]
        public async Task<IActionResult> SeedRolesAsync()
        {
            bool isCustomerRoleExists = await _roleManager.RoleExistsAsync("User");
            bool isAdminRoleExists = await _roleManager.RoleExistsAsync("ADMIN");
            bool isSupperAdminRoleExists = await _roleManager.RoleExistsAsync("SUPERADMIN");

            if (isCustomerRoleExists && isAdminRoleExists && isSupperAdminRoleExists)
                return BadRequest("Roles Seeding is Already Done");
            //{
            //    IsSuccess = true,
            //    DisplayMessage = "Roles Seeding is Already Done"
            //};

            //await _roleManager.CreateAsync(new IdentityRole("USER"));
            await _roleManager.CreateAsync(new IdentityRole("ADMIN"));
            await _roleManager.CreateAsync(new IdentityRole("User"));
            await _roleManager.CreateAsync(new IdentityRole("SUPERADMIN"));

            return Ok("Role Seeding Done Successfully");
            //{
            //    IsSuccess = true,
            //    DisplayMessage = "Role Seeding Done Successfully"
            //};
        }


        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterAsync(RegisterDto model)
        {
            //TODO:: create admin Role
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return BadRequest("Email is already registered!");
            // return new AuthResponseModel { Message = "Email is already registered!" };

            if (await _userManager.FindByNameAsync(model.UserName) is not null)
                return BadRequest("Username is already registered!");

            var user = new RegistrationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.Phone,
                
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = new StringBuilder();
                foreach (var error in result.Errors)
                    errors.Append($"{error.Description},");
                return BadRequest(errors.ToString());
            }

            await _userManager.AddToRoleAsync(user, "User");

            var jwtSecurityToken = await CreateJwtToken(user);

            return Ok(new
            {
                email = user.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { "User" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Username = user.UserName
            });
        }


        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginAsync(LoginDto model)
        {


            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {

                return Ok("Email or Password is incorrect!");
            }

            var jwtSecurityToken = await CreateJwtToken(user);
            var rolesList = await _userManager.GetRolesAsync(user);

            #region Testing
            ////// Store the token in the AspNetUserTokens table(may:: get all loged users)
            //if (user != null)
            //{
            //    await _userManager.SetAuthenticationTokenAsync(user, "TwoFactorTokenProvider", "token", new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken));
            //}
            var IsSigned = _signInManager.IsSignedIn(User);
            

            var isAuth = User.Identity?.IsAuthenticated;
            var NameUser = User.Identity?.Name;

             await _signInManager.SignInAsync(user, isPersistent: true);

            #endregion
            return Ok(new
            {
                email = user.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles = rolesList.ToList(),// new List<string> { "User" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Username = user.UserName,
                IsSignedUser = IsSigned,
                isAuthUser = isAuth,
                NameUser2 = NameUser
            });

        }


        private async Task<JwtSecurityToken> CreateJwtToken(RegistrationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            var roleClaims = userRoles.Select(r => new Claim("roles", r)).ToList();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddDays(int.Parse(_configuration["JWT:DurationInDays"]!)),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
        [HttpGet("DeleteUser/{id}")]


        public async Task<IActionResult> DeleteUserAsync(string id)
        {
            var result = await _context.Users.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (result != null) {
                _context.Users.Remove(result);
                return Ok("deleted");

            }
            return BadRequest("not existed");
        }
        [HttpGet("GetUserById/{id}")]

        public async Task<IActionResult> GetUserByIdAsync(string id)
        {
            var user = await _context.Users.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (user != null)
            { return Ok(user); }
            return BadRequest("this user does not existed!");
        }
        [HttpGet]
        [Route("GetAllUsers")]
        public async Task<IEnumerable<RegistrationUser>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        [Authorize] // Make sure the user is authenticated
        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            //_signInManager.IsSignedIn(User);

            //var isAuth = User.Identity.IsAuthenticated;
            //var s = User.Identity.Name;
            var isAuth = User.Identity?.IsAuthenticated;
            if(isAuth == true)
            {
                await _signInManager.SignOutAsync(); //do not work

            }

            //  await _userManager. 
            return Ok("Logout successful");
        }
        [Authorize]
        [HttpGet("test")]
        public async Task<IActionResult> testing(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {

                return Ok("Email or Password is incorrect!");
            }
            var IsSigned = _signInManager.IsSignedIn(User);
            //_signInManager.SignInAsync(User,)
            var isAuth = User.Identity?.IsAuthenticated;
            var NameUser = User.Identity?.Name;
            
            return Ok(new
            {
                IsSignedUser = IsSigned,
                isAuthUser = isAuth,
                NameUser2 = NameUser
            });


        }

    }
}
