using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace NotifyBackend.Models
{
    // If your table name is strictly uppercase or lowercase, specify it here
    [Table("Notes")]
    public class Notes
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }

        [ForeignKey(nameof(Users))]
        [Column("UserID")]
        public int UserID { get; set; }

        [JsonIgnore]
        public Users Users { get; set; }

        [Column("Title")]
        public string Title { get; set; }

        // Force EF Core to use the exact column name from your SQL database
        [Column("NoteContent")]
        public string NoteContent { get; set; }

        [Column("Summary")]
        public string? Summary { get; set; }

        [Column("HasMedia")]
        public bool HasMedia { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Column("LastModified")]
        public DateTime LastModified { get; set; }
    }
}
