using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data.Entities;
using TaskManager.Enums;
using TaskManager.Helpers;
using TaskManager.Models;

public class AccountController : Controller
{
    private readonly IUserHelper _userHelper;
    public AccountController(IUserHelper userHelper)
    {
        _userHelper = userHelper;
    }
    public IActionResult Login()
    {
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }
        return View(new LoginViewModel());
    }
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            Microsoft.AspNetCore.Identity.SignInResult result = await _userHelper.LoginAsync(model);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos.");
        }
        return View(model);
    }
    public async Task<IActionResult> Logout()
    {
        await _userHelper.LogoutAsync();
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Register()
    {
        AddUserViewModel model = new AddUserViewModel
        {
            Id = Guid.Empty.ToString(),
            UserType = UserType.User,
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(AddUserViewModel model)
    {
        if (ModelState.IsValid)
        {                       
            Usuario user = await _userHelper.AddUserAsync(model);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Este correo ya está siendo usado.");
                return View(model);
            }
            LoginViewModel loginViewModel = new LoginViewModel
            {
                Password = model.Password,
                RememberMe = false,
                Username = model.Username
            };
            var result2 = await _userHelper.LoginAsync(loginViewModel);
            if (result2.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
        }
        return View(model);
    }

    public async Task<IActionResult> ChangeUser()
    {
        Usuario user = await _userHelper.GetUserAsync(User.Identity.Name);
        if (user == null)
        {
            return NotFound();
        }
        EditUserViewModel model = new()
        {
            
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,            
            Id = user.Id,            
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeUser(EditUserViewModel model)
    {
        if (ModelState.IsValid)
        {
           
            Usuario user = await _userHelper.GetUserAsync(User.Identity.Name);
            user.FirstName = model.FirstName;            
            user.LastName = model.LastName;           
            user.PhoneNumber = model.PhoneNumber;           
            await _userHelper.UpdateUserAsync(user);
            return RedirectToAction("Index", "Home");
        }
        return View(model);
    }

    public IActionResult ChangePassword()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userHelper.GetUserAsync(User.Identity.Name);
            if (user != null)
            {
                var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    return RedirectToAction("ChangeUser");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "User no found.");
            }
        }
        return View(model);
    }

}

