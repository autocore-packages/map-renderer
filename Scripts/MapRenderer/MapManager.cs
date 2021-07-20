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

        public Action<string> OnGetMapData;

        public List<Map> mapList;

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
            UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();
            Debug.Log(www.uri);
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                string content = www.downloadHandler.text;
                Debug.Log(content);
                OnGetMapData.Invoke(content);
            }
        }
    }

}

