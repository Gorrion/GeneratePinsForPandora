using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratePinsForPandora.Be.Model
{
    public class FormatSpace
    {
        public string Name { get; set; }
        public string SpaceCount { get; set; }
        public string Description { get; set; }
        public string ImgUrl { get; set; }
        public string Price { get; set; }
        public List<string> Worksheet { get; set; }
    }
}
