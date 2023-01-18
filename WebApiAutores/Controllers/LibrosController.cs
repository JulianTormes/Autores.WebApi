using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers;

[ApiController]
[Route("api/libros")]

public class LibrosController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public LibrosController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    [HttpGet("{id:int}", Name = "ObtenerLibro")]
    public async Task<ActionResult<LibroDTOConAutores>> Get(int id)
    {
        var libro = await _context.Libros
            .Include(libroDB => libroDB.AutoresLibros)
            .ThenInclude(autorLibroDB => autorLibroDB.Autor)
            .Include(libroDB => libroDB.Comentarios)
            .FirstOrDefaultAsync(x => x.Id == id);
        libro.AutoresLibros = libro.AutoresLibros.OrderBy(x => x.Orden).ToList();

        return _mapper.Map<LibroDTOConAutores>(libro);
    }
    [HttpPost]
    public async Task<ActionResult> Post(LibroCreacionDTO libroCreacionDTO)
    {
        if (libroCreacionDTO.AutoresIds == null)
        {
            return BadRequest("No se puede crear un libro sin autores");
        }

        var autoresIds = await _context.Autores
            .Where(autorBD => libroCreacionDTO.AutoresIds.Contains(autorBD.Id)).Select(x => x.Id).ToListAsync();
        if (libroCreacionDTO.AutoresIds.Count != autoresIds.Count)
        {
            return BadRequest("No existe uno de los autores enviados");
        }

        var libro = _mapper.Map<Libro>(libroCreacionDTO);
        AsignarOrdenAutores(libro);
        _context.Add(libro);
        await _context.SaveChangesAsync();
        var libroDTO = _mapper.Map<LibroDTO>(libro);
        return CreatedAtRoute("ObtenerLibro", new { id = libro.Id }, libroDTO);
    }
    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, LibroCreacionDTO libroCreacionDTO)
    { 
        var libroDB= await _context.Libros
            .Include(x=>x.AutoresLibros)
            .FirstOrDefaultAsync(x=>x.Id == id);
        if (libroDB == null)
        { 
            return NotFound();
        }
        libroDB = _mapper.Map(libroCreacionDTO, libroDB);

        AsignarOrdenAutores(libroDB);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    private void AsignarOrdenAutores(Libro libro)
    {
        if (libro.AutoresLibros != null)
        {
            for (int i = 0; i < libro.AutoresLibros.Count; i++)
            {
                libro.AutoresLibros[i].Orden = i;
            }

        }
    }
}
