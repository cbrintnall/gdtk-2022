using Arc.Lib.Debug;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class StartCombatEvent : BaseEvent
{
  public Vector2Int EnemyPosition;
  public ICombatParticipant[] ExistingParticipants;
}

public enum CombatIntention
{
  Heal,
  Damage,
  Block
}

public class EnemyIntention
{
  public CombatIntention Intent;
  public int Amount;
}

class IntentionData
{

}

[RequireComponent(typeof(GridMovement))]
[RequireComponent(typeof(HpPool))]
public class EnemyController : MonoBehaviour, ICombatParticipant
{

  public GameObject Owner => gameObject;
  public HpPool Health => health;

  [Header("IntentionPath")]
  public SpriteRenderer IntentionSprite;
  public TMPro.TextMeshProUGUI IntentionText;
  [Header("Combat")]
  public GameObject TargetIndicator;
  [Header("Intention Textures")]
  public Sprite Attack;
  public Sprite Defend;
  public Sprite Heal;
  [Header("Combat Sounds")]
  public AudioClip HealSound;
  public AudioClip AttackSound;
  public AudioClip BlockedSound;

  private Queue<EnemyIntention> _intentions = new();

  HpPool health;
  GridMovement GridMover;
  DebugManager dbg;
  LevelManager levelManager;
  EventManager eventManager;
  new AudioManager audio;

  private AudioSource audioplayer;

  void Awake()
  {
    IntentionSprite.enabled = false;
    IntentionText.enabled = false;

    health = GetComponent<HpPool>();
    TargetIndicator.SetActive(false);
    GridMover = GetComponent<GridMovement>();
    audioplayer = GetComponent<AudioSource>();

    health.Died.AddListener(() => Destroy(gameObject));

    _intentions.Enqueue(new EnemyIntention()
    {
      Intent = CombatIntention.Damage,
      Amount = 3
    });
    _intentions.Enqueue(new EnemyIntention()
    {
      Intent = CombatIntention.Block,
      Amount = 2
    });
    _intentions.Enqueue(new EnemyIntention()
    {
      Intent = CombatIntention.Heal,
      Amount = 4
    });
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
    audio = FindObjectOfType<AudioManager>();
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
    var seq = DOTween.Sequence();

    seq.Insert(
      1,
      IntentionSprite.DOFade(0f, .5f)
    );

    seq.Insert(
      1,
      IntentionText.DOFade(0f, .5f)
    );

    seq.AppendCallback(DoCurrentIntention);

    seq.Insert(
      3,
      IntentionSprite.DOFade(1f, .2f)
    );

    seq.Insert(
      3,
      IntentionText.DOFade(1f, .2f)
    );

    seq.AppendCallback(() => levelManager.CurrentCombat.EndCurrentTurn());
  }

  void DoCurrentIntention()
  {
    EnemyIntention myIntent = _intentions.Dequeue();

    switch (myIntent.Intent)
    {
      case CombatIntention.Damage:
        levelManager.CurrentCombat.Player.Health.Damage(myIntent.Amount);
        audio.Play(AttackSound);
        break;
      case CombatIntention.Heal:
        Health.Heal(myIntent.Amount);
        audio.Play(HealSound);
        break;
      case CombatIntention.Block:
        audio.Play(BlockedSound);
        Debug.LogWarning($"Enemy intention {nameof(CombatIntention.Block)} not yet implemented, but enemy attempted to use!");
        break;
    }

    _intentions.Enqueue(myIntent);

    EnemyIntention nextIntent = _intentions.Peek();

    switch (nextIntent.Intent)
    {
      case CombatIntention.Damage:
        IntentionSprite.sprite = Attack;
        break;
      case CombatIntention.Heal:
        IntentionSprite.sprite = Heal;
        break;
      case CombatIntention.Block:
        IntentionSprite.sprite = Defend;
        break;
    }

    IntentionText.text = nextIntent.Amount.ToString();
  }

  public void OnStartCombat()
  {
    IntentionSprite.enabled = true;
    IntentionText.enabled = true;
  }

  public void OnEndCombat()
  { }
}
