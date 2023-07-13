using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WirelessSensorNetworkSimpleSecond
{
    public class NetworkManager
    {
        private Network network;

        public NetworkManager()
        {
            network = new Network();
        }

        public void GenerateNetwork(int sensorCount, int baseStationCount, string topology, double energyLevel)
        {
            network.GenerateNetwork(sensorCount, baseStationCount, topology, energyLevel);
        }

        public double CalculateAverageEnergy()
        {
            return network.CalculateAverageEnergy();
        }
        public IEnumerable<Sensor> GetSensors()
        {
            return network.Sensors;
        }

        public IEnumerable<BaseStation> GetBaseStations()
        {
            return network.BaseStations;
        }

        public IEnumerable<Node> GetAllNodes()
        {
            return network.GetAllNodes();
        }
        // Другие методы и свойства для работы с сетью
    }
}
