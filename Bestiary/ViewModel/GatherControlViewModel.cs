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
        public bool[] FlightSelections { get; set; }

        public GatherControlViewModel()
        {
            FlightSelections = new bool[11];
        }

        public List<Flights> GetSelectedFlights()
        {
            List < Flights > flightList = new List<Flights>();

            for(int i = 0; i < FlightSelections.Length; i++)
            {
                if(FlightSelections[i])
                {
                    flightList.Add((Flights)i);
                }
            }
            
            return flightList;
        }
    }
}
