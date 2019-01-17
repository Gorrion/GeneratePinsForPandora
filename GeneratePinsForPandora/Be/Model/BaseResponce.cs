using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratePinsForPandora.Be.Model
{
    public class BaseResponce<T>
    {
        public int Status { get; set; }
        public object Message { get; set; }
        public T Data { get; set; }
        public int Version { get; set; }
    }
}
