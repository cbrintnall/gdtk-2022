using Arc.Lib.Debug;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StartCombatEvent : BaseEvent
{
  public Vector2Int EnemyPosition;
  public string EnemyName;
}

[RequireComponent(typeof(GridMovement))]
[RequireComponent(typeof(HpPool))]
public class EnemyController : MonoBehaviour
{
  GridMovement GridMover;
  DebugManager dbg;
  LevelManager levelManager;
  EventManager eventManager;

  Vector3 targetPos;

  private AudioSource audioplayer;

  void Start()
  {
    levelManager = FindObjectOfType<LevelManager>();
    eventManager = FindObjectOfType<EventManager>();
    eventManager.Register<PlayerMoveEvent>(OnPlayerMove);
    eventManager.Register<StartCombatEvent>(OnStartCombat);

    GridMover = GetComponent<GridMovement>();
    dbg = FindObjectOfType<DebugManager>();
    audioplayer = GetComponent<AudioSource>();
  }

  public void OnPlayerMove(PlayerMoveEvent e)
  {
    Debug.Log("Enemy got a player move event");
    StartCoroutine(Utils.Defer(ActEnemy));
  }

  public void ActEnemy()
  {
    Debug.Log("MoveEnemy");
    Vector3 dir;
    if (IsPlayerVisible(out dir))
    {
      dir.y = 0;
      Vector3 moveDir = dir.z < 0 ? new Vector3(0, 0, -1) : new Vector3(0, 0, +1);
      if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z))
      {
        moveDir = dir.x < 0 ? new Vector3(-1, 0, 0) : new Vector3(+1, 0, 0);
      }
      transform.forward = moveDir;

      Vector2Int targetTile = GridMover.GetTargetTile();
      targetPos = GridMover.Grid.CellToWorld(Utils.xyz(targetTile)) - GridMover.Grid.cellSize / 2;
      Vector2Int playerTile = levelManager.Player.GetComponent<GridMovement>().CurrentTile;

      if (targetTile == playerTile)
      {
        Debug.Log("Entering combat");
        eventManager.Publish(new StartCombatEvent { EnemyPosition = GridMover.CurrentTile, EnemyName = name });
        audioplayer.Play();
      }
      else
      {
         GridMover.Move(1);
      }
    }
  }

  bool IsPlayerVisible(out Vector3 playerDirection)
  {
    var playerTransform = levelManager.Player.transform;
    var playerPrevPos = levelManager.Player.GetComponent<PlayerController>().prevTilePos;
    RaycastHit hitInfo;
    var dirVec = playerTransform.position - transform.position;
    playerDirection = dirVec;
    Ray ray = new Ray(transform.position, dirVec);
    if (Physics.Raycast(ray, out hitInfo, dirVec.magnitude))
    {
      Debug.Log("Hit a raycast, tag is " + hitInfo.collider.tag);
      if (hitInfo.collider.CompareTag("Player")) { return true; }
      Debug.Log("We didn't return from the first raycast");
      var target2 = GridMover.Grid.CellToWorld(new Vector3Int(playerPrevPos.x, 0, playerPrevPos.y));
      var dirVec2 = target2 - transform.position;
      Ray ray2 = new Ray(transform.position, dirVec2);
      if (!Physics.Raycast(ray2, out hitInfo, dirVec2.magnitude))
      {
        Debug.Log("We didn't hit anything in the second raycast");
        playerDirection = dirVec2;
        return true;
      }
      return false;
    }
    Debug.Log("Didn't hit a raycast");
    return true;
  }

  public void OnStartCombat(StartCombatEvent e)
  {
    if (e.EnemyName == name)
    {

    }
  }

  void Update()
  {

  }

  private void OnDrawGizmos()
  {
    //Gizmos.DrawSphere(targetPos, 4);
  }
}
