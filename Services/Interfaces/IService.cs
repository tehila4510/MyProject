using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Interfaces
{
    public interface IService<T>
    {
        Task<T> Add(T item);
        Task<T> GetById(int id);
        Task<List<T>> GetAll();
        Task<T> Update(int id, T item);
        Task Delete(int id);
    }
}
