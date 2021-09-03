using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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

        public void GPS2XY(double I, double B, out double x, out double y)
        {
            try
            {
                I = I * Math.PI / 180;
                B = B * Math.PI / 180;
                double B0 = 30 * Math.PI / 180;
                double N = 0, e = 0, a = 0, b = 0, e2 = 0, K = 0;
                a = 6378137;
                b = 6356752.3142;
                e = Math.Sqrt(1 - (b / a) * (b / a));
                e2 = Math.Sqrt((a / b) * (a / b) - 1);

                double CosB0 = Math.Cos(B0);
                N = (a * a / b) / Math.Sqrt(1 + e2 * e2 * CosB0 * CosB0);
                K = N * CosB0;

                double SinB = Math.Sin(B);
                double tan = Math.Tan(Math.PI / 4 + B / 2);
                double E2 = Math.Pow((1 - e * SinB) / (1 + e * SinB), e / 2);
                double xx = tan * E2;

                x = K * Math.Log(xx);
                y = K * I;

                return;

            }
            catch (Exception ErrInfo)
            {
            }
            x = -1;
            y = -1;
        }

        public List<Tag> Tags=new List<Tag>();
        protected void ReadTags(XmlNode node)
        {
            XmlNodeList tags = node.SelectNodes("tag");
            foreach (XmlNode t in tags)
            {
                Tag tag = new Tag(GetAttribute<string>("k", t.Attributes), GetAttribute<string>("v", t.Attributes));
                //string key = GetAttribute<string>("k", t.Attributes);
                //if (key == "ele")
                //{
                //    ele = GetAttribute<float>("v", t.Attributes);
                //}
                //else if (key == "local_x")
                //{
                //    local_x = GetAttribute<float>("v", t.Attributes);
                //}
                //else if (key == "local_y")
                //{
                //    local_y = GetAttribute<float>("v", t.Attributes);

                Tags.Add(tag);
            }
        }
    }
    public struct Tag
    {
        public Tag(string key, string value) 
        {
            Key = key;
            Value = value;
        }

        public string Key { get; set; }
        public string Value { get; set; }
    }
}
