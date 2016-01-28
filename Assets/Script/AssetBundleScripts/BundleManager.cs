using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#region LoadedBundle
public class LoadedBundle
{
    public AssetBundle mBundle;
    public int mRefCount;

    public LoadedBundle(AssetBundle bundle)
    {
        mBundle = bundle;
        mRefCount = 1;
    }
}
#endregion

#region AssetBundleLoadOperation
public abstract class AssetBundleLoadOperation : IEnumerator
{
    public object Current
    {
        get
        {
            return null;
        }
    }
    public bool MoveNext()
    {
        return !IsDone();
    }

    public void Reset()
    {
    }

    abstract public bool Update();

    abstract public bool IsDone();
}
#endregion

#region AssetBundleLoadAssetOperation
public class AssetBundleLoadAssetOperation : AssetBundleLoadOperation
{
    string mBundleName;
    string mAssetName;
    string mError;
    System.Type mType;
    AssetBundleRequest request = null;

    public AssetBundleLoadAssetOperation() { }

    public AssetBundleLoadAssetOperation(string bundleName,string assetName, System.Type type)
    {
        mBundleName = bundleName;
        mAssetName = assetName;
        mType = type;
    }

    public override bool IsDone()
    {
        if (request == null && mError != null)
            return true;

        else
            return request != null && request.isDone;
    }

    public override bool Update()
    {
        if (request != null)
            return false;

        LoadedBundle bundle = BundleManager.GetLoadedBundle(mBundleName, out mError);
        if (bundle != null)
        {
            request = bundle.mBundle.LoadAssetAsync(mAssetName, mType);
            return false;
        }

        else
            return true;
    }

    public T GetAsset<T>()
        where T : UnityEngine.Object
    {
        if (request != null && request.isDone)
            return request.asset as T;

        else
            //Debug.Log("Null cmnr");
            return null;
    }

}

#endregion

#region AssetBundleLoadLevelOperation

public class AssetBundleLoadLevelOperation : AssetBundleLoadOperation
{
    protected string m_AssetBundleName;
    protected string m_LevelName;
    protected bool m_IsAdditive;
    protected string m_DownloadingError;
    protected AsyncOperation m_Request;

    public AssetBundleLoadLevelOperation(string assetbundleName, string levelName, bool isAdditive)
    {
        m_AssetBundleName = assetbundleName;
        m_LevelName = levelName;
        m_IsAdditive = isAdditive;
    }

    public override bool Update()
    {
        if (m_Request != null)
            return false;

        LoadedBundle bundle = BundleManager.GetLoadedBundle(m_AssetBundleName, out m_DownloadingError);
        if (bundle != null)
        {
            if (m_IsAdditive)
                m_Request = Application.LoadLevelAdditiveAsync(m_LevelName);
            else
                m_Request = Application.LoadLevelAsync(m_LevelName);
            return false;
        }
        else
            return true;
    }

    public override bool IsDone()
    {
        // Return if meeting downloading error.
        // m_DownloadingError might come from the dependency downloading.
        if (m_Request == null && m_DownloadingError != null)
        {
            Debug.LogError(m_DownloadingError);
            return true;
        }

        return m_Request != null && m_Request.isDone;
    }
}

#endregion


#region BundleManager
public class BundleManager : MonoBehaviour
{


    #region Field
    public static string manifestName = "AssetBundles";

    // Đường dẫn trỏ về thư mục chứa các assetbundles
    //public static string baseURL = "file:///E:/Example/Unity/NCKH_2015_TheMuseum/BaoTang/AssetBundles/";

    public static string baseURL = "http://localhost/AssetBundles/Webplayer/";

    // Manifest chứa thông tin về hash và tất cả dependency.
    private static AssetBundleManifest manifest = null;

    // Chưa những assetbunlde đã được load thành công
    static Dictionary<string, LoadedBundle> loadedBundles = new Dictionary<string, LoadedBundle>();

    // Chứa những www đang thực hiện quá trình download
    static Dictionary<string, WWW> downloadingWWWs = new Dictionary<string, WWW>();

    // Chứa lỗi trong quá trình download
    static Dictionary<string, string> downloadingErrors = new Dictionary<string, string>();

