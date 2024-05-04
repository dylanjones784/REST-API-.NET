using System.ComponentModel.DataAnnotations.Schema;

namespace sp_project_guide_api.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public DateTime PubDate { get; set; }

        [NotMapped]
        public ICollection<Link>? Links { get; set; }

    }
}
