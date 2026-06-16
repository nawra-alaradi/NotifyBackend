using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using NotifyBackend.Models;
using NotifyBackend.Utils;
using System.Security.Claims;

namespace NotifyBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class NotesController : ControllerBase
    {
        private readonly DatabaseContext _db;

        private readonly ILogger<NotesController> _logger;

        public NotesController(ILogger<NotesController> logger, DatabaseContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        public ActionResult<ApiResponse> Get()
        {
            var notes = new List<Notes>();
            if (User.Identity == null || !User.Identity.IsAuthenticated) 
                return Unauthorized(new ApiResponse {
                    Success = false,
                    Message = "User is not authenticated",
                    Data = null, DataCount = 0 });

            Users? user = User.Identity.GetNotifyUser(_db);
            if (user == null) return Unauthorized(new ApiResponse{
                Success = false,
                Message = "Authenticated user record not found",
                Data = null,DataCount = 0
            });
            notes = _db.Notes.Where(n => n.UserID == user.ID).ToList();
            if ( notes.Count == 0)
            {
                return NotFound(new ApiResponse { Success = false, Message = "No notes found for the user", Data = null, DataCount = 0 });
            }

            return Ok(new ApiResponse { Success = true, Message = "Successfully retrieved user notes", Data = notes, DataCount = notes.Count });
        }


        [HttpGet("{id}")]
        public ActionResult<ApiResponse> Get(int id)
        {
            dynamic? note = null;
            if (User.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized(new ApiResponse
                {
                    Success = false,
                    Message = "User is not authenticated",
                    Data = null,
                    DataCount = 0
                });

            Users? user = User.Identity.GetNotifyUser(_db);
            if (user == null) return Unauthorized(new ApiResponse
            {
                Success = false,
                Message = "Authenticated user record not found",
                Data = null,
                DataCount = 0
            });
            note = _db.Notes.Where(n => n.UserID == user.ID && n.ID == id).FirstOrDefault();
            if (note == null)
            {
                return NotFound(new ApiResponse { Success = false, Message = "Note not found for the user", Data = null, DataCount = 0 });
            }

            return Ok(new ApiResponse { Success = true, Message = "Successfully retrieved user note", Data = note, DataCount = 1 });
        }

        //method to edit or create a new note, if the note has an ID then it will be edited otherwise a new note will be created
        [HttpPost("EditNote")]
        public ActionResult<ApiResponse> Edit([FromBody] Notes note)
        {
            bool success = false;
            string message = ""; 
            dynamic? newNote = null;
            if (User.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized(new ApiResponse
                {
                    Success = false,
                    Message = "User is not authenticated",
                    Data = null,
                    DataCount = 0
                });

            Users? user = User.Identity.GetNotifyUser(_db);
            if (user == null) return Unauthorized(new ApiResponse
            {
                Success = false,
                Message = "Authenticated user record not found",
                Data = null,
                DataCount = 0
            });

            if (note.ID == 0)
            {
                        newNote = new Notes
                        {
                            UserID = note.UserID,
                            Title = note.Title,
                            NoteContent = note.NoteContent,
                            Summary = note.Summary,
                            HasMedia = note.HasMedia,
                            CreatedAt = DateTime.UtcNow.AddHours(3),
                            LastModified = DateTime.UtcNow.AddHours(3)
                        };
                        ModelState.Clear();
                        _db.Notes.Add(newNote);
                        _db.SaveChanges();
                        success = true;
                        message = "Note Created Successfully";
             }
             else
             {
                        newNote = _db.Notes.FirstOrDefault(n => n.ID == note.ID);
                        if (newNote == null)
                        {
                            return NotFound(new ApiResponse { Success = false, Message = "Note not found for the user", Data = null, DataCount = 0 });

                }
                else
                        {
                            newNote.Title = note.Title;
                            newNote.NoteContent = note.NoteContent;
                            newNote.Summary = note.Summary;
                            newNote.HasMedia = note.HasMedia;
                            newNote.LastModified = DateTime.Now;
                            ModelState.Clear();
                            _db.Notes.Update(newNote);
                            _db.SaveChanges();

                            success = true;
                            message = "Note Updated Successfully";
                        }

                    }

            

            return Ok(new ApiResponse { Success = success, Message = message, Data = newNote, DataCount = (newNote == null) ? 0 : 1 } );
        }

        [HttpDelete("DeleteNote{id}")]
        public ActionResult<ApiResponse> Delete(int id)
        {

            dynamic? targetNote = null;
            if (User.Identity == null || !User.Identity.IsAuthenticated)
                return Unauthorized(new ApiResponse
                {
                    Success = false,
                    Message = "User is not authenticated",
                    Data = null,
                    DataCount = 0
                });

            Users? user = User.Identity.GetNotifyUser(_db);
            if (user == null) return Unauthorized(new ApiResponse
            {
                Success = false,
                Message = "Authenticated user record not found",
                Data = null,
                DataCount = 0
            });


                    targetNote = _db.Notes.FirstOrDefault(n => n.ID == id && n.UserID == user.ID);
            if (targetNote == null)
            {
                return NotFound(new ApiResponse { Success = false, Message = "Note not found for the user", Data = null, DataCount = 0 });
            }

                        ModelState.Clear();
                        _db.Notes.Remove(targetNote);
                        _db.SaveChanges();
                
                      
            return Ok(new ApiResponse { Success = true, Message = "Note Deleted Successfully", Data = targetNote, DataCount = (targetNote == null) ? 0 : 1 });
        }

        // For a single public endpoint:
        [AllowAnonymous]
        [HttpGet("health")]
        public ActionResult<ApiResponse> HealthCheck() => Ok(new ApiResponse { Success = true, Message = "Health check passed", Data = null, DataCount = 0 });
    }
}
