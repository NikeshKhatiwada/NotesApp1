using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotesApp1.Models;
using NotesApp1.Services;
using NotesApp1.ViewModels;
using System.Security.Claims;
using BC = BCrypt.Net.BCrypt;

namespace NotesApp1.Controllers
{
	public class AccountController : Controller
	{
        private readonly NoteItemDataService itemDataService;
        private readonly NoteUserDataService userDataService;
        public AccountController()
		{
            itemDataService = new NoteItemDataService();
            userDataService = new NoteUserDataService();
        }

		[HttpGet, AllowAnonymous]
		public IActionResult Register()
		{
			UserRegistrationModel userRegistrationModel = new UserRegistrationModel();
			return View(userRegistrationModel);
		}

		[HttpPost, AllowAnonymous]
		public IActionResult Register(UserRegistrationModel userRegistrationModel)
		{
			if(ModelState.IsValid)
			{
				var user = new NoteUser
				{
					UserName = userRegistrationModel.UserName,
					Email = userRegistrationModel.Email
				};
				user.Id = Guid.NewGuid();
				user.Password = BC.HashPassword(userRegistrationModel.Password);
				user.CreatedAt = DateTime.UtcNow;
				user.UpdatedAt = DateTime.UtcNow;
				userDataService.InsertNoteUser(user);
				return Redirect("/");
			}
			ModelState.AddModelError("message", "Email already exists.");
			return View(Request);
		}

		[HttpGet, AllowAnonymous]
		public IActionResult Login()
		{
			UserLoginModel userLoginModel = new UserLoginModel();
			return View(userLoginModel);
		}

		[HttpPost, AllowAnonymous]
		public IActionResult Login(UserLoginModel userLoginModel)
		{
			if(ModelState.IsValid)
			{
				var user = userDataService.GetNoteUsers().Where(NoteUser => NoteUser.Email == userLoginModel.Email).FirstOrDefault();
				if(user != null)
				{
                    var result = BC.Verify(userLoginModel.Password, user.Password);
                    if (result)
					{
						var claimIdentity = new ClaimsIdentity(new[]
						{
							new Claim(ClaimTypes.Name, user.UserName)
						}, CookieAuthenticationDefaults.AuthenticationScheme);
						var principal = new ClaimsPrincipal(claimIdentity);
						HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
						HttpContext.Session.SetString("Username", user.UserName);
						return Redirect("/");
                    }
                    else
                    {
                        ModelState.AddModelError("message", "Invalid password.");
                        return View(userLoginModel);
                    }
                }
				else
				{
					ModelState.AddModelError("message", "Invalid login attempt.");
					return View(userLoginModel);
				}
            }
			return View(userLoginModel);
		}

		[HttpPost, Authorize]
		public IActionResult Logout()
		{
			HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return Redirect("/");
		}
	}
}
