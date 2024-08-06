using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentObject : MonoBehaviour
{
    private static PersistentObject instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("PersistentManager created and marked as DontDestroyOnLoad.");
        }
        else
        {
            Debug.Log("Duplicate PersistentManager found, destroying it.");
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        Debug.Log("PersistentManager is being destroyed.");
    }
}