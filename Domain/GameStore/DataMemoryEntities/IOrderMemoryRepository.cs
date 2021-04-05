
namespace GameStore
{
    public interface IOrderMemoryRepository
    {
        OrderMemoryEntity Create();
        void Update(OrderMemoryEntity order);
        OrderMemoryEntity GetById(int id);

       
    }
}
