using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewFactoryProject.Models;

namespace NewFactoryProject.Controllers
{
    public class ShiftController : Controller
    {
        static ShiftBL shiftBL = new ShiftBL();
        // GET: Shift
        public ActionResult Index()
        {
            if (Session["fullName"] != null)
            {
                return View("Show");
            }
            else
            {
                return Redirect("/LogIn/");
            }
        }

        public ActionResult Show()
        {
            if (Session["fullName"] != null)
            {
                ViewBag.ShiftsList = shiftBL.GetAllEmployeeShifts();
                EmployeeBL employeeBL = new EmployeeBL();
                ViewBag.AllShifts = employeeBL.GetAllShifts();
                return View("Show");
            }
            else
            {
                return Redirect("/LogIn/");
            }
        }

        //Add Shift :
        public ActionResult Add()
        {
            if (Session["fullName"] != null)
            {
                EmployeeBL employeeBL = new EmployeeBL();
                ViewBag.AllShifts = employeeBL.GetAllShifts();
                return View("Add");
            }
            else
            {
                return Redirect("/LogIn/");
            }
        }

        [HttpPost]
        public ActionResult AddShift(EmployeesData shift)
        {
            shiftBL.Add(shift);
            int numOfActions = this.UpdateNumberOfActions();
            if (numOfActions == 0)
            {
                return Redirect("/LogIn/LogOut");
            }
            return Redirect("Show");
        }


        //< UpdateNumberOfActions > :
        private int UpdateNumberOfActions()
        {
            int numberOfActions = (int)Session["numOfActions"];
            numberOfActions = numberOfActions - 1;
            Session["numOfActions"] = numberOfActions;

            return numberOfActions;
        }
    }
}