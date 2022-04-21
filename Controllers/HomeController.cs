using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Exam1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Exam1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private MyContext _context;

        public HomeController(ILogger<HomeController> logger, MyContext context)
        {
            _logger = logger;
            _context = context;
        }
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(User newUser)
        {

            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.email == newUser.email))
                {
                    ModelState.AddModelError("email", "Email is already in Use");
                    return View("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.password = Hasher.HashPassword(newUser, newUser.password);
                _context.Add(newUser);
                _context.SaveChanges();
                HttpContext.Session.SetInt32("LoggedIn", newUser.UserId);
                return RedirectToAction("Dashboard");
            }
            else
            {
                return View("Index");
            }
        }

        [HttpPost("login")]
        public IActionResult Login(Login loginUser)
        {
            if (ModelState.IsValid)
            {
                User userInDb = _context.Users.FirstOrDefault(u => u.email == loginUser.lEmail);
                if (userInDb == null)
                {
                    // If it's null then they are NOT in the database
                    ModelState.AddModelError("lemail", "Invalid login attempt");
                    return View("Index");
                }
                // If we are the correct email it's time to check the password
                // First we make another instance of the password hasher
                PasswordHasher<Login> Hasher = new PasswordHasher<Login>();
                PasswordVerificationResult result = Hasher.VerifyHashedPassword(loginUser, userInDb.password, loginUser.lPassword);

                if (result == 0)
                {
                    ModelState.AddModelError("lemail", "Invalid login attempt");
                    return View("Index");
                }
                HttpContext.Session.SetInt32("LoggedIn", userInDb.UserId);
                return RedirectToAction("Dashboard");
            }
            else
            {
                return View("Index");
            }
        }

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            int? loggedIn = HttpContext.Session.GetInt32("LoggedIn");
            if (loggedIn != null)
            {
                ViewBag.LoggedIn = _context.Users.FirstOrDefault(d => d.UserId == (int)loggedIn);
                ViewBag.AllFundays= _context.Fundays.OrderBy(a => a.Fundaydate).Include(g => g.guestlist).ToList();
                return View("Dashboard");
            }
            else
            {
                return View("Index");
            }
        }

        [HttpGet("addFunday")]
        public IActionResult AddFunday()
        {
            int? LoggedIn = HttpContext.Session.GetInt32("LoggedIn");
            if (LoggedIn == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.UserId = Convert.ToInt32(LoggedIn);
            return View();
        }

        [HttpPost("planFunday")]
        public IActionResult PlanFunday(Funday plannedFunday)
        {
            if (ModelState.IsValid)
            {
                if (plannedFunday.Fundaydate > DateTime.Now)
                {
                    _context.Add(plannedFunday);
                    _context.SaveChanges();
                    return Redirect("Dashboard");
                }
                else
                {
                    int? LoggedIn = HttpContext.Session.GetInt32("LoggedIn");
                    ViewBag.UserId = Convert.ToInt32(LoggedIn);
                    ModelState.AddModelError("Fundaydate", "Date should be in the future.");
                    return View("AddFunday");
                }
            }
            else
            {
                return View("AddFunday");
            }
            }

[HttpGet("RSVP/{Fundayid}/{userid}")]
        public IActionResult JoinFunday(int Fundayid, int userid)
        {
            if (HttpContext.Session.GetInt32("LoggedIn") == null)
            {
                return RedirectToAction("Index");
            }
            if ((int)HttpContext.Session.GetInt32("LoggedIn") != userid)
            {
                return RedirectToAction("Logout");
            }
           
            RSVP newRSVP = new RSVP();
            newRSVP.FundayId = Fundayid;
            newRSVP.UserId = userid;
            _context.Add(newRSVP);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

[HttpGet("unRSVP/{Fundayid}/{userid}")]
        public IActionResult LeaveFunday(int Fundayid, int userid)
        {
            if (HttpContext.Session.GetInt32("LoggedIn") == null)
            {
                return RedirectToAction("Index");
            }
            if ((int)HttpContext.Session.GetInt32("LoggedIn") != userid)
            {
                return RedirectToAction("Logout");
            }
            RSVP unRSVP = _context.RSVPs.FirstOrDefault(f => f.FundayId == Fundayid && f.UserId == userid);
            _context.RSVPs.Remove(unRSVP);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

[HttpGet("Funday/{fundayId}")]
        public IActionResult OneFunday(int FundayId)
        {
            int? LoggedIn = HttpContext.Session.GetInt32("LoggedIn");
            if (LoggedIn == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                Funday one = _context.Fundays.Include(c => c.guestlist).ThenInclude(ti => ti.User).FirstOrDefault(fd => fd.FundayId == FundayId);
                ViewBag.AllUsers = _context.Users.OrderBy(u => u.firstname).ToList();
                return View(one);
            }
        }
[HttpGet("delete/{FundayId}")]
public IActionResult Delete(int Fundayid)
{
    Funday toDelete = _context.Fundays.SingleOrDefault(f => f.FundayId == Fundayid);
    if (HttpContext.Session.GetInt32("LoggedIn") != toDelete.UserId)
    {
        return RedirectToAction("Logout");
    }
    else
    {
        _context.Fundays.Remove(toDelete);
        _context.SaveChanges();
        return RedirectToAction("Dashboard");
    }
}
[HttpGet("logout")]
public IActionResult Logout()
{
    HttpContext.Session.Clear();
    return RedirectToAction("Index");
}

        }
        

    }



