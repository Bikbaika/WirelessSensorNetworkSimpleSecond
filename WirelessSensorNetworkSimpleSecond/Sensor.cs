using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WirelessSensorNetworkSimpleSecond
{
    public class Sensor : Node
    {
        public string Topology { get; set; }
        public BaseStation BaseStation { get; set; }
        public string Data { get; set; }
        public double SensingRange { get; set; }
        private DateTime lastActivityTime;
        public EnvironmentEvent environmentEvent { get; set; }
        public Network network { get; set; }
        public TaskScheduler Scheduler { get; set; }
        public double SoundLevel { get; set; }
        public double LightLevel { get; set; }
        public double HumidityLevel { get; set; }
        public int SensorId { get; set; }
        private static int nextSensorId = 1;
        public Sensor()
        {
            // Конструктор по умолчанию
            // инициализируем время последней активности временем создания сенсора
            lastActivityTime = DateTime.Now;
        }
        public Sensor(Point location) : base(location)
        {
            SensorId = nextSensorId++;
            // Дополнительная инициализация для класса Sensor
        }

        public Sensor(double energy, string topology) : base(energy)
        {
            SensorId=nextSensorId++;
            Topology = topology;
        }

        public Sensor(Point location, double energy, double sensingRange) : base(location, energy)
        {
            SensorId = nextSensorId++;
            this.State = NodeState.Sleep;
            this.SensingRange = sensingRange;
        }
        public void CollectEnvironmentData()
        {
            // Симуляция сбора данных о звуке, свете и влажности окружающей среды
            Random random = new Random();

            // Здесь можете использовать фактические датчики или другие алгоритмы для получения значений
            SoundLevel = random.NextDouble() * 100;
            LightLevel = random.NextDouble() * 100;
            HumidityLevel = random.NextDouble() * 100;

            // Дополнительная логика обработки собранных данных
            // Например, сохранение данных, анализ, реакция на определенные значения и т.д.
        }
        public Sensor GetNearestSensor()
        {
            Sensor nearestSensor = null;
            double shortestDistance = double.MaxValue;

            foreach (var connectedNode in ConnectedNodes)
            {
                if (connectedNode is Sensor sensor)
                {
                    double distance = MathOperations.CalculateDistance(Location, sensor.Location);
                    if (distance < shortestDistance)
                    {
                        nearestSensor = sensor;
                        shortestDistance = distance;
                    }
                }
            }

            return nearestSensor;
        }
        public void TryCollectData()
        {
            if (State == NodeState.Sleep)
            {
                // Проверка уровня энергии
                if (Energy < 10.0)
                {
                    Sleep();
                }
                else
                {
                    // Создаем задачу сбора данных
                    var task = new Task(() => CollectData(environmentEvent));

                    // Добавляем задачу в планировщик
                    ScheduleTask(task);

                    // Уменьшение уровня энергии
                    Energy -= 1.0;
                }
            }
        }
        public void ScheduleTask(Task task)
        {
            Scheduler.ScheduleTask(task);
        }
        public void ManageEnergyConsumption()
        {
            // Реализуйте логику управления энергопотреблением в сети датчиков
            // Включайте и выключайте датчики в зависимости от необходимости сбора данных
            // Регулируйте частоту передачи данных в зависимости от текущего уровня энергии
                  
    }
        public void CheckEnvironmentEvent(EnvironmentEvent environmentEvent)
        {
            if (CanSeeEvent(environmentEvent))
            {
                // Обработка обнаруженного события окружающей среды
                // Выполнение соответствующих действий на основе типа события и его параметров
                // Например:
                if (environmentEvent.Temperature > 30)
                {
                    // Реакция на высокую температуру
                    // ...
                }

                if (environmentEvent.Humidity > 80)
                {
                    // Реакция на высокую влажность
                    // ...
                }

                // Дополнительная логика обработки события
                // ...
            }
        }

        public void CollectData(EnvironmentEvent environmentEvent)
        {
            if (this.State == NodeState.Sleep && MathOperations.CalculateDistance(this.Location, environmentEvent.Location) < environmentEvent.Radius)
            {
                this.State = NodeState.CollectingData;
                this.Data = environmentEvent.Data;
                Debug.WriteLine($"Node {Id} collected data: {Data}");

                // Переходим обратно в состояние Sleep после сбора данных.
                this.Sleep();
            }
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
                targetNode.ReceiveData(this, data);

                // Переходим обратно в состояние Active после передачи данных
                StopSendingData();
            }
        }

        // Метод для потребления энергии.
        public void ConsumeEnergy(double amount)
        {
            this.Energy = Math.Max(0, this.Energy - amount);
            if (this.Energy == 0)
            {
                // Если у узла закончилась энергия, он переходит в состояние сна.
                this.State = NodeState.Sleep;
            }
        }
        private double CalculateEnergyConsumption(Point nearestSensorLocation)
        {
            double distance = MathOperations.CalculateDistance(Location, nearestSensorLocation);
            double energyConsumption;

            switch (Topology)
            {
                case "mesh":
                    energyConsumption = Data.Length * 1.5 * distance;
                    break;
                case "star":
                    energyConsumption = Data.Length * distance;
                    break;
                case "tree":
                    energyConsumption = Data.Length * 0.5 * distance;
                    break;
                default:
                    throw new ArgumentException("Invalid topology");
            }

            return energyConsumption;
        }

        public string CollectData()
        {
            // здесь должна быть реализация сбора данных
            return "collected data";
        }
        public bool CanSeeEvent(EnvironmentEvent e)
        {
            // Вычисляем расстояние от узла до местоположения события
            double distance = MathOperations.CalculateDistance(this.Location, e.Location);
              
            // Если расстояние меньше или равно sensingRange, то узел может "видеть" событие
            return distance <= this.SensingRange;
        }

        // метод возвращает ближайший узел
        public Node GetNearestNode()
        {
            // Этот метод должен возвращать узел, который является ближайшим к текущему узлу.
            // В этом примере мы просто возвращаем первый подключенный узел, но в реальной реализации
            // вы бы, вероятно, хотели рассчитать расстояние до каждого узла и вернуть самый близкий.
            return ConnectedNodes.FirstOrDefault();
        }
        // метод проверяет, прошло ли достаточно много времени с последней активности сенсора
        public bool NoActivityForSomeTime(int inactivityPeriod)
        {
            // Этот метод должен проверять, была ли активность на узле в течение определенного периода времени.
            // Вы можете определить "период времени бездействия", который считается достаточно долгим.
            // В этом примере мы просто проверяем, была ли активность на узле в последние 5 минут.
          //  var inactivityPeriod = TimeSpan.FromSeconds(10);
            return (int)(DateTime.Now - lastActivityTime).TotalSeconds > inactivityPeriod;
        }
    }
}