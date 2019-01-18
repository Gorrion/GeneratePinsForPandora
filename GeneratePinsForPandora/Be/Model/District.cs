using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratePinsForPandora.Be.Model
{
    public class District
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int Level { get; set; }
    }

    public static class Districts
    {
        public static List<District> Moscow = new List<District>
        {
           new District {
               Id = 102269,
               Level = 4,
               Name = "Москва",
               Code = "moscow",
           },

           new District {
               Id = 2263059,
               ParentId = 102269,
               Level = 5,
               Name = "Троицкий административный округ",
               Code = "troitsk",
           },

           new District {
               Id = 2263058,
               ParentId = 102269,
               Level = 5,
               Name = "Новомосковский административный округ",
               Code = "novomosk",
           },

           new District {
               Id = 226149,
               ParentId = 102269,
               Level = 5,
               Name = "Западный административный округ",
               Code = "zao",
           },

            new District {
               Id = 1320234,
               ParentId = 102269,
               Level = 5,
               Name = "Восточный административный округ",
               Code = "vao",
           },

           new District {
               Id = 1278703,
               ParentId = 102269,
               Level = 5,
               Name = "Юго-Восточный административный округ",
               Code = "uvao",
           },

           new District {
               Id = 162903,
               ParentId = 102269,
               Level = 5,
               Name = "Северный административный округ",
               Code = "sao",
           },

            new District {
               Id = 1252558,
               ParentId = 102269,
               Level = 5,
               Name = "Северо-Восточный административный округ",
               Code = "svao",
           },

             new District {
               Id = 1282181,
               ParentId = 102269,
               Level = 5,
               Name = "Южный административный округ",
               Code = "uao",
           },

           new District {
               Id = 1304596,
               ParentId = 102269,
               Level = 5,
               Name = "Юго-Западный административный округ",
               Code = "uzao",
           },

           new District {
               Id = 446092,
               ParentId = 102269,
               Level = 5,
               Name = "Северо-Западный административный округ",
               Code = "szao",
           },

           new District {
               Id = 2162196,
               ParentId = 102269,
               Level = 5,
               Name = "Центральный административный округ",
               Code = "cao",
           },

           new District {
               Id = 1320358,
               ParentId = 102269,
               Level = 5,
               Name = "Зеленоград",
               Code = "zel",
           },
        };
    }

}
