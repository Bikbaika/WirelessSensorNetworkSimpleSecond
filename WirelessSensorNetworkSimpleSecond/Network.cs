using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using System.Xml.Linq;
using System.IO;
using static WirelessSensorNetworkSimpleSecond.Node;

namespace WirelessSensorNetworkSimpleSecond
{
    public class Network
    {
        public List<Sensor> Sensors { get; private set; }
        public List<BaseStation> BaseStations { get; private set; }
        public string Topology { get; private set; }
        private double TotalInitialEnergy { get; set; }
        public List<Node> Nodes { get; set; }

        public Network()
        {
            Sensors = new List<Sensor>();
            BaseStations = new List<BaseStation>();
        }
        public void AddSensor(Sensor sensor)
        {
            if (sensor != null)
            {
                Sensors.Add(sensor);
                TotalInitialEnergy += sensor.Energy;
            }
        }

        public void AddBaseStation(BaseStation baseStation)
        {
            if (baseStation != null)
            {
                BaseStations.Add(baseStation);
            }
        }

        public void GenerateNetwork(int sensorCount, int baseStationCount, string topology, double energyLevel)
        {
            Sensors.Clear();
            BaseStations.Clear();
            var random = new Random();
            Debug.WriteLine("Generating network...");
            GenerateBaseStations(baseStationCount, random);
            GenerateSensors(sensorCount, topology, energyLevel, random);
            Topology = topology;
            GenerateConnections();
            Debug.WriteLine("Network generation completed.");
        }

        private void GenerateBaseStations(int baseStationCount, Random random)
        {
            for (int i = 0; i < baseStationCount; i++)
            {
                var baseStation = new BaseStation
                {
                    BaseStationId = i,
                    Location = GenerateRandomPoint(random)
                };
                AddBaseStation(baseStation);
            }
        }
        public void RemoveSensor(Sensor sensor)
        {
            Sensors.Remove(sensor);
        }

        public void RemoveBaseStation(BaseStation baseStation)
        {
            BaseStations.Remove(baseStation);
        }

        public void AddSensor(Point nodeLocation)
        {
            Sensor newSensor = new Sensor(nodeLocation);
            Sensors.Add(newSensor);
            UpdateNetwork();
            
        }

        public void AddBaseStation(Point nodeLocation)
        {
            BaseStation newBaseStation = new BaseStation(nodeLocation);
            BaseStations.Add(newBaseStation);
            UpdateNetwork();
        }

        private void GenerateSensors(int sensorCount, string topology, double energyLevel, Random random)
        {
            Debug.WriteLine("Expected Node Locations:");
            for (int i = 1; i < sensorCount; i++)
            {
                var sensor = new Sensor
                {
                    SensorId = i,
                    Location = GenerateRandomPoint(random),
                    Energy = energyLevel,
                    Topology = topology
                };
                AddSensor(sensor);
                Debug.WriteLine("Adding sensor: " + sensor.Location);
            }
        }
        private Point GenerateRandomPoint(Random random, int minX = 60, int maxX = 400, int minY = 0, int maxY = 350)
        {
            return new Point(random.Next(minX, maxX), random.Next(minY, maxY));
        }
        private void GenerateConnections()
        {
            switch (Topology)
            {
                case "mesh":
                    GenerateMeshConnections();
                    break;
                case "star":
                    GenerateStarConnections();
                    break;
                case "tree":
                    GenerateTreeConnections();
                    break;
                default:
                    throw new ArgumentException("Invalid topology");
            }
        }

        private void GenerateMeshConnections()
        {
            var allNodes = GetAllNodes();
            foreach (var node in allNodes)
            {
                node.ConnectedNodes = allNodes.Where(n => n != node).ToList();
            }
        }

        private void GenerateStarConnections()
        {
            if (!BaseStations.Any())
            {
                throw new InvalidOperationException("Network must have at least one base station for star topology");
            }

            foreach (var sensor in Sensors)
            {
                BaseStation closestBaseStation = BaseStations.OrderBy(bs => MathOperations.CalculateDistance(bs.Location, sensor.Location)).First();
                sensor.ConnectedNodes.Add(closestBaseStation);
                sensor.BaseStation = closestBaseStation;
            }

            foreach (var baseStation in BaseStations)
            {
                baseStation.ConnectedNodes = Sensors.Where(s => s.BaseStation == baseStation).Cast<Node>().ToList();
            }
        }
        private void GenerateTreeConnections()
        {
            if (!BaseStations.Any())
            {
                throw new InvalidOperationException("Network must have at least one base station for tree topology");
            }

            var baseStation = BaseStations[0];
            var remainingNodes = GetAllNodes();
            var connectedNodes = new List<Node> { baseStation };

            remainingNodes.Remove(baseStation);

            while (remainingNodes.Any())
            {
                Node closestNode = null;
                Node closestConnectedNode = null;
                double closestDistance = double.MaxValue;

                foreach (var connectedNode in connectedNodes)
                {
                    var closestCandidateNodes = FindClosestNodes(connectedNode, remainingNodes, 1);

                    if (closestCandidateNodes.Any())
                    {
                        var closestCandidateNode = closestCandidateNodes[0];
                        var distance = MathOperations.CalculateDistance(connectedNode.Location, closestCandidateNode.Location);

                        if (distance < closestDistance)
                        {
                            closestNode = closestCandidateNode;
                            closestConnectedNode = connectedNode;
                            closestDistance = distance;
                        }
                    }
                }

                if (closestNode != null)
                {
                    closestConnectedNode.ConnectedNodes.Add(closestNode);
                    closestNode.ConnectedNodes.Add(closestConnectedNode);
                    if (closestNode is Sensor sensor)
                    {
                        sensor.BaseStation = baseStation;
                    }

                    remainingNodes.Remove(closestNode);
                    connectedNodes.Add(closestNode);
                }
            }
        }

