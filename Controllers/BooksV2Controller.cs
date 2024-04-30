using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using sp_project_guide_api.Models;

namespace sp_project_guide_api.Controllers
{
    //controller level auth
    [Authorize]
    //URI Versioning by explicit mention of API version in resource indicator.
    [Route("api/v2/[controller]")]
    [ApiController]
    public class BooksV2Controller : Controller
    {
        private readonly BookSystemContext _context;
        public BooksV2Controller(BookSystemContext context)
        {
            _context = context;
        }

        //Clients can set the max results they want returned from the Get Books.
        [HttpGet]
        public async Task<ActionResult<BookV2>> GetBooks([FromHeader] int? maxResults, [FromHeader] int? apiVersion)
        {
            //To demonstrate the header versioning style, where the API version is stated within the request, this method will reject any request that does not contain the API version.
            if (apiVersion.HasValue && apiVersion == 2)
            {
                //we allow this to be called only when the API version is specified in the URI and the header.
                //List<Book> lBooks = await _context.Books.ToListAsync();
                List<BookV2> books = await _context.BooksV2.ToListAsync();

                if (maxResults.HasValue && maxResults.Value < 10)
                {
                    //return a list of books that is the value sent by user
                    return Ok(books.Take(maxResults.Value));
                }
                else
                {
                    return Ok(books);
                }
            }
            //otherwise, return bad request if api version is not set or invalid
            return BadRequest($"Invalid API Version");
        }

        //Duration is set to 100 seconds, and the response can be cached for the GetBook by the Client.
        [HttpGet("{id}")]
        [ResponseCache(Duration = 100, Location=ResponseCacheLocation.Client)]
        public async Task<ActionResult<BookV2>> GetBook(int id)
        {
            
            var book = await _context.BooksV2.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            book.Links = new List<Link>
            {
                    new Link($"/api/v2/BooksV2/{book.Id}", "self", "PUT",1),
                    new Link($"/api/v2/BooksV2/{book.Id}", "self", "DELETE",0),
            };

            return book;
        }

        //Demonstrating a Custom Method, which creates an Order based off a specific JSON body named Oreq.
        //We dont use a PATCH for a custom method. Separate the Custom verb from the Resource via :
        [HttpPost(":createBookOrder")]
        public async Task<ActionResult<Order>> CreateBookOrder([FromBody]OrderRequest oreq)
        {
            //check if object is not null 
            if (oreq != null) {
                //attempt to find the data they want to create the order with
                var member = await _context.Members.FindAsync(oreq.MemberId);
                var book = await _context.Books.FindAsync(oreq.BookId);
                if(book != null && member != null)
                {
                    //if both are not null, then we can make an order
                    var order = new Order();
                    //set the values for the new Order

                    order.MemberID = oreq.MemberId;
                    order.BookId = oreq.BookId;
                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();
                    //return order object with hypermedia links
                    order.Links = new List<Link>
                    {
                        new Link($"/api/Orders/{order.Id}", "self", "GET", 0),
                        new Link($"/api/Orders/{order.Id}", "self", "DELETE",0),
                        new Link($"/api/Books/{order.BookId}", "book", "GET",0),
                        new Link($"/api/Books/{order.MemberID}", "member", "GET",0),

                    };
                    return Ok(order);
                }else { return NotFound(); }
            }
            //if it is null, return bad request
            return BadRequest(ModelState);
        }
    }
}
