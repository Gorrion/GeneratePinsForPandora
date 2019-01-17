using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratePinsForPandora.Be.Model
{
    public class Datum
    {
        public int Id { get; set; }
        public int InnoObjectTypeID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public string Fio { get; set; }
        public string Website { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public List<BaseDictionary> BaseDictionaries { get; set; }
        public DatumExt Data { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public int InnoObjectStatusID { get; set; }
        public int DistrictID { get; set; }
        public int? RegionID { get; set; }
        public PhotoInfo Image { get; set; }
        public object LogoUrl { get; set; }
        public string InnoObjectLogoUrl { get; set; }
        public object PresentationUrl { get; set; }
        public bool Deleted { get; set; }
        public object Visitors { get; set; }
        public object EquipmentUse { get; set; }
        public object EmployeesNumber { get; set; }
    }
}
