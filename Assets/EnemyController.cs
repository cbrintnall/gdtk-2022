using Arc.Lib.Debug;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StartCombatEvent : BaseEvent
{
  public Vector2Int EnemyPosition;
  public ICombatParticipant[] ExistingParticipants;
}

[RequireComponent(typeof(GridMovement))]
[RequireComponent(typeof(HpPool))]
public class EnemyController : MonoBehaviour, ICombatParticipant
{
  public GameObject Owner => gameObject;
  public HpPool Health => health;

  [Header("Combat")]
  public GameObject TargetIndicator;

  HpPool health;
  GridMovement GridMover;
  DebugManager dbg;
  LevelManager levelManager;
  EventManager eventManager;

  private AudioSource audioplayer;

  void Awake()
  {
    health = GetComponent<HpPool>();
    TargetIndicator.SetActive(false);
    GridMover = GetComponent<GridMovement>();
    audioplayer = GetComponent<AudioSource>();

    health.Died.AddListener(() => Destroy(gameObject));
  }

  private void OnDestroy()
  {
    eventManager.Unregister<PlayerMoveEvent>(OnPlayerMove);
  }

  private void Start()
  {
    levelManager = FindObjectOfType<LevelManager>();
    eventManager = FindObjectOfType<EventManager>();
    eventManager.Register<PlayerMoveEvent>(OnPlayerMove);
    dbg = FindObjectOfType<DebugManager>();
  }

  public void OnPlayerMove(PlayerMoveEvent e)
  {
    StartCoroutine(Utils.Defer(ActEnemy));
  }

  public void ActEnemy()
  {
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
      Vector2Int playerTile = levelManager.Player.GetComponent<GridMovement>().CurrentTile;

      if (targetTile == playerTile)
      {
        Debug.Log("Entering combat");
        var participants = new ICombatParticipant[] { this, levelManager.Player };

        eventManager.Publish(new StartCombatEvent { EnemyPosition = GridMover.CurrentTile, ExistingParticipants = participants });
        audioplayer.Play();
      }
      else
      {
         GridMover.Move(1);
      }
    }
  }

  public void SetIsTargeted(bool yes) => TargetIndicator.SetActive(yes);

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

  public void StartTurn()
  {
    StartCoroutine(FakeAttack(1));
  }

  // TODO: this is just behavior stubbing, will replace with real combat
  IEnumerator FakeAttack(int dmg)
  {
    yield return new WaitForSeconds(2.5f);
    FindObjectOfType<PlayerController>().Health.Damage(dmg);
    levelManager.CurrentCombat.EndCurrentTurn();
  }
}
