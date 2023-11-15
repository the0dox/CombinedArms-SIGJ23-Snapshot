using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// created by skeletor
// used to create instances of state from inspector friendly enums
public static class EnemyStateFactory
{
    public static readonly Dictionary<StateKey, Func<State<EnemyBehavior>>> s_states = new Dictionary<StateKey, Func<State<EnemyBehavior>>>()
    {
        {StateKey.SimpleAttack, () => new EnemyAttackState()},
        {StateKey.StrafeVolley, () => new EnemyStrafeVolleyState()},
        {StateKey.Burst, () => new EnemyBurstAttackState()},
        {StateKey.Rocket, () => new EnemyRocketAttackState()},
        {StateKey.Snipe, () => new EnemySnipeAttackState()}
    }; 

    public static State<EnemyBehavior> BuildState(StateKey key)
    {
        if(!s_states.ContainsKey(key))
        {
            return new EnemyAttackState();
        }
        return s_states[key].Invoke();
    }
}

public enum StateKey
{
    None,
    SimpleAttack,
    StrafeVolley,
    Burst,
    Rocket,
    Snipe
}