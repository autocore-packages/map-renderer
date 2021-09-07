
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapRenderer
{

    public class Area_Lane : Area
    {
        public List<Point> leftPoints;
        public List<Point> rightPoints;
        public override void ElementUpdateRenderer()
        {
            Vector3 dirL = leftPoints[leftPoints.Count - 1].position - leftPoints[0].position;
            Vector3 dirR = rightPoints[rightPoints.Count - 1].position - rightPoints[0].position;
            if (!IsSameDir(dirL, dirR))
            {
                rightPoints.Reverse();
            }


            elemenrMaterial = new Material(Shader.Find("Custom/DoubleTransparent"));
            //elemenrMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            //elemenrMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            //elemenrMaterial.SetInt("_ZWrite", 0);
            //elemenrMaterial.DisableKeyword("_ALPHATEST_ON");
            //elemenrMaterial.EnableKeyword("_ALPHABLEND_ON");
            //elemenrMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            //elemenrMaterial.renderQueue = 3000;
            //elemenrMaterial.EnableKeyword("_EMISSION");
            color.a =0.5f;
            elemenrMaterial.color = color;
            elemenrMaterial.SetColor("_EmissionColor", color);


            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();

            points = new List<Point>();
            points.AddRange(leftPoints);
            points.AddRange(rightPoints);

            int leftCount = leftPoints.Count;
            int rightCount = rightPoints.Count;

            Vertexes = new Vector3[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                Vertexes[i] = points[i].position;
            }

            //得到三角形的数量
            int trianglesCount = points.Count - 2;
            //三角形顶点ID数组
            indices = new List<int>();
            int indexLeft = 0;
            int indexRight = 0;
            while (indexLeft < leftCount-1 && indexRight < rightCount-1)
            {
                indexLeft += 1;
                indexRight += 1;
                if (indexLeft >= leftCount) indexLeft = leftCount - 1;
                else
                {
                    indices.Add(indexLeft);
                    indices.Add(leftCount + indexRight - 1);
                    indices.Add(indexLeft - 1);
                }
                if (indexRight >= rightCount) indexRight = rightCount - 1;
                else
                {
                    indices.Add(indexLeft);
                    indices.Add(leftCount + indexRight);
                    indices.Add(leftCount + indexRight - 1);
                }
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

        private bool IsSameDir(Vector3 v1,Vector3 v2)
        {
            float dot=  Vector3.Dot(v1, v2);
            return dot >= 0;
        }
    }
}
