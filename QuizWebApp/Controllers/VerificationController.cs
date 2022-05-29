using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuizWebApp.Models;
using System.Collections;

namespace QuizWebApp.Controllers
{
    public class VerificationController : Controller
    {
        //Calling in the model of the database
        private SchoolApplicationAppEntities db = new SchoolApplicationAppEntities();

        // GET: Verification
        public ActionResult Index()
        {
            return View();
        }

        //ActionResult that will be used for the log in of a user
        //-------------------------------------------------------------------------
        public ActionResult Login()
        {
            //Used if the view is opened and a user is not loged in
            if (Session["loginck"] as string == "failed")
            {
                ViewBag.Error = "Log In Required";
            }

            return View();
        }
        //-------------------------------------------------------------------------

        //ActionResult (HttpPost) that will be used for the log in of a user to check if the user is in the database
        //-------------------------------------------------------------------------
        [HttpPost]
        public ActionResult login(string tbxUsername, string tbxPassword)
        {
            bool found = false;
            //process the login
            foreach (User c in db.Users)
            {
                //see if the username is in the database
                if (c.User_ID.ToString().Equals(tbxUsername) && c.Password.ToString().Equals(tbxPassword))
                {
                    //makes a session if the user is in the database
                    Session["User"] = c.User_ID;// create session varible 
                    Session["UserRole"] = c.UserRole;// create session varible

                    found = true;
                    //go strait to the users favourite page so they can view their forecast
                    if (c.UserRole == "student")
                    {
                        //redirect to a succes page
                        return RedirectToAction("AllTests", "Student");
                    }
                    else
                    {
                        //redirect to a succes page
                        return RedirectToAction("AllTestCreated", "Lecture");
                    }
                }
            }
            //if username is not in database
            if (found == false)
            {
                //if username is Incorrect
                ViewBag.Error = "Username or Password is Incorrect";
            }
            return View();
        }
        //-------------------------------------------------------------------------

        //Sign out of the application
        //-------------------------------------------------------------------------
        public ActionResult SignOut()
        {
            Session["User"] = null;
            return RedirectToAction("Index", "Home");
        }
        //-------------------------------------------------------------------------
    }
}