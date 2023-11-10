using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


// created by Skeletor
// tools used to filter lists
namespace GameFilters 
{
    // set of extension methods that can be used for any IEnumerable 
    public static class GameFiltersExtensionMethods
    {
        // returns all components in a child based on a condition 
        public static T[] GetComponentsInChildrenByCondition<T>(this GameObject original, Func<T, bool> condition)
        {
            T[] components = original.GetComponentsInChildren<T>();
            T[] output = new T[150];
            int index = 0;
            IEnumerator<T> filteredComponents = components.FilterBy(condition);
            while(filteredComponents.MoveNext())
            {
                output[index] = filteredComponents.Current;
                index++;
            }
            Array.Resize(ref output, index);
            return output;
        }

        // filters a an IEnumerable by a condtion
        public static IEnumerator<T> FilterBy<T>(this IEnumerable<T> original, Func<T, bool> condition)
        {
            foreach(T item in original)
            {
                if(condition.Invoke(item))
                {
                    yield return item;
                }
            }
        }

        // returns a random element from an array
        public static T GetRandomElement<T>(this T[] original)
        {
            return original[Random.Range(0, original.Length)];
        }
    }
}
