using UnityEngine;

namespace MapRenderer
{
    public class Sign : MapElement
    {
        public Vector3 position;
        public float height;
        public float width;
        public float yaw;

        private GameObject plane;

        private void Start()
        {
            //plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            //plane.transform.SetParent(transform);
            //elemenrMaterial = new Material(Shader.Find("Standard"));
            //elemenrMaterial.EnableKeyword("_EMISSION");
            //elemenrMaterial.SetColor("_EmissionColor", Color.blue);
            //plane.GetComponent<MeshRenderer>().material = elemenrMaterial;
            //ElementUpdateRenderer();
        }
        public override void ElementUpdateRenderer()
        {
            plane.transform.position = position;
            plane.transform.localScale = new Vector3(width,1,width);
            plane.transform.rotation = Quaternion.Euler(new Vector3(0, yaw, 0));
        }
    }
}
