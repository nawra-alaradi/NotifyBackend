using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace NotifyBackend.Models
{
    public class Media
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey(nameof(Notes))]
        public int NoteID { get; set; }
        [JsonIgnore]
        public Notes Notes { get; set; }

        public string ObjectKey { get; set; }

        public string Type { get; set; }

        public string FileUrl { get; set; }
        public string? Summary { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime LastModified { get; set; }
    }
}
