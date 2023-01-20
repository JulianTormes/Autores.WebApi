using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.DTOs
{
    public class EditarAdminDTO
    {
        [Required]
        [EmailAddress]
        public String Email { get; set; }
    }
}
