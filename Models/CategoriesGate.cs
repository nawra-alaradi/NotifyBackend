using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace NotifyBackend.Models
{
    public class CategoriesGate
    {
        [Key]
        public int ID { get; set; }

        [ForeignKey(nameof(CategoriesDefinitions))]
        public int CategoryID { get; set; }

        [JsonIgnore]
        public CategoriesDefinitions CategoriesDefinitions { get; set; }

        [ForeignKey(nameof(Notes))]

        public int NoteID { get; set; }
       
        [JsonIgnore]
        public Notes Notes { get; set; }



    }
}
