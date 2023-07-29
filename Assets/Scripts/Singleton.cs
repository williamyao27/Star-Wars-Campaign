using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class provides the singleton design pattern for manager and manager-like classes to inherit from. DontDestroyOnLoad() is not called as current implementations of the Singleton do not require it.
/// </summary>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T instance;

    private void Awake()
    {
        CreateSingleton();
    }

    private void CreateSingleton()
    {
        if (instance == null)
        {
            instance = this as T;
        }
        else
        {
            Destroy(gameObject);
        }
        // DontDestroyOnLoad(gameObject);
    }
}
