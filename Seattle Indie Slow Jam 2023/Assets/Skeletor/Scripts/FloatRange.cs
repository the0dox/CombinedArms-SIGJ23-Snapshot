using System;
using Random = UnityEngine.Random;

// created by skeletor
// an editor friendly struct where you can define a random float range
[Serializable]
public struct FloatRange
{
    // minimum value of the float range (inclusive)
    public float Min;
    // maximum value of the float range (inclusive)
    public float Max;
    // returns a random value between min and maximum
    public float RandomValue => Random.Range(Min, Max);

    // simple constructor when declared in code
    public FloatRange(float Min, float Max)
    {
        this.Min = Min;
        this.Max = Max;
    }
} 
