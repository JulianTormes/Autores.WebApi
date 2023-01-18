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
    [HttpGet("{id:int}")]
    public async Task<ActionResult<LibroDTO>> Get(int id)
    {
        var libro = await _context.Libros.
            Include(libroDB=>libroDB.Comentarios)
            .FirstOrDefaultAsync(x => x.Id == id);

        return _mapper.Map<LibroDTO>(libro);
    }
    [HttpPost]
    public async Task<ActionResult> Post (LibroCreacionDTO libroCreacionDTO)
    {
        var autoresIds = await _context.Autores
            .Where(autorBD => libroCreacionDTO.AutoresIds.Contains(autorBD.Id)).Select (x => x.Id).ToListAsync();
        if (libroCreacionDTO.AutoresIds.Count != autoresIds.Count)
        {
            return BadRequest("No existe uno de los autores enviados");
        }

        var libro = _mapper.Map<Libro>(libroCreacionDTO);
        _context.Add(libro);
        await _context.SaveChangesAsync();
        return Ok();
    }
}
