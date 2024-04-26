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
    //URI Versioning by explicit mention of API version in resource indicator.
    [Route("api/v2/[controller]")]
    [ApiController]
    public class BooksV2Controller : Controller
    {
        private readonly BookSystemContext _context;
        public BooksV2Controller(BookSystemContext context)
        {
            _context = context;
            PopulateDatabase(_context);
        }

        private void PopulateDatabase(BookSystemContext dbContext)
        {
            // Add dummy data to the database
            dbContext.BooksV2.Add(new BookV2 { Title = "Alexander", Author = "Jones Alexander", ISBN = "ISBN-232354245234", Id = 9, Status = "Available" });
            dbContext.BooksV2.Add(new BookV2 { Title = "The Catcher in the Rye", Author = "J.D. Salinger", ISBN = "ISBN-1234567890", Id = 10, Status = "Available" });
            dbContext.BooksV2.Add(new BookV2 { Title = "To Kill a Mockingbird", Author = "Harper Lee", ISBN = "ISBN-0987654321", Id = 11, Status = "Available" });
            dbContext.BooksV2.Add(new BookV2 { Title = "1984", Author = "George Orwell", ISBN = "ISBN-9876543210", Id = 12, Status = "Available" });
            dbContext.BooksV2.Add(new BookV2 { Title = "Pride and Prejudice", Author = "Jane Austen", ISBN = "ISBN-2468135790", Id = 13, Status = "Available" });
            dbContext.BooksV2.Add(new BookV2 { Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", ISBN = "ISBN-1357924680", Id = 14, Status = "Available" });
            dbContext.BooksV2.Add(new BookV2 { Title = "Moby Dick", Author = "Herman Melville", ISBN = "ISBN-9876543210", Id = 15, Status = "Available" });
            dbContext.BooksV2.Add(new BookV2 { Title = "War and Peace", Author = "Leo Tolstoy", ISBN = "ISBN-2468135790", Id = 16, Status = "Available" });
            dbContext.BooksV2.Add(new BookV2 { Title = "Hamlet", Author = "William Shakespeare", ISBN = "ISBN-1234567890", Id = 17, Status = "Available" });
            dbContext.BooksV2.Add(new BookV2 { Title = "The Lord of the Rings", Author = "J.R.R. Tolkien", ISBN = "ISBN-1357924680", Id = 18, Status = "Available" });
            dbContext.SaveChangesAsync();

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
        [HttpPost("createBookOrder")]
        public async Task<ActionResult<Order>> CreateBookOrder([FromBody]OrderRequest oreq)
        {
            if (oreq != null) {
                if(oreq.BookId > 0 && oreq.MemberId > 0)
                {
                    var order = new Order();
                    //set the values for the new Order
                    order.MemberID = oreq.MemberId;
                    order.BookId = oreq.BookId;
                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();
                    order.Links = new List<Link>
                    {
                        new Link($"/api/Orders/{order.Id}", "self", "GET", 0),
                        new Link($"/api/Orders/{order.Id}", "self", "DELETE",0),
                        new Link($"/api/Books/{order.BookId}", "book", "GET",0),
                        new Link($"/api/Books/{order.MemberID}", "member", "GET",0),

                    };

                    return Ok(order);
                }

            }

            return BadRequest(ModelState);
        }
    }
}
