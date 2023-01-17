using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebApiAutores.Controllers;
using WebApiAutores.Filtros;
using WebApiAutores.Middlewares;

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
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
            services.AddAutoMapper(typeof(Startup));
        }
        public void Configure (IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            app.UseLoguearRespuestaHttps();

          
           

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
