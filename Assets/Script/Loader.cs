using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Loader : MonoBehaviour {

    public string levelBundleName;
    public string levelName;

    public GameObject eventManager;
    public GameObject instructionView;
    public GameObject dataStorage;

    public static bool isManifestOK = false;
	// Use this for initialization
	IEnumerator Start () {
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
