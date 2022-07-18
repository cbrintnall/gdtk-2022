using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Arc.Lib.Debug;

public class DiceMovement : MonoBehaviour
{
  public Camera Overlay;
  public CinemachineVirtualCamera Main;

  public LayerMask DiceMouseCastLayer;
  public Vector3 Anchor;
  public float ZOffset = 1f;
  public float SnapSpeed = 5f;

  // If the dice is attached to the player or being moved into world space
  public bool attachedToPlayer = true;
  DebugManager dbg;
  Vector3 lastCastPosition;
  ConstantRotation rotation;
  LevelManager levelManager;
  Rigidbody rb;
  EnemyController enemyTarget;
  EnemyController lastEnemyTargeted;
  DiceController controller;
  Vector2 lastMousePosition;
  
  private void Awake()
  {
    controller = GetComponent<DiceController>();
    rb = GetComponent<Rigidbody>();
    rotation = GetComponent<ConstantRotation>();
    dbg = FindObjectOfType<DebugManager>();
    attachedToPlayer = true;
  }

  public void ResetPosition() => transform.position = Overlay.ViewportToWorldPoint(Anchor);

  private void Start()
  {
    levelManager = FindObjectOfType<LevelManager>();
  }

  private void Update()
  {
    rotation.enabled = attachedToPlayer;

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
          attachedToPlayer = false;
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
      if (enemyTarget != null)
      {
        OnEnemySelected(enemyTarget);
      }
      else
      {
        attachedToPlayer = true;
      }
    }
  }

  private void FixedUpdate()
  {
    rb.freezeRotation = attachedToPlayer;
    rb.isKinematic = attachedToPlayer;

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
      if (Physics.Raycast(transform.position, Overlay.transform.forward, out RaycastHit hit, float.PositiveInfinity))
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

    enemy.Health.Adjust(-attackState.damage);
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.blue;
    Gizmos.DrawSphere(lastCastPosition, .25f);
  }
}
