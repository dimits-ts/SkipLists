using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using SkipLists;

namespace FlightDemo {

    public static class Demo {
        
        public static void Main() {

            //get file name
            Console.Write("Enter the path to the flights file:\n>");
            string file = Console.ReadLine();
            if (!File.Exists(file)) {
                Console.WriteLine("Invalid path: " + file);
                return;
            }

            //get departure and arrival airports
            Console.Write("Enter the closest airport to you:\n>");
            string startName = Console.ReadLine().ToUpper();
            Console.Write("Enter the airport closest to your destination:\n>");
            string destinationName = Console.ReadLine().ToUpper();

            //get departure and arrival times
            DateTime startTime;
            DateTime arrivalTime;
            try {
                Console.Write("Enter the ideal time of your flight's departure in HH::MM format:\n>");
                startTime = DateTime.ParseExact(Console.ReadLine(), "HH:mm", null);
                Console.Write("Enter the ideal time of your flight's arrival in HH::MM format:\n>");
                arrivalTime = DateTime.ParseExact(Console.ReadLine(), "HH:mm", null);
            } catch(FormatException exc) {
                Console.WriteLine("Invalid time: " + exc.Message);
                return;
            }

            //calculate best flights
            var startTimeDict = new SkipListDictionary<TimeSpan, FlightInfo>();
            var arrivalTimeDict =  new SkipListDictionary<TimeSpan, FlightInfo>();

            IEnumerable<FlightInfo> flights;
            try {
                flights = ReadFile(file);
            } catch(IOException exc) {
                Console.WriteLine("Could not access flights file: " + exc.Message);
                return;
            }
            
            foreach (FlightInfo flight in flights) {
                if (flight.Start == startName && flight.Destination == destinationName) {
                    startTimeDict.Add((flight.StartTime - startTime).Duration(), flight);
                    arrivalTimeDict.Add((flight.ArrivalTime - arrivalTime).Duration(), flight);
                }
            }
            
            //print best flights
            if(startTimeDict.Count == 0) {
                Console.WriteLine(string.Format("There are no scheduled flights between {0} and {1}.",
                    startName, destinationName));
                return;
            }

            Console.WriteLine("Most convinient flights to leave:\n" + 
                PrintFlights(startTimeDict.Values, 5));

            Console.WriteLine("Most convinient flights to arrive:\n" +
                PrintFlights(arrivalTimeDict.Values, 5));
        }

        private static IEnumerable<FlightInfo> ReadFile(string filePath) {
            LinkedList<FlightInfo> flightList = new LinkedList<FlightInfo>();

            foreach (string flight in File.ReadAllLines(filePath))
                flightList.AddLast(ParseFlightData(flight));

            return flightList;
        }

        private static FlightInfo ParseFlightData(string data) {
            string[] tokens = data.Split(" ");
            return new FlightInfo(tokens[0], tokens[1], tokens[2], tokens[3]);
        }

        private static string PrintFlights(IEnumerable<FlightInfo> flights, int count) {
            StringBuilder str = new StringBuilder();
            int flightsPrinted = 0;

            foreach (var flight in flights) {
                str.AppendLine(flight.ToString());
                flightsPrinted++;
                if (flightsPrinted >= count)
                    break;
            }

            return str.ToString();
        }
    }
}
