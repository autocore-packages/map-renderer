using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Networking;

namespace MapRenderer
{
    public class MapManager : MonoBehaviour
    {
        public static MapManager Instance;

        public Action<string> OnGetOSM;

        public Action<byte[]> OnGetOpenDrive;

        public List<Map> mapList;

        public LineRenderer laneLR;
        public Material laneMaterial;

        private void Awake()
        {
            Instance = this;
            mapList = new List<Map>();
            Map[] maps = GetComponentsInChildren<Map>();
            foreach (Map item in maps)
            {
                mapList.Add(item);
            }
        }
        private void Start()
        {
            laneLR = new GameObject("LaneLR").AddComponent<LineRenderer>();
            laneLR.transform.SetParent(transform);
            laneLR.transform.rotation = Quaternion.Euler(90, 0, 0);
            laneLR.alignment = LineAlignment.TransformZ;
            laneLR.textureMode = LineTextureMode.Tile;
            laneLR.material = laneMaterial;
            laneLR.enabled = false;
            //Vector3[] vector3s = new Vector3[5] 
            //{ 
            //    new Vector3(0,0,0),
            //    new Vector3(0,0,10),
            //    new Vector3(1,0,15),
            //    new Vector3(2,0,16),
            //    new Vector3(3,0,18)
            //};
            //ShowLane(vector3s);
        }
        public void ShowLane(Vector3[] poses)
        {
            laneLR.enabled = true;
            laneLR.positionCount = poses.Length;
            laneLR.SetPositions(poses);
        }
        public float offset;
        private void Update()
        {
            if (laneLR.enabled)
            {
                offset -= Time.deltaTime * 0.5f;
                laneMaterial.SetTextureOffset("_MainTex", new Vector2(offset, 0));
            }
            
        }
        public Map GetOrCreateMap(string mapName)
        {
            foreach (Map map in mapList)
            {
                if (map.name == mapName)
                {
                    return map;
                }
            }
            Map newMap = new GameObject(mapName).AddComponent<Map>();
            newMap.transform.SetParent(transform);
            newMap.Init();
            return newMap;
        }
        public IEnumerator GetDataFromURL(string url)
        {
            if (url.EndsWith(".osm"))
            {
                Debug.Log("osm");
                UnityWebRequest www = UnityWebRequest.Get(url);
                yield return www.SendWebRequest();
                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    // Show results as text
                    string content = www.downloadHandler.text;
                    OnGetOSM.Invoke(content);
                }
            }
            else if (url.EndsWith(".xodr"))
            {
                Debug.Log("xodr");
                UnityWebRequest www = UnityWebRequest.Get(url);
                yield return www.SendWebRequest();
                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    // Show results as text
                    byte[] content = www.downloadHandler.data;
                    OnGetOpenDrive.Invoke(content);
                }
            }
            
        }
    }

}

