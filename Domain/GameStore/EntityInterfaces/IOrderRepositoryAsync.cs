using GameStore.DataEF;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore
{
    public interface IOrderRepositoryAsync
    {
        Task<Order> CreateAsync();
        Task<Order> GetByIdAsync(int id);
        Task UpdateAsync(Order order);
        Task RemoveAsync(Order order);
        Task<Order[]> GetOrdersByUserIdAsync(string userId);
        IQueryable<OrderDTO> GetAllOrders();

    }
}
