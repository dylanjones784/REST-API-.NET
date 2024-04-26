namespace sp_project_guide_api.Models
{
    public class DTOBook
    {
        public ICollection<Book> Books { get; set; }
        public ICollection<Link> Links { get; set; }
    }
}
