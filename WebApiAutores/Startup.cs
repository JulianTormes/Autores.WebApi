using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebApiAutores.Controllers;
using WebApiAutores.Servicios;

namespace WebApiAutores
{
    //3
    public class Startup
    {
        public Startup(IConfiguration configuration) 
            {
            Configuration = configuration;
            }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(X => X.JsonSerializerOptions.ReferenceHandler= ReferenceHandler.IgnoreCycles);
            services.AddValidatorsFromAssemblyContaining<Program>();
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("defaulConnection")));
            services.AddTransient<IServicio,ServicioA>();
            services.AddTransient<ServicioTransient>();
            services.AddScoped<ServicioScoped>();
            services.AddSingleton<ServicioSingleton>();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }
        public void Configure (IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            app.Use(async (contexto, siguiente) =>
            {
                using (var ms = new MemoryStream())
                {
                    var cuerpoOriginalRespuesta = contexto.Response.Body;
                    contexto.Response.Body = ms;

                    await siguiente.Invoke();

                    ms.Seek (0, SeekOrigin.Begin);
                    string respuesta = new StreamReader (ms).ReadToEnd();  
                    ms.Seek(0, SeekOrigin.Begin);

                    await ms.CopyToAsync(cuerpoOriginalRespuesta);
                    contexto.Response.Body = cuerpoOriginalRespuesta;

                    logger.LogInformation(respuesta);
                }
            });
            
            app.Map("/ruta1", app =>
                {
                    app.Run(async contenxto =>
                    {
                        await contenxto.Response.WriteAsync("Estoy intereceptando la tuberia");
                    });
                });
           

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting ();

            app.UseAuthorization();

            app.UseEndpoints (endpoints => { endpoints.MapControllers (); });

        }
    }
}
