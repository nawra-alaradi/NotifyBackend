using Microsoft.AspNetCore.Mvc;
using NotifyBackend.Models;
using NotifyBackend.Utils;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NotifyBackend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly DatabaseContext _db;

        private readonly ILogger<NotesController> _logger;

        public UsersController(ILogger<NotesController> logger, DatabaseContext db)
        {
            _logger = logger;
            _db = db;
        }
        // GET: api/<UsersController>
        [HttpGet]
        public ApiResponse Get()
        {
            var userID = 1;
            bool success = false;
            string message = "";
            dynamic? data = null;
            int dataCount = 0;

            try
            {
                data = _db.Users.Where(u => u.ID == userID).FirstOrDefault();
                if(data ==null)
                {
                    message = "User not found";
                }
                else
                {
                    success = true;
                    message = "User retrieved successfully";
                }
            }
            catch (Exception ex) {
                message = $"Error retrieving user: {ex.Message}";
            }

            return new ApiResponse
            {
                Success = success,
                Message = message,
                DataCount = data != null ? 1 : 0,
                Data = data
            };
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public ApiResponse Get(int id)
        {
            var userID = 1;
            bool success = false;
            string message = "";
            dynamic? data = null;
            int dataCount = 0;

            try
            {
                data = _db.Users.Where(u => u.ID == id).FirstOrDefault();
                if (data == null)
                {
                    message = "User not found";
                }
                else
                {
                    success = true;
                    message = "User retrieved successfully";
                }
            }
            catch (Exception ex)
            {
                message = $"Error retrieving user: {ex.Message}";
            }

            return new ApiResponse
            {
                Success = success,
                Message = message,
                DataCount = data != null ? 1 : 0,
                Data = data
            };
        }

        // POST api/<UsersController>
        [HttpPost("EditUser")]
        public ApiResponse Post([FromBody] Users user)
        {
            var userID = 1;
            bool success = false;
            string message = "";
            dynamic? data = null;

            try
            {
                if (user.ID == 0)
                {
                    data = new Users
                    {
                       ID= user.ID,
                       Name = user.Name,
                        Email = user.Email,
                        Password = user.Password,
                        ThemeSetting = user.ThemeSetting,
                        CreatedOn = DateTime.UtcNow.AddHours(3),
                        LastModified = DateTime.UtcNow.AddHours(3)
                    };
                    _db.Users.Add(data);
                    _db.SaveChanges();
                    ModelState.Clear();

                    success = true;
                    message = "User Created Successfully";
                }
                else
                {
                    data = _db.Users.FirstOrDefault(n => n.ID == user.ID);
                    if (data == null)
                    {
                        success = false;
                        message = "Failed to Update user. The user does not exist";
                        //return NotFound("Note not found.");
                    }
                    else
                    {
                        data.Title = data.Title;
                        data.Content = data.Content;
                        data.Summary = data.Summary;
                        data.HasMedia = data.HasMedia;
                        data.LastModified = DateTime.Now;
                        _db.Notes.Update(data);
                        _db.SaveChanges();
                        ModelState.Clear();

                        success = true;
                        message = "Note Updated Successfully";
                    }

                }

            }
            catch (Exception ex)
            {
                success = false;
                message = "An error occurred while trying to update /create note: " + ex.Message;
            }

            return new ApiResponse { Success = success, Message = message, Data = data, DataCount = (data == null) ? 0 : 1 };
        }


        //// PUT api/<UsersController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] Users user)
        //{
        //}

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public ApiResponse Delete(int id)
        {
            
                int userID = 1;
                bool success = false;
                string message = "";
                dynamic? targetUser = null;
                try
                {

                    targetUser = _db.Users.FirstOrDefault(n => n.ID == id);

                    if (targetUser != null)
                    {
                        success = true;
                        message = "User Deleted Successfully";
                        ModelState.Clear();
                        _db.Users.Remove(targetUser);
                        _db.SaveChanges();
                    }
                    else
                    {
                        success = false;
                        message = "User Not Found. Can't Delete the User";
                    }
                }
                catch (Exception ex)
                {
                    success = false;
                    message = "An error occurred while trying to delete user: " + ex.Message;

                }
                return new ApiResponse { Success = success, Message = message, Data = targetUser, DataCount = (targetUser == null) ? 0 : 1 };
            }
        
    }
}
