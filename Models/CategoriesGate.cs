using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace NotifyBackend.Models
{
    [Table("CategoriesGate")]
    public class CategoriesGate
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }

        [ForeignKey(nameof(CategoriesDefinitions))]
        [Column("CategoryID")]
        public int CategoryID { get; set; }

        [JsonIgnore]
        public CategoriesDefinitions CategoriesDefinitions { get; set; }

        [ForeignKey(nameof(Notes))]
        [Column("NoteID")]

        public int NoteID { get; set; }
       
        [JsonIgnore]
        public Notes Notes { get; set; }



    }
}
