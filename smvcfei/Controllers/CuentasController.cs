using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using smvcfei.Data;
using smvcfei.Models;

namespace smvcfei.Controllers
{
    [Authorize]

    public class CuentasController : Controller
    {
        private readonly IdentityContext _context;
        private readonly UserManager<CustomIdentityUser> _userManager;
        private readonly SignInManager<CustomIdentityUser> _signInManager;

        public CuentasController(IdentityContext context, UserManager<CustomIdentityUser> userManager, SignInManager<CustomIdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> LoginAsync(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bool signInResult = false;
                    var result = await _signInManager.PasswordSignInAsync(model.Correo, model.Password, isPersistent: false, lockoutOnFailure: false);
                    signInResult = result.Succeeded;

                    if (signInResult)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("Correo", "Credenciales no válidas. Inténtelo nuevamente");
                    }
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Registro(bool creado = false)
        {
            ViewData["creado"] = creado;
            ViewData["RolesSelect"] = new SelectList(await _context.Roles.OrderBy(r => r.Name).AsNoTracking().ToListAsync(), "Name", "Name", null);
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> RegistroAsync(UsuarioViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var usuario = await _userManager.FindByEmailAsync(model.Correo);
                    if(usuario == null){
                        var usuarioToCreate = new CustomIdentityUser
                        {
                            UserName = model.Correo,
                            Email = model.Correo,
                            NormalizedEmail = model.Correo.ToUpper(),
                            Nombrecompleto = model.Nombre,
                            NormalizedUserName = model.Correo.ToUpper(),
                        };
                        IdentityResult result = await _userManager.CreateAsync(usuarioToCreate, model.Password);
                        if (result.Succeeded)
                        {
                            await _userManager.AddToRoleAsync(usuarioToCreate, model.Rol);
                            return RedirectToAction(nameof(RegistroAsync), new { creado = true });
                        }

                        List<IdentityError> errorList = result.Errors.ToList();
                        var errors = string.Join(" ", errorList.Select(e => e.Description));
                        ModelState.AddModelError("Password", errors);
                    }
                    else
                    {
                        ModelState.AddModelError("Correo", $"El usuario {usuario.UserName} ya existe");
                    }
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            ViewData["creado"] = false;
            return View();
        }

        public async Task<IActionResult> PerfilAsync()
        {
            var identityUser = await _userManager.FindByEmailAsync(User.Identity.Name);
            var roles = _userManager.GetRolesAsync(identityUser).Result;
            var rol = string.Join(",", roles);

            UsuarioViewModel usuario = new()
            {
                Nombre = identityUser.Nombrecompleto,
                Correo = identityUser.Email,
                Rol = rol
            };
        return View(usuario);
        }

        public async Task<IActionResult> LogoutAsync(string returUrl = null)
        {
            await _signInManager.SignOutAsync();

            if(returUrl != null)
            {
                return LocalRedirect(returUrl);
            }
            else 
            {
                return RedirectToAction("Login");
            }
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
