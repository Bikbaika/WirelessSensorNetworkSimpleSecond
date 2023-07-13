using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WirelessSensorNetworkSimpleSecond
{
    public class BaseStation:Node
    {
        private static int nextBaseStationId = 1;

        public int BaseStationId { get; set; }

        public BaseStation()
        {
            BaseStationId = nextBaseStationId++;
        }
        public BaseStation(Point location) : base(location)
        {
            BaseStationId = nextBaseStationId++;
            // Дополнительная инициализация для класса BaseStation
        }
    }
}
