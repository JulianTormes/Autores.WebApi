using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApiAutores.DTOs;
using WebApiAutores.Servicios;

namespace WebApiAutores.Utilidades
{
    public class HATEOASAutorFilterAttribute : HATEOASFiltroAttribute
    {
        private readonly GeneradorEnlaces _generadorEnlaces;

        public HATEOASAutorFilterAttribute(GeneradorEnlaces generadorEnlaces)
        {
            _generadorEnlaces = generadorEnlaces;
        }
        public override async Task OnResultExecutionAsync(ResultExecutingContext context,
            ResultExecutionDelegate next)
        {
            var debeIncluir = debeIncluirHATEOAS(context);
            if (!debeIncluir)
            { 
                await next();
                return;
            }

            var resultado= context.Result as ObjectResult;
            var modelo = resultado.Value as AutorDTO ?? throw new
                ArgumentException("Se esperaba una instancia de AutorDTO");
            await _generadorEnlaces.GenerarEnlaces(modelo);
            await next();
        
        }
    }
}
