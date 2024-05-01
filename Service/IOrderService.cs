using sp_project_guide_api.Models;

namespace sp_project_guide_api.Service
{
    public interface IOrderService
    {
        Task<Order> GetOrder(int id);
        Task<List<Order>> GetOrders();
        Task UpdateOrder(int id, Order order);
        Task DeleteOrder(int id);
    }
}
