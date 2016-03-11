using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestClient
{
    public interface IRestSerializer
    {
        string Serialize(object instance);
        T Deserialize<T>(string text);
    }
}
