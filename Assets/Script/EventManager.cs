using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{

    #region Instance
    public static EventManager Instance
    {
        get { return instance; }
        set { }
    }

    private static EventManager instance = null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;

        }
        else
            DestroyImmediate(gameObject);

        DontDestroyOnLoad(gameObject);
    }
    #endregion

    public delegate void OnEvent(string eventType, Component sender, object param = null);

    private Dictionary<string, List<OnEvent>> listeners = new Dictionary<string, List<OnEvent>>();

    // Use this for initialization
    public void AddListener(string eventType, OnEvent listener)
    {
        List<OnEvent> listenList = null;

        if (listeners.TryGetValue(eventType, out listenList))
        {
            listenList.Add(listener);
            return;
        }

        listenList = new List<OnEvent>();
        listenList.Add(listener);

        listeners.Add(eventType, listenList);
        //Debug.Log(eventType);
    }

    public void PostNotification(string eventType, Component sender, object param = null)
    {
        List<OnEvent> listenList = null;
        if (!listeners.TryGetValue(eventType, out listenList))
        {
            return;
        }

        for (int i = 0; i < listenList.Count; i++)
        {
            if (!listenList.Equals(null))
            {
                listenList[i](eventType, sender, param);
            }
        }
    }

    public bool RemoveEvent(string eventType, OnEvent x = null)
    {
        if (x==null)
            listeners.Remove(eventType);
        else
        {
            foreach (KeyValuePair<string, List<OnEvent>> item in listeners)
            {
                for (int i = item.Value.Count - 1; i >= 0; i--)
                {
                    if (item.Value[i]==x)
                    {
                        item.Value.RemoveAt(i);
                        Debug.Log("Remove");
                    }
                }
            }
        }
        if (listeners.ContainsKey(eventType))
            return false;
        return true;
    }

    public void RemoveRedundancies()
    {
        Dictionary<string, List<OnEvent>> temp = new Dictionary<string, List<OnEvent>>();
        foreach (KeyValuePair<string, List<OnEvent>> item in listeners)
        {
            for (int i = item.Value.Count - 1; i >= 0; i--)
            {
                if (item.Equals(null))
                    item.Value.RemoveAt(i);
            }

            if (item.Value.Count > 0)
                temp.Add(item.Key, item.Value);
        }

        listeners = temp;
    }

    void OnLevelWasLoaded()
    {
        RemoveRedundancies();
    }
}
