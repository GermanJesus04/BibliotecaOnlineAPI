using BibliotecaOnlineApi.Model.Modelo;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaOnlineApi.Infraestructura.HelpierConfiguracion
{
    public class DatosSemilla
    {
        public static async Task Inicializar(
            IServiceProvider serviceProvider, 
            UserManager<User> userManager, 
            RoleManager<IdentityRole> roleManager, 
            AdminSettings adminSettings
            )
        {
            // Crear roles predeterminados
            string[] roles = { "User", "Admin" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Crear usuario administrador predeterminado
            var adminEmail = adminSettings.Email;
            var adminPassword = adminSettings.Password;

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
