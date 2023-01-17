using Microsoft.Extensions.Logging;

namespace WebApiAutores.Middlewares
{


    public static class LoguearRespuestaHttpsMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoguearRespuestaHttps(this IApplicationBuilder app)
        { 
            return app.UseMiddleware<LoguearRespuestaHttpsMiddleware>();
        }

    }

    public class LoguearRespuestaHttpsMiddleware
    {
        private readonly RequestDelegate _siguiente;
        private readonly ILogger<LoguearRespuestaHttpsMiddleware> _logger;

        public LoguearRespuestaHttpsMiddleware(RequestDelegate siguiente, ILogger<LoguearRespuestaHttpsMiddleware> logger)
        {
            _siguiente = siguiente;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext contexto)
        {
            using (var ms = new MemoryStream())
            {
                var cuerpoOriginalRespuesta = contexto.Response.Body;
                contexto.Response.Body = ms;

                await _siguiente(contexto);

                ms.Seek(0, SeekOrigin.Begin);
                string respuesta = new StreamReader(ms).ReadToEnd();
                ms.Seek(0, SeekOrigin.Begin);

                await ms.CopyToAsync(cuerpoOriginalRespuesta);
                contexto.Response.Body = cuerpoOriginalRespuesta;

                _logger.LogInformation(respuesta);
            }
        }
    }
}
