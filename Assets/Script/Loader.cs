using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Loader : MonoBehaviour {

    public string levelBundleName;
    public string levelName;

    public GameObject eventManager;
    public GameObject instructionView;
    public GameObject dataStorage;

    public static string url = "";

    public static bool isManifestOK = false;
	// Use this for initialization
	IEnumerator Start () {
        string dataPath = Application.dataPath;
        WWW www;
#if UNITY_EDITOR
        www = new WWW("file:///" + dataPath + "/config.txt");
#elif UNITY_WEBPLAYER
        www = new WWW(dataPath + "/config.txt");
#endif
        yield return www;
        Debug.Log(www.text);
        if (www.text == "")
            Debug.Log("NULL URL");
        else
            url = www.text;

        yield return StartCoroutine(BundleManager.Initialize());
        isManifestOK = true;

        Instantiate(eventManager);
        Instantiate(instructionView);
        Instantiate(dataStorage);

        AssetBundleLoadOperation request = BundleManager.LoadLevelAsync(levelBundleName, levelName, false);
        if (request == null)
            yield break;
        yield return StartCoroutine(request);
	}


}
