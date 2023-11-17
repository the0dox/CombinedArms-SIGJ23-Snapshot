using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// created by skeletor
// collection of useful extensions
public static class Extensions
{
    public static void Invoke(this MonoBehaviour mb, Action f, float delay)
    {
        mb.StartCoroutine(InvokeRoutine(f, delay));
    }
 
    private static IEnumerator InvokeRoutine(System.Action f, float delay)
    {
        yield return new WaitForSeconds(delay);
        f();
    }
}

