using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sp_project_guide_api.Models;
using System.Text.RegularExpressions;

namespace sp_project_guide_api.Service
{
    /*
    public class BookService : IBookService
    {
        private readonly BookSystemContext _context;
        private IConfiguration _config;

        public BookService(BookSystemContext context) {
            _context = context;
        }
        private static string SanitiseInput(string i)
        {
            //regex pattern to catch any letters not any alphanumeric value or a space
            Regex r = new Regex("[^a-zA-Z0-9 ]");
            //replace the value if found.
            return r.Replace(i, "");
        }

        public async Task<ActionResult<DTOBook>> GetBooks([FromQuery] int? page, [FromQuery] int? pageSize, [FromQuery] string? sort)
        {
            List<Book> lBooks = await _context.Books.ToListAsync();
            DTOBook dtoBook = new DTOBook();

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
                return dtoBook;

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
    }
    */
}
