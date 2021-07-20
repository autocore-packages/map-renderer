using System.Collections.Generic;
using UnityEngine;

namespace MapRenderer
{
    public class Area : MapElement
    {
        public List<Point> points;
        public Color color = Color.white;


        private List<int> indices;
        //顶点数组
        private Vector3[] Vertexes;
        private Vector2[] uvs;
        //网格过滤器
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private void Start()
        {
        }
        public override void ElementUpdateRenderer()
        {
            base.ElementUpdateRenderer();
            elemenrMaterial = new Material(Shader.Find("Standard"));
            elemenrMaterial.EnableKeyword("_EMISSION");
            elemenrMaterial.color = color;
            elemenrMaterial.SetColor("_EmissionColor", color);

            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();

            if (points[points.Count - 1].position == points[0].position)
            {
                points.RemoveAt(points.Count - 1);
            }
            Vertexes = new Vector3[points.Count*2];
            for (int i = 0; i < points.Count; i++)
            {
                Vertexes[i] = Vertexes[points.Count + i] = points[i].position;
            }

            //得到三角形的数量
            int trianglesCount = points.Count - 2;
            //三角形顶点ID数组
            indices = new List<int>();
            //三角形顶点索引,确保按照顺时针方向设置三角形顶点
            for (int i = 0; i < trianglesCount; i++)
            {
                indices.Add(0);
                indices.Add(i + 1);
                indices.Add(i + 2);
            }
            for (int i = 0; i < trianglesCount; i++)
            {
                indices.Add(points.Count + 0);
                indices.Add(points.Count + i + 2);
                indices.Add(points.Count + i + 1);
            }
            Mesh mesh = new Mesh();
            mesh.vertices = Vertexes;
            //mesh.uv = uvs;
            mesh.triangles = indices.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            _meshFilter.mesh = mesh;

            //areaMaterial.color = areaColor;
            _meshRenderer.material = elemenrMaterial;
        }
    }
}