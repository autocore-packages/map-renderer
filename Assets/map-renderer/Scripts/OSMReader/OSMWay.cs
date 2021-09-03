using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace assets.OSMReader
{

    public class OSMWay : OSMBase
    {
        public bool Visible { get; private set; }
        public List<long> NodeIDs { get; private set; }
        public bool IsBoundary { get; private set; }
        public bool IsBuilding { get; private set; }
        public bool IsRoad { get; private set; }
        public float Height { get; private set; }

        public enum WayType
        {
            line_thin,
            stop_line,
            traffic_light,
            traffic_sign,
            area,
            building
        }
        public enum WaySubType
        {
            solid,
            dashed,
            stop_sign,
            parking,
            parking_spot,
            parking_access,
            Floors,
            Kerbs,
            Columns,
            Walls,
            Windows,
            Doors,
            junction
        }
        public WayType OSMWayType { get; private set; }
        public WaySubType OSMSubType { get; private set; }
        public float height;
        public OSMWay()
        {
            NodeIDs = new List<long>();
        }

        public OSMWay(XmlNode node)
        {
            NodeIDs = new List<long>();
            ID = GetAttribute<long>("id", node.Attributes);
            Visible = GetAttribute<bool>("visible", node.Attributes);
            XmlNodeList nds = node.SelectNodes("nd");

            foreach (XmlNode n in nds)
            {
                long refNo = GetAttribute<long>("ref", n.Attributes);
                NodeIDs.Add(refNo);
            }
            if (NodeIDs.Count >= 3)
            {
                IsBoundary = NodeIDs[0] == NodeIDs[NodeIDs.Count - 1];
            }
            Height = 10.0f;

            ReadTags(node);
            foreach (Tag t in Tags)
            {
                switch (t.Key)
                {
                    case "type":
                        OSMWayType = (WayType)Enum.Parse(typeof(WayType),t.Value);
                        break;
                    case "subtype":
                        OSMSubType = (WaySubType)Enum.Parse(typeof(WaySubType), t.Value);
                        break;
                    case "height":
                        Height = float.Parse(t.Value);
                        break;
                    case "building:levels":
                        Height = 3.0f * float.Parse(t.Value);
                        break;
                    case "building":
                        Height = 10.0f;
                        break;
                    case "highway":
                        IsRoad = true;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
