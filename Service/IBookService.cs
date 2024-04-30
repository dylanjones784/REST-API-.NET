using Microsoft.AspNetCore.Mvc;
using sp_project_guide_api.Models;

namespace sp_project_guide_api.Service
{
    public interface IBookService
    {
        Task<Book> GetBook(int id);
        Task<ActionResult<DTOBook>> GetBooks(int? page, int? pageSize, string? sort);
        Task<Book> PostBook(Book book);
        Task UpdateBook(int id, Book book);
        Task DeleteBook(int id);

    }
}
