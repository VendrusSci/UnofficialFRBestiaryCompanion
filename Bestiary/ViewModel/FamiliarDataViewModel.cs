using Bestiary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bestiary.ViewModel
{
    class FamiliarDataViewModel
    {
        public FamiliarViewModel FamiliarDisplayInfo { get; set; }

        public Familiar Familiar { get; set; }

        public FamiliarDataViewModel(FamiliarViewModel info)
        {
            FamiliarDisplayInfo = info;
            Familiar = FamiliarDisplayInfo.Info.Familiar;
        }
    }
}
