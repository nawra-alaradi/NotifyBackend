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

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }
        [Column("LastModified")]
        public DateTime LastModified { get; set; }
        [Column("CognitoSub")]

        public string CognitoSub { get; set; } = string.Empty; // ← add this

    }
}
