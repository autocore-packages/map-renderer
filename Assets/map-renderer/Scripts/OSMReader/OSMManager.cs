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
        public MapOrigin MapOrigin;

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
                map.AddPoint(node.ID.ToString(), new Vector3(node.local_x, node.Ele, node.local_y)) ;
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
                        line.color.a = 0.3f;
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

                            Line areLine = map.AddLine(way.ID.ToString() + "border");
                            areLine.points = points;
                            areLine.color = Color.white;
                            areLine.color.a = 0.4f;
                        }
                        else if (way.OSMSubType == OSMWay.WaySubType.parking_access)
                        {
                            Area area = map.AddArea(way.ID.ToString());
                            area.points = points;
                            area.color = Color.green;

                            Line areLine = map.AddLine(way.ID.ToString()+"border");
                            areLine.points = points;
                            areLine.color = Color.white;
                            areLine.color.a = 0.4f;
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
            foreach (OSMRelation relation in data.relations)
            {
                switch (relation.relationType)
                {
                    case OSMRelation.RelationType.lanelet:
                        if (relation.relationSubType == OSMRelation.RelationSubType.lane)
                        {
                            Area_Lane area_Lane = map.AddArea_Lane(relation.ID.ToString());
                            foreach (Member member in relation.menbers)
                            {
                                if (member.roleType == Member.RoleType.left)
                                {
                                    if (map.Elements.TryGetValue(member.refID.ToString(),out MapElement mapElement))
                                    {
                                        if(mapElement is Line lineL)
                                        {
                                            area_Lane.leftPoints = lineL.points;
                                        }
                                        else
                                        {
                                            Debug.Log(member.refID.ToString() + "not fond");
                                        }
                                    }
                                    else
                                    {
                                        Debug.Log(member.refID.ToString()+"not fond");
                                    }
                                }
                                else if (member.roleType == Member.RoleType.right)
                                {
                                    if (map.Elements.TryGetValue(member.refID.ToString(), out MapElement mapElement))
                                    {
                                        if (mapElement is Line lineR)
                                        {
                                            area_Lane.rightPoints = lineR.points;
                                        }
                                        else
                                        {
                                            Debug.Log(member.refID.ToString() + "not fond");
                                        }
                                    }
                                    else
                                    {
                                        Debug.Log(member.refID.ToString() + "not fond");
                                    }
                                }
                            }
                            area_Lane.color = Color.green;
                            area_Lane.color.a = 0.2f;
                        }
                        break;
                    case OSMRelation.RelationType.regulatory_element:
                        break;
                    case OSMRelation.RelationType.multipolygon:
                        if (relation.relationSubType == OSMRelation.RelationSubType.parking_spot)
                        {
                            Area area = map.AddArea(relation.ID.ToString()+"r");
                            foreach (Member member in relation.menbers)
                            {
                                if (member.roleType == Member.RoleType.outer)
                                {
                                    if (map.Elements.TryGetValue(member.refID.ToString(), out MapElement mapElement))
                                    {
                                        if (mapElement is Line a)
                                        {
                                            area.points.AddRange(a.points);
                                        }
                                        else
                                        {
                                            Debug.Log(member.refID.ToString() + "not fond");
                                        }
                                    }
                                    else
                                    {
                                        Debug.Log(member.refID.ToString() + "not fond");
                                    }
                                }
                            }
                            area.color = Color.green;
                            area.color.a = 0.2f;
                        }
                        break;
                    default:
                        break;
                }
                if (relation.relationSubType == OSMRelation.RelationSubType.traffic_light)
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
        public void ReadOSMWithStr(string xmlStr)
        {
            MapOrigin = MapOrigin.Find();
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
            foreach (XmlNode nodeNode in nodeXmlList)
            {
                data.nodes.Add(new OsmNode(nodeNode));
            }
            if (data.nodes.Count > 1)
            {
                OsmNode originNode = data.nodes[0];
                double latitude, longitude;
                longitude = originNode.Longitude;
                latitude = originNode.Latitude;
                int zoneNumber = MapOrigin.GetZoneNumberFromLatLon(latitude, longitude);
                MapOrigin.UTMZoneId = zoneNumber;
                double northing, easting;
                MapOrigin.FromLatitudeLongitude(latitude, longitude, out northing, out easting);

                MapOrigin.OriginNorthing = northing;
                MapOrigin.OriginEasting = easting;
            }
            XmlNodeList wayXmlList = OSMNode.SelectNodes("way");
            foreach (XmlNode wayNode in wayXmlList)
            {
                data.ways.Add(new OSMWay(wayNode));
            }
            XmlNodeList relationXmlList = OSMNode.SelectNodes("relation");
            foreach (XmlNode relationNode in relationXmlList)
            {
                data.relations.Add(new OSMRelation(relationNode));
            }

            return data;
        }
        Vector3 GetVector3FromNode(OsmNode node)
        {
            double lat = (double)node.Latitude;
            double lon = (double)node.Longitude;
            double northing, easting;

            MapOrigin.FromLatitudeLongitude(lat, lon, out northing, out easting);
            Vector3 positionVec = MapOrigin.FromNorthingEasting(northing, easting); // note here y=0 in vec

            if (node.Tags?.Count > 0)
            {
                foreach (var item in node.Tags)
                {
                    if (item.Key == "ele")
                    {
                        var y = float.Parse(item.Value);
                        positionVec.y = y;
                    }
                }
            }

            return positionVec;
        }
    }
}
