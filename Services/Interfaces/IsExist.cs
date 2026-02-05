using Common.Dto.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Interfaces
{
    public interface IsExist<T>
    {
       public T Exist(LoginDto l);
    }
}
