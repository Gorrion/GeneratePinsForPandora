using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratePinsForPandora.Be.Model
{
    public class DatumExt
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public object ShortDescription { get; set; }
        public string Description { get; set; }
        public string Website { get; set; }
        public object WorkingHours { get; set; }
        public object Area { get; set; }
        public int Workplaces { get; set; }
        public object FreeWorkplaces { get; set; }
        public object Period { get; set; }
        public object Requirements { get; set; }
        public object Year { get; set; }
        public string Fio { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string CompanyInn { get; set; }
        public string CompanyName { get; set; }
        public string CompanyShortName { get; set; }
        public string CompanyOkfs { get; set; }
        public List<object> Equipments { get; set; }
        public List<PhotoInfo> Photos { get; set; }
        public List<object> Events { get; set; }
        public List<object> News { get; set; }
        public List<object> InnoObjects { get; set; }
        public List<FormatSpace> FormatSpaces { get; set; }
        public List<object> Buildings { get; set; }
        public List<PartnersProgramm> PartnersProgramm { get; set; }
        public List<object> Residents { get; set; }
        public List<object> Tenants { get; set; }
        public int CoworkingTypeID { get; set; }
        public Seo Seo { get; set; }
    }
}