        private List<Node> FindClosestNodes(Node node, List<Node> nodes, int count)
        {
            return nodes.OrderBy(n => MathOperations.CalculateDistance(node.Location, n.Location)).Take(count).ToList();
        }
     
        public double CalculateAverageEnergy()
        {
            return Sensors.Any() ? Sensors.Average(s => s.Energy) : 0;
        }


        public List<Node> GetAllNodes()
        {
            var allNodes = new List<Node>();
            allNodes.AddRange(Sensors);
            allNodes.AddRange(BaseStations);
            return allNodes;
        }
        public List<Sensor> GetAllSensors()
        {
            var allSensors = new List<Sensor>();
            allSensors.AddRange(Sensors);
            return allSensors;
        }

        public void UpdateNetwork()
        {
            // Обновление сети - реализация алгоритмов для динамического перестроения сети
            // Здесь можно включить логику обнаружения новых узлов, обновление маршрутных таблиц и связей

            // Пример:
            // Обнаружение новых узлов
            foreach (var node in GetAllNodes())
            {
                if (node is Sensor sensor && !Sensors.Contains(sensor))
                {
                    // Добавление нового датчика
                    AddSensor(sensor);
                }
                else if (node is BaseStation baseStation && !BaseStations.Contains(baseStation))
                {
                    // Добавление новой базовой станции
                    AddBaseStation(baseStation);
                }
            }

            // Обновление маршрутных таблиц и связей
           // UpdateRoutingTables();
            //UpdateConnections();
        }

        public void RouteData()
        {
                // Реализуйте алгоритм маршрутизации данных для каждого датчика
                // Определите оптимальный путь передачи данных от датчика к базовой станции
                // и передайте данные по этому пути с помощью метода TransmitData
            foreach (var sensor in Sensors)
            {
                if (sensor.State == NodeState.Active)
                {
                    var nearestSensor = sensor.GetNearestSensor();
                    if (nearestSensor != null)
                    {
                        sensor.TransmitData(nearestSensor, sensor.Data);
                    }
                }
            }
        }
        private void TransmitData(Node sourceNode, Node targetNode, string data)
        {
            // Реализуйте передачу данных от исходного узла к целевому узлу
            // Можно использовать существующий метод TransmitData или создать новый метод
        }
        public double GetTotalEnergyConsumption()
        {
            double totalCurrentEnergy = Sensors.Sum(s => s.Energy);
            return TotalInitialEnergy - totalCurrentEnergy;
        }
        public void OptimizeNodePlacement()
        {
            // Реализация алгоритма оптимизации расположения узлов в сети
            // Включает поиск оптимальных координат для базовых станций и сенсоров
            // с учетом покрытия области и минимизации препятствий
        }
 
        public void WriteNetworkDataToFile(string fileName)
        {
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                // Запись информации о датчиках
                writer.WriteLine("Sensor Count: " + Sensors.Count);
                writer.WriteLine("Sensors:");
                foreach (Sensor sensor in Sensors)
                {
                    writer.WriteLine($"Sensor ID: {sensor.Id}, Location: {sensor.Location}, Energy: {sensor.Energy}");
                }
                writer.WriteLine();

                // Запись информации о базовых станциях
                writer.WriteLine("Base Station Count: " + BaseStations.Count);
                writer.WriteLine("Base Stations:");
                foreach (BaseStation baseStation in BaseStations)
                {
                    writer.WriteLine($"Base Station ID: {baseStation.BaseStationId}, Location: {baseStation.Location}");
                }
                writer.WriteLine();

                // Запись информации о соединениях
                writer.WriteLine("Connections:");
                foreach (Node node in GetAllNodes())
                {
                    writer.WriteLine($"Node ID: {node.Id}, Connected Nodes: {string.Join(", ", node.ConnectedNodes.Select(n => n.Id))}");
                }
            }
        }

    }
}
