using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sp_project_guide_api.Models;
using static System.Reflection.Metadata.BlobBuilder;

namespace sp_project_guide_api.Controllers
{
    //For authorisation with JWT, we would also have an API that can create the tokens. That is not what we are doing here.  It would be good to have, and something to definitely keep in mind, but for this project
    //it is enough to implement the recieval of one and make it up for examples sake, rather than actually create one etc.
    //User should be able to make a /token claim and attacha  userid, email, customClaim with admin false etc.
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookSystemContext _context;
        private IConfiguration _config;

        //[FromBody] for objects in the body of the request
        //[FromRoute] to get variables from the root. Will need this for the other func to filter the queries.

        public BooksController(BookSystemContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
            PopulateDatabase(_context);
        }

        private void PopulateDatabase(BookSystemContext dbContext)
        {
            // Add dummy data to the database
            dbContext.Books.Add(new Book { Title = "Alexander", Author = "Jones Alexander", ISBN = "ISBN-232354245234", Id = 9 });
            dbContext.Books.Add(new Book { Title = "The Catcher in the Rye", Author = "J.D. Salinger", ISBN = "ISBN-1234567890", Id = 10 });
            dbContext.Books.Add(new Book { Title = "To Kill a Mockingbird", Author = "Harper Lee", ISBN = "ISBN-0987654321", Id = 11 });
            dbContext.Books.Add(new Book { Title = "1984", Author = "George Orwell", ISBN = "ISBN-9876543210", Id = 12 });
            dbContext.Books.Add(new Book { Title = "Pride and Prejudice", Author = "Jane Austen", ISBN = "ISBN-2468135790", Id = 13 });
            dbContext.Books.Add(new Book { Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", ISBN = "ISBN-1357924680", Id = 14 });
            dbContext.Books.Add(new Book { Title = "Moby Dick", Author = "Herman Melville", ISBN = "ISBN-9876543210", Id = 15 });
            dbContext.Books.Add(new Book { Title = "War and Peace", Author = "Leo Tolstoy", ISBN = "ISBN-2468135790", Id = 16 });
            dbContext.Books.Add(new Book { Title = "Hamlet", Author = "William Shakespeare", ISBN = "ISBN-1234567890", Id = 17 });
            dbContext.Books.Add(new Book { Title = "The Lord of the Rings", Author = "J.R.R. Tolkien", ISBN = "ISBN-1357924680", Id = 18 });

            
            dbContext.SaveChangesAsync();

        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<DTOBook>> GetBooks([FromQuery] int? page, [FromQuery] int? pageSize, [FromQuery] string? sort)
        {
            List<Book> lBooks = await _context.Books.ToListAsync();
            DTOBook dtoBook =  new DTOBook();

            foreach (Book book in lBooks)
            {
                //foreach book add the hypermedia links related to the resource
                book.Links = new List<Link>
                {
                    new Link($"/api/Books/{book.Id}", "self", "GET",0),
                    new Link($"/api/Books/{book.Id}", "self", "PUT",1),
                    new Link($"/api/Books/{book.Id}", "self", "DELETE",0)
                };

            }

            //Pagination Filter
            if (page.HasValue && pageSize.HasValue)
            {
                //get the total count, page size, total page, nav links to the next page
                var totalCount = lBooks.Count();
                //avoid collision with ? by using .Value
                var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize.Value);
                var booksPP = lBooks.Skip((int)((page - 1) * pageSize)).Take(pageSize.Value).ToList();
                dtoBook.Books = booksPP;
                //now we add the next page url, would need some sort of catch incase theres no pages left, no point showing them.
                dtoBook.Links = new List<Link>
                {
                    new Link($"/api/Books?page={page+1}&pageSize={pageSize}","self","GET",0)
                };
                return Ok(dtoBook);
                
            }


            //check sort if page is not set
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort.ToLower())
                {
                    case "id":
                        lBooks = lBooks.OrderBy(s => s.Id).ToList();

                        break;
                    case "title":
                        lBooks = lBooks.OrderBy(s => s.Title).ToList();

                        break;
                    default:
                        return BadRequest($"Invalid field: {sort}");
                }
            }

            dtoBook.Books = lBooks;
            return dtoBook;
        }

        // GET: api/Books/5
        [HttpGet("{id}")]

        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            book.Links = new List<Link>
                {
                    new Link($"/api/Books/{book.Id}", "self", "PUT",1),
                    new Link($"/api/Books/{book.Id}", "self", "DELETE",0),
                };

            return book;
        }

        // PUT: api/Books/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }

            try
            {
                //check the inputs on the book.
                Book newBook = new Book();
                if (book.PubDate.GetType() == typeof(DateTime))
                {
                    newBook.PubDate = book.PubDate;
                }
                //sanitise the inputs, stripping any escape characters
                newBook.Title = SanitiseInput(book.Title);
                newBook.Author = SanitiseInput(book.Author);
                newBook.ISBN = SanitiseInput(book.ISBN);

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        [HttpPost]
        public async Task<ActionResult<Book>> PostBook([FromBody]Book book)
        {
            //Check Book Contents BEFORE we add book straight to db.
            //we wanna check that the book Details are actually valid,
            if (book != null)
            {
                //check the inputs
                Book newBook = new Book();
                if(book.PubDate.GetType() == typeof(DateTime))
                {
                    newBook.PubDate = book.PubDate;
                }

                newBook.Title = SanitiseInput(book.Title);
                newBook.Author = SanitiseInput(book.Author);
                newBook.ISBN = SanitiseInput(book.ISBN);
                //we've checked the data, now we can add it to the database

                _context.Books.Add(newBook);
                await _context.SaveChangesAsync();
                //add links to book object after saved for return
                book.Links = new List<Link>
                {
                    new Link($"/api/Books/{book.Id}", "self", "GET",0),
                    new Link($"/api/Books/{book.Id}", "self", "PUT",1),
                    new Link($"/api/Books/{book.Id}", "self", "DELETE",0)
                };

                return CreatedAtAction("GetBook", new { id = book.Id }, book);
            }

            return BadRequest();

        }

        public static string SanitiseInput(string i)
        {
            //Sanitising our Inputs - We put the string "i" into this function, and we have a set of accepted characters
            Regex r = new Regex("[^a-zA-Z0-9]");
            // Replace any characters not matching the pattern with an empty string
            return r.Replace(i, "");
        }

        // DELETE: api/Books/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}
