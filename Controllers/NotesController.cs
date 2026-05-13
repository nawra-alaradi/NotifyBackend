using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using NotifyBackend.Models;
using NotifyBackend.Utils;

namespace NotifyBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
        public ApiResponse Get()
        {
            var userID = 1;
            bool success = false;
            string message = "";
            var notes = new List<Notes>();
            try
            {
                notes = _db.Notes.Where(n => n.UserID == userID).ToList();
                if (notes == null || notes.Count == 0)
                {
                    message = "Not Found";
                    //return NotFound("No notes found for the user.");
                }

                success = true;
                message = "Found User Notes";

            }
            catch (Exception ex)
            {
                success = false;
                message = "An error occurred while retrieving notes: " + ex.Message;
            }

            return new ApiResponse { Success = success, Message = message, Data = notes, DataCount = notes == null ? 0 : notes.Count };

        }



        [HttpGet("{id}")]
        public ApiResponse Get(int id)
        {
            var userID = 1;
            bool success = false;
            string message = "";
            dynamic? note = null;
            try
            {
                note = _db.Notes.FirstOrDefault(n => n.UserID == userID && n.ID==id);
                if (note == null)
                {
                    message = "Not Found";
                    //return NotFound("No notes found for the user.");
                }

                success = true;
                message = "Found User Note";

            }
            catch (Exception ex)
            {
                success = false;
                message = "An error occurred while retrieving the note: " + ex.Message;
            }

            return new ApiResponse { Success = success, Message = message, Data = note, DataCount = note == null ? 0 : 1};

        }

        //method to edit or create a new note, if the note has an ID then it will be edited otherwise a new note will be created
        [HttpPost("EditNote")]
        public ApiResponse Edit([FromBody] Notes note)
        {
            var userID = 1;
            bool success = false;
            string message = "";
            dynamic? newNote=null;
            try
            {
                if (note.ID == 0)
                {
                    newNote = new Notes
                    {
                        UserID = note.UserID,
                        Title = note.Title,
                        Content = note.Content,
                        Summary = note.Summary,
                        HasMedia = note.HasMedia,
                        CreatedOn = DateTime.UtcNow.AddHours(3),
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
                        success = false;
                        message = "Failed to Update Note. The note does not exist";
                        //return NotFound("Note not found.");
                    }
                    else {
                        newNote.Title = note.Title;
                        newNote.Content = note.Content;
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

            }
            catch (Exception ex)
            {
                success = false;
                message = "An error occurred while trying to update /create note: " + ex.Message;
            }

            return new ApiResponse { Success = success, Message = message, Data = newNote, DataCount = (newNote == null) ? 0 : 1 };
        }

        [HttpDelete("DeleteNote{id}")]
        public ApiResponse Delete(int id)
        {
            int userID = 1;
            bool success = false;
            string message = "";
            dynamic? targetNote=null;
            try
            {

                targetNote = _db.Notes.FirstOrDefault(n => n.ID == id && n.UserID == userID);

                if (targetNote != null)
                {
                    success = true;
                    message = "Note Deleted Successfully";
                    ModelState.Clear();
                    _db.Notes.Remove(targetNote);
                    _db.SaveChanges();
                }
                else {
                    success = false;
                    message = "Note Not Found. Can't Delete the Note";
                }
            } catch (Exception ex)
            {
                success = false;
                message = "An error occurred while trying to delete note: " + ex.Message;

        }
            return new ApiResponse { Success = success, Message = message, Data = targetNote, DataCount = (targetNote == null) ? 0 : 1 };
        }

        
    }
}
