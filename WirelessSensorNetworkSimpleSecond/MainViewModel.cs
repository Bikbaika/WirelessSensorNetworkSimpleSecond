using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;

namespace WirelessSensorNetworkSimpleSecond
{


    public class MainViewModel
    {
        public MainViewModel()
        {
            TemperatureModel = new PlotModel { Title = "Temperature Over Time" };
            HumidityModel = new PlotModel { Title = "Humidity Over Time" };
            EnergyModel = new PlotModel { Title = "Energy Level Over Time" };

            TemperatureSeries = new LineSeries { Title = "Temperature", MarkerType = MarkerType.Circle };
            HumiditySeries = new LineSeries { Title = "Humidity", MarkerType = MarkerType.Circle };
            EnergySeries = new LineSeries { Title = "Energy", MarkerType = MarkerType.Circle };

            TemperatureModel.Series.Add(TemperatureSeries);
            HumidityModel.Series.Add(HumiditySeries);
            EnergyModel.Series.Add(EnergySeries);
        }

        public PlotModel TemperatureModel { get; private set; }
        public PlotModel HumidityModel { get; private set; }
        public PlotModel EnergyModel { get; private set; }

        public LineSeries TemperatureSeries { get; private set; }
        public LineSeries HumiditySeries { get; private set; }
        public LineSeries EnergySeries { get; private set; }

        public void UpdateData(List<Node> nodes)
        {
            // Clear existing data
            TemperatureSeries.Points.Clear();
            HumiditySeries.Points.Clear();
            EnergySeries.Points.Clear();

            // Assume the nodes list is sorted by time
            for (int i = 0; i < nodes.Count; i++)
            {
                TemperatureSeries.Points.Add(new DataPoint(i, nodes[i].Temperature));
                HumiditySeries.Points.Add(new DataPoint(i, nodes[i].Humidity));
                EnergySeries.Points.Add(new DataPoint(i, nodes[i].Energy));
            }

            // Update the plot models
            TemperatureModel.InvalidatePlot(true);
            HumidityModel.InvalidatePlot(true);
            EnergyModel.InvalidatePlot(true);
        }
    }
}
