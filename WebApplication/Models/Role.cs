using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using HR_API.Utils;

namespace HR_API.Models
{
    public class Role
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "The Name of the Role")]
        public string Name{ get; set; }

        [Display(Name = "Each role is associate to one or more users")]
        public virtual List<User> Users { get; set; }
    }
}
