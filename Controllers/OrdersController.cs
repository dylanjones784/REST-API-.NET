using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sp_project_guide_api.Models;

namespace sp_project_guide_api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly BookSystemContext _context;

        public OrdersController(BookSystemContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            List<Order> lOrders = await _context.Orders.ToListAsync();

            foreach(Order o in lOrders)
            {
                o.Links = new List<Link>
                {
                    new Link($"/api/Orders/{o.Id}", "self", "GET",0),
                    new Link($"/api/Orders/{o.Id}", "self", "PUT",1),
                    new Link($"/api/Orders/{o.Id}", "self", "DELETE",0)
                };
            }
            return await _context.Orders.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }
            order.Links = new List<Link>
            {
                    new Link($"/api/Orders/{order.Id}", "self", "PUT",1),
                    new Link($"/api/Orders/{order.Id}", "self", "DELETE",0),
            };

            return order;
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.Id)
            {
                return BadRequest();
            }

            //check that Member ID and Book ID exist
            var member = await _context.Members.FindAsync(order.MemberID);
            var book = await _context.Books.FindAsync(order.BookId);
            if (book != null && member != null)
            {
                _context.Entry(order).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(id))
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
            return BadRequest("Your book or member ID does not exist.");

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
