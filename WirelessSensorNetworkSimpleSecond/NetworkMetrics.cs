using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WirelessSensorNetworkSimpleSecond.Node;

namespace WirelessSensorNetworkSimpleSecond
{
    public class NetworkMetrics
    {
        public int TotalDataTransmitted { get; private set; }
        public int SuccessfulTransmissions { get; private set; }
        public int FailedTransmissions { get; private set; }
        public TimeSpan TotalTransmissionDelay { get; private set; }
        public double TotalEnergyConsumption { get; private set; }
        public double AverageEnergyConsumptionPerNode { get; private set; }
        public Network network { get; private set; }
        public List<Node> FailedNodes { get; private set; }

        public void DetectFailures(Network network)
        {
            FailedNodes = new List<Node>();

            foreach (var node in network.GetAllNodes())
            {
                if (!IsNodeResponsive(node))
                {
                    FailedNodes.Add(node);
                }
            }
        }

        private bool IsNodeResponsive(Node node)
        {
            // Реализуйте здесь механизм проверки связи с узлами
            // Верните true, если узел отвечает на запросы, и false в противном случае
            // Это может включать проверку времени ответа, проверку состояния узла и т. д.

            // Пример реализации: проверка состояния узла
            return node.State != NodeState.Failed;
        }

        public void UpdateEnergyConsumption(Node node, double consumedEnergy)
        {
            TotalEnergyConsumption += consumedEnergy;
            AverageEnergyConsumptionPerNode = TotalEnergyConsumption / network.GetAllNodes().Count;
        }

        public void UpdateMetrics(DataPacket packet, bool transmissionSuccessful, TimeSpan transmissionDelay)
        {
            TotalDataTransmitted += packet.Size;
            TotalTransmissionDelay += transmissionDelay;

            if (transmissionSuccessful)
            {
                SuccessfulTransmissions++;
            }
            else
            {
                FailedTransmissions++;
            }
        }

        public void ResetStatistics()
        {
            TotalDataTransmitted = 0;
            SuccessfulTransmissions = 0;
            FailedTransmissions = 0;
            TotalTransmissionDelay = TimeSpan.Zero;
            TotalEnergyConsumption = 0;
            AverageEnergyConsumptionPerNode = 0;
        }
        public void DetectFailures()
        {
            // Реализуйте здесь механизм обнаружения отказов, например, проверку связи с узлами и обновление статуса работоспособности.
        }
    }
}
