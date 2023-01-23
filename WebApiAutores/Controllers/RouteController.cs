﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiAutores.DTOs;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
    public class RouteController:ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;

        public RouteController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }
        [HttpGet(Name = "ObtenerRoot")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DatoHATEOAS>>> Get()
        { 
            var datosHateoas = new List<DatoHATEOAS>();
            var esAdmin = await _authorizationService.AuthorizeAsync(User, "esAdmin");
            datosHateoas.Add(new DatoHATEOAS(enlace:Url.Link("ObtenerRoot", new { }),
                descripcion: "self",metodo:"GET")) ;

            datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("obtenerAutores", new { }), descripcion: "autores", metodo: "GET"));
            if (esAdmin.Succeeded)
            
            {
                datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("crearAutor", new { }), descripcion: "autor-crear", metodo: "POST"));
                datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("crearAutor", new { }), descripcion: "libro-crear", metodo: "POST"));
            }
            return datosHateoas;
        }

    }
}
