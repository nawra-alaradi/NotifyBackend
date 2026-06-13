using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace NotifyBackend.Models
{
    [Table("Media")]
    public class Media
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }
        [ForeignKey(nameof(Notes))]
        [Column("NoteID")]
        public int NoteID { get; set; }
        [JsonIgnore]

        public Notes Notes { get; set; }
        [Column("ObjectKey")]
        public string ObjectKey { get; set; }
        [Column("Type")]
        public string Type { get; set; }
        [Column("FileUrl")]
        public string FileUrl { get; set; }
        [Column("Summary")]
        public string? Summary { get; set; }
        [Column("CreatedOn")]
        public DateTime CreatedOn { get; set; }
        [Column("LastModified")]
        public DateTime LastModified { get; set; }
    }
}
