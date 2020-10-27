using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singletion<T> : MonoBehaviour where T : MonoBehaviour
{
    public static string rootName = "MonoSingletionRoot";
    private static GameObject monoSingletionRoot;


    private static T instance;
    public static T Instance
    {
        get
        {
            if (monoSingletionRoot == null)
            {
                monoSingletionRoot = GameObject.Find(rootName);
                if (monoSingletionRoot == null) Debug.Log("Please create a gameobject named " + rootName);
            }
            if (instance == null)
            {
                instance = monoSingletionRoot.GetComponent<T>();
                if (instance == null) instance = monoSingletionRoot.AddComponent<T>();
            }
            return instance;
        }
    }

    private void OnDestroy()
    {
        instance = null;
    }
}