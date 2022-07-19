using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Arc.Lib.Debug;
using DG.Tweening;

public class DiceMovement : MonoBehaviour
{
  [Header("Cameras")]
  public Camera Overlay;
  public CinemachineVirtualCamera Main;

  [Header("Audio")]
  public AudioSource Player;
  public AudioClip Pickup;
  public AudioClip Drop;
  public AudioClip RollSound;

  [Header("Positioning")]
  public LayerMask DiceMouseCastLayer;
  public Vector3 Anchor;
  public float ZOffset = 1f;
  public float SnapSpeed = 5f;

  // If the dice is attached to the player or being moved into world space
  public bool attachedToPlayer = true;
  bool attacking;
  DebugManager dbg;
  Vector3 lastCastPosition;
  ConstantRotation rotation;
  LevelManager levelManager;
  Rigidbody rb;
  EnemyController enemyTarget;
  EnemyController lastEnemyTargeted;
  DiceController controller;
  PlayerController player;
  Vector2 lastMousePosition;
  
  private void Awake()
  {
    controller = GetComponent<DiceController>();
    rb = GetComponent<Rigidbody>();
    rotation = GetComponent<ConstantRotation>();
    dbg = FindObjectOfType<DebugManager>();
    player = FindObjectOfType<PlayerController>();
    attachedToPlayer = true;
  }

  public void ResetPosition() => transform.position = Overlay.ViewportToWorldPoint(Anchor);

  private void Start()
  {
    levelManager = FindObjectOfType<LevelManager>();
  }

  private void Update()
  {
    rotation.enabled = attachedToPlayer && !attacking;

    if (attacking) return;

    if (attachedToPlayer)
    {
      Vector3 targetPosition = Overlay.ViewportToWorldPoint(Anchor);

      transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * SnapSpeed);

      var ray = Overlay.ScreenPointToRay(Input.mousePosition);

      string diceHitName = "";

      if (Physics.Raycast(ray, out RaycastHit hit, float.PositiveInfinity, DiceMouseCastLayer))
      {
        diceHitName = hit.collider.name;
        if (Input.GetMouseButtonDown(0))
        {
          attachedToPlayer = false;
          Player?.PlayOneShot(Pickup);
        }
      }

      dbg.Track("Dice hit", diceHitName);
    }
    else
    {
      Vector3 mouseTranslationVec = new Vector3(Input.mousePosition.x, Input.mousePosition.y, ZOffset);

      dbg.Track("Mouse position", mouseTranslationVec);

      Vector3 targetPosition = Overlay.ScreenToWorldPoint(mouseTranslationVec);

      transform.position = targetPosition;

      dbg.Track("Dice mouse position", targetPosition);
    }

    if (!Input.GetMouseButton(0))
    {
      if (levelManager.CurrentCombat != null && levelManager.CurrentCombat.IsTurn(player) && enemyTarget != null)
      {
        OnEnemySelected(enemyTarget);
      }
      else
      {
        if (!attachedToPlayer)
        {
          attachedToPlayer = true;
          Player?.PlayOneShot(Drop);
        }
      }
    }
  }

  private void FixedUpdate()
  {
    rb.freezeRotation = attachedToPlayer;
    rb.isKinematic = attachedToPlayer;

    if (attacking) return;

    Vector2 mouse = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    var dirFromMouse = (mouse - lastMousePosition).sqrMagnitude;

    dbg.Track("Torque", dirFromMouse);

    rb.AddRelativeTorque(Vector3.up * dirFromMouse, ForceMode.Impulse);

    if (levelManager.CurrentCombat == null)
      return;

    dbg.Track("dice raycast", "");
    dbg.Track("Targeted enemy", "");

    enemyTarget = null;

    if (!attachedToPlayer)
    {
      if (!levelManager.CurrentCombat.IsTurn(player))
      {
        // TODO: make error noise, not your turn bozo!
      }
      else if (Physics.Raycast(transform.position, Overlay.transform.forward, out RaycastHit hit, float.PositiveInfinity))
      {
        dbg.Track("dice raycast", hit.collider.name);
        if (hit.collider.TryGetComponent(out EnemyController enemy))
        {
          if(levelManager.CurrentCombat.IsActiveParticipant(enemy))
          {
            dbg.Track("Targeted enemy", enemy.name);
            enemyTarget = enemy;
            enemyTarget.SetIsTargeted(true);
          }
        }
      }
    }

    if (lastEnemyTargeted != null && enemyTarget != null && enemyTarget != lastEnemyTargeted)
    {
      lastEnemyTargeted.SetIsTargeted(false);
    }
    else if(lastEnemyTargeted != null && enemyTarget == null)
    {
      lastEnemyTargeted.SetIsTargeted(false);
    }

    lastEnemyTargeted = enemyTarget;
    lastMousePosition = Input.mousePosition;
  }

  private void OnEnemySelected(EnemyController enemy)
  {
    TargetState targetState = controller.getTargetState();
    AttackState attackState = controller.getAttackState();

    Player?.PlayOneShot(RollSound);

    var seq = DOTween.Sequence();

    attacking = true;

    seq.Append(
      transform.DORotate(transform.up * 360f * 5f, 1f, RotateMode.LocalAxisAdd)
    );

    seq.Append(
      transform.DOLookAt(Overlay.transform.position, .1f)
    );

    seq.Append(
      transform.DOPunchScale(Vector3.one * .05f, 1f)
    );

    seq.AppendInterval(2.5f);

    seq.OnComplete(() =>
    {
      attacking = false;
      attachedToPlayer = true;
      enemy.Health.Damage(attackState.damage);
      levelManager.CurrentCombat.EndCurrentTurn();
    });
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.blue;
    Gizmos.DrawSphere(lastCastPosition, .25f);
  }
}
