using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static WirelessSensorNetworkSimpleSecond.Node;

namespace WirelessSensorNetworkSimpleSecond
{
    public class NetworkViewControl : Control
    {
        private Network network;
        private List<EnvironmentEvent> events;
        private List<AnimationData> animations;

        public NetworkViewControl()
        {
            DoubleBuffered = true; // Включаем двойную буферизацию для сглаживания отрисовки
            events = new List<EnvironmentEvent>();
            animations = new List<AnimationData>();
        }

        public Network Network
        {
            get { return network; }
            set
            {
                network = value;
                Refresh();
            }
        }
        public void AddAnimation(AnimationData animationData)
        {
            animations.Add(animationData);
        }
        public event EventHandler<NodeClickEventArgs> NodeClick;

        public class NodeClickEventArgs : EventArgs
        {
            public Node ClickedNode { get; }

            public NodeClickEventArgs(Node clickedNode)
            {
                ClickedNode = clickedNode;
            }
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            Point clickedPoint = e.Location;
            Node clickedNode = GetNodeAt(clickedPoint);
            NodeClick?.Invoke(this, new NodeClickEventArgs(clickedNode));
        }
        private Node GetNodeAt(Point location)
        {
            foreach (var sensor in network.Sensors)
            {
                if (IsPointInNode(location, sensor))
                {
                    return sensor;
                }
            }

            foreach (var baseStation in network.BaseStations)
            {
                if (IsPointInNode(location, baseStation))
                {
                    return baseStation;
                }
            }

            return null;
        }

        private bool IsPointInNode(Point point, Node node)
        {
            const int nodeSize = 20;
            Rectangle nodeBounds;

            if (node is Sensor)
            {
                nodeBounds = new Rectangle(node.Location, new Size(nodeSize, nodeSize));
            }
            else if (node is BaseStation)
            {
                nodeBounds = new Rectangle(node.Location, new Size(nodeSize, nodeSize));
            }
            else
            {
                return false;
            }

            return nodeBounds.Contains(point);
        }
        public void SetNetwork(Network network)
        {
            this.network = network;
        }

        public void SetEvents(List<EnvironmentEvent> events)
        {
            this.events = events;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (network != null)
            {
                Graphics graphics = e.Graphics;
                graphics.Clear(Color.White);
                double scaleFactor = Math.Min(Width, Height) / 100.0;

                DrawNodes(graphics, scaleFactor);
                DrawConnections(graphics, scaleFactor);
                DrawEvents(graphics, scaleFactor);
                DrawAnimations(graphics, scaleFactor);
            }
        }
        private void DrawAnimations(Graphics graphics, double scaleFactor)
        {
            foreach (var animationData in animations)
            {
                DrawAnimation(graphics, animationData, scaleFactor);
            }
        }
        private void DrawAnimation(Graphics graphics, AnimationData animationData, double scaleFactor)
        {
            Point sourcePoint = ScalePoint(animationData.SourceNode.Location, scaleFactor);
            Point targetPoint = ScalePoint(animationData.TargetNode.Location, scaleFactor);

            Pen animationPen = new Pen(Color.Black, 2);
            graphics.DrawLine(animationPen, sourcePoint, targetPoint);

            // Отобразить данные анимации
            string animationText = animationData.EventData;
            Point textPosition = new Point((sourcePoint.X + targetPoint.X) / 2, (sourcePoint.Y + targetPoint.Y) / 2);
            DrawText(graphics, animationText, textPosition);
        }
        private void DrawText(Graphics graphics, string text, Point position)
        {
            Font textFont = new Font(FontFamily.GenericSansSerif, 10);
            Brush textBrush = Brushes.Black;

            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            graphics.DrawString(text, textFont, textBrush, position.X, position.Y, format);
        }

        private Point ScalePoint(Point point, double scaleFactor)
        {
            int scaledX = (int)(point.X * scaleFactor);
            int scaledY = (int)(point.Y * scaleFactor);
            return new Point(scaledX, scaledY);
        }
        private void DrawNodes(Graphics graphics, double scaleFactor)
        {
            foreach (var sensor in network.Sensors)
            {
                DrawNode(graphics, sensor.Location, scaleFactor, sensor);
            }

            foreach (var baseStation in network.BaseStations)
            {
                DrawNode(graphics, baseStation.Location, scaleFactor, baseStation);
            }
        }

        private void DrawNode(Graphics graphics, Point location, double scaleFactor, Node node)
        {
            int size = 20;
            Color nodeColor;

            switch (node.State)
            {
                case NodeState.Sleep:
                    nodeColor = Color.Blue;
                    break;
                case NodeState.CollectingData:
                    nodeColor = Color.Green;
                    break;
                case NodeState.TransmittingData:
                    nodeColor = Color.Yellow;
                    break;
                case NodeState.SendingData:
                    nodeColor = Color.Red;
                    break;
                default:
                    nodeColor = Color.Blue;
                    break;
            }

            if (node is BaseStation)
            {
                graphics.FillRectangle(new SolidBrush(nodeColor), location.X, location.Y, size, size);
                DrawNodeNumber(graphics, location, node);
            }
            else if (node is Sensor)
            {
                graphics.FillEllipse(new SolidBrush(nodeColor), location.X, location.Y, size, size);
                DrawNodeNumber(graphics, location, node);
            }
        }
        private void DrawNodeNumber(Graphics graphics, Point location, Node node)
        {
            var numberFont = new Font("Arial", 8);
            var numberBrush = new SolidBrush(Color.White);

            string nodeNumber;
            if (node is BaseStation)
            {
                nodeNumber = (node as BaseStation).BaseStationId.ToString();
            }
            else if (node is Sensor)
            {
                nodeNumber = (node as Sensor).SensorId.ToString();
            }
            else
            {
                return;
            }

            graphics.DrawString(nodeNumber, numberFont, numberBrush, new PointF(location.X + 5, location.Y + 5));
        }

    private void DrawConnections(Graphics graphics, double scaleFactor)
        {
            foreach (var node in network.GetAllNodes())
            {
                foreach (var connectedNode in node.ConnectedNodes)
                {
                    DrawConnection(graphics, node.Location, connectedNode.Location, scaleFactor);
                }
            }
        }

        private void DrawConnection(Graphics graphics, Point location1, Point location2, double scaleFactor)
        {
            graphics.DrawLine(new Pen(Color.Black), location1.X, location1.Y, location2.X, location2.Y);

        }
        private void DrawEvents(Graphics graphics, double scaleFactor)
        {
            foreach (var ev in events)
            {
                DrawEvent(graphics, ev.Location, scaleFactor);
            }
        }

        private void DrawEvent(Graphics graphics, Point location, double scaleFactor)
        {
            int size = 10;
            Color eventColor = Color.Red;
            graphics.FillEllipse(new SolidBrush(eventColor), location.X, location.Y, size, size);

        }
    }
}
