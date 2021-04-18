using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewFactoryProject.Models;

namespace NewFactoryProject.Controllers
{
    public class DepartmentController : Controller
    {
        private static DepartmentBL departmentBL = new DepartmentBL();
        private static string errorMessage = null;

        // GET: Department
        public ActionResult Index()
        {
            if (Session["fullName"] != null)
            {
                return Redirect("Show");
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
                ViewBag.departmentManagers = departmentBL.GetAllDepartmentsManagerData();
                ViewBag.ErrorMessage = errorMessage;
                errorMessage = null;

                return View("Show");
            }
            else
            {
                return Redirect("/LogIn/");
            }
        }

       //Add Department:
        public ActionResult Add()
        {
            if (Session["fullName"] != null)
            {
                EmployeeBL employeeBL = new EmployeeBL();
                ViewBag.EmployeesThatAreNotManagers = employeeBL.GetEmployeesThatAreNotManagers();
                ViewBag.Employees = employeeBL.GetEmployees();
                return View("Add");
            }
            else
            {
                return Redirect("/LogIn/");
            }
        }

        [HttpPost]
        public ActionResult AddDepartment(DepartmentManager departmentManager)
        {
            bool isSuccessful = departmentBL.Add(departmentManager);

            if (isSuccessful == false)
            {
                errorMessage = "Error: Either Department already exists\nOr Employee is not registered in system\nOr Employee is already a manager\nOr no manager was selected";
            }
            else
            {
                errorMessage = null;

                int numOfActions = this.UpdateNumberOfActions();

                if (numOfActions == 0)
                {
                    return Redirect("/LogIn/LogOut");
                }
            }

            return Redirect("Show");
        }

        //Edit Department:
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (Session["fullName"] != null)
            {
                EmployeeBL employeeBL = new EmployeeBL();
                DepartmentManager departmentManager = departmentBL.GetDepartmentManager(id);
                ViewBag.EmployeesThatAreNotManagers = employeeBL.GetEmployeesThatAreNotManagers();
                ViewBag.Employees = employeeBL.GetEmployees();
                ViewBag.ManagerID = departmentManager.ID;
                return View("Edit", departmentManager);
            }
            else
            {
                return Redirect("/LogIn/");
            }
        }

        [HttpPost]
        public ActionResult EditDepartment(DepartmentManager departmentManager)
        {
            bool isSuccessful = departmentBL.Edit(departmentManager); 

            if (isSuccessful == false)
            {
                errorMessage = "Error: There is no Employee with that ID";
            }
            else
            {
                errorMessage = null;

                int numOfActions = this.UpdateNumberOfActions();

                if (numOfActions == 0)
                {
                    return Redirect("/LogIn/LogOut");
                }
            }

            return Redirect("Show");
        }


        //Delete Department:
        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (Session["fullName"] != null)
            {
                departmentBL.Delete(id);

                int numOfActions = this.UpdateNumberOfActions();

                if (numOfActions == 0)
                {
                    return Redirect("/LogIn/LogOut");
                }

                return Redirect("../Show");
            }
            else
            {
                return Redirect("/LogIn/");
            }

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