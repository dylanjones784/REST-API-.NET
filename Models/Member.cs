using System.ComponentModel.DataAnnotations.Schema;

namespace sp_project_guide_api.Models
{
    public class Member
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public DateTime DateRegistered { get; set; }

        [NotMapped]
        public ICollection<Link>? Links { get; set; }

        public Member() { }
    }
   
}
