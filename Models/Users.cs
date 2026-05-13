using System.ComponentModel.DataAnnotations;

namespace NotifyBackend.Models
{
    public class Users
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public string Password { get; set; }
        public string ThemeSetting { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime LastModified { get; set; }

    }
}
