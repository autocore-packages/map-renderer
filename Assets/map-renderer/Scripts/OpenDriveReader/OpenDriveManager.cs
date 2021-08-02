using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using MapRenderer;
using System.Xml;
using System;
using System.Xml.Serialization;
using System.Text;

namespace assets.OpenDriveReader
{
    public class OpenDriveManager : MonoBehaviour
    {
        public MapManager mapManager;
        public static OpenDriveManager Instance;
        private int pointIndex=0;
        public Map map;
        public int PointIndex
        {
            get
            {
                return pointIndex++;
            }
        }
        private int lineIndex = 0;
        public int LineIndex
        {
            get
            {
                return lineIndex++;
            }
        }
        [SerializeField]
        OpenDRIVE openDrive;
        public void Awake()
        {
            Instance = this;
        }

        public void Start()
        {
            map = MapManager.Instance.GetOrCreateMap("Map");
            mapManager = GetComponent<MapManager>();
            MapManager.Instance.OnGetOpenDrive += ReadOSMWithStr;
        }

        private void ReadOSMWithStr(string value)
        {
            Debug.Log(value);
            XmlSerializer serializer = new XmlSerializer(typeof(OpenDRIVE));
            using(var stream=new MemoryStream(Encoding.UTF8.GetBytes(value)))
            {
                openDrive = (OpenDRIVE)serializer.Deserialize(stream);
                BuildMap();
            }
        }


        private void BuildMap()
        {
            foreach (Road road in openDrive.roads)
            {
                BezierCurvePath bezierCurvePath= new GameObject("path").AddComponent<BezierCurvePath>();
                bezierCurvePath.transform.SetParent(transform);

                float x = road.plainView.geometry.x;
                float y = road.plainView.geometry.y;
                float length_s = road.plainView.geometry.length;
                float angle = road.plainView.geometry.hdg;

                var center = road.lanes.laneSection.center;
                if (center != null)
                {
                    LaneAttribute laneAttribute = new LaneAttribute()
                    {
                        laneID = 0,
                        color = center.lane[0].roadMark.color,
                        material = center.lane[0].roadMark.material,
                        type = center.lane[0].roadMark.type
                    };
                    bezierCurvePath.laneAttributes.Add(laneAttribute);
                }
                else
                {
                    LaneAttribute laneAttribute = new LaneAttribute()
                    {
                        laneID = 0,
                        color = "yellow",
                        material = "standard",
                        type = "none"
                    };
                    bezierCurvePath.laneAttributes.Add(laneAttribute);
                }
                var left = road.lanes.laneSection.left;
                if (left != null)
                {
                    bezierCurvePath.leftCount = left.lane[0].id;
                    foreach (var lane in left.lane)
                    {
                        LaneAttribute laneAttribute = new LaneAttribute()
                        {
                            laneID = lane.id,
                            width = lane.width.a,
                            color = lane.roadMark.color,
                            material = lane.roadMark.material,
                            type= lane.roadMark.type
                        };
                        bezierCurvePath.laneAttributes.Add( laneAttribute);
                    }
                }
                else
                {
                    bezierCurvePath.leftCount = 0;
                }
                var right = road.lanes.laneSection.right;
                if (right != null)
                {
                    bezierCurvePath.rightCount = right.lane[0].id;
                    foreach (var lane in right.lane)
                    {
                        var roadMark = lane.roadMark;
                        if (roadMark == null)
                        {
                            roadMark = new RoadMark()
                            {
                                material = "standard",
                                color="white",
                                type="none"
                            };
                        }
                        LaneAttribute laneAttribute = new LaneAttribute()
                        {
                            laneID = lane.id,
                            width = lane.width.a,
                            color = roadMark.color,
                            material = roadMark.material,
                            type = roadMark.type
                        };
                        bezierCurvePath.laneAttributes.Add(laneAttribute);
                    }
                }
                else
                {
                    bezierCurvePath.rightCount = 0;
                }
                float curvature = 0;
                try
                {
                    curvature = road.plainView.geometry.arc.curvature;
                }
                catch (NullReferenceException)
                {
                }
                bezierCurvePath.Generator(x, y, length_s, angle, curvature);
                bezierCurvePath.CreatePathFragments();
                bezierCurvePath.CreateLines();
            }


            map.UpdateRenderer();
            map.BuildComplete();
        }




    }
}
