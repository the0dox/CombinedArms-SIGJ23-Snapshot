using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// created by Skeletor
// attach this to a loader to load projectiles into the scene
public class ObjectLoaderBehavior: MonoBehaviour
{
    // reference to the prefab this loader creates
    [SerializeField] private GameObject _prefab;
    // number of instances created from the prefab
    [SerializeField] private int _count;

    // Start is called before the first frame update
    void Awake()
    {
        new ObjectLoader(_prefab, _count, transform);
    }
}

// this is a generic class that is created in runtime to help with handling the loading and unloading of different objects
public class ObjectLoader
{
    // static dictionary connecting loaders to different object types
    // keys are the name of the prefab and values are the loaders that contain instances of the prefab
    private static Dictionary<string, ObjectLoader> s_loaders = new Dictionary<string, ObjectLoader>();
    // the loaded instances of the prefab
    private GameObject[] _objects;
    // the name of the prefab this loader is using
    private string _prefabName;
    // current index of this object loader
    private int _index = -1;
    // setter for the object loader index
    private int Index {get => _index; set {_index = value < _objects.Length ? value : 0;}}

    // creates an object loader of a specific prefab 
    // generates instances and hides them until called
    public ObjectLoader(GameObject prefab, int count, Transform parent)
    {
        _prefabName = prefab.name;
        _objects = new GameObject[count];
        for(int i = 0; i < _objects.Length; i++)
        {
            _objects[i] = GameObject.Instantiate(prefab, parent.transform);
            _objects[i].gameObject.SetActive(false);
        }
        if(!s_loaders.ContainsKey(_prefabName))
        {
            s_loaders.Add(_prefabName, this);
        }
        else
        {
            s_loaders[_prefabName] = this;
        }
    }

    // reads through all object loaders in the scene and returns an instance from an object loader that matches prefabName
    public static GameObject LoadObject(string prefabName)
    {
        // throw an error if prefab name doesn't have a corresponding object loader
        if(!s_loaders.ContainsKey(prefabName))
        {
            throw new ArgumentException($"Unable to find a {prefabName} object loader, consider adding a new object loader of that type to the current scene");
        }
        else
        {
            return s_loaders[prefabName].LoadObject();
        }
    }

    // loads an inactive instance from this loader
    private GameObject LoadObject()
    {
        GameObject output = LoadObjectRecursive(3);
        output.gameObject.SetActive(true);
        output.transform.SetParent(null);
        return output;
    }

    // recursively searches for the next disabled ILoadable object
    private GameObject LoadObjectRecursive(int depth)
    {
        // throw an error if there is no loader associated with this type
        if(_objects == null)
        {
            throw new NullReferenceException($"Unable to find a {_prefabName} object loader, consider adding a new object loader of that type to the current scene");
        }
        Index++;
        // check if the current object in the loader is active
        if(_objects[Index].gameObject.activeInHierarchy)
        {
            // if the current object is active and I haven't reached my max depth, attempt to load the next object
            if(depth > 0)
            {
                return LoadObjectRecursive(depth--);
            }
            // if I have reached max depth cancel the operation
            else
            {
                throw new StackOverflowException($"Unable to find an unloaded {_prefabName} object, consider increasing the count value on the corresponding object loader");
            }
        }
        // if the current object is unloaded load it
        else
        {
            return _objects[Index];
        }

    }
}
