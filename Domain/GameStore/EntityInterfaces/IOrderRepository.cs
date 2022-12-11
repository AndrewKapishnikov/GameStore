
namespace GameStore.EntityInterfaces
{
    public interface IOrderRepository
    {
        Order Create();
        Order GetById(int id);
        void Update(Order order);
        void Remove(Order order);
    }
}
