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
        public async Task<ActionResult<RespuestaAutenticacion>> Registrar(CreedencialesUsuario creedencialesUsuario)
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
        public async Task<ActionResult<RespuestaAutenticacion>> Login(CreedencialesUsuario credencialesUsuario)
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
        private RespuestaAutenticacion ConstruirToken(CreedencialesUsuario creedencialesUsuario)
        {
            var claims = new List<Claim>()
            {
               new Claim ("email", creedencialesUsuario.Email),
               new Claim ("lo que yo quiera","Cualquier otro valor")
            };
            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["llavejwt"]));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);
            var expiracion = DateTime.UtcNow.AddMonths(3);
            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                expires: expiracion, signingCredentials:creds);
            return new RespuestaAutenticacion()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = expiracion,
            };
        }
    }
}
