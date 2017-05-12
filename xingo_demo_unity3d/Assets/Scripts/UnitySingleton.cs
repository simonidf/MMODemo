using UnityEngine;
using System.Collections;

public abstract class UnitySingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;    
    private static object _lock = new object();
    private static bool gameIsQuitting = false;

    public void OnDestroy()
    {
        gameIsQuitting = true;
    }

    public static T Instance
    {
        get
        {
            if (gameIsQuitting)
            {
                Debug.LogWarning(typeof(T) + " singleton already destroyed! returning null.");
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        Debug.LogError("there should never be more than one singleton!");
                        return _instance;
                    }

                    if (_instance == null)
                    {
                        GameObject singleton = new GameObject();

                        _instance = singleton.AddComponent<T>();
                        singleton.name = "(singleton)" + typeof(T).ToString();

                        DontDestroyOnLoad(singleton);

                        Debug.Log(typeof(T) + " is instance");
                    } 
                    else
                    {
                        Debug.Log(typeof(T) + " singleton already created!");
                    }
                }

                return _instance;
            }
        }
    }
}
