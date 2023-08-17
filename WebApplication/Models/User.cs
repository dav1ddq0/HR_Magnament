using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HR_API.Models
{
    public class User
    {

        [Key]
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "The name of the User")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "The  lastname of the User")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "The email account of the User")]
        public string Email { get; set; }

        [Display(Name = "The personal address of the User")]
        public string? PersonalAddress { get; set; }

        [Display(Name = "The telephone number of the User")]

        public string? Phone { get; set; }

        [Required]
        [Display(Name = "The date when the user is start to work")]
        public DateTime WorkingStartDate { get; set; }

        [Display(Name = "Each user is associate to one or more roles")]
        public virtual List<Role> Roles { get; set; }

    }
}
