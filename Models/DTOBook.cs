using System.ComponentModel.DataAnnotations.Schema;

namespace sp_project_guide_api.Models
{
    public class DTOBook
    {
        public ICollection<Book> Books { get; set; }

        [NotMapped]
        public ICollection<Link> Links { get; set; }
    }
}
