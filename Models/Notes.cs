using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace NotifyBackend.Models
{
    public class Notes
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey(nameof(Users))]
        public int UserID { get; set; }

        [JsonIgnore]
        public Users Users { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public string? Summary { get; set; }

        public bool HasMedia { get; set; }

       
        public DateTime CreatedOn { get; set; }

        public DateTime LastModified { get; set; }
    }
}
