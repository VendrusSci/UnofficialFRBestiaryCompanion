using Bestiary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Bestiary.Services
{
    class CsvImportExport
    {
        private IModel m_Model;
        public CsvImportExport(IModel model)
        {
            m_Model = model;
        }

        public string MakeCsvString(List<CsvFamiliar> familiarList)
        {
            StringBuilder sb = new StringBuilder();

            var properties = typeof(CsvFamiliar)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .OrderBy(p => p.Name);

            

            return sb.ToString();
        }
    }

    class CsvFamiliar
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public OwnershipStatus Owned { get; set; }
        public BondingLevels BondLevel { get; set; }
        public LocationTypes Location { get; set; }
        public BookmarkState Bookmarked { get; set; }
    }
}
