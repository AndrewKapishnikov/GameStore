using GameStore.DataEF;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore
{
    public interface IOrderRepository
    {
        Order Create();
        Task<Order> CreateAsync();

        Order GetById(int id);
        Task<Order> GetByIdAsync(int id);

        void Update(Order order);
        Task UpdateAsync(Order order);

        Task RemoveAsync(Order order);
        Task<Order[]> GetOrdersByUserIdAsync(string userId);

        IQueryable<OrderDTO> GetAllOrders();

    }
}
