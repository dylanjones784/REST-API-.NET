namespace sp_project_guide_api.Models
{
    public class Member
    {
        //Name, Address, Sign up date
        //Getters and setters? 

        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public DateTime DateRegistered { get; set; }

        public ICollection<Link>? Links { get; set; }
    }
}
