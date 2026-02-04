using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Dto
{
    public class UserUpdateDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string AvatarUrl { get; set; }
    }
}
