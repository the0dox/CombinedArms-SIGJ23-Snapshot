using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// created by skeletor
// attached to a melee hit box that can only hit an enemy once
public class MeleeHitboxBehavior : DamageZone
{
    // called when a player enters the damage zone
    protected override void OnDamageTriggered(IAttackable target)
    {
        gameObject.SetActive(false);
    }
}
