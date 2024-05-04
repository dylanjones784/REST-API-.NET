//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Moq;
//using sp_project_guide_api.Controllers;
//using sp_project_guide_api.Models;
//using Xunit;

//namespace sp_project_guide_api.Tests
//{
//    public class MemberTests
//    {
//        private readonly Mock<DbSet<Member>> _mockSet;
//        private readonly Mock<BookSystemContext> _mockContext;
//        private readonly MembersController _controller;

//        private readonly DbContextOptions<BookSystemContext> _options;


//        public MemberTests()
//        {
//            //_mockSet = new Mock<DbSet<Member>>();
//            //_mockContext = new Mock<BookSystemContext>();
//            //_mockContext.Setup(m => m.Members).Returns(_mockSet.Object);
//            //_controller = new MembersController(_mockContext.Object);
//            //_options = new DbContextOptionsBuilder<BookSystemContext>().UseInMemoryDatabase(databaseName: "BookSystem").Options;
//        }
//        //[Fact]
//        //public async Task GetMember_ReturnsNotFound_WhenMemberDoesNotExist()
//        //{
//        //    // Arrange
//        //    var memberId = 1;
//        //    _mockContext.Setup(db => db.Members.FindAsync(memberId))
//        //                .ReturnsAsync((Member)null);

//        //    // Act
//        //    var result = await _controller.GetMember(memberId);

//        //    // Assert
//        //    Assert.IsType<NotFoundResult>(result.Result);
//        //}
//        [Fact]
//        public async Task PostMember_AddsMemberSuccessfully_ReturnsCreatedAtActionResult()
//        {
//            // Arrange
//            var member = new Member
//            {
//                Name = "John Doe",
//                Address = "1234 Main St",
//                DateRegistered = DateTime.Now
//            };

//            _mockSet.Setup(m => m.Add(It.IsAny<Member>())).Verifiable();
//            _mockContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);

//            // Act
//            var result = await _controller.PostMember(member);

//            // Assert
//            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
//            var returnValue = Assert.IsType<Member>(createdAtActionResult.Value);
//            Assert.Equal("John Doe", returnValue.Name);
//            _mockSet.Verify(m => m.Add(It.IsAny<Member>()), Times.Once());
//            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once());
//        }

//        private async Task<BookSystemContext> CreatePopulatedContextAsync()
//        {
//            var context = new BookSystemContext(_options);
//            var member = new Member { Id = 1, Name = "John Doe", Address = "123 Main St", DateRegistered = DateTime.Now };
//            context.Members.Add(member);
//            await context.SaveChangesAsync();
//            return context;
//        }

//        [Fact]
//        public async Task DeleteMember_RemovesMember_ReturnsNoContentResult()
//        {
//            // Arrange
//            using var context = await CreatePopulatedContextAsync();
//            var controller = new MembersController(context);

//            // Act
//            var deleteResult = await controller.DeleteMember(1);

//            // Assert
//            var result = Assert.IsType<NoContentResult>(deleteResult);
//            var memberExists = await context.Members.AnyAsync(m => m.Id == 1);
//            Assert.False(memberExists);
//        }

//        [Fact]
//        public async Task DeleteMember_WhenMemberDoesNotExist_ReturnsNotFoundResult()
//        {
//            // Arrange
//            using var context = new BookSystemContext(_options);
//            var controller = new MembersController(context);

//            // Act
//            var result = await controller.DeleteMember(45);  // Assuming 45 is a non-existent ID

//            // Assert
//            Assert.IsType<NotFoundResult>(result);
//        }
//        [Fact]
//        public async Task GetMember_ReturnsMember_WhenMemberExists()
//        {
//            using var context = await CreatePopulatedContextAsync();
//            var controller = new MembersController(context);

//            var result = await controller.GetMember(1); // Assuming the member with ID 1 exists

//            var okResult = Assert.IsType<OkObjectResult>(result.Result);
//            var member = Assert.IsType<Member>(okResult.Value);
//            Assert.Equal(1, member.Id);
//        }

//        [Fact]
//        public async Task PutMember_UpdatesMemberSuccessfully_ReturnsNoContent()
//        {
//            using var context = await CreatePopulatedContextAsync();
//            var controller = new MembersController(context);
//            var updatedMember = new Member { Id = 1, Name = "Jane Doe", Address = "1234 Main St", DateRegistered = DateTime.Now };

//            var result = await controller.PutMember(1, updatedMember);

//            var noContentResult = Assert.IsType<NoContentResult>(result);
//            var member = await context.Members.FindAsync(1);
//            Assert.Equal("Jane Doe", member.Name);
//        }
//    }
//}
