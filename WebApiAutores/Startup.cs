using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebApiAutores.Controllers;
using WebApiAutores.Filtros;
using WebApiAutores.Middlewares;
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
            services.AddControllers(opciones => 
            {
                opciones.Filters.Add(typeof(FiltroDeExcepcion));
            })
                .AddJsonOptions(X => X.JsonSerializerOptions.ReferenceHandler= ReferenceHandler.IgnoreCycles);
            services.AddValidatorsFromAssemblyContaining<Program>();
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("defaulConnection")));
            services.AddTransient<IServicio,ServicioA>();
            services.AddTransient<ServicioTransient>();
            services.AddScoped<ServicioScoped>();
            services.AddSingleton<ServicioSingleton>();
            services.AddTransient<MiFiltroDeAccion>();
            services.AddHostedService<EscribirEnArchivo>();
            services.AddResponseCaching();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        }
        public void Configure (IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            app.UseLoguearRespuestaHttps();

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

            app.UseResponseCaching();

            app.UseAuthorization();

            app.UseEndpoints (endpoints => { endpoints.MapControllers (); });

        }
    }
}
