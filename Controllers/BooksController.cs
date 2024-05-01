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
using sp_project_guide_api.Service;
using static System.Reflection.Metadata.BlobBuilder;

namespace sp_project_guide_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookSystemContext _context;
        private IConfiguration _config;
        private readonly IBookService _bookService;

        public BooksController(BookSystemContext context, IBookService bookService, IConfiguration config)
        {
            _context = context;
            _config = config;
            _bookService = bookService;
        }

        //public BooksController(IBookService bookService)
        //{
        //    //_context = context;
        //    //_config = config;
        //    _bookService = bookService;
        //}

        [HttpGet]
        public async Task<ActionResult<DTOBook>> GetBooks([FromQuery] int? page, [FromQuery] int? pageSize, [FromQuery] string? sort)
        {
            var books = await _bookService.GetBooks(page, pageSize, sort);
            if (books == null)
            {
                return NotFound();
            }
            return Ok(books);

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _bookService.GetBook(id);

            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }


        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }
            try
            {
                await _bookService.UpdateBook(id, book);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); 
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message); 
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook([FromBody]Book book)
        {
            try
            {
                Book createdBook = await _bookService.PostBook(book);
                return CreatedAtAction("GetBook", new { id = createdBook.Id }, createdBook);
            }
            catch (ArgumentNullException ex)
            {
                //use this to catch 400 bad request from the service since we cant return the HTTP errors from that layer
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                //when the state is not as expected
                return Conflict(ex.Message);
            }
            catch (ArgumentException ex)
            {
                //if a field in the body does not match what is expected.
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                //any other issues, we return a 500 as something gone wrong internally somehow
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }


        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                await _bookService.DeleteBook(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
