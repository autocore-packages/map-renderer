using System.Collections.Generic;
using UnityEngine;

namespace MapRenderer
{
    public class Map : MonoBehaviour
    {
        public Dictionary<string, MapElement> Elements;
        public List<Point> points;
        public List<Line> lines;
        public List<Area> areas;
        public List<Sign> signs;
        public List<Structure> structures;


        public Transform lineParent;
        public Transform SignParent;
        public Transform AreaParent;
        public Transform StructureParent;
        public Transform pointParent;

        GameObject goTrafficLight;
        Material panelMaterial;

        GameObject goLine; 
        Material LineMaterial;
        Material LineStopMaterial;
        Material LineLaneMaterial;

        GameObject goStructure;

        GameObject goArea; 
        public void Init()
        {
            if (lineParent == null)
            {
                lineParent = new GameObject("Lines").transform;
                lineParent.SetParent(transform);
            }
            if (SignParent == null)
            {
                SignParent = new GameObject("Signs").transform;
                SignParent.SetParent(transform);
            }
            if (AreaParent == null)
            {
                AreaParent = new GameObject("Areas").transform;
                AreaParent.SetParent(transform);
            }
            if (StructureParent == null)
            {
                StructureParent = new GameObject("Structures").transform;
                StructureParent.SetParent(transform);
            }
            if (pointParent == null)
            {
                pointParent = new GameObject("Points").transform;
                pointParent.SetParent(transform);
            }
            Elements = new Dictionary<string, MapElement>();
            points = new List<Point>();
            lines = new List<Line>();
            areas = new List<Area>();
            signs = new List<Sign>();
            structures = new List<Structure>();
            while (lineParent.childCount != 0)
            {
                DestroyImmediate(lineParent.GetChild(0).gameObject);
            }
            while (SignParent.childCount != 0)
            {
                DestroyImmediate(SignParent.GetChild(0).gameObject);
            }
            while (AreaParent.childCount != 0)
            {
                DestroyImmediate(AreaParent.GetChild(0).gameObject);
            }
            while (StructureParent.childCount != 0)
            {
                DestroyImmediate(StructureParent.GetChild(0).gameObject);
            }

            panelMaterial = new Material(Shader.Find("Standard"));
            panelMaterial.EnableKeyword("_EMISSION");
            panelMaterial.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            panelMaterial.SetColor("_EmissionColor", new Color(0.5f, 0.5f, 0.5f));
            panelMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            panelMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            panelMaterial.SetInt("_ZWrite", 0);
            panelMaterial.DisableKeyword("_ALPHATEST_ON");
            panelMaterial.EnableKeyword("_ALPHABLEND_ON");
            panelMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            panelMaterial.renderQueue = 3000;

            goTrafficLight = GameObject.CreatePrimitive(PrimitiveType.Cube);
            goTrafficLight.name = "Panel";
            goTrafficLight.transform.localScale = new Vector3(0.5f, 0.2f, 0.05f);
            goTrafficLight.GetComponent<MeshRenderer>().material = panelMaterial;
            Destroy(goTrafficLight.GetComponent<BoxCollider>());

            Material redMaterial = new Material(Shader.Find("Standard"));
            redMaterial.EnableKeyword("_EMISSION");
            redMaterial.SetColor("_EmissionColor", new Color(1, 0, 0));
            Material yellowMaterial = new Material(Shader.Find("Standard"));
            yellowMaterial.EnableKeyword("_EMISSION");
            yellowMaterial.SetColor("_EmissionColor", new Color(1, 1, 0));
            Material greenMaterial = new Material(Shader.Find("Standard"));
            greenMaterial.EnableKeyword("_EMISSION");
            greenMaterial.SetColor("_EmissionColor", new Color(0, 1, 0));

            GameObject red = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Destroy(goTrafficLight.GetComponent<CapsuleCollider>());
            red.name = "Red";
            red.transform.localScale = new Vector3(0.15f, 0.03f, 0.15f);
            red.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
            red.GetComponent<MeshRenderer>().material = redMaterial;
            red.transform.SetParent(goTrafficLight.transform);

            GameObject yellow = Instantiate(red, goTrafficLight.transform);
            yellow.name = "Yellow";
            yellow.transform.position = new Vector3(0.175f, 0, 0);
            yellow.GetComponent<MeshRenderer>().material = yellowMaterial;

            GameObject green = Instantiate(red,goTrafficLight.transform);
            green.name = "Green";
            green.transform.position = new Vector3(-0.175f, 0, 0);
            green.GetComponent<MeshRenderer>().material = greenMaterial;


            //LineMaterial = new Material(Shader.Find("Standard"));
            //LineMaterial.EnableKeyword("_EMISSION");

            //LineStopMaterial = new Material(Shader.Find("Standard"));
            //LineStopMaterial.EnableKeyword("_EMISSION");

            //LineLaneMaterial = new Material(Shader.Find("Standard"));
            //LineStopMaterial.EnableKeyword("_EMISSION");

            goLine = new GameObject("goLine");


            goStructure = new GameObject("goStructure");
            goStructure.gameObject.AddComponent<MeshFilter>();
            goStructure.gameObject.AddComponent<MeshRenderer>();

            goArea = new GameObject("goArea");
            goArea.AddComponent<MeshFilter>();
            goArea.AddComponent<MeshRenderer>();
        }
        public void BuildComplete()
        {
            Destroy(goTrafficLight.gameObject);
            Destroy(goLine.gameObject);
            Destroy(goStructure.gameObject);
            Destroy(goArea.gameObject);
        }
        /// <summary>
        /// 添加点
        /// </summary>
        /// <param name="point"></param>
        public void AddPoint(Point point)
        {
            points.Add(point);
            point.transform.SetParent(pointParent);
            Elements.Add(point.name, point);
        }
        public Point AddPoint(string name, Vector3 pos)
        {
            if (Elements.ContainsKey(name))
            {
                Debug.LogError("name 重复");
                return null;
            }
            Point point = new GameObject(name).AddComponent<Point>();
            point.transform.position = point.position = pos;
            AddPoint(point);
            return point;
        }
        /// <summary>
        /// 删除点
        /// </summary>
        /// <param name="point"></param>
        public void RemovePoint(Point point)
        {
            points.Remove(point);
            Elements.Remove(point.name);
        }
        /// <summary>
        /// 修改点坐标
        /// </summary>
        /// <param name="point"></param>
        /// <param name="position"></param>
        public void UpdatePoint(Point point, Vector3 position)
        {

        }
        public void AddLine(Line line)
        {
            Elements.Add(line.name, line);
            line.transform.SetParent(lineParent);
            lines.Add(line);
        }
        public Line AddLine(string name)
        {
            if (Elements.ContainsKey(name))
            {
                Debug.LogError("name 重复");
                return null;
            }
            Line line = Instantiate(goLine).AddComponent<Line>();
            line.name = name;
            AddLine(line);
            return line;
        }
        public Line_Stop AddLine_Stop(string name)
        {
            if (Elements.ContainsKey(name))
            {
                Debug.LogError("name 重复");
                return null;
            }
            Line_Stop line = Instantiate(goLine).AddComponent<Line_Stop>();
            line.name = name;
            AddLine(line);
            return line;
        }
        public void RemoveLine(Line line)
        {
            Elements.Remove(line.name);
            lines.Remove(line);
        }
        public void AddArea(Area area)
        {
            Elements.Add(area.name, area);
            area.transform.SetParent(AreaParent);
            areas.Add(area);
        }
        public Area AddArea(string name)
        {
            if (Elements.ContainsKey(name))
            {
                Debug.LogError("name 重复");
                return null;
            }
            Area area = Instantiate(goArea).AddComponent<Area>();
            area.name = name;
            AddArea(area);
            return area;
        }
        public void RemoveArea(Area area)
        {
            Elements.Remove(area.name);
            areas.Remove(area);
        }
        public void AddSign(Sign sign)
        {
            Elements.Add(sign.name, sign);
            sign.transform.SetParent(SignParent);
            signs.Add(sign);
        }
        public Sign AddSign(string name)
        {
            if (Elements.ContainsKey(name))
            {
                Debug.LogError("name 重复");
                return null;
            }
            Sign sign = new GameObject(name).AddComponent<Sign>();
            AddSign(sign);
            return sign;
        }
        public Sign_TrafficLight AddTrafficLight(string name)
        {
            if (Elements.ContainsKey(name))
            {
                Debug.LogError("name 重复");
                return null;
            }
            Sign_TrafficLight sign = Instantiate(goTrafficLight).AddComponent<Sign_TrafficLight>();
            sign.name = name;
            AddSign(sign);
            return sign;
        }
        public void RemoveSign(Sign sign)
        {
            Elements.Remove(sign.name);
            signs.Remove(sign);
        }
        public void AddStructrue(Structure structure)
        {
            Elements.Add(structure.name, structure);
            structure.transform.SetParent(StructureParent);
            structures.Add(structure);
        }
        public Structure AddStructrue(string name)
        {
            if (Elements.ContainsKey(name))
            {
                Debug.LogError("name 重复");
                return null;
            }
            Structure structure = Instantiate(goStructure).AddComponent<Structure>();
            structure.name = name;
            AddStructrue(structure);
            return structure;
        }
        public void RemoveStructrue(Structure structure)
        {
            Elements.Remove(structure.name);
            structures.Remove(structure);
        }
        public void UpdateRenderer()
        {
            foreach (KeyValuePair<string, MapElement> item in Elements)
            {
                item.Value.ElementUpdateRenderer();
            }
        }
    }
}