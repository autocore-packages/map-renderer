using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;

namespace assets.OSMReader
{

    public class OsmNode : OSMBase
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public float Ele { get; set; }
        public float local_x;
        public float local_y;

        public static implicit operator Vector3(OsmNode node)
        {
            return new Vector3(node.local_x, node.Ele, node.local_y);
        }
        public OsmNode() { }

        [DllImport("GeographicWarpper")]
        extern static void UTMUPS_Forward(double lat, double lon, out int zone, out bool northp, out double x, out double y);
        public OsmNode(XmlNode node)
        {
            ID = GetAttribute<long>("id", node.Attributes);
            Latitude = GetAttribute<float>("lat", node.Attributes);
            Longitude = GetAttribute<float>("lon", node.Attributes);

            if (OSMManager.Instance.isLongitude)
            {
                UTMUPS_Forward(Latitude, Longitude, out int zone, out bool northp, out double x, out double y);
                x %= 1e5;
                y %= 1e5;
                local_x = (float)x;
                local_y = (float)y;
            }
            ReadTags(node);

            foreach (Tag tag in Tags)
            {
                if (tag.Key == "local_x")
                {
                    local_x = float.Parse(tag.Value);
                }
                else if (tag.Key == "ele")
                {
                    Ele = float.Parse(tag.Value);
                }
                else if (tag.Key == "local_y")
                {
                    local_y = float.Parse(tag.Value);
                }
            }
        }
        public Vector3 GetPosition()
        {
            return OSMManager.Instance.MapOrigin.FromGpsLocation(Latitude,Longitude);
            //return new Vector3(local_x, ele, local_y);
            //return new Vector3(Latitude, ele, Longitude);
        }
    }
}
