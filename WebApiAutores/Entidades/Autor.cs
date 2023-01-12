using FluentValidation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Entidades;

public class Autor :IValidatableObject
{
    public int Id { get; set; }
    [Required(ErrorMessage ="El campo {0} es requerido")]
    [StringLength(maximumLength: 100, ErrorMessage = "El campo {0} no debe tener mas de {1} caracteres")]
    //[PrimeraLetraMayuscula]
    public string Nombre { get; set; }

    /*[Range(18,99)]
    [NotMapped]
    public int Edad {get; set; }
    [CreditCard]
    [NotMapped]
    public string TarjetaDeCredito { get;set;}
    [Url]
    [NotMapped]
    public string URL {get;set; }
    public int Menor {get; set; }
    public int Mayor { get; set; }  */
    public List <Libro> Libros { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrEmpty(Nombre))
        {
            var PrimeraLetra = Nombre[0].ToString();
            if (PrimeraLetra != PrimeraLetra.ToUpper())
            {
                yield return new ValidationResult ("La primera letra debe ser mayuscula",
                    new string[] {nameof(Nombre)});
            }

        }
        /*if (Menor>Mayor)
        { 
            yield return new ValidationResult ("Este valor no puede ser mas grande que el campo mayor",
                new string[] {nameof(Menor) });   
        }*/
    }
}

/*public class AutorValidator : AbstractValidator<Autor>
{
    public bool ValidateUri(string uri)
    {
        if (string.IsNullOrEmpty(uri))
        {
            return true;
        }
        return Uri.TryCreate(uri, UriKind.Absolute, out _);
    }

    public AutorValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El campo {PropertyName} es requerido")
            .MaximumLength(10).WithMessage("El campo {PropertyName} no debe tener mas de {MaxLength} caracteres");

        RuleFor(x => x.Edad)
            .InclusiveBetween(18, 99).WithMessage("La edad debe estar entre 18 y 99");


        RuleFor(x => x.TarjetaDeCredito)
            .CreditCard().WithMessage("Tarjeta de credito no es valida");

        RuleFor(x => x.URL)
            .NotEmpty().WithMessage("LA URL no puede estar vacia")
            .Must(x=> ValidateUri(x)).WithMessage("URL no es valida");
    }
}
*/