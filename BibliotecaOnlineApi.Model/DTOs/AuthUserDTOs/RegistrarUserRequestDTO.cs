using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaOnlineApi.Model.DTOs.AuthUserDTOs
{
    public class RegistrarUserRequestDTO
    {

        [Required]
        public string name { get; set; }

        [Required]
        public string Apellido { get; set; }

        [Required]
        public int Edad{ get; set; }

        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        [PasswordPropertyText]
        public string password { get; set; }

        [Required]
        public int NumeroTelefono { get; set; }
    }
}
