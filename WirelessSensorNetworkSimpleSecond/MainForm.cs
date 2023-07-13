using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static WirelessSensorNetworkSimpleSecond.NetworkViewControl;
using static WirelessSensorNetworkSimpleSecond.Node;

namespace WirelessSensorNetworkSimpleSecond
{
    public partial class MainForm : Form
    {
        private Network network;
        private Node selectedNode;
        private Timer animationTimer;
        private MainViewModel viewModel;
        private List<EnvironmentEvent> events;
        private NetworkViewControl networkViewControl;
        private bool isExperimentRunning; // Флаг, указывающий на выполнение эксперимента
        private int remainingIterations; // Количество оставшихся итераций эксперимента
        private double nodeRadius; // Радиус действия узлов
        private int dataCollectionDelay; // Время задержки сбора данных
        private int dataCollectionFrequency; // Частота сбора данных
        private Logger logger;

        private Label lblModelTime;
        private TextBox txtSensorCount;
        private TextBox txtBaseStationCount;
        private ComboBox cmbTopology;
        private Button btnGenerateNetwork;
        private Button btnSaveData;
        private Button btnRunExperiment;
        private Label lblTotalEnergy;
        private Label lblAverageEnergy;
        private TextBox txtEnergyLevel;
        private Button btnUpdateEnergy;
        private TextBox txtSelectedNode;
        private Label lblSelectedNodeInfo;
        private TextBox txtIterations;
        private TextBox txtNodeRadius;
        private TextBox txtDataCollectionDelay;
        private TextBox txtDataCollectionFrequency;
        private Button btnAddNode;
        private Button btnDeleteNode;

        public int ModelTime { get; private set; }

        public MainForm()
        {
            // Инициализация логгера
            logger = new Logger("log.txt");
            InitializeComponent();
            InitializeViewModel();
            InitializeControls();
            InitializeNetwork();
            InitializeEventHandlers();
            InitializeAnimationTimer();
        }

        private void InitializeViewModel()
        {
            viewModel = new MainViewModel();
        }

        private void InitializeAnimationTimer()
        {
            animationTimer = new Timer();
            animationTimer.Interval = 100; // Обновляем анимацию каждые 100 мс.
            animationTimer.Tick += AnimationTimer_Tick;
        }

        private void InitializeControls()
        {
            txtSensorCount = CreateTextBox(new Point(20, 20));
            txtBaseStationCount = CreateTextBox(new Point(20, 50));
            cmbTopology = CreateComboBox(new Point(20, 80), new string[] { "mesh", "star", "tree" });
            btnGenerateNetwork = CreateButton(new Point(20, 110), "Generate Network", btnGenerateNetwork_Click);
            btnRunExperiment = CreateButton(new Point(20, 140), "Run Experiment", btnRunExperiment_Click);
            lblTotalEnergy = CreateLabel(new Point(20, 170));
            lblAverageEnergy = CreateLabel(new Point(20, 200));
            btnUpdateEnergy = CreateButton(new Point(20, 250), "Update Energy", BtnUpdateEnergy_Click);
            txtSelectedNode = CreateTextBox(new Point(20, 280));
            txtEnergyLevel = CreateTextBox(new Point(20, 310));
            txtIterations = CreateTextBox(new Point(20, 420));
            btnSaveData = CreateButton(new Point(20, 330), "Save data", btnSaveData_Click);
            lblModelTime = CreateLabel(new Point(20, 360));
            txtSelectedNode.Multiline = true;
            lblSelectedNodeInfo = CreateLabel(new Point(20, 390));
            txtNodeRadius = CreateTextBox(new Point(20, 440));
            txtDataCollectionDelay = CreateTextBox(new Point(20, 460));
            txtDataCollectionFrequency = CreateTextBox(new Point(20, 480));
            btnAddNode = CreateButton(new Point(20, 500), "Добавить узел", btnAddNode_Click);
            btnDeleteNode = CreateButton(new Point(20, 520), "Удалить узел", btnDeleteNode_Click);
            networkViewControl = new NetworkViewControl
            {
                Location = new Point(250, 20),
                Size = new Size(720, 480)
            };
            Controls.Add(btnAddNode);
            Controls.Add(btnDeleteNode);
            Controls.Add(lblModelTime);
            Controls.Add(lblSelectedNodeInfo);
            Controls.Add(txtSensorCount);
            Controls.Add(txtBaseStationCount);
            Controls.Add(cmbTopology);
            Controls.Add(btnGenerateNetwork);
            Controls.Add(btnRunExperiment);
            Controls.Add(lblTotalEnergy);
            Controls.Add(lblAverageEnergy);
            Controls.Add(btnUpdateEnergy);
            Controls.Add(txtEnergyLevel);
            Controls.Add(txtSelectedNode);
            Controls.Add(btnSaveData);
            Controls.Add(networkViewControl);
            Controls.Add(txtIterations);
            Controls.Add(txtNodeRadius);
            Controls.Add(txtDataCollectionDelay);
            Controls.Add(txtDataCollectionFrequency);
        }

