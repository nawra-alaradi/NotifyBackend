using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NotifyBackend.Models;
using NotifyBackend.Utils;

namespace NotifyBackend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {

        private readonly DatabaseContext _db;

        private readonly ILogger<NotesController> _logger;

        public CategoriesController(ILogger<NotesController> logger, DatabaseContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        public ApiResponse Get()
        {
            var userID = 1;
            bool success = false;
            string message = "";
            var categories = new List<CategoriesDefinitions>();
            try
            {
                categories = _db.CategoriesDefinitions.ToList();

                success = true;
                message = "Successfully Retreived the categories";

            }
            catch (Exception ex)
            {
                success = false;
                message = "An error occurred while retrieving categories: " + ex.Message;
            }

            return new ApiResponse { Success = success, Message = message, Data = categories, DataCount = categories == null ? 0 : categories.Count };

        }


        [HttpPost("EditCategory")]
        public ApiResponse Edit([FromBody] CategoriesDefinitions category)
        {
            var userID = 1;
            bool success = false;
            string message = "";
            dynamic? newCategory = null;
            try
            {
                if (category.ID == 0)
                {
                    newCategory = new CategoriesDefinitions
                    {
                        
                        Title = category.Title,
                       
                    };
                    ModelState.Clear();
                    _db.CategoriesDefinitions.Add(newCategory);
                    _db.SaveChanges();
                    success = true;
                    message = "Category Created Successfully";
                }
                else
                {
                    newCategory = _db.CategoriesDefinitions.FirstOrDefault(n => n.ID == category.ID);
                    if (newCategory == null)
                    {
                        success = false;
                        message = "Failed to Update catgeory. The category does not exist";
                        //return NotFound("Note not found.");
                    }
                    else
                    {
                        newCategory.Title = category.Title;
                        
                        ModelState.Clear();
                        _db.CategoriesDefinitions.Update(newCategory);
                        _db.SaveChanges();

                        success = true;
                        message = "Category Updated Successfully";
                    }

                }

            }
            catch (Exception ex)
            {
                success = false;
                message = "An error occurred while trying to update /create catgeory: " + ex.Message;
            }

            return new ApiResponse { Success = success, Message = message, Data = newCategory, DataCount = (newCategory == null) ? 0 : 1 };
        }
    }
}