    //static List<AssetBundleLoadOperation> m_InProgressOperations = new List<AssetBundleLoadOperation>();

    // Chứa tên của những bundle mà bundle hiện tại đang cần. Vd như các object cần material.
    static Dictionary<string, string[]> mDependencies = new Dictionary<string, string[]>();

    // 
    static List<AssetBundleLoadOperation> inProgressOperations = new List<AssetBundleLoadOperation>();

    #endregion Field


    #region Method

    // 
    public static IEnumerator Initialize()
    {
        baseURL = "http://" + Loader.url + "AssetBundles/Webplayer/";
        Debug.Log(baseURL);

        GameObject assetManagerObject = new GameObject("BundleManager", typeof(BundleManager));
        DontDestroyOnLoad(assetManagerObject);

        WWW www = new WWW(baseURL + manifestName);
        yield return www;
        Debug.Log(www);
        AssetBundle manifestBundle = www.assetBundle;
        Debug.Log(manifestBundle);
        manifest = manifestBundle.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
        //Debug.Log(manifest);
        manifestBundle.Unload(false);
    }


    // Get những bundle đã load xong
    public static LoadedBundle GetLoadedBundle(string bundleName, out string error)
    {
        // Nếu xảy ra lỗi trong quá trình download thì trả về null
        if (downloadingErrors.TryGetValue(bundleName, out error))
            return null;

        // Nếu bundle cần load chưa được tải về thì trả về null
        LoadedBundle bundle = null;
        loadedBundles.TryGetValue(bundleName, out bundle);
        if (bundle == null)
            return null;


        // Nếu không phụ thuộc vào các bundle khác thì trả về
        string[] dependencies = null;
        if (!mDependencies.TryGetValue(bundleName, out dependencies))
            return bundle;

        else
        {
            // Với mỗi sự phụ thuộc 
            foreach (string dependency in dependencies)
            {
                // Nếu bundle mà phụ thuộc vào có lỗi thì trả về luôn
                if (downloadingErrors.TryGetValue(dependency, out error))
                    return bundle;

                // Nếu bundle mà phụ thuộc vào chưa được tải xong thì trả về null
                LoadedBundle dependBundle = null;
                loadedBundles.TryGetValue(dependency, out dependBundle);
                if (loadedBundles == null)
                    return null;
            }
        }

        return bundle;
    }

    // Load bundle dựa theo tên và những bundle nó phụ thuộc vào
    public static void LoadBunlde(string bundleName)
    {
        // Nếu bundle đó mới bắt đầu download về hoặc load từ bộ nhớ cache
        // thì phải load cả những bundle nó phụ thuộc vào.
        bool isAlreadyProcess = LoadBundleInternal(bundleName);
        if (!isAlreadyProcess)
            LoadBundleDependencies(bundleName);
    }

    // Load bản thân bundle dựa theo tham số là tên
    public static bool LoadBundleInternal(string bundleName)
    {
        LoadedBundle bundle = null;

        // Xem trong từ điển đã có chưa
        loadedBundles.TryGetValue(bundleName, out bundle);
        if (bundle != null)
        {
            // Nếu có rồi thì tăng biến số lượng tham chiếu và trả về true
            bundle.mRefCount++;
            return true;
        }

        // Nếu vẫn còn đang download cũng tả về true
        if (downloadingWWWs.ContainsKey(bundleName))
            return true;

        // Nếu chưa có trong từ điển và cũng chưa trong download thì bắt đầu download về hoặc load từ bộ nhớ cache (nếu có)
        WWW download = WWW.LoadFromCacheOrDownload(baseURL + bundleName, manifest.GetAssetBundleHash(bundleName));

        // Sau đó add vào danh sách đang download và trả về false
        downloadingWWWs.Add(bundleName, download);

        return false;

    }

    // Load những bundle mà bundle có tên dựa theo tham số phải phụ thuộc vào.
    // Ví dụ tham số là tên bundle material thì sẽ load bundle texture gắn vào material đó
    public static void LoadBundleDependencies(string bundleName)
    {
        // Nếu không có manifest thì không thể load
        if (manifest == null)
            return;

        // Dựa vào manifest để biết được bundle đó phụ thuộc vào những bundle nào khác.
        string[] dependencies = manifest.GetAllDependencies(bundleName);
        if (dependencies.Length == 0)
            return;

        // Cho vào từ điển bundle phụ thuộc
        mDependencies.Add(bundleName, dependencies);

        // Sau đó Load những bundle phụ thuộc đó
        for (int i = 0; i < dependencies.Length; i++)
        {
            LoadBundleInternal(dependencies[i]);
        }
    }

