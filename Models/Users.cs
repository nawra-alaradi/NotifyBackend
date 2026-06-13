using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotifyBackend.Models
{
    [Table("Users")] 
    public class Users
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }
        [Column("Name")]
        public string Name { get; set; }
        [Column("Email")]
        public string Email { get; set; }

        //public string Password { get; set; }

        [Column("CreatedOn")]
        public DateTime CreatedOn { get; set; }
        [Column("LastModified")]
        public DateTime LastModified { get; set; }

    }
}
