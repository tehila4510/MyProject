using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Interfaces
{
    public interface IOpenAi
    {
        public Task<string> chatGpt(string question);
    }
}
