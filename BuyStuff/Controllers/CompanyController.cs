using Microsoft.AspNetCore.Mvc;
using BuyStuffOnline.DataAccess.Repository.IRepository;
using BuyStuffOnline.DataAccess.Repository;
using BuyStuffOnline.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using BuyStuffOnline.Models.ViewModels;
using System.Runtime.Serialization.Formatters;
using Microsoft.AspNetCore.Authorization;
using BuyStuffOnline.Utility;

namespace BuyStuff.Controllers
{
    [Authorize (Roles = StaticDetails.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly ICompanyRepository _CompanyRepo;
        private readonly ICategoryRepository _categoryRepo;
        private IWebHostEnvironment _webHostEnvironment;
        public CompanyController(ICompanyRepository _db)
        {
            _CompanyRepo = _db;
        }

        public IActionResult Index()
         {
            //List<Company> objCompanyList = _CompanyRepo.GetAll().ToList();
            //GetAll();
            return View();
        }


        public IActionResult UpSert(int? ID) //Update+Insert
        {
            Company objCompany = new Company();

            if (ID != null && ID != 0)
            {
                objCompany = _CompanyRepo.Get(x => x.Id == ID);
            }
            //ViewBag.CategoryList = CategoryList;
            return View(objCompany);

        }
        [HttpPost]
        public IActionResult UpSert(Company objCompany)
        {
            if (ModelState.IsValid)
            {               

                if (objCompany.Id == 0)
                {
                    _CompanyRepo.Add(objCompany);
                }
                else
                    _CompanyRepo.Update(objCompany);


                _CompanyRepo.Save();

                return RedirectToAction("Index");
            }
            TempData["error"] = "Failed To Add Company";
            return View();
        }

        #region Edit

        //Edit region is not in use as we have combined Create and Edit pages.
        //The action for create and edit pages in UpSert()
        public IActionResult Edit(int ID)
        {
            if (ID <= 0)
            {
                return NotFound();
            }
            Company? _objCompany = _CompanyRepo.Get(x => x.Id == ID);

            if (_objCompany == null)
                return NotFound();

            IEnumerable<SelectListItem> CategoryList = _categoryRepo.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.ID.ToString()
            });
            ViewBag.CategoryList = CategoryList;
            return View(_objCompany);
        }

        [HttpPost]
        public IActionResult Edit(Company obj)
        {
            if (ModelState.IsValid)
            {
                _CompanyRepo.Update(obj);
                _CompanyRepo.Save();
                TempData["success"] = "Company Updated Successfully";
            }

            return RedirectToAction("Index");
        }
        #endregion


        //Delete Action is not being used as we are routing the request through DeleteConfirmation() which is present in Company.js
        //DeleteConfirmation() uses SweetAlert
        public IActionResult Delete(int ID)
        {

            if (ID == 0)
            {
                return NotFound();
            }
            Company? _objCompany = _CompanyRepo.Get(x => x.Id == ID);

            if (_objCompany == null)
                return NotFound();


            return View(_objCompany);
        }

        //[HttpPost, ActionName("Delete")]
        //This fucntion is called using Ajax call from the DeleteConfirmation() which is present in Company.js
        public IActionResult DeletePost(int ID)
        {
            if (ID == 0)
            {
                return Json(new { success = false, message = "Error while deleting" });
                //return NotFound();
            }
            Company? _objCompany = _CompanyRepo.Get(x => x.Id == ID);

            if (_objCompany == null) 
            {
                return Json(new { success = false, message =  "Error while deleting"});
            }
            _CompanyRepo.Remove(_objCompany);
            _CompanyRepo.Save();
            return Json(new { success = true, message = "Company Deleted" });
            //return RedirectToAction("Index");
        }

        #region API
        [HttpGet]
        public IActionResult GetAll() 
        {
            List<Company> objCompanyList = _CompanyRepo.GetAll().ToList();
            return Json(new { data = objCompanyList });
        }
        #endregion
    }
}
