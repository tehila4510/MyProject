using Common.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Interfaces
{
    public interface IsExist<T>
    {
       public T Exist(Login l);
    }
}
