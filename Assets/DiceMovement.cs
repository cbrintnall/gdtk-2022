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

  // If the dice is attached to the player or being moved into world space
  public bool attachedToPlayer = true;
  DebugManager dbg;
  Vector3 lastCastPosition;
  ConstantRotation rotation;
  LevelManager levelManager;

  EnemyController enemyTarget;
  EnemyController lastEnemyTargeted;
  
  private void Awake()
  {
    rotation = GetComponent<ConstantRotation>();
    dbg = FindObjectOfType<DebugManager>();
    attachedToPlayer = true;
  }

  private void Start()
  {
    levelManager = FindObjectOfType<LevelManager>();
  }

  private void Update()
  {
    rotation.enabled = attachedToPlayer;

    if (attachedToPlayer)
    {
      transform.position = Overlay.ViewportToWorldPoint(Anchor);

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
      attachedToPlayer = true;
  }

  private void FixedUpdate()
  {
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
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.blue;
    Gizmos.DrawSphere(lastCastPosition, .25f);
  }
}
