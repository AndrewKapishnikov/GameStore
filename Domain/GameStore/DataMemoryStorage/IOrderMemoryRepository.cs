using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore
{
    public interface IOrderMemoryRepository
    {
        OrderMemoryStorage Create();
        void Update(OrderMemoryStorage order);
        OrderMemoryStorage GetById(int id);

       
    }
}
