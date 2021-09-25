using System;

namespace FlightDemo {

    /// <summary>
    /// A struct containing information about a flight on a given day.
    /// </summary>
    public readonly struct FlightInfo {
        private readonly string start;
        private readonly string destination;
        private readonly DateTime start_time;
        private readonly DateTime arrival_time;

        public FlightInfo(string start, string destination, string start_time, string arrival_time) {
            this.start = start;
            this.destination = destination;
            this.start_time = DateTime.ParseExact(start_time, "HH:mm", null);
            this.arrival_time = DateTime.ParseExact(arrival_time, "HH:mm", null);
        }

        public string Start {
            get {
                return start;
            }
        }

        public string Destination {
            get {
                return destination;
            }
        }

        public DateTime StartTime {
            get {
                return start_time;
            }
        }

        public DateTime ArrivalTime {
            get {
                return arrival_time;
            }
        }

        public override string ToString() {
            return string.Format("{0} to {1} leaving at {2}:{3} and arriving at {4}:{5}",
                start, destination, HourFormat(start_time.Hour), HourFormat(start_time.Minute),
                HourFormat(arrival_time.Hour), HourFormat(arrival_time.Minute));
        }

        private static string HourFormat(int time) {
            string str = time.ToString();

            if (str.Length == 1)
                return "0" + str;
            else
                return str;
        }
    }
}
