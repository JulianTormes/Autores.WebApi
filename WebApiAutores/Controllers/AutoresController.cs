using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entidades;
using WebApiAutores.Filtros;
using WebApiAutores.Servicios;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/autores")]
    //[Authorize]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IServicio _servicio;
        private readonly ServicioTransient _servicioTransient;
        private readonly ServicioScoped _servicioScoped;
        private readonly ServicioSingleton _servicioSingleton;


        //private readonly IValidator<Autor> _validator;
        private readonly ILogger<AutoresController> _logger;

        public AutoresController(ApplicationDbContext context, IServicio servicio,
            ServicioTransient servicioTransient, ServicioScoped servicioScoped, ServicioSingleton servicioSingleton, ILogger<AutoresController> logger) //, IValidator<Autor> validator

        {
            _context = context;
            _servicio = servicio;
            _servicioTransient = servicioTransient;
            _servicioScoped = servicioScoped;
            _servicioSingleton = servicioSingleton;
            //_validator = validator;
            _logger = logger;
            //var x = new AutoresController(context, _validator);
        }
        [HttpGet("GUID")]
        //[ResponseCache(Duration = 10)]
        [ServiceFilter(typeof(MiFiltroDeAccion))]
        public ActionResult ObtenerGuids()
        {
            return Ok(new
            {   
                AutoresControllerTransient = _servicioTransient.Guid,
                ServicioA_Transient = _servicio.ObtenerTransient(),
                AutoresControllerScoped = _servicioScoped.Guid,
                ServicioA_Scoped = _servicio.ObtenerScoped(),
                AutoresControllerSingleton = _servicioSingleton.Guid,
                ServicioA_Singleton = _servicio.ObtenerSingleton()
            });
        }

        [HttpGet]//api/autores (hereda ruta)
        [HttpGet("listado")]//api/autores/listado (Concatena ruta)
        [HttpGet("/listado")]//listado (crea una nueva ruta)
        [ServiceFilter(typeof(MiFiltroDeAccion))]
        //[ResponseCache(Duration = 10)]

        //(Multiples rutas)
        public async Task<ActionResult<List<Autor>>> Get()
        {
            throw new NotImplementedException();
            _logger.LogInformation("Estamos obteniendo los autores");
            _logger.LogWarning("Este es un mensaje de prueba");
            _servicio.RealizarTarea();
            return await _context.Autores.Include(X => X.Libros).ToListAsync();
        }
        [HttpGet("primero")] // api/autores/primero?nombre=felipe&apellido=gavilan  (Concatena la ruta)
        public async Task<ActionResult<Autor>> PrimerAutor([FromHeader] int mivalor, [FromQuery] string nombre)
        {
            return await _context.Autores.FirstOrDefaultAsync();
        }

        [HttpGet("{id:int}/{param2=persona}")]
        public async Task<ActionResult<Autor>> Get(int id, string param2)
        {
            var autor = await _context.Autores.FirstOrDefaultAsync(x => x.Id == id);

            if (autor == null)
            {
                return NotFound();
            }
            return autor;

        }
        [HttpGet("{nombre}")]
        public async Task<ActionResult<Autor>> Get([FromRoute] string nombre)
        {
            var autor = await _context.Autores.FirstOrDefaultAsync(x => x.Nombre.Contains(nombre));

            if (autor == null)
            {
                return NotFound();
            }
            return autor;

        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Autor autor)
        {
            /*var result = await ValidateAsync(autor);
            if (!result.IsValid)
            {
                _logger.LogWarning("{@errors}", result.Errors);
                return BadRequest(result.Errors);
            }*/
            var ExisteAutorConElMismoNombre = await _context.Autores.AnyAsync(x=>x.Nombre==autor.Nombre);
            if (ExisteAutorConElMismoNombre)
            {
                return BadRequest($"Ya existe un autor con el nombre{autor.Nombre}");
            }

            _context.Add(autor);
            await _context.SaveChangesAsync();
            return Ok();
        }

            [HttpPut("{id:int}")]// api/autores/"IdAutor"
        public async Task<ActionResult> Put(Autor autor, int id)
        {
            if (autor.Id != id)
            {
                return BadRequest("El id del autor no coincide con el id de la URL");
            }

            var existe = await _context.Autores.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            _context.Update(autor);
            await _context.SaveChangesAsync();
            return Ok();

        }
        [HttpDelete("{id:int}")] // api/autores/IdAutor
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await _context.Autores.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            _context.Remove(new Autor() { Id = id });
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
