using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewFactoryProject.Models;

using static NewFactoryProject.Models.LogInBL;

namespace NewFactoryProject.Controllers
{
    public class LogInController : Controller
    {
        private static LogInBL loginBL = new LogInBL();
        // GET: LogIn
        public ActionResult Index()
        {
            return View("LogIn");
        }


        public ActionResult HomePage()
        {
            if (Session["fullName"] != null)
            {
                return View("HomePage");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }


        //LogIn :
        [HttpPost]
        public ActionResult LogIn(string username, string password)
        {
            int id;
            int numOfActions;
            string fullName;

            Status status = loginBL.LogIn(username, password, out id, out fullName, out numOfActions);

            if (status.Equals(Status.Authorized))
            {
                Session["id"] = id;
                Session["fullName"] = fullName;
                Session["numOfActions"] = numOfActions;

                return Redirect("HomePage");
            }
            else
            {
                switch (status)
                {
                    case Status.Unauthorized:
                        ViewBag.ErrorMessage = "Invalid Data";
                        break;
                    case Status.AuthorizedButReachMaximumActionsPerDay:
                        ViewBag.ErrorMessage = "Maximum allowed actions per day has been reached";
                        break;
                }
                return View("LogIn");
            }
        }


        //LogOut :
        public ActionResult LogOut()
        {
            if (Session["id"] != null)
            {
                int id = (int)Session["id"];
                int numOfActions = (int)Session["numOfActions"];

                loginBL.LogOut(id, numOfActions);

                Session["id"] = null;
                Session["fullName"] = null;
                Session["numOfActions"] = null;
            }

            return View("LogIn");
        }
    }
}