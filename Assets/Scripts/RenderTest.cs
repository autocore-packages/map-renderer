using MapRenderer;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class RenderTest : MonoBehaviour
{
    public string path;
    public Button buttonRender;
    public InputField inputFieldPath;
    public InputField inputFieldURL;
    // Start is called before the first frame update
    void Start()
    {
        inputFieldPath.onEndEdit.AddListener((string value) =>
        {
            path = Path.Combine(Application.streamingAssetsPath, value);
        });
        inputFieldURL.onEndEdit.AddListener((string value) =>
        {
            path = value;
        });
        buttonRender.onClick.AddListener(()=> 
        {
            StartCoroutine(MapManager.Instance.GetDataFromURL(path));
        });
    }
}
