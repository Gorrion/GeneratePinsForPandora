using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratePinsForPandora.Be.Model
{
    public class PartnersProgramm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public object ResidentsForPart { get; set; }
        public object ResidentsForNonParts { get; set; }
        public object Fio { get; set; }
        public object Email { get; set; }
        public object Phone { get; set; }
        public List<object> BaseDictionaries { get; set; }
        public object Company { get; set; }
    }
}
