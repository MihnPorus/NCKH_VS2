using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DataStorage : MonoBehaviour {

    private static DataStorage instance = null;
    public static DataStorage Instance
    {
        get { return instance; }
        private set { }
    }

    // Chứa dữ liệu
    private Dictionary<int, PictureData> data = new Dictionary<int, PictureData>();
    private List<Picture> temp = new List<Picture>();

    // Kiểm tra trạng thái có đang download hay không
    private bool isDownloading = false;

    void Awake()
    {
        // Singleton Pattern
        if (instance == null)
            instance = this;
        else
            DestroyImmediate(gameObject);

        DontDestroyOnLoad(gameObject);
    }
    
    public IEnumerator Download(Picture p, bool downloadMore)
    {
        int id = p.id;
        if (!data.ContainsKey(id))
        {
            // Nếu đang trong trạng thái download 
            // và có tiếp tục download từ danh sách chờ
            // thì add vào danh sách chờ download
            if (isDownloading && downloadMore)
            {
                // Add vào danh sách chờ
                temp.Add(p);
            }
            else
            {
                // Nếu chỉ download 1 phần tử thì không chuyển trạng thái download
                // Đồng thời xóa phần tử đó trong danh sách chờ
                if (!downloadMore)
                {
                    if (temp.Contains(p))
                        temp.Remove(p);
                    yield return StartCoroutine(p.DownloadData());
                }

                else {
                    // Chuyen sang trang thai dang download
                    isDownloading = true;
                    yield return StartCoroutine(p.DownloadData());
                    isDownloading = false;
                    if (temp.Count > 0)
                    {
                        StartCoroutine(Download(temp[0], true));
                        temp.RemoveAt(0);
                    }
                }
                data.Add(id, p.data);
            }
        }
    }
    
}