    // Unload bundle thì cần phải unload chính nó và unload những bundle nó phụ thuộc vào
    public static void UnloadBundle(string bundleName)
    {
        UnloadBundleInternal(bundleName);
        UnloadBundleDependencies(bundleName);
    }


    // Unload bản thân bundle
    public static void UnloadBundleInternal(string bundleName)
    {
        // Nếu bundle không có trong từ điển thì không cần unload
        string error;
        LoadedBundle bundle = GetLoadedBundle(bundleName, out error);
        if (bundle == null)
            return;

        // Giảm số lượng tham chiếu xuống
        // Nếu = 0 tức là hoàn toàn không cần bundle này nữa nên sẽ unload và xóa khỏi từ điển
        if (--bundle.mRefCount == 0)
        {
            bundle.mBundle.Unload(false);
            loadedBundles.Remove(bundleName);
        }
    }

    // Unload những phụ thuộc
    public static void UnloadBundleDependencies(string bundleName)
    {
        // Dựa vào từ điển phụ thuộc có thể tìm ra những bundle mà bundleName phụ thuộc
        // Nếu không tìm ra thì tức là không có, không cần unload
        string[] dependencies = null;
        if (!mDependencies.TryGetValue(bundleName, out dependencies))
            return;

        // Nếu có thì Unload từng bundle
        // Loop dependencies.
        foreach (string dependency in dependencies)
        {
            UnloadBundleInternal(dependency);
        }


        // Xóa khỏi danh sách phụ thuộc
        mDependencies.Remove(bundleName);
    }

    void Update()
    {
        // Kiểm tra ngay khi download xong thì add nó vào từ điển bundle và xóa nó khỏi danh sách download
        // nếu download lỗi thì cho vào từ điển lỗi,
        List<string> keysToRemove = new List<string>();
        foreach (KeyValuePair<string, WWW> keyValue in downloadingWWWs)
        {
            WWW download = keyValue.Value;

            // If downloading fails.
            if (download.error != null)
            {
                downloadingErrors.Add(keyValue.Key, download.error);
                keysToRemove.Add(keyValue.Key);
                continue;
            }

            // If downloading succeeds.
            if (download.isDone)
            {
                //Debug.Log("Downloading " + keyValue.Key + " is done at frame " + Time.frameCount);
                loadedBundles.Add(keyValue.Key, new LoadedBundle(download.assetBundle));
                keysToRemove.Add(keyValue.Key);
            }
        }

        // Remove the finished WWWs.
        foreach (string key in keysToRemove)
        {
            WWW download = downloadingWWWs[key];
            downloadingWWWs.Remove(key);
            download.Dispose();
        }

        // Update all in progress operations
        for (int i = 0; i < inProgressOperations.Count; )
        {
            // Chạy hàm update của operation. Nếu Update trả về false
            // cũng có nghĩa là quá trình đã xong. Vậy nên xóa khỏi danh sách operation
            if (!inProgressOperations[i].Update())
            {
                inProgressOperations.RemoveAt(i);
            }
            else
                i++;
        }
    }

    public static AssetBundleLoadAssetOperation LoadAssetAsync(string bundleName, string assetName, System.Type type)
    {
        AssetBundleLoadAssetOperation operation = null;

        LoadBunlde(bundleName);
        operation = new AssetBundleLoadAssetOperation(bundleName, assetName, type);

        inProgressOperations.Add(operation);

        return operation;
    }

    public static AssetBundleLoadOperation LoadLevelAsync(string bundleName, string levelName, bool isAdditive)
    {
        Debug.Log("Load level: " + bundleName + " - " + levelName + " - " + isAdditive);

        LoadBunlde(bundleName);
        AssetBundleLoadOperation operation = new AssetBundleLoadLevelOperation(bundleName, levelName, isAdditive);
        inProgressOperations.Add(operation);

        return operation;
    }

    #endregion Method


}

#endregion
