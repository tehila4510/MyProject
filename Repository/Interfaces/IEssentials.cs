using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IEssentials<T>
    {
        Task DeleteRange(IEnumerable<int> ids);
        Task UpdateRange(IEnumerable<T> items);
    }
}
