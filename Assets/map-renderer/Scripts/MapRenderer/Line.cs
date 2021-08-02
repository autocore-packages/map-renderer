using System.Collections.Generic;
using UnityEngine;

namespace MapRenderer
{
    public class Line : MapElement
    {
        public List<Point> points;
        public float lineWidth = 0.15f;
        public Color color;

        private LineRenderer lineRenderer;

        public override void ElementUpdateRenderer()
        {
            elemenrMaterial = new Material(Shader.Find("Standard"));
            elemenrMaterial.EnableKeyword("_EMISSION");
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.positionCount = points.Count;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            lineRenderer.alignment = LineAlignment.TransformZ;
            transform.rotation = Quaternion.Euler(90,0,0);
            for (int i = 0; i < points.Count; i++)
            {
                lineRenderer.SetPosition(i, points[i].position);
            }
            elemenrMaterial.SetColor("_EmissionColor", color);
            elemenrMaterial.color = color;
            lineRenderer.material = elemenrMaterial;
        }
    }
}
