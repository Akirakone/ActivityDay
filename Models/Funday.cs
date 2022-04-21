using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Exam1.Models
{
    public class Funday
    {
        [Key]
        public int FundayId { get; set; }
        [Required]
        public string Fundayname { get; set; }
        
        [Required(ErrorMessage ="Funday must have a Date/Time.")]
        [DataType(DataType.DateTime)]
        public DateTime Fundaydate { get; set; }
        [Required]
        public string Duration { get; set; }
        [Required(ErrorMessage ="Funday must have a Duration.")]
        public string length {get;set;}
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public List<RSVP> guestlist { get; set; }

        public int UserId { get; set; }
        public User Coordinator { get; set; }
        [Required(ErrorMessage ="Funday must have a description.")]
        public string description {get;set;}

    }
}
