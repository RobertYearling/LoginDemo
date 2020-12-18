using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LoginDemo.Models;
using Microsoft.AspNetCore.Http;
using LoginDemo.Context;
using Microsoft.AspNetCore.Identity;

namespace LoginDemo.Controllers
{
    public class HomeController : Controller
    {
        private HomeContext _context;

        public HomeController(HomeContext context)
        {
            _context = context;
        }

        private User GetUserFromDB()
        {
            return _context.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("signin")]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost("register")]
        public  IActionResult Register(User reg)
        {
            if(ModelState.IsValid)
            {
                if(_context.Users.Any(u => u.Email == reg.Email))
                {
                    ModelState.AddModelError("Email", "Email is already used.");
                    return View("Index");
                }
                PasswordHasher<User> hasher = new PasswordHasher<User>();
                reg.Password = hasher.HashPassword(reg,reg.Password);
                _context.Users.Add(reg);
                _context.SaveChanges();
                HttpContext.Session.SetInt32("UserId",reg.UserId);
                return RedirectToAction("Dashboard");
            }
            return View("Index");
        }

        [HttpPost("login")]
        public IActionResult Login(LoginUser log)
        {
            if(ModelState.IsValid)
            {
                User userInDb = _context.Users.FirstOrDefault(u => u.Email == log.LoginEmail);
                if(userInDb == null)
                {
                    ModelState.AddModelError("LoginEmail","Invalid Email/Passowrd");
                    ModelState.AddModelError("LoginPassword","Invalid Email/Passowrd");
                    return View("SignIn");
                }
                PasswordHasher<LoginUser> hash = new PasswordHasher<LoginUser>();
                var result = hash.VerifyHashedPassword(log,userInDb.Password,log.LoginPassword);
                if(result == 0)
                {
                    ModelState.AddModelError("LoginEmail","Invalid Email/Passowrd");
                    ModelState.AddModelError("LoginPassword","Invalid Email/Passowrd");
                    return View("SignIn");
                }
                HttpContext.Session.SetInt32("UserId",userInDb.UserId);
                return RedirectToAction("Dashboard");
            }
            return View("SignIn");
        }

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            User userInDb = GetUserFromDB();
            if(userInDb == null)
            {
                return RedirectToAction("Logout");
            }
            return View(userInDb);
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
