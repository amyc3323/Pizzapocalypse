using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get { return GetInstance(); }
    }

    public static T GetInstance()
    {
        T TinScene = FindObjectOfType<T>();
        if (TinScene != null)
            _instance = TinScene;

        if (_instance == null)
        {
            GameObject temp = new GameObject();
            temp.name = typeof(T).ToString();
            _instance = temp.AddComponent<T>();
        }
        return _instance;
    }
}
