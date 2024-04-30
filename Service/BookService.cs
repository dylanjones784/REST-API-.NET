using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sp_project_guide_api.Models;
using System.Net;
using System.Text.RegularExpressions;


namespace sp_project_guide_api.Service
{
    public class BookService : IBookService
    {
        private readonly BookSystemContext _context;

        public BookService(BookSystemContext context)
        {
            _context = context;
        }

        public async Task<Book> GetBook(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return book;
            }

            book.Links = new List<Link>
                {
                    new Link($"/api/Books/{book.Id}", "self", "PUT",1),
                    new Link($"/api/Books/{book.Id}", "self", "DELETE",0),
            };

            return book;
        }
        public async Task<ActionResult<DTOBook>> GetBooks(int? page, int? pageSize, string? sort)
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
                    //default:
                        //handle it somehow
                        //return BadRequest($"Invalid field: {sort}");
                }
            }
            dtoBook.Books = lBooks;
            return dtoBook;
        }

        public async Task<Book> PostBook(Book book)
        {

            if (_context.Books.Any(b => b.Id == book.Id))
            {
                throw new KeyNotFoundException($"Book already exists with ID of{book.Id}");
            }

            //check the inputs
            Book newBook = new Book();

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

            return nBook;


        }

        public async Task UpdateBook(int id, Book book)
        {

            //return not found if the Book ID doesnt exist in the context
            var existingBook = await _context.Books.FindAsync(id);
            if (existingBook == null)
            {
                throw new KeyNotFoundException($"Book ID does not exist with ID of{id}");
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
                    throw new KeyNotFoundException($"Book ID does not exist with ID of{id}");
                }
                else
                {
                    throw;
                }
            }

        }
        public async Task DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }else{
                throw new KeyNotFoundException($"No book found with ID {id}.");

            }
        }

        private static string SanitiseInput(string i)
        {
            //regex pattern to catch any letters not any alphanumeric value or a space
            Regex r = new Regex("[^a-zA-Z0-9 ]");
            //replace the value if found.
            return r.Replace(i, "");
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }

}
//return BadRequest();

//var existingBook = await _context.Books.FindAsync(id);
//if (existingBook == null)
//{
//    return; // Handle how you wish to manage not found, maybe throw an exception
//}

//// Map the updated fields from book to existingBook as necessary
//existingBook.Title = book.Title;
//existingBook.Author = book.Author;
//// Add other fields as necessary

//_context.Entry(existingBook).State = EntityState.Modified;
//await _context.SaveChangesAsync();