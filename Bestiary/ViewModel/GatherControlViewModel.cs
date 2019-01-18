using Bestiary.Model;
using System.Collections.Generic;

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
            List<Flights> flightList = new List<Flights>();

            for (int i = 0; i < FlightSelections.Length; i++)
            {
                if (FlightSelections[i])
                {
                    flightList.Add((Flights)i);
                }
            }
            MainViewModel.UserActionLog.Info($"Flights in list: {flightList}");
            return flightList;
        }
    }
}