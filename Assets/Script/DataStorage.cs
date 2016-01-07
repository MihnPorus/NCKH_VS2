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
    private List<int> temp = new List<int>();

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
    
}
