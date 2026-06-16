using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotifyBackend.Models;
using NotifyBackend.Utils;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NotifyBackend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]  // all endpoints require a valid Cognito JWT

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

            Users? user = User.Identity.GetNotifyUser(_db);
            if (user == null) return NotFound(new ApiResponse
            {
                Success = false,
                Message = "Authenticated user record not found",
                Data = null,
                DataCount = 0
            });


            return Ok(new ApiResponse { Success = true, Message = "User Record Found Successfully", Data = user, DataCount = 1 });
        }


        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public ActionResult<ApiResponse> Get(int id)
        {


            if (User.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized(new ApiResponse
                {
                    Success = false,
                    Message = "User is not authenticated",
                    Data = null,
                    DataCount = 0
                });

            Users? user = _db.Users.Where(u => u.ID == id).FirstOrDefault();
            if (user == null) return NotFound(new ApiResponse
            {
                Success = false,
                Message = "User Record Not Found",
                Data = null,
                DataCount = 0
            });


            return Ok(new ApiResponse
            {
                Success = true,
                Message = "User Record Found Successfully",
                DataCount = 1,
                Data = user
            });
        }

        // POST api/<UsersController>
        [HttpPost("EditUser")]
        public ActionResult<ApiResponse> Post([FromBody] Users user)
        {
            bool success = false;
            string message = "";
            dynamic? data = null;

            if (User.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized(new ApiResponse
                {
                    Success = false,
                    Message = "User is not authenticated",
                    Data = null,
                    DataCount = 0
                });


            if (user.ID == 0)
            {
                //must create user in cognito as well
                data = new Users
                {
                    ID = user.ID,
                    Name = user.Name,
                    Email = user.Email,
                    //Password = user.Password,
                    CreatedAt = DateTime.UtcNow.AddHours(3),
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
                    return NotFound(new ApiResponse { Success = false, Message = "Failed to Update user. The user does not exist", Data = null, DataCount = 0 });
                }

                data.Title = data.Title;
                data.Content = data.Content;
                data.Summary = data.Summary;
                data.HasMedia = data.HasMedia;
                data.LastModified = DateTime.Now;
                _db.Notes.Update(data);
                _db.SaveChanges();
                ModelState.Clear();

                success = true;
                message = "User Record Updated Successfully";


            }

            return Ok(new ApiResponse { Success = success, Message = message, Data = data, DataCount = (data == null) ? 0 : 1 });
        }


        //// PUT api/<UsersController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] Users user)
        //{
        //}

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public ActionResult<ApiResponse> Delete(int id)
        {

     
            dynamic? targetUser = null;
            if (User.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized(new ApiResponse
                {
                    Success = false,
                    Message = "User is not authenticated",
                    Data = null,
                    DataCount = 0
                });
            targetUser = _db.Users.FirstOrDefault(n => n.ID == id);

            if (targetUser == null) return NotFound(new ApiResponse
            {
                Success = false,
                Message = "Target user record not found",
                Data = null,
                DataCount = 0
            });



              
                    ModelState.Clear();
                    _db.Users.Remove(targetUser);
                    _db.SaveChanges();

          
            return Ok(new ApiResponse { Success = true, Message = "User Deleted Successfully", Data = targetUser, DataCount =  1 });
        }



        // Called by Flutter right after login to sync the Cognito user
        // into your SQL Server Users table
        [HttpPost("sync")]
        public async Task<IActionResult> SyncUser()
        {

            // Cognito puts the unique user ID in the "sub" claim
            var cognitoSub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst("sub")?.Value;
            var email = User.FindFirst("email")?.Value;
            var name = User.FindFirst("name")?.Value;

            if (cognitoSub == null)
                return Unauthorized("No sub claim found in token.");

            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.CognitoSub == cognitoSub);

            if (user == null)
            {
                user = new Users
                {
                    CognitoSub = cognitoSub,
                    Email = email ?? string.Empty,
                    Name = name ?? string.Empty,
                    CreatedAt = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow
                };
                _db.Users.Add(user);
                await _db.SaveChangesAsync();
            }

            return Ok(user);
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var cognitoSub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst("sub")?.Value;

            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.CognitoSub.Equals(cognitoSub));

            return user == null ? NotFound("User Not Found") : Ok(user);
        }
    }

}
