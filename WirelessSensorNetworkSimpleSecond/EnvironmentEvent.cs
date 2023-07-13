using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WirelessSensorNetworkSimpleSecond
{
    public class EnvironmentEvent
    {
        public Point Location { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double Radius { get; set; }
        public string Data { get; set; }
        public EventType Type { get; set; }

        public enum EventType
        {
            Movement,
            Sound,
            Light
        }
        public EnvironmentEvent GenerateRandomEvent()
        {
            var random = new Random();
            var x = random.Next(0, 100); 
            var y = random.Next(0, 100);

            return new EnvironmentEvent
            {
                Location = new Point(x, y),
            Temperature = random.NextDouble() * 50,
                Humidity = random.NextDouble() * 100,
             };
        }
        public void HandleMultipleEvents(List<EnvironmentEvent> events)
        {
            
        }
        public bool CanSeeEvent(Sensor sensor)
        {
            double distance = MathOperations.CalculateDistance(Location, sensor.Location);
            return distance <= Radius;
        }
    }
}
