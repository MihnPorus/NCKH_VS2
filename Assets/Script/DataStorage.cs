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
    
    private Dictionary<int, Item> data = new Dictionary<int, Item>();

    private List<Item> temp = new List<Item>();

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
    

    public IEnumerator Download<T>(T item, bool downloadMore)
        where T : Item
    {
        int id = item.id;
        if (!data.ContainsKey(id))
        {
            // Nếu đang trong trạng thái download 
            // và có tiếp tục download từ danh sách chờ
            // thì add vào danh sách chờ download
            if (isDownloading && downloadMore)
            {
                // Add vào danh sách chờ
                temp.Add(item);
            }
            else
            {
                // Nếu chỉ download 1 phần tử thì không chuyển trạng thái download
                // Đồng thời xóa phần tử đó trong danh sách chờ
                if (!downloadMore)
                {
                    Debug.Log("abc");
                    if (temp.Contains(item))
                        temp.Remove(item);
                    yield return StartCoroutine(item.DownloadData());
                }

                else {
                    // Chuyen sang trang thai dang download
                    isDownloading = true;
                    yield return StartCoroutine(item.DownloadData());
                    isDownloading = false;
                    if (temp.Count > 0)
                    {
                        StartCoroutine(Download(temp[0], true));
                        temp.RemoveAt(0);
                    }
                }
                data.Add(id, item);
            }
        }
    }
}