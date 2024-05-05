using Microsoft.AspNetCore.Mvc;
using Moq;
using sp_project_guide_api.Controllers;
using sp_project_guide_api.Models;
using sp_project_guide_api.Service;
using Xunit;

namespace sp_project_guide_api.Tests
{
    public class BookTests
    {
        
        private readonly BooksController booksController;
        private readonly Mock<Book> book;
        private readonly Mock<IBookService> _bookService;

        public BookTests()
        {
            //booksController = new BooksController(_bookService.Object);
            book = new Mock<Book>();
            //_bookService = new Mock<IBookService>();
            _bookService = new Mock<IBookService>();
            booksController = new BooksController(new Mock<BookSystemContext>().Object, _bookService.Object, new Mock<IConfiguration>().Object);
        }

        [Fact]
        public async Task DeleteBook_Returns204NoContent_WhenBookIsDeletedSuccessfully()
        {
            var newBook = new Book { Title = "New Book" };
            _bookService.Setup(service => service.PostBook(newBook))
                        .ReturnsAsync(new Book { Id = 1, Title = "New Book" });

            // Act
            var createBook = await booksController.PostBook(newBook);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Book>>(createBook);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(createBook.Result);


            // Arrange
            _bookService.Setup(service => service.DeleteBook(newBook.Id)).Returns(Task.CompletedTask);
                
            // Act
            var result = await booksController.DeleteBook(newBook.Id);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PostBook_Returns201Created_WhenBookIsAddedSuccessfully()
        {
            // Arrange
            var newBook = new Book { Title = "New Book" };
            _bookService.Setup(service => service.PostBook(newBook))
                        .ReturnsAsync(new Book { Id = 1, Title = "New Book" });

            // Act
            var result = await booksController.PostBook(newBook);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Book>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.Equal(201, createdAtActionResult.StatusCode);
            Assert.NotNull(createdAtActionResult.Value);
        }


    }
}
