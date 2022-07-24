using Arc.Lib.Debug;
using Cinemachine;
using UnityEngine;

public class PlayerMoveEvent : BaseEvent
{
  public Vector2Int OldTile;
  public Vector2Int NewTile;
}

[RequireComponent(typeof(GridMovement))]
[RequireComponent(typeof(HpPool))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour, ICombatParticipant
{
  public Camera OverlayCamera;
  public CinemachineVirtualCamera FirstPersonCamera;
  public GameObject Owner => gameObject;
  public HpPool Health => health;

  [Header("Audio")]
  public AudioClip IntroRockCrumbleNoise;

  [Header("UI")]
  public PlayerUI ui;

  StackingBool canMove = new();
  AudioSource player;
  FlagManager flagManager;
  GridMovement GridMover;
  DebugManager dbg;
  EventManager eventManager;
  LevelManager levelManager;
  HpPool health;

  public Vector2Int prevTilePos;

  private void Start()
  {
    health = GetComponent<HpPool>();
    GridMover = GetComponent<GridMovement>();
    dbg = FindObjectOfType<DebugManager>();
    eventManager = FindObjectOfType<EventManager>();
    levelManager = FindObjectOfType<LevelManager>();
    prevTilePos = GridMover.CurrentTile;
    flagManager = FindObjectOfType<FlagManager>();
    eventManager.Register<StartCombatEvent>(ev => canMove.Incr());
    eventManager.Register<CombatEndEvent>(ev => canMove.Decr());
    eventManager.Register<DialogueOpenEvent>(ev => canMove.Incr());
    eventManager.Register<DialogueCloseEvent>(ev => canMove.Decr());

    if (!flagManager.HasHappenedAtLeastOnce(Flag.INTRO_FINISHED))
    {
      ui.ShowText(
        new TextUIPayload()
        {
          TopText = "You wake up in a mysterious swamp...",
          BottomText = "The cave you came in had collapsed, the exit now blocked. The only path is forward.",
          OpenSound = IntroRockCrumbleNoise
        }
      );
    }
  }

  public void NotifyPlayerMoved()
  {
    eventManager.Publish(new PlayerMoveEvent
    {
      OldTile = prevTilePos,
      NewTile = GridMover.CurrentTile
    });

    flagManager.Incr(Flag.PLAYER_TILES_MOVED);
  }

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.F))
    {
      GetComponent<HpPool>().Damage(1);
    }

    if (Input.GetKeyDown(KeyCode.D))
    {
      GridMover.Rotate(true);
    }
    if (Input.GetKeyDown(KeyCode.A))
    {
      GridMover.Rotate(false);
    }

    if (Input.GetKeyDown(KeyCode.W) && canMove)
    {
      Vector2Int targetTile = GridMover.GetTargetTile();
      var enemies = FindObjectsOfType<EnemyController>();
      EnemyController collidedWith = null;
      foreach (EnemyController enemy in enemies)
      {
        var enemyMover = enemy.GetComponent<GridMovement>();
        Vector2Int enemyPos = enemyMover.CurrentTile;
        if (enemyPos == targetTile)
        {
          collidedWith = enemy;
          break;
        }
      }

      if (collidedWith == null)
      {
        prevTilePos = GridMover.CurrentTile;
        GridMover.Move(1);
      }
      else
      {
        var participants = new ICombatParticipant[]
        {
          this,
          collidedWith.GetComponent<EnemyController>()
        };

        eventManager.Publish(new StartCombatEvent { 
          ExistingParticipants = participants,
          EnemyPosition = collidedWith.GetComponent<GridMovement>().CurrentTile});
      }
    }

    dbg.Track("Player Tile", GridMover.CurrentTile);
  }

  public void StartTurn()
  {
    
  }

  public void OnStartCombat()
  { }

  public void OnEndCombat()
  { }
}
