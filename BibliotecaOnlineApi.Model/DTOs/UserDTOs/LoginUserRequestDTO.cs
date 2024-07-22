using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaOnlineApi.Model.DTOs.UserDTOs
{
    public class LoginUserRequestDTO
    {
        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        [PasswordPropertyText]
        public string password { get; set; }
    }
}