        private void btnDeleteNode_Click(object sender, EventArgs e)
        {
            // Проверяем, выбран ли узел для удаления
            if (selectedNode != null)
            {
                // Удаляем выбранный узел из сети
                if (selectedNode is Sensor sensor)
                {
                    network.RemoveSensor(sensor);
                }
                else if (selectedNode is BaseStation baseStation)
                {
                    network.RemoveBaseStation(baseStation);
                }

                // Сбрасываем выбранный узел
                selectedNode = null;
                txtSelectedNode.Text = "No selected node";

                // Обновляем отображение сети
                networkViewControl.Network = network;
                networkViewControl.Refresh();
            }
        }

        private void btnAddNode_Click(object sender, EventArgs e)
        {
            // Открываем диалоговое окно для добавления узла
            using (var addNodeDialog = new AddNodeDialog())
            {
                if (addNodeDialog.ShowDialog() == DialogResult.OK)
                {
                    // Получаем данные о новом узле из диалогового окна
                    string nodeType = addNodeDialog.SelectedNodeType;
                    Point nodeLocation = addNodeDialog.SelectedNodeLocation;

                    // Добавляем новый узел в сеть
                    if (nodeType == "Sensor")
                    {
                        network.AddSensor(nodeLocation);
                    }
                    else if (nodeType == "BaseStation")
                    {
                        network.AddBaseStation(nodeLocation);
                    }

                    // Обновляем отображение сети
                    networkViewControl.Network = network;
                    networkViewControl.Refresh();
                }
            }
        }
        private void UpdateSelectedNodeInfo(Node node)
        {
            if (node != null)
            {
                txtSelectedNode.Text = $"Selected node: {node.Id}, Packets Sent: {node.PacketsSent}";
            }
            else
            {
                txtSelectedNode.Text = "No selected node";
            }
        }

        private void InitializeNetwork()
        {
            network = new Network();
        }

        private void InitializeEventHandlers()
        {
            networkViewControl.NodeClick += NetworkViewControl_NodeClick;
        }

        private TextBox CreateTextBox(Point location)
        {
            return new TextBox { Location = location };
        }

        private ComboBox CreateComboBox(Point location, string[] items)
        {
            var comboBox = new ComboBox { Location = location };
            comboBox.Items.AddRange(items);
            return comboBox;
        }
        private Button CreateButton(Point location, string text, EventHandler clickEvent)
        {
            var button = new Button { Text = text, Location = location };
            button.Click += clickEvent;
            return button;
        }

        private Label CreateLabel(Point location)
        {
            return new Label { Location = location };
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            IncrementModelTime();
            networkViewControl.Refresh();
        }

