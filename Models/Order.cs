namespace sp_project_guide_api.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int MemberID { get; set; }

        public ICollection<Link>? Links { get; set; }


    }
}
