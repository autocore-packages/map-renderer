using UnityEngine;

namespace MapRenderer
{

    public class Sign_TrafficLight : Sign
    {
        public GameObject parent;
        public override void ElementUpdateRenderer()
        {
            float scale = width / transform.localScale.x;
            transform.localScale = transform.localScale*scale;
            transform.position = position;
            transform.rotation = Quaternion.Euler(new Vector3(0, yaw, 0));
        }
    }
}
