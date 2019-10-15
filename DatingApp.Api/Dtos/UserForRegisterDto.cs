using System.ComponentModel.DataAnnotations;

namespace DatingApp.Api.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        public string username{get;set;}
        [Required]
        [StringLength(8,MinimumLength=4,ErrorMessage="You Must Specify password between 4 to 8 characters")]
        public string password{get;set;}
    }
}