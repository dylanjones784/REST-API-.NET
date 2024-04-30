using System.ComponentModel.DataAnnotations.Schema;

namespace sp_project_guide_api.Models
{
    public class BookV2
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public DateTime PubDate { get; set; }

        //new field for V2 is Status.
        public string Status { get; set; }

        [NotMapped]
        public ICollection<Link>? Links { get; set; }

    }
}
