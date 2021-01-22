using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore
{
    public interface IOrderRepository
    {
        Order Create();
        void Update(Order order);
        Order GetById(int id);

       
    }
}
