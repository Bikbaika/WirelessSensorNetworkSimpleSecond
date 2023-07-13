namespace WirelessSensorNetworkSimpleSecond
{
    public class AnimationData
    {
        public Node SourceNode { get; }
        public Node TargetNode { get; }
        public string EventData { get; }

        public AnimationData(Node sourceNode, Node targetNode, EnvironmentEvent eventItem)
        {
            SourceNode = sourceNode;
            TargetNode = targetNode;
            EventData = eventItem.Data;
        }
    }
}