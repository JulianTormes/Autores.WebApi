using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;
using WebApiAutores.Filtros;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/autores")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        //private readonly IValidator<Autor> _validator;

        public AutoresController(ApplicationDbContext context,IMapper mapper) 

        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]//api/autores (hereda ruta)

        //(Multiples rutas)
        public async Task<ActionResult<List<AutorDTO>>> Get()
        {
            var autores = await _context.Autores.ToListAsync();
            return _mapper.Map<List<AutorDTO>>(autores);
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<AutorDTO>> Get(int id)
        {
            var autor = await _context.Autores.FirstOrDefaultAsync(autorBD => autorBD.Id == id);

            if (autor == null)
            {
                return NotFound();
            }
            return _mapper.Map<AutorDTO>(autor);

        }
        [HttpGet("{nombre}")]
        public async Task<ActionResult<List<AutorDTO>>> Get([FromRoute] string nombre)
        {
            var autores = await _context.Autores.Where(autorBD => autorBD.Nombre.Contains(nombre)).ToListAsync();

            return _mapper.Map<List<AutorDTO>>(autores);

        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] AutorCreacionDTO autorCreacionDTO)
        {
            /*var result = await ValidateAsync(autor);
            if (!result.IsValid)
            {
                _logger.LogWarning("{@errors}", result.Errors);
                return BadRequest(result.Errors);
            }*/
            var ExisteAutorConElMismoNombre = await _context.Autores.AnyAsync(x=>x.Nombre==autorCreacionDTO.Nombre);
            if (ExisteAutorConElMismoNombre)
            {
                return BadRequest($"Ya existe un autor con el nombre{autorCreacionDTO.Nombre}");
            }

            var autor = _mapper.Map<Autor>(autorCreacionDTO);

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

            var existe = await _context.Autores.AnyAsync(autorBD => autorBD.Id == id);
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
