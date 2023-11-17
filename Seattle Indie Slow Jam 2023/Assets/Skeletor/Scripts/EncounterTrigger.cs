using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// created by skeletor
// script that activates a group of enemy behaviors if the enemy enters an area or attacks/is seen by any memeber of the group
public class EncounterTrigger : MonoBehaviour
{
    // reference to all of the enemies in this encounter
    [SerializeField] private EnemyBehavior[] _enemies;
    // encounter can only be started once
    [SerializeField] private bool _triggered;

    // on startup assign listeners
    void Start()
    {
        foreach(EnemyBehavior enemy in _enemies)
        {
            enemy.Injured += OnEnemyActivated;
            enemy.SeenPlayer += OnEnemyActivated;
        }
    }

    // when a player enters the area trigger for the first time, start the encounter
    void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Player") && !_triggered)
        {
            StartEncounter();
        }
    }

    // activates all of the enemies in the encounter
    public void StartEncounter()
    {
        if(!_triggered)
        {
            _triggered = true;
            foreach(EnemyBehavior enemy in _enemies)
            {
                enemy.DetectPlayer();
            }
        }
    }

    // event called when an enemy detects the player, starts the encounter
    public void OnEnemyActivated(object source, EventArgs e)
    {
        StartEncounter();
    }
}

