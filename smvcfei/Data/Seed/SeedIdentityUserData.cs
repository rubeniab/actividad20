//Revisado
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using smvcfei.Models;

namespace smvcfei.Data.Seed
{
    public static class SeedIdentityUserData
    {
        public static void SeedUserIdentityData(this ModelBuilder modelBuilder)
        {
            string AdministradorGeneralRoleId = Guid.NewGuid().ToString();
            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = AdministradorGeneralRoleId,
                Name = "Administrador",
                NormalizedName = "Administrador".ToUpper()
            });

            string UsuarioGeneralRoleId = Guid.NewGuid().ToString();
            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = UsuarioGeneralRoleId,
                Name = "Usuario general",
                NormalizedName = "Usuario general".ToUpper()
            });
            var UsuarioId = Guid.NewGuid().ToString();
            modelBuilder.Entity<CustomIdentityUser>().HasData(
                new CustomIdentityUser
                {
                    Id = UsuarioId,
                    UserName = "rubens@uv.mx",
                    Email = "rubens@uv.mx",
                    NormalizedEmail = "rubens@.uv.mx".ToUpper(),
                    Nombrecompleto = "Rubén Isaí Alejo Barrientos",
                    NormalizedUserName = "rubens@uv.mx".ToUpper(),
                    PasswordHash = new PasswordHasher<CustomIdentityUser>().HashPassword(null, "patito")
                }
            );

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = AdministradorGeneralRoleId,
                    UserId = UsuarioId
                }
            );
        }
    }
}
