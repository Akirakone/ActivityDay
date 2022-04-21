using System;
using System.ComponentModel.DataAnnotations;

namespace Exam1.Models
{
    public class RSVP
    {
        [Key]
        public int RSVPId { get; set; }
        public int UserId { get; set; }
        public int FundayId { get; set; }
        public User User { get; set; }
        public Funday Funday { get; set; }
    }
}