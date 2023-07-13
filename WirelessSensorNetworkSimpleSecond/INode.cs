using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WirelessSensorNetworkSimpleSecond
{
    public interface INode
    {
        Point Location { get; set; }
        List<INode> ConnectedNodes { get; set; }
        // other common properties and methods...
    }
}
