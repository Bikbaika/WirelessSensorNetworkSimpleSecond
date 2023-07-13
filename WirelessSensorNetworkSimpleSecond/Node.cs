using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace WirelessSensorNetworkSimpleSecond
{
    public class Node
    {
        public enum NodeState
        {
            Sleep,
            TransmittingData,
            SendingData,
            ReceivingData,
            CollectingData,
            Awake,
            Active,
            Failed
        }
        private static int nextId = 1;

        public int Id { get; set; }
        public Color NodeColor { get; set; }
        public Point Location { get; set; }
        public List<Node> ConnectedNodes { get; set; }
        public double Energy { get; set; }
        public NodeState State { get; set; }
        public List<Node> RoutingPath { get; private set; }
        public bool Active { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public Queue<DataPacket> CollectedData { get; private set; } = new Queue<DataPacket>();
        public int PacketsSent { get; set; }
        public Node()
        {
            Id = nextId++;
            ConnectedNodes = new List<Node>();
        }
        public Node(Point location)
        {
            Id = nextId++;
            Location = location;
            // Дополнительная инициализация для класса Node
        }
        public Node(Point location, double energy)
        {
            this.Location = location;
            this.ConnectedNodes = new List<Node>();
            this.State = NodeState.Sleep;
            this.Energy = energy;
        }
        public Node(double energy)
        {
            this.Energy = energy;
            this.ConnectedNodes = new List<Node>();
        }
        public void Connect(Node otherNode)
        {
            if (!ConnectedNodes.Contains(otherNode))
            {
                ConnectedNodes.Add(otherNode);
                otherNode.Connect(this);
            }
        }

        public void WakeUp()
        {
            if (State == NodeState.Sleep)
            {
                State = NodeState.Awake;
                Debug.WriteLine($"Node {Id} woke up.");
            }
            else
            {
                Debug.WriteLine($"Node {Id} is already awake.");
            }
        }
        public void Sleep()
        {
            if (State != NodeState.Sleep)
            {
                State = NodeState.Sleep;
                Debug.WriteLine($"Node {Id} went to sleep.");
            }
            else
            {
                Debug.WriteLine($"Node {Id} is already asleep.");
            }
        }
        public void StartTransmittingData()
        {
            if (State == NodeState.Active)
            {
                State = NodeState.TransmittingData;
                Debug.WriteLine($"Node {Id} started transmitting data.");
            }
            else
            {
                Debug.WriteLine($"Node {Id} cannot start transmitting data in its current state.");
            }
        }
        public void DischargeBattery()
        {
            double dischargeRate = 1.0;

            if (Temperature > 30) // Вы можете выбрать другую температуру в качестве порогового значения
            {
                dischargeRate *= 1.5; // Увеличивает скорость разрядки на 50%
            }

            if (Humidity > 80) // Вы можете выбрать другую влажность в качестве порогового значения
            {
                dischargeRate *= 1.5; // Увеличивает скорость разрядки на 50%
            }

            Energy -= dischargeRate;
        }

        public void StartReceivingData()
        {
            if (State == NodeState.Active)
            {
                State = NodeState.ReceivingData;
                Debug.WriteLine($"Node {Id} started receiving data.");
            }
            else
            {
                Debug.WriteLine($"Node {Id} cannot start receiving data in its current state.");
            }
        }
        public void StartSendingData()
        {
            if (State == NodeState.Active)
            {
                State = NodeState.SendingData;
                Debug.WriteLine($"Node {Id} started sending data.");
            }
            else
            {
                Debug.WriteLine($"Node {Id} cannot start sending data in its current state.");
            }
        }
        public void StartCollectingData()
        {
            if (State == NodeState.Active)
            {
                State = NodeState.CollectingData;
                Debug.WriteLine($"Node {Id} started collecting data.");
            }
            else
            {
                Debug.WriteLine($"Node {Id} cannot start collecting data in its current state.");
            }
        }

        public void StopTransmittingData()
        {
            if (State == NodeState.TransmittingData)
            {
                State = NodeState.Active;
                Debug.WriteLine($"Node {Id} stopped transmitting data.");
            }
            else
            {
                Debug.WriteLine($"Node {Id} is not currently transmitting data.");
            }
        }

        public void SetRoutingPath(List<Node> path)
        {
            RoutingPath = path;
        }

        public void TransmitDataWithRouting(Node targetNode, string data)
        {
            if (State == NodeState.Sleep)
            {
                Debug.WriteLine($"Node {Id} is sleeping and cannot transmit data.");
                return;
            }

            if (State == NodeState.Active)
            {
                // Переходим в состояние SendingData перед передачей данных
                StartSendingData();

                // Уменьшаем уровень энергии узла при передаче данных
                if (this is Sensor sensor)
                {
                    sensor.ConsumeEnergy(10); // Потребляем некоторое количество энергии
                }

                // Отправляем данные целевому узлу по заданному маршруту
                foreach (var node in RoutingPath)
                {
                    if (node == targetNode)
                    {
                        // Передача данных целевому узлу
                        targetNode.ReceiveData(this, data);
                        break;
                    }
                    else
                    {
                        // Передача данных следующему узлу в маршруте
                        node.ReceiveData(this, data);
                    }
                }

                // Переходим обратно в состояние Active после передачи данных
                StopSendingData();
            }
        }
        public void ManageEnergyConsumption()
        {
            // Реализуйте логику управления энергопотреблением в сети датчиков
            // Включайте и выключайте датчики в зависимости от необходимости сбора данных
            // Регулируйте частоту передачи данных в зависимости от текущего уровня энергии

            // Пример реализации: выключение датчиков с низким уровнем энергии
            if (Energy < 5.0)
            {
                StopCollectingData();
                StopTransmittingData();
                StopReceivingData();
                Sleep();
            }
        }
        public void StopReceivingData()
        {
            if (State == NodeState.ReceivingData)
            {
                State = NodeState.Active;
                Debug.WriteLine($"Node {Id} stopped receiving data.");
            }
            else
            {
                Debug.WriteLine($"Node {Id} is not currently receiving data.");
            }
        }

        public void StopSendingData()
        {
            if (State == NodeState.SendingData)
            {
                State = NodeState.Active;
                Debug.WriteLine($"Node {Id} stopped sending data.");
            }
            else
            {
                Debug.WriteLine($"Node {Id} is not currently sending data.");
            }
        }
        public void StopCollectingData()
        {
            if (State == NodeState.CollectingData)
            {
                State = NodeState.Active;
                Debug.WriteLine($"Node {Id} stopped collecting data.");
            }
            else
            {
                Debug.WriteLine($"Node {Id} is not currently collecting data.");
            }
        }
        public void ProcessState()
        {
            switch (State)
            {
                case NodeState.Awake:
                    // Выполните необходимые действия в состоянии Awake
                    break;
                case NodeState.Active:
                    // Выполните необходимые действия в состоянии Active
                    break;
                case NodeState.Sleep:
                    // Выполните необходимые действия в состоянии Sleep
                    break;
                case NodeState.TransmittingData:
                    // Выполните необходимые действия в состоянии TransmittingData
                    break;
                case NodeState.ReceivingData:
                    // Выполните необходимые действия в состоянии ReceivingData
                    break;
                case NodeState.SendingData:
                    // Выполните необходимые действия в состоянии SendingData
                    break;
                case NodeState.CollectingData:
                    // Выполните необходимые действия в состоянии CollectingData
                    break;
                default:
                    break;
            }
        }
        public void GoToSleep()
        {
            this.State = NodeState.Sleep;
        }
        public Sensor GetNearestSensor()
        {
            double minDistance = double.MaxValue;
            Sensor nearestSensor = null;

            foreach (var node in ConnectedNodes)
            {
                if (node is Sensor sensor && sensor.State != NodeState.Sleep)
                {
                    double distance = MathOperations.CalculateDistance(Location, sensor.Location);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestSensor = sensor;
                    }
                }
            }

            return nearestSensor;
        }

        public bool IsConnected(Node otherNode)
        {
            return ConnectedNodes.Contains(otherNode);
        }

        public void Disconnect(Node otherNode)
        {
            ConnectedNodes.Remove(otherNode);
            otherNode.ConnectedNodes.Remove(this);
        }
        public void TransmitData(Node targetNode, string data)
        {
            if (State == NodeState.Sleep)
            {
                Debug.WriteLine($"Node {Id} is sleeping and cannot transmit data.");
                return;
            }

            if (State == NodeState.Active)
            {
                // Переходим в состояние SendingData перед передачей данных
                StartSendingData();

                // Уменьшаем уровень энергии узла при передаче данных
                if (this is Sensor sensor)
                {
                    sensor.ConsumeEnergy(10); // Потребляем некоторое количество энергии
                }

                // Отправляем данные целевому узлу
                if (targetNode.ReceiveData(this, data))
                {
                    Debug.WriteLine($"Node {Id} successfully transmitted data to Node {targetNode.Id}.");
                }
                else
                {
                    // Ретрансляция данных через промежуточные узлы
                    var nearestNode = GetNearestNode();
                    if (nearestNode != null)
                    {
                        Debug.WriteLine($"Node {Id} is relaying data to Node {nearestNode.Id}.");
                        nearestNode.TransmitData(targetNode, data);
                    }
                    else
                    {
                        Debug.WriteLine($"Node {Id} failed to transmit data to Node {targetNode.Id}.");
                    }
                }

                // Переходим обратно в состояние Active после передачи данных
                StopSendingData();
            }
        }
        public Node GetNearestNode()
        {
            Node nearestNode = null;
            double shortestDistance = double.MaxValue;

            foreach (var connectedNode in ConnectedNodes)
            {
                double distance = MathOperations.CalculateDistance(Location, connectedNode.Location);
                if (distance < shortestDistance)
                {
                    nearestNode = connectedNode;
                    shortestDistance = distance;
                }
            }

            return nearestNode;
        }

        public bool ReceiveData(Node sourceNode, string data)
        {
            if (State == NodeState.Sleep)
            {
                Debug.WriteLine($"Node {Id} is asleep and cannot receive data.");
                return false;
            }

            if (State == NodeState.ReceivingData)
            {
                Debug.WriteLine($"Node {Id} is already receiving data.");
                return false;
            }

            StartReceivingData();

            // Process the received data
            Debug.WriteLine($"Node {Id} received data from Node {sourceNode.Id}: {data}");

            StopReceivingData();

            return true;
        }
        public void SendDataToBaseStation(string data)
        {
            if (State == NodeState.Sleep)
            {
                Debug.WriteLine($"Node {Id} is asleep and cannot send data to the base station.");
                return;
            }

            if (State == NodeState.SendingData)
            {
                Debug.WriteLine($"Node {Id} is already sending data to the base station.");
                return;
            }

            StartSendingData();

            // Find the base station among the connected nodes
            BaseStation baseStation = ConnectedNodes.OfType<BaseStation>().FirstOrDefault();
            if (baseStation != null)
            {
                if (baseStation.ReceiveData(this, data))
                {
                    Debug.WriteLine($"Node {Id} successfully sent data to the base station.");
                }
                else
                {
                    Debug.WriteLine($"Node {Id} failed to send data to the base station.");
                }
            }
            else
            {
                Debug.WriteLine($"Node {Id} is not connected to a base station.");
            }

            StopSendingData();
        }
    }
}