        public void IncrementModelTime()
        {
            ModelTime++;
            lblModelTime.Text = $"Model Time: {ModelTime}"; // Обновляем текст с текущим модельным временем
        }
        private void NetworkViewControl_NodeClick(object sender, NodeClickEventArgs e)
        {
            if (e.ClickedNode != null && e.ClickedNode is Node node)
            {
                selectedNode = e.ClickedNode;
                if (selectedNode is BaseStation baseStation)
                {
                    txtSelectedNode.Text = $"Selected node: {baseStation.BaseStationId}, узел на позиции {selectedNode.Location}, состояние: {selectedNode.State}.";
                }
                else if (selectedNode is Sensor sensor)
                {
                    txtSelectedNode.Text = $"Selected node: {sensor.SensorId}, узел на позиции {selectedNode.Location}, состояние: {selectedNode.State}.";
                }
                            }
            else
            {
                selectedNode = null;
                txtSelectedNode.Text = "No selected node";
            }
            logger.Log($"Клик на узле: {e.ClickedNode.Id}");
        }

        private void btnSaveData_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files|*.txt";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveFileDialog.FileName;
                network.WriteNetworkDataToFile(fileName);
                MessageBox.Show("Network data saved successfully.", "Save Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void SaveNetworkData()
        {
            string fileName = "NetworkData.txt";
            network.WriteNetworkDataToFile(fileName);
        }

        private void BtnUpdateEnergy_Click(object sender, EventArgs e)
        {
            if (selectedNode != null && selectedNode is Sensor sensor && double.TryParse(txtEnergyLevel.Text, out double newEnergyLevel))
            {
                sensor.Energy = newEnergyLevel;
                txtSelectedNode.Text = $"Узел на позиции {sensor.Location}. Энергия: {sensor.Energy}";
            }
            else
            {
                MessageBox.Show("Выберите узел и введите корректное значение энергии", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGenerateNetwork_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtSensorCount.Text, out int sensorCount))
            {
                MessageBox.Show("Введите корректное значение для количества датчиков", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(txtBaseStationCount.Text, out int baseStationCount))
            {
                MessageBox.Show("Введите корректное значение для количества базовых станций", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string topology = cmbTopology.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(topology))
            {
                MessageBox.Show("Выберите топологию", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            double energyLevel = 100.0;
            network.GenerateNetwork(sensorCount, baseStationCount, topology, energyLevel);
            double averageEnergy = network.CalculateAverageEnergy();
            lblAverageEnergy.Text = "Среднее: " + averageEnergy.ToString("0.00");
            networkViewControl.Network = network;
            networkViewControl.Refresh();
        }

        private void btnRunExperiment_Click(object sender, EventArgs e)
        {
            if (isExperimentRunning)
            {
                // Если эксперимент уже выполняется, остановить его
                StopExperiment();
            }
            else
            {
                // Если эксперимент не выполняется, запустить его
                StartExperiment();
            }
        }
        private void StartExperiment()
        {
            if (!int.TryParse(txtIterations.Text, out int iterations))
            {
                MessageBox.Show("Введите корректное значение для количества итераций", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!double.TryParse(txtNodeRadius.Text, out double radius))
            {
                MessageBox.Show("Введите корректное значение для радиуса действия узлов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(txtDataCollectionDelay.Text, out int delay))
            {
                MessageBox.Show("Введите корректное значение для времени задержки сбора данных", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(txtDataCollectionFrequency.Text, out int frequency))
            {
                MessageBox.Show("Введите корректное значение для частоты сбора данных", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Инициализация всех узлов в активном состоянии
            foreach (var sensor in network.GetAllSensors())
            {
                sensor.State = NodeState.Active;
                sensor.StartCollectingData();
            }

            // Создание случайных событий
            events = new List<EnvironmentEvent>();
            Random random = new Random();
            int eventCount = random.Next(1, 10); // Генерируем случайное количество событий от 1 до 10
            for (int i = 0; i < eventCount; i++)
            {
                int x = random.Next(50, 350);
                int y = random.Next(0, 300);
                events.Add(new EnvironmentEvent { Data = $"Event {i + 1}", Location = new Point(x, y) });
            }
            networkViewControl.SetEvents(events);

            remainingIterations = iterations;
            isExperimentRunning = true;
            btnRunExperiment.Text = "Stop Experiment";

            // Запуск таймера для анимации и симуляции работы сети
            animationTimer.Tick += (s, args) =>
            {
                // Для каждого сенсора в сети
                foreach (var sensor in network.GetAllSensors())
                {
                    // Проверяем каждое событие
                    foreach (var eventItem in events)
                    {
                        // Если сенсор может видеть событие
                        if (sensor.CanSeeEvent(eventItem))
                        {
                            // Собираем данные события
                            sensor.CollectData(eventItem);

                            // Передаем данные ближайшему узлу
                            Sensor nearestSensor = sensor.GetNearestSensor();
                            if (nearestSensor != null)
                            {
                                sensor.TransmitData(nearestSensor, eventItem.Data);
                                // Увеличиваем счетчик отправленных пакетов
                                sensor.PacketsSent++;
                            }
                        }
                    }

                    // Если сенсор не активен в течение некоторого времени
                    if (sensor.NoActivityForSomeTime(dataCollectionDelay))
                    {
                        // Установить состояние сенсора в "спящий"
                        sensor.State = NodeState.Sleep;
                    }
                    else
                    {
                        sensor.State = NodeState.Active;
                    }

                    // Обновить цвет узла в зависимости от его состояния
                    switch (sensor.State)
                    {
                        case NodeState.Sleep:
                            sensor.NodeColor = Color.Blue;
                            break;
                        case NodeState.CollectingData:
                            sensor.NodeColor = Color.Green;
                            break;
                        case NodeState.TransmittingData:
                            sensor.NodeColor = Color.Yellow;
                            break;
                        case NodeState.SendingData:
                            sensor.NodeColor = Color.Red;
                            break;
                        default:
                            sensor.NodeColor = Color.Blue;
                            break;
                    }

                    // Обновить информацию на форме
                    txtSelectedNode.Text = $"Selected node: {sensor.Id}, узел на позиции {sensor.Location}, состояние: {sensor.State}.";
                    txtEnergyLevel.Text = sensor.Energy.ToString();
                    lblAverageEnergy.Text = "Среднее: " + network.CalculateAverageEnergy().ToString("0.00");
                    lblTotalEnergy.Text = "Всего: " + network.GetTotalEnergyConsumption().ToString("0.00");
                }

                // Обновить отображение сети
                networkViewControl.Refresh();

                remainingIterations--;
                if (remainingIterations == 0)
                {
                    StopExperiment();
                }
            };

            animationTimer.Start();
        }
        private void StopExperiment()
        {
            animationTimer.Stop();
            isExperimentRunning = false;
            btnRunExperiment.Text = "Run Experiment";
        }
        private void AnimateDataTransmission(Node sourceNode, EnvironmentEvent eventItem)
        {
            Sensor nearestSensor = sourceNode.GetNearestSensor();
            if (nearestSensor != null)
            {
                // Анимация движения данных от исходного узла к ближайшему сенсору
                AnimateDataTransmission(sourceNode, nearestSensor, eventItem);
            }
        }
        private void AnimateDataTransmission(Node sourceNode, Node targetNode, EnvironmentEvent eventItem)
        {
            // Создание анимации движения данных
            AnimationData animationData = new AnimationData(sourceNode, targetNode, eventItem);

            // Добавление анимации в контрол отображения сети
            networkViewControl.AddAnimation(animationData);
        }
        private void UpdateNodeColor(Node node)
        {
            switch (node.State)
            {
                case Node.NodeState.Sleep:
                    node.NodeColor = Color.Blue;
                    break;
                case Node.NodeState.CollectingData:
                    node.NodeColor = Color.Green;
                    break;
                case Node.NodeState.TransmittingData:
                    node.NodeColor = Color.Yellow;
                    break;
                case Node.NodeState.SendingData:
                    node.NodeColor = Color.Red;
                    break;
                default:
                    node.NodeColor = Color.Blue;
                    break;
            }
        }
    }

}

