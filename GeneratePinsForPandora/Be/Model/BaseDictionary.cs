using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratePinsForPandora.Be.Model
{
    public class BaseDictionary
    {
        public int DictionaryID { get; set; }
        public string Value { get; set; }
    }

    public class DictionaryData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int? ParentId { get; set; }
        public List<DictionaryData> Dictionaries { get; set; }
    }

}
