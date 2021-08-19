using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using MapRenderer;
using System.Xml;
using System;

namespace assets.OSMReader
{

    public class OSMManager : MonoBehaviour
    {
        public MapManager mapManager;
        public static OSMManager Instance;
        public bool isLongitude = false;

        OSMData oSMData;
        public void Start()
        {
            mapManager = GetComponent<MapManager>();
            MapManager.Instance.OnGetOSM += ReadOSMWithStr;
        }
        public Map map;
        private void Awake()
        {
            Instance = this;
        }



        public void BuildMap(OSMData data)
        {
            if (data == null)
            {
                Debug.LogError("data is null");
            }
            map = mapManager.GetOrCreateMap(data.name);
            foreach (OsmNode node in data.nodes)
            {
                map.AddPoint(node.ID.ToString(), node.GetPosition());
            }
            foreach (OSMWay way in data.ways)
            {
                List<Point> points = new List<Point>();
                foreach (int node in way.NodeIDs)
                {
                    if (map.Elements.TryGetValue(node.ToString(), out MapElement element))
                    {
                        if (element is Point point)
                        {
                            points.Add(point);
                        }
                        else
                        {
                            Debug.LogError("map error");
                        }
                    }
                }
                //if (way.IsBoundary)
                //{
                //    Area area= new GameObject(way.ID.ToString()).AddComponent<Area>();
                //    area.points = points;
                //    area.areaColor = Color.green;
                //    map.AddArea(area);
                //    continue;
                //}
                switch (way.OSMWayType)
                {
                    case OSMWay.WayType.line_thin:
                        Line line = map.AddLine(way.ID.ToString());
                        line.points = points;
                        line.color = Color.white;
                        break;
                    case OSMWay.WayType.stop_line:
                        Line_Stop line_stop = map.AddLine_Stop(way.ID.ToString());
                        line_stop.points = points;
                        line_stop.color = Color.red;
                        break;
                    case OSMWay.WayType.traffic_light:
                        Sign_TrafficLight sign_traffic = map.AddTrafficLight(way.ID.ToString());
                        if (points.Count != 2)
                        {
                            Debug.LogError("count error");
                        }
                        Vector3 pos1 = points[0].position / 2 + points[1].position / 2;
                        sign_traffic.position = pos1;
                        float distance = Vector3.Distance(points[0].position, points[1].position);
                        sign_traffic.width = distance;
                        Vector3 vector = points[1].position - points[0].position;
                        Quaternion quaternion = Quaternion.AngleAxis(90, new Vector3(0, 1, 0)) * Quaternion.Euler(vector);
                        sign_traffic.yaw = quaternion.eulerAngles.y;
                        break;
                    case OSMWay.WayType.traffic_sign:
                        //Sign sign = new GameObject(way.ID.ToString()).AddComponent<Sign>();
                        //if (points.Count != 2)
                        //{
                        //    Debug.LogError("count error");
                        //}
                        //Vector3 pos1 = points[0].position/2+points[1].position/2;
                        //sign.position = pos1;
                        //float distance =Vector3.Distance( points[0].position, points[1].position);
                        //sign.width = sign.height = distance;
                        //map.AddSign(sign);
                        break;
                    case OSMWay.WayType.area:
                        if (way.OSMSubType == OSMWay.WaySubType.parking_spot)
                        {
                            Area area = map.AddArea(way.ID.ToString());
                            area.points = points;
                            area.color = Color.yellow;
                        }
                        break;
                    case OSMWay.WayType.building:
                        Structure structure = map.AddStructrue(way.ID.ToString());
                        structure.points = points;
                        structure.height = way.Height;
                        structure.color = new Color(0, 1, 0, 0.2f);
                        break;
                    default:
                        break;
                }

            }
            foreach (Relation relation in data.relations)
            {
                if (relation.subType == Relation.RelationSubType.traffic_light)
                {
                    Line_Stop lineStopTemp = null;
                    Sign_TrafficLight trafficTemp = null;
                    foreach (Member member in relation.menbers)
                    {
                        if (map.Elements.TryGetValue(member.refID.ToString(), out MapElement element))
                        {
                            if (member.roleType == Member.RoleType.ref_line && element is Line_Stop lineStop)
                            {
                                lineStopTemp = lineStop;
                            }
                            else if (member.roleType == Member.RoleType.refers && element is Sign_TrafficLight trafficLight)
                            {
                                trafficTemp = element.GetComponent<Sign_TrafficLight>();
                            }
                        }
                    }
                    if (lineStopTemp != null && trafficTemp != null)
                    {
                        lineStopTemp.sign_Stop = trafficTemp;
                    }
                }
            }
            map.UpdateRenderer();
            map.BuildComplete();
        }
        //public void ReadOSMWithPath(string path, out OSMData data)
        //{
        //    if (File.Exists(path))
        //    {
        //        XmlDocument xml = new XmlDocument();
        //        xml.Load(path);
        //        ReadOSMWithXmlDoc(xml, out data);
        //    }
        //    else
        //    {
        //        data = null;
        //        Debug.LogError("No Document");
        //    }
        //}
        public void ReadOSMWithStr(string xmlStr)
        {
            xmlStr = System.Text.RegularExpressions.Regex.Replace(xmlStr, "^[^<]", "");
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlStr);
            oSMData = ReadOSMWithXmlDoc(xml);
            BuildMap(oSMData);
        }
        public OSMData ReadOSMWithXmlDoc(XmlDocument xmlDoc)
        {
            OSMData data = new OSMData();
            XmlNode OSMNode = xmlDoc.SelectSingleNode("osm");
            data.name = xmlDoc.Name;
            XmlNodeList nodeXmlList = OSMNode.SelectNodes("node");
            Debug.Log(nodeXmlList.Count);
            foreach (XmlNode nodeNode in nodeXmlList)
            {
                data.nodes.Add(new OsmNode(nodeNode));
            }

            XmlNodeList wayXmlList = OSMNode.SelectNodes("way");
            foreach (XmlNode wayNode in wayXmlList)
            {
                data.ways.Add(new OSMWay(wayNode));
            }

            return data;
        }
    }
}
