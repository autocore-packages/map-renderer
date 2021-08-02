using System;
using System.Xml.Serialization;
namespace assets.OpenDriveReader
{
    [Serializable]
    public class OpenDRIVE
    {
        /// <summary>
        /// 头
        /// </summary>
        [XmlElement("header")]
        public Header header;
        /// <summary>
        /// 路径
        /// </summary>
        [XmlElement("road")]
        public Road[] roads;
        [XmlElement("controller")]
        public Controller[] controllers;
        [XmlElement("junction")]
        public Junction[] junctions;
    }
    [Serializable]
    public class Header
    {
        /// <summary>
        /// OpenDRIVE格式主版本号
        /// </summary>
        [XmlAttribute]
        public int revMajor;
        /// <summary>
        /// OpenDRIVE格式次版本号
        /// </summary>
        [XmlAttribute]
        public int revMinor;
        /// <summary>
        /// 数据库的名称
        /// </summary>
        [XmlAttribute]
        public string name;
        /// <summary>
        /// 本路网的版本号
        /// </summary>
        [XmlAttribute]
        public float version;
        /// <summary>
        /// 创建时间/日期
        /// </summary>
        [XmlAttribute]
        public string date;
        /// <summary>
        /// 惯性坐标系中y轴最大值
        /// </summary>
        [XmlAttribute]
        public float north;
        /// <summary>
        /// 惯性坐标系中y轴最小值
        /// </summary>
        [XmlAttribute]
        public float south;
        /// <summary>
        /// 惯性坐标系中x轴最大值
        /// </summary>
        [XmlAttribute]
        public float east;
        /// <summary>
        /// 惯性坐标系中x轴最小值
        /// </summary>
        [XmlAttribute]
        public float west;

    }

    [Serializable]
    public class Road
    {
        [XmlAttribute]
        public string name;
        /// <summary>
        /// 道路总长度,此时不考虑高程的影响
        /// </summary>
        [XmlAttribute]
        public float length;
        /// <summary>
        /// 道路ID(唯一)
        /// </summary>
        [XmlAttribute]
        public int id;
        /// <summary>
        /// 如果该条道路在交叉口内，那么该值就代表交叉口的id，否则该值为-1
        /// </summary>
        [XmlAttribute]
        public int junction;
        [XmlElement("link")]
        public Link link;
        /// <summary>
        /// 道路参考线(每条道路必须有且只有一条道路参考线。)
        /// </summary>
        [XmlElement("planView")]
        public PlainView plainView;
        [XmlElement("lanes")]
        public Lanes lanes;
        [XmlElement("signals")]
        public Signals signals;
    }
    [Serializable]
    public class Signals
    {
        [XmlElement("signal")]
        public Signal signal;
    }

    [Serializable]
    public class Signal
    {
        [XmlAttribute]
        public int s;
        [XmlAttribute]
        public string id;
    }

    [Serializable]
    public class Link
    {
        [XmlElement("predecessor")]
        public Predecessor predecessor;

