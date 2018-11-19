using Bestiary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bestiary.ViewModel
{
    class GatherControlViewModel
    {
        bool[] flightSelections;

        public GatherControlViewModel()
        {
            flightSelections = new bool[11];
        }

        public List<Flights> GetSelectedFlights()
        {
            List < Flights > flightList = new List<Flights>();

            for(int i = 0; i < flightSelections.Length; i++)
            {
                if(flightSelections[i])
                {
                    flightList.Add((Flights)i);
                }
            }
            
            return flightList;
        }
    }
}
