using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using System.Runtime.InteropServices;
namespace assets.OSMReader
{

    public class OSMBase
    {
        public long ID;
        protected T GetAttribute<T>(string attName, XmlAttributeCollection attributes)
        {
            string strValue = attributes[attName].Value;
            return (T)Convert.ChangeType(strValue, typeof(T));
        }
    }

    public class OsmNode : OSMBase
    {
        public float ele;
        public float Latitude;
        public float Longitude;
        public float local_x;
        public float local_y;
        public static implicit operator Vector3(OsmNode node)
        {
            return new Vector3(node.local_x, node.ele, node.local_y);
        }
        public OsmNode() { }

        [DllImport("GeographicWarpper")]
        extern static void UTMUPS_Forward(double lat, double lon, out int zone, out bool northp, out double x, out double y);
        public OsmNode(XmlNode node)
        {
            ID =GetAttribute<long>("id", node.Attributes);
            Latitude = GetAttribute<float>("lat", node.Attributes);
            Longitude = GetAttribute<float>("lon", node.Attributes);
            if (OSMManager.Instance.isLongitude)
            {
                UTMUPS_Forward(Latitude, Longitude, out int zone, out bool northp, out double x, out double y);
                x %= 1e5;
                y %= 1e5;
                Latitude = (float)x;
                Longitude = (float)y;
            }
            XmlNodeList tags = node.SelectNodes("tag");
            foreach (XmlNode t in tags)
            {
                string key = GetAttribute<string>("k", t.Attributes);
                if (key == "ele")
                {
                    ele = GetAttribute<float>("v", t.Attributes);
                }
                else if (key == "local_x")
                {
                    local_x = GetAttribute<float>("v", t.Attributes);
                }

                else if (key == "local_y")
                {
                    local_y = GetAttribute<float>("v", t.Attributes);
                }
            }
        }
        public Vector3 GetPosition()
        {
            //return new Vector3(local_x, ele, local_y);
            return new Vector3(Latitude, ele, Longitude);
        }
    }
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
            XmlNodeList tags = node.SelectNodes("tag");
            foreach (XmlNode t in tags)
            {
                string key = GetAttribute<string>("k", t.Attributes);
                switch (key)
                {
                    case "type":
                        OSMWayType = (WayType)Enum.Parse(typeof(WayType), GetAttribute<string>("v", t.Attributes));
                        break;
                    case "subtype":
                        OSMSubType = (WaySubType)Enum.Parse(typeof(WaySubType), GetAttribute<string>("v", t.Attributes));
                        break;
                    case "height":
                        Height = GetAttribute<float>("v", t.Attributes);
                        break;
                    case "building:levels":
                        Height = 3.0f * GetAttribute<float>("v", t.Attributes);
                        break;
                    case "building":
                        IsBuilding = GetAttribute<string>("v", t.Attributes) == "yes";
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

    public class Relation : OSMBase
    {
        public enum RelationType
        {
            lanelet = 0,
            regulatory_element = 1
        }
        public enum RelationSubType
        {
            road = 0,
            traffic_light = 1,
            traffic_sign = 2
        }
        public enum TurnDirection
        {
            left = 0,
            right = 1
        }
        public RelationType type;
        public RelationSubType subType;
        public TurnDirection turn_direction;
        public List<Member> menbers;
        public Relation()
        {
            menbers = new List<Member>();
        }

    }

    public class Member
    {
        public enum MemberType
        {
            way = 0,
            relation = 1
        }
        public enum RoleType
        {
            left = 0,
            right = 1,
            regulatory_element = 2,
            refers = 3,
            ref_line = 4

        }
        public MemberType menberType;
        public int refID;
        public RoleType roleType;
    }

    public class OSMData
    {
        public string name;
        public List<OsmNode> nodes;
        public List<OSMWay> ways;
        public List<Relation> relations;
        public OSMData()
        {
            nodes = new List<OsmNode>();
            ways = new List<OSMWay>();
            relations = new List<Relation>();
        }
    }
}