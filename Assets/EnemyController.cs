using Arc.Lib.Debug;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    GridMovement GridMover;
    DebugManager dbg;
    LevelManager levelManager;
    EventManager eventManager;

    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        eventManager = FindObjectOfType<EventManager>();
        eventManager.Register<PlayerMoveEvent>(OnPlayerMove);

        GridMover = GetComponent<GridMovement>();
        dbg = FindObjectOfType<DebugManager>();
    }

    public void OnPlayerMove(PlayerMoveEvent e)
    {
        Debug.Log("Enemy got a player move event");
        GridMover.Move(1);
    }

    void Update()
    {
        
    }
}
