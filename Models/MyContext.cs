using Microsoft.EntityFrameworkCore;
namespace Exam1.Models
{ 
    // the MyContext class representing a session with our MySQL 
    // database allowing us to query for or save data
    public class MyContext : DbContext 
    { 
        public MyContext(DbContextOptions options) : base(options) { }
        
        public DbSet<User> Users { get; set; }
        public DbSet<Funday> Fundays { get; set; }
        public DbSet<RSVP> RSVPs { get; set; }
    }
}