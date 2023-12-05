using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Brightflow_mvc_opdracht.Functions;
using Brightflow_mvc_opdracht.Models;
using Microsoft.Ajax.Utilities;

namespace Brightflow_mvc_opdracht.Controllers
{
    public class UserController : Controller
    {
        private DBEntities db = new DBEntities();
        private globalFunctions GF = new globalFunctions();

        public ActionResult Login(User loggedUser)
        {
            if (ModelState.IsValid) { //checken als het formulier is ingevoerd
                string hashedPassword = GF.HashPassword(loggedUser.Wachtwoord); //hashen van het wachtwoord globaal zodat ik het vaker kan gebruiken

                User user = db.User.FirstOrDefault(u => u.Mail.Equals(loggedUser.Mail) && u.Wachtwoord.Equals(hashedPassword));

                if (user == null) {
                    ModelState.AddModelError("", "We kunnen je niet vinden, heb je wat verkeerds ingevoerd?");
                    return View("Login");
                }
                HttpCookie loginCookie = new HttpCookie("loginCookie");
                loginCookie["UserId"] = user.UserId.ToString();
                loginCookie["Naam"] = user.Naam;
                DateTime expireDate = DateTime.Now.AddDays(7);

                if (user.Role == "Admin") {// check voor admin, anders krijg je niet eens een cookie
                    loginCookie["Role"] = "Admin";
                    expireDate = DateTime.Now.AddDays(1);
                }

                loginCookie.Expires = expireDate; // datum zetten en cookie zetten
                Response.Cookies.Add(loginCookie);

                return RedirectToAction("Index", "Home");
            }
            return View("Login");
        }

        public ActionResult Logout()
        {
            if (Response.Cookies["loginCookie"] != null) // checken als de cookie nog bestaat
                Response.Cookies["loginCookie"].Expires = DateTime.Now.AddDays(-1); // -1 dag laat de cookie gelijk verlopen
            
            return RedirectToAction("Index", "Home"); // redirect terug naar een pagina welke elke user kan zien
        }

        public ActionResult Register(User user)
        {
            if (ModelState.IsValid) //als het model valid is
            {
                if (db.User.Any(u => u.Mail.Equals(user.Mail))) { // als de email is ingevoerd of niet
                    ModelState.AddModelError("", "Er bestaat al een account met dit email adres");
                    return View("Register");
                } if (db.User.Any(u => u.Naam.Equals(user.Naam))) { // als het wachtwoord is ingevoerd of niet
                    ModelState.AddModelError("", "Er bestaat al een account met deze gebruikersnaam");
                    return View("Register");
                }

                string hashedPassword = GF.HashPassword(user.Wachtwoord); //het hashen gebeurt natuurlijk in een andere file zodat ik het vaker kan gebruiken
                user.Wachtwoord = hashedPassword;
                user.Role = "User";
                user.ProfilePicture = ""; //dingen die de user nog niet kan doen zoals het hele gebeuren met foto's en de rollen waar een user natuurlijk nog niet aan mag zitten

                db.User.Add(user); //naar de database zetten
                db.SaveChanges();

                HttpCookie loginCookie = new HttpCookie("loginCookie"); // de user alvast inloggen zodat die niet na het registreren nog een keer moet inloggen
                loginCookie["UserId"] = user.UserId.ToString();
                loginCookie["Naam"] = user.Naam;

                Response.Cookies.Add(loginCookie); //de cookie erbij zetten (heel gedonder gehad door response en request)

                return RedirectToAction("Index", "Home"); //redirect naar
            }
            return View("Register"); // als het formulier nog niet is ingevoerd zet de user door naar het formulier
        }

        // GET: User
        public ActionResult Index()
        {
            return View(db.User.ToList());
        }

        // GET: User/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            User user = db.User.Find(id);
            if (user == null)
                return HttpNotFound();

            return View(user);
        }

        // GET: User/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,Naam,Tussenvoegsel,Achternaam,Mail,Phone,Wachtwoord,Role,ProfilePicture")] User user)
        {
            if (ModelState.IsValid) {

                if (user.ProfilePicture == null) //als de user geen foto heeft ingevoerd
                    user.ProfilePicture = ""; //zet de foto op een lege string zodat de user nog steeds opgeslagen kan worden

                if (user.Role == null) //als de admin geen rol heeft ingevoerd
                    user.Role = "User"; //zet de rol op user zodat de user nog steeds opgeslagen kan worden

                db.User.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: User/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            User user = db.User.Find(id);
            if (user == null)
                return HttpNotFound();

            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,Naam,Tussenvoegsel,Achternaam,Mail,Phone,Wachtwoord,Role,ProfilePicture")] User user)
        {
            if (ModelState.IsValid)
            {
                if (user.ProfilePicture == null) //als de user geen foto heeft ingevoerd
                    user.ProfilePicture = ""; //zet de foto op een lege string zodat de user nog steeds opgeslagen kan worden

                db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: User/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            User user = db.User.Find(id);
            if (user == null)
                return HttpNotFound();
            
            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.User.Find(id);
            db.User.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}
