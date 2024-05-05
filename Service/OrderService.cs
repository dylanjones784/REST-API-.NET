using Microsoft.EntityFrameworkCore;
using sp_project_guide_api.Models;

namespace sp_project_guide_api.Service
{
    public class OrderService : IOrderService
    {

        private readonly BookSystemContext _context;

        public OrderService(BookSystemContext context)
        {
            _context = context;
        }

        public async Task DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"No order found with ID {id}.");

            }
        }

        public async Task<Order> GetOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if(order == null)
            {
                return order;
            }

            order.Links = new List<Link>
            {
                    new Link($"/api/Orders/{order.Id}", "self", "PUT",1),
                    new Link($"/api/Orders/{order.Id}", "self", "DELETE",0),
            };
            return order;
        }

        public async Task<List<Order>> GetOrders()
        {
            List<Order> lOrders = await _context.Orders.ToListAsync();

            foreach (Order o in lOrders)
            {
                o.Links = new List<Link>
                {
                    new Link($"/api/Orders/{o.Id}", "self", "GET",0),
                    new Link($"/api/Orders/{o.Id}", "self", "PUT",1),
                    new Link($"/api/Orders/{o.Id}", "self", "DELETE",0)
                };
            }
            return lOrders;
        }

        public async Task UpdateOrder(int id, Order order)
        {

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
                        throw new KeyNotFoundException($"Order ID does not exist with ID of {id}");
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