        [XmlElement("successor")]
        public Successor successor;
    }
    /// <summary>
    /// 前驱
    /// </summary>
    [Serializable]
    public class Predecessor
    {
        [XmlAttribute]
        public string elementType;
        [XmlAttribute]
        public int elementId;
        [XmlAttribute]
        public string contactPoint;
    }
    /// <summary>
    /// 后继
    /// </summary>
    [Serializable]
    public class Successor
    {
        /// <summary>
        /// 被连接道路的类型，一般为“road”或“junction”
        /// </summary>
        [XmlAttribute]
        public string elementType;
        /// <summary>
        /// 被连接道路的id
        /// </summary>
        [XmlAttribute]
        public int elementId;
        /// <summary>
        /// 被连接道路的连接接触点，一般被连接道路的起点为start，终点为end，用来表明连接的方向。
        /// </summary>
        [XmlAttribute]
        public string contactPoint;
    }
    [Serializable]
    public class PlainView
    {
        /// <summary>
        /// 参考线的几何形状
        /// </summary>
        [XmlElement("geometry")]
        public Geometry geometry;
    }
    [Serializable]
    public class Geometry
    {
        [XmlElement("arc")]
        public Arc arc;
        /// <summary>
        /// 该段曲线起始位置的s坐标 （参考线坐标系）
        /// </summary>
        [XmlAttribute]
        public float s;
        /// <summary>
        /// 该段曲线起始位置的x值 （惯性坐标系）
        /// </summary>
        [XmlAttribute]
        public float x;
        /// <summary>
        /// y 该段曲线起始位置的y值 （惯性坐标系）
        /// </summary>
        [XmlAttribute]
        public float y;
        /// <summary>
        /// 该段曲线起始点的方向 （惯性航向角/偏航角 yaw）
        /// </summary>
        [XmlAttribute]
        public float hdg;
        /// <summary>
        /// 该段曲线的长度 （参考线坐标系）
        /// </summary>
        [XmlAttribute]
        public float length;
    }
    [Serializable]
    public class Arc
    {
        [XmlAttribute]
        public float curvature;
    }
    [Serializable]
    public class Lanes
    {
        [XmlElement("laneSection")]
        public LaneSection laneSection;
    }

    [Serializable]
    public class LaneSection
    {
        [XmlAttribute]
        public float s;
        [XmlElement("right")]
        public Right right;
        [XmlElement("left")]
        public Left left;
        [XmlElement("center")]
        public Right center;
    }

    [Serializable]
    public class Right
    {
        [XmlElement("lane")]
        public Lane[] lane;
    }
    [Serializable]
    public class Left
    {
        [XmlElement("lane")]
        public Lane[] lane;
    }
    [Serializable]
    public class Center
    {
        [XmlElement("lane")]
        public Lane[] lane;
    }
    [Serializable]
    public class Lane
    {
        [XmlAttribute]
        public int id;
        [XmlAttribute]
        public string type;
        [XmlAttribute]
        public bool level;
        [XmlElement("width")]
        public Width width;
        [XmlElement("roadMark")]
        public RoadMark roadMark;
    }
    [Serializable]
    public class Width
    {
        [XmlAttribute]
        public float sOffset;
        [XmlAttribute]
        public float a;
        [XmlAttribute]
        public float b;
        [XmlAttribute]
        public float c;
        [XmlAttribute]
        public float d;
    }
    [Serializable]
    public class RoadMark
    {
        [XmlAttribute]
        public float sOffset;
        [XmlAttribute]
        public string type;
        [XmlAttribute]
        public string material;
        [XmlAttribute]
        public string color;
        [XmlAttribute]
        public string laneChange;
    }
    [Serializable]
    public class Controller
    {
        [XmlAttribute]
        public string id;
        [XmlElement("control")]
        public Control[] control;
    }
    [Serializable]
    public class Control
    {
        [XmlAttribute]
        public string signalId;
        [XmlAttribute]
        public string type;
    }
    [Serializable]
    public class Junction
    {
        /// <summary>
        /// 交叉口的唯一ID
        /// </summary>
        [XmlAttribute]
        public int id;
        /// <summary>
        /// 交叉口的名称
        /// </summary>
        [XmlAttribute]
        public string name;
        [XmlElement("connection")]
        public Connection[] connection;
    }
    [Serializable]
    public class Connection
    {
        [XmlAttribute]
        public int id;
        /// <summary>
        /// 来路ID
        /// </summary>
        [XmlAttribute]
        public int incomingRoad;
        /// <summary>
        /// 连接道路ID
        /// </summary>
        [XmlAttribute]
        public int connectingRoad;
        /// <summary>
        /// 连接道路的连接点
        /// </summary>
        [XmlAttribute]
        public string contactPoint;
        /// <summary>
        /// 至少一个车道连接
        /// </summary>
        [XmlElement("laneLink")]
        public LaneLink laneLink;
    }
    [Serializable]
    public class LaneLink
    {
        /// <summary>
        /// 来路的车道ID
        /// </summary>
        [XmlAttribute]
        public int from;
        /// <summary>
        /// 连接道路的车道ID

        /// </summary>
        [XmlAttribute]
        public int to;
    }
}