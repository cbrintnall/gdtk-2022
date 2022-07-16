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
        Vector3 dir;
        if (IsPlayerVisible(out dir))
        {
            dir.y = 0;
            Vector3 moveDir = dir.z < 0 ? new Vector3(0, 0, -1) : new Vector3(0, 0, +1);
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z))
            {
                moveDir =  dir.x < 0 ? new Vector3(-1, 0, 0) : new Vector3(+1, 0, 0);
            }
            transform.forward = moveDir;
            GridMover.Move(1);
        }
    }

    bool IsPlayerVisible(out Vector3 playerDirection)
    {
        var playerTransform = levelManager.Player.transform;
        RaycastHit hitInfo;
        var dirVec = playerTransform.position - transform.position;
        playerDirection = dirVec.normalized;
        Ray ray = new Ray(transform.position, dirVec);
        if (Physics.Raycast(ray, out hitInfo, dirVec.magnitude))
        {
            return hitInfo.collider.CompareTag("Player");
        }
        return true;
    }

    void Update()
    {
        
    }
}
