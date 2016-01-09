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
    private Dictionary<int, PictureData> pData = new Dictionary<int, PictureData>();
    private Dictionary<int, Object3Ddata> oData = new Dictionary<int, Object3Ddata>();

    private List<Picture> tempP = new List<Picture>();
    private List<Object3D> tempO = new List<Object3D>();

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
    
    public IEnumerator DownloadPicture(Picture p, bool downloadMore)
    {
        int id = p.id;
        if (!pData.ContainsKey(id))
        {
            // Nếu đang trong trạng thái download 
            // và có tiếp tục download từ danh sách chờ
            // thì add vào danh sách chờ download
            if (isDownloading && downloadMore)
            {
                // Add vào danh sách chờ
                tempP.Add(p);
            }
            else
            {
                // Nếu chỉ download 1 phần tử thì không chuyển trạng thái download
                // Đồng thời xóa phần tử đó trong danh sách chờ
                if (!downloadMore)
                {
                    if (tempP.Contains(p))
                        tempP.Remove(p);
                    yield return StartCoroutine(p.DownloadData());
                }

                else {
                    // Chuyen sang trang thai dang download
                    isDownloading = true;
                    yield return StartCoroutine(p.DownloadData());
                    isDownloading = false;
                    if (tempP.Count > 0)
                    {
                        StartCoroutine(DownloadPicture(tempP[0], true));
                        tempP.RemoveAt(0);
                    }
                }
                pData.Add(id, p.data);
            }
        }
    }

    public IEnumerator DownloadObject3D(Object3D p, bool downloadMore)
    {
        int id = p.id;
        if (!pData.ContainsKey(id))
        {
            // Nếu đang trong trạng thái download 
            // và có tiếp tục download từ danh sách chờ
            // thì add vào danh sách chờ download
            if (isDownloading && downloadMore)
            {
                // Add vào danh sách chờ
                tempO.Add(p);
            }
            else
            {
                // Nếu chỉ download 1 phần tử thì không chuyển trạng thái download
                // Đồng thời xóa phần tử đó trong danh sách chờ
                if (!downloadMore)
                {
                    if (tempO.Contains(p))
                        tempO.Remove(p);
                    yield return StartCoroutine(p.DownloadData());
                }

                else {
                    // Chuyen sang trang thai dang download
                    isDownloading = true;
                    yield return StartCoroutine(p.DownloadData());
                    isDownloading = false;
                    if (tempP.Count > 0)
                    {
                        StartCoroutine(DownloadPicture(tempP[0], true));
                        tempP.RemoveAt(0);
                    }
                }
                oData.Add(id, p.data);
            }
        }
    }
}
