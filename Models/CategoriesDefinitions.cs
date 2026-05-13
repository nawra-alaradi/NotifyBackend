using System.ComponentModel.DataAnnotations;

namespace NotifyBackend.Models
{
    public class CategoriesDefinitions
    {
        [Key]
        public int ID { get; set; }
        public string Title { get; set; }
    }
}
