using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sp_project_guide_api.Models;

namespace sp_project_guide_api.Controllers
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RootController : ControllerBase
    {

        private readonly BookSystemContext _context;
        public RootController(BookSystemContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<RootResponse> RootCall()
        {
            //i think we will need an extra field to add like content types etc.
            List<Link> bookLinks = new List<Link> {
                new Link($"/api/Books/", "books", "GET",0),
                new Link($"/api/Books/bookId", "books", "GET", 0),
                new Link($"/api/Books/bookId", "books", "PUT", 1),
                new Link($"/api/Books/bookId", "books", "DELETE", 0),
            };

            List<Link> memberLinks = new List<Link> {
                new Link($"/api/Members/", "members", "GET",0),
                new Link($"/api/Members/", "members", "POST", 1),
                new Link($"/api/Members/bookId", "members", "GET", 0),
                new Link($"/api/Members/bookId", "members", "PUT", 1),
                new Link($"/api/Members/bookId", "members", "DELETE", 0)
            };


            List<Link> booksV2Links = new List<Link> {
                new Link($"/api/v2/BooksV2/book", "books", "GET", 0),
                new Link($"/api/v2/BooksV2/book", "books", "GET", 0),
                new Link($"/api/v2/BooksV2/createBookOrder", "books", "POST", 1)
            };


            return new RootResponse
            {
                Books = bookLinks,
                Members = memberLinks,
                BooksV2 = booksV2Links

            };
           
        }

        public class RootResponse
        {
            public List<Link> Books { get; set; }
            public List<Link> Members { get; set; }

            public List<Link> BooksV2 { get; set; }

        }
    }
}
