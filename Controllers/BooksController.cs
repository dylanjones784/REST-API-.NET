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
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookSystemContext _context;
        private IConfiguration _config;

        public BooksController(BookSystemContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

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


        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }
            //return not found if the Book ID doesnt exist in the context
            var existingBook = await _context.Books.FindAsync(id);
            if (existingBook == null)
            {
                return NotFound();
            }

            if (book.PubDate.GetType() != typeof(DateTime))
            {
                //return bad request if
                return BadRequest("Publish Date is not a valid value");
            }
            //sanitise the inputs, stripping any escape characters
            book.Title = SanitiseInput(book.Title);
            book.Author = SanitiseInput(book.Author);
            book.ISBN = SanitiseInput(book.ISBN);
            _context.Entry(existingBook).CurrentValues.SetValues(book);
            //_context.Books.Update(book);

            try
            {
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

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook([FromBody]Book book)
        {
            if (book != null)
            {
                if (_context.Books.Any(b => b.Id == book.Id))
                {
                    return Conflict("A book with the same ID already exists.");
                }

                //check the inputs
                Book newBook = new Book();
                if(book.PubDate.GetType() != typeof(DateTime))
                {
                    return BadRequest("Publish Date is not a valid value");
                }

                // clean entry and make new book for entry
                Book nBook = new Book
                {
                    PubDate = book.PubDate,
                    Title = SanitiseInput(book.Title),
                    Author = SanitiseInput(book.Author),
                    ISBN = SanitiseInput(book.ISBN)
                };

                //we've checked the data, now we can add it to the database

                _context.Books.Add(nBook);
                await _context.SaveChangesAsync();
                //add links to book object after saved for return
                nBook.Links = new List<Link>
                {
                    new Link($"/api/Books/{book.Id}", "self", "GET",0),
                    new Link($"/api/Books/{book.Id}", "self", "PUT",1),
                    new Link($"/api/Books/{book.Id}", "self", "DELETE",0)
                };

                return CreatedAtAction("GetBook", new { id = nBook.Id }, nBook);
            }
            return BadRequest();

        }

        private static string SanitiseInput(string i)
        {
            //regex pattern to catch any letters not any alphanumeric value or a space
            Regex r = new Regex("[^a-zA-Z0-9 ]");
            //replace the value if found.
            return r.Replace(i, "");
        }

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
