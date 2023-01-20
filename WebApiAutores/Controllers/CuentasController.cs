using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiAutores.DTOs;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/cuentas")]
    public class CuentasController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<IdentityUser> _signInManager;

        public CuentasController(UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            SignInManager<IdentityUser> signInManager) 
        {
            _userManager = userManager;
            _configuration = configuration;
            _signInManager = signInManager;
        }
        [HttpPost("Registar")] //api/cuentas/registrar
        public async Task<ActionResult<RespuestaAutenticacionDTO>> Registrar(CredencialesUsuario creedencialesUsuario)
        {
            var usuario = new IdentityUser { UserName = creedencialesUsuario.Email,
                Email = creedencialesUsuario.Email };
            var resultado = await _userManager.CreateAsync(usuario, creedencialesUsuario.Password);
            if (resultado.Succeeded)
            {
                return ConstruirToken(creedencialesUsuario);
            }
            else
            {
                return BadRequest(resultado.Errors);
            }
        }
        [HttpPost("login")]
        public async Task<ActionResult<RespuestaAutenticacionDTO>> Login(CredencialesUsuario credencialesUsuario)
        {
            var resultado = await _signInManager.PasswordSignInAsync(credencialesUsuario.Email,
                credencialesUsuario.Password, isPersistent: false, lockoutOnFailure: false);
            if (resultado.Succeeded)
            {
                return ConstruirToken(credencialesUsuario);
            }
            else
            {
                return BadRequest("Login incorrecto");
            }
        }
        [HttpGet("RenovarToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult<RespuestaAutenticacionDTO> Renovar()
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;
            var credencialesUsuario = new CredencialesUsuario()
            {
                Email= email
            };
            return ConstruirToken(credencialesUsuario);
            
        }
        private RespuestaAutenticacionDTO ConstruirToken(CredencialesUsuario creedencialesUsuario)
        {
            var claims = new List<Claim>()
            {
               new Claim ("email", creedencialesUsuario.Email),
               new Claim ("lo que yo quiera","Cualquier otro valor")
            };
            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["llavejwt"]));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);
            var expiracion = DateTime.UtcNow.AddMinutes(30);
            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                expires: expiracion, signingCredentials:creds);
            return new RespuestaAutenticacionDTO()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = expiracion,
            };
        }
    }
}
