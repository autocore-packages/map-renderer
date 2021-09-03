using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using System.Runtime.InteropServices;
namespace assets.OSMReader
{
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
            ref_line = 4,
            outer=5
        }
        public MemberType menberType;
        public long refID;
        public RoleType roleType;
    }

    public class OSMData
    {
        public string name;
        public List<OsmNode> nodes;
        public List<OSMWay> ways;
        public List<OSMRelation> relations;
        public OSMData()
        {
            nodes = new List<OsmNode>();
            ways = new List<OSMWay>();
            relations = new List<OSMRelation>();
        }
    }
}