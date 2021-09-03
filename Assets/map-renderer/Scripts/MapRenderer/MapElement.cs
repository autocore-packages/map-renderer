using UnityEngine;

namespace MapRenderer
{
    public class MapElement : MonoBehaviour
    {
        public Map map;
        public Material elemenrMaterial;

        public virtual void ElementUpdateRenderer()
        {
        }
    }
}
