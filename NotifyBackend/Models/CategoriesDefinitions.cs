using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotifyBackend.Models
{
    [Table("CategoriesDefinitions")]
    public class CategoriesDefinitions
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }
        [Column("Title")]

        public string Title { get; set; }
    }
}
