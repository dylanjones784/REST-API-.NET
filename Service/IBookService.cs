using Microsoft.AspNetCore.Mvc;
using sp_project_guide_api.Models;

namespace sp_project_guide_api.Service
{
    public interface IBookService
    {
        public Task<ActionResult<DTOBook>> GetBooks();

        public Task<ActionResult<Book>> GetBook(int id);
        public Task<IActionResult> PutBook(int id, Book book);

        public Task<ActionResult<Book>> PostBook([FromBody] Book book);

        public Task<IActionResult> DeleteBook(int id);


    }
}
