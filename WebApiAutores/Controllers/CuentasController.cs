using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using WebApiAutores.DTOs;
using WebApiAutores.Servicios;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/cuentas")]
    public class CuentasController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly HashService _hashService;
        private readonly IDataProtector _dataProtector;

        public CuentasController(UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            SignInManager<IdentityUser> signInManager,
            IDataProtectionProvider dataProtectionProvider,
            HashService hashService) 
        {

            _userManager = userManager;
            _configuration = configuration;
            _signInManager = signInManager;
            _hashService = hashService;
            _dataProtector = dataProtectionProvider.CreateProtector("valor_unico_y_quizas_secreto");
        }
        [HttpPost("Registar",Name = "registrarUsuario")] //api/cuentas/registrar
        public async Task<ActionResult<RespuestaAutenticacionDTO>> Registrar(CredencialesUsuario creedencialesUsuario)
        {
            var usuario = new IdentityUser { UserName = creedencialesUsuario.Email,
                Email = creedencialesUsuario.Email };
            var resultado = await _userManager.CreateAsync(usuario, creedencialesUsuario.Password);
            if (resultado.Succeeded)
            {
                return await ConstruirToken(creedencialesUsuario);
            }
            else
            {
                return BadRequest(resultado.Errors);
            }
        }
        [HttpPost("login",Name = "loginUsuario")]
        public async Task<ActionResult<RespuestaAutenticacionDTO>> Login(CredencialesUsuario credencialesUsuario)
        {
            var resultado = await _signInManager.PasswordSignInAsync(credencialesUsuario.Email,
                credencialesUsuario.Password, isPersistent: false, lockoutOnFailure: false);
            if (resultado.Succeeded)
            {
                return await ConstruirToken(credencialesUsuario);
            }
            else
            {
                return BadRequest("Login incorrecto");
            }
        }
        [HttpGet("RenovarToken",Name = "renovarToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async  Task <ActionResult<RespuestaAutenticacionDTO>> Renovar()
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;
            var credencialesUsuario = new CredencialesUsuario()
            {
                Email= email
            };
            return await ConstruirToken(credencialesUsuario);
            
        }
        private async Task <RespuestaAutenticacionDTO> ConstruirToken(CredencialesUsuario creedencialesUsuario)
        {
            var claims = new List<Claim>()
            {
               new Claim ("email", creedencialesUsuario.Email),
               new Claim ("lo que yo quiera","Cualquier otro valor")
            };
            var usuario = await _userManager.FindByEmailAsync(creedencialesUsuario.Email);
            var claimsDB= await _userManager.GetClaimsAsync(usuario);

            claims.AddRange(claimsDB);

            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["llavejwt"]));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);
            var expiracion = DateTime.UtcNow.AddYears(1);
            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                expires: expiracion, signingCredentials:creds);
            return new RespuestaAutenticacionDTO()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = expiracion,
            };
        }
        [HttpPost("HacerAdmin", Name = "hacerAdmin")]
        public async Task<ActionResult> HacerAdmin(EditarAdminDTO editarAdminDTO)
        { 
            var usuario = await _userManager.FindByEmailAsync(editarAdminDTO.Email);
            await _userManager.AddClaimAsync(usuario, new Claim("esAdmin", "1"));
            return NoContent();
        }
        [HttpPost("RemoverAdmin", Name = "removerAdmin")]
        public async Task<ActionResult> RemoverAdmin(EditarAdminDTO editarAdminDTO)
        {
            var usuario = await _userManager.FindByEmailAsync(editarAdminDTO.Email);
            await _userManager.RemoveClaimAsync(usuario, new Claim("esAdmin", "1"));
            return NoContent();
        }
    }
}
