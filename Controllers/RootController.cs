using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sp_project_guide_api.Models;

namespace sp_project_guide_api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class RootController : ControllerBase
    {

        private readonly BookSystemContext _context;
        private readonly IAuthorizationService _authorizationService;

        public RootController(BookSystemContext context, IAuthorizationService authorizationService)
        {
            _context = context;
            _authorizationService = authorizationService;
        }
        
        [HttpGet]
        public async Task<RootResponse> RootCall()
        {
            Link identificationLink = new Link($"/api/Identity/", "JWT", "GET", 4);

            // This checks if the user meets a specific authorization requirement. If they arent authorised, they have to prove their identity to the system and get a JWT
            if (User.Identity.IsAuthenticated){
                //root discovery of all the resources following the best standards set in the BPG Guide for
                //Hypermedia links.
                List<Link> bookLinks = new List<Link> {
                new Link($"/api/Books/", "books", "GET",0),
                new Link($"/api/Books/bookId", "books", "GET", 0),
                new Link($"/api/Books/bookId", "books", "PUT", 1),
                new Link($"/api/Books/bookId", "books", "DELETE", 0),
                };
                //I inlcude one for each controller, with their URIs, relation and what stuff needs to go into it.
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

                List<Link> ordersLinks = new List<Link> {
                new Link($"/api/Orders/", "orders", "GET",0),
                new Link($"/api/Orders/orderId", "orders", "GET", 0),
                new Link($"/api/Orders/orderId", "orders", "PUT", 1),
                new Link($"/api/Orders/orderId", "orders", "DELETE", 0),
                };

                //we provide the link for the identification so that an auth user

                return new RootResponse
                {
                    Books = bookLinks,
                    Members = memberLinks,
                    BooksV2 = booksV2Links,
                    Orders = ordersLinks,
                };

            }

            return new RootResponse
            {

                Identification = identificationLink

            };


        }

        public class RootResponse
        {
            public List<Link>? Books { get; set; }
            public List<Link>? Members { get; set; }

            public List<Link>? BooksV2 { get; set; }

            public List<Link>? Orders { get; set; }

            public Link? Identification { get; set; }

        }
    }
}
