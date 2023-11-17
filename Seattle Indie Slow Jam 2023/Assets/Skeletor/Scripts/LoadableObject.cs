using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// attach this to a game object in the resources/prefabs/ObjectLoader to specify how many instances of this object should be created
public class LoadableObject : MonoBehaviour
{
    // public accessors 
    public int Count => _count;
    public int Order => _order;
    
    // the number of this instance that should be loaded
    [SerializeField] private int _count;
    // determines which object is loaded first
    [SerializeField] private int _order;
}
