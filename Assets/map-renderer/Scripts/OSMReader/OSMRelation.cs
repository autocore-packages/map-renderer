using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace assets.OSMReader
{
    public class OSMRelation : OSMBase
    {
        public OSMRelation(XmlNode node)
        {
            menbers = new List<Member>();
            ID = GetAttribute<long>("id", node.Attributes);
            XmlNodeList members = node.SelectNodes("member");

            foreach (XmlNode n in members)
            {
                Member member = new Member();
                member.refID = GetAttribute<long>("ref", n.Attributes);
                member.menberType = (Member.MemberType)Enum.Parse(typeof(Member.MemberType), GetAttribute<string>("type", n.Attributes));
                member.roleType = (Member.RoleType)Enum.Parse(typeof(Member.RoleType), GetAttribute<string>("role", n.Attributes));
                menbers.Add(member);
            }

            ReadTags(node);

            foreach (Tag t in Tags)
            {
                switch (t.Key)
                {
                    case "type":
                        relationType = (RelationType)Enum.Parse(typeof(RelationType), t.Value);
                        break;
                    case "subtype":
                        relationSubType = (RelationSubType)Enum.Parse(typeof(RelationSubType), t.Value);
                        break;
                    default:
                        break;
                }
            }
        }

        public enum RelationType
        {
            lanelet = 0,
            regulatory_element = 1,
            multipolygon = 2
        }
        public enum RelationSubType
        {
            road = 0,
            traffic_light = 1,
            traffic_sign = 2,
            lane = 3,
            parking_spot,
            parking_access
        }
        public enum TurnDirection
        {
            left = 0,
            right = 1
        }
        public RelationType relationType { get; private set; }
        public RelationSubType relationSubType { get; private set; }
        public TurnDirection turn_direction { get; private set; }
        public List<Member> menbers { get; private set; }

    }
}
