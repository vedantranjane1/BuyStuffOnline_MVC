using BuyStuffOnline.DataAccess.Data;
using BuyStuffOnline.DataAccess.Repository.IRepository;
using BuyStuffOnline.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuyStuffOnline.Models
{
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepo;
        public CategoryController(ICategoryRepository db) 
        {
            _categoryRepo = db;
        } 
        public IActionResult Index()
        {
            List<Category> objCategoryList = _categoryRepo.GetAll().ToList();
            return View(objCategoryList);
        }

        #region Create
        public IActionResult Create() 
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {

            if (obj.Name == obj.DisplayOrder.ToString()) 
            {
                ModelState.AddModelError("Name","Name cannot exactly match display order");
            }
            if (ModelState.IsValid)
            {
                _categoryRepo.Add(obj);//.Categories.Add(obj);
                _categoryRepo.Save();
                TempData["success"] = "Category Created Successfully";
                return RedirectToAction("Index");
            }
            
            return View();
        }

        #endregion


        #region Edit
        public IActionResult Edit(int Id)
        {

            if (Id <= 0)
                return NotFound();


            Category? EditCategory = _categoryRepo.Get(u=>u.ID==Id); 

            if (EditCategory == null)             
                return NotFound();
            

            return View(EditCategory);
        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {

            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "Name cannot exactly match display order");
            }
            if (ModelState.IsValid)
            {                
                    _categoryRepo.Update(obj);
                    _categoryRepo.Save();
                    TempData["success"] = "Category Updated Successfully";
            }

            return RedirectToAction("Index");
        }

        #endregion


        #region Delete
        public IActionResult Delete(int Id)
        {

            if (Id <= 0)
                return NotFound();


            Category? DeletCategory = _categoryRepo.Get(u => u.ID == Id);

            if (DeletCategory == null)
                return NotFound();


            return View(DeletCategory);
        }

        [HttpPost,ActionName("Delete")]
        public IActionResult DeletePost(int Id)
        {
            Category? DeletCategory = _categoryRepo.Get(u => u.ID == Id);
            if (DeletCategory != null)
            {
                _categoryRepo.Remove(DeletCategory);
                _categoryRepo.Save();
                TempData["success"] = "Category Deleted Successfully";
            }

            return RedirectToAction("Index");
        }

        #endregion
    }
}
