using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exam1.Models
{
    
    public class Login
    {

        [Required]
        [EmailAddress]
        public string lEmail { get; set;}

        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string lPassword { get; set;}

        
    
    }}