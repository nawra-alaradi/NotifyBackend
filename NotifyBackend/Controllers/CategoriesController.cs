using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NotifyBackend.Models;
using NotifyBackend.Utils;

namespace NotifyBackend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
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
        public ActionResult<ApiResponse> Get()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized(new ApiResponse
                {
                    Success = false,
                    Message = "User is not authenticated",
                    Data = null,
                    DataCount = 0
                });

            var categories = _db.CategoriesDefinitions.ToList();
            return Ok(new ApiResponse { Success = true, Message = "Successfully Retreived the categories", Data = categories, DataCount = categories.Count });

        }


        [HttpPost("EditCategory")]
        public ActionResult<ApiResponse> Edit([FromBody] CategoriesDefinitions category)
        {

            if (User.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized(new ApiResponse
                {
                    Success = false,
                    Message = "User is not authenticated",
                    Data = null,
                    DataCount = 0
                });
            bool success = false;
            string message = "";
            dynamic? newCategory = null;
         
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

           
            return Ok(new ApiResponse { Success = success, Message = message, Data = newCategory, DataCount = (newCategory == null) ? 0 : 1 });
        }
    }
}
