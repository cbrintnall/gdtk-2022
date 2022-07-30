using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TargetState
{
    public List<Vector2Int> relativeTargets = new List<Vector2Int>();
}

public class ProbState
{
    public List<double> probs = new List<double>();
}

public class AttackState
{
    public int rollResult;
    public int damage;
}

public class DiceGainedEvent : BaseEvent
{
  public Dice DiceGained;
}

[RequireComponent(typeof(PlayerController))]
public class PlayerDiceController : MonoBehaviour
{
  [Header("Audio")]
  public AudioClip RollDiceSound;
  public AudioClip DamageSound;

  public bool Attacking
  {
    get => attacking;
    private set => attacking = value;
  }

  // TODO: allow holding multiple dice, for now theres only one
  public DiceController HeldDie;
  public Queue<DiceSideItem> CombatItemOptions;
  [Tooltip("How many items a player is able to choose from in combat, queue based.")]
  public int OptionsShownInCombat = 3;
  [SerializeField]
  private List<DiceSideItem> items = new List<DiceSideItem>();
  private PlayerController playerController;

  bool attacking;
  EventManager eventManager;
  LevelManager levelManager;
  AudioManager player;
  PlayerUI ui;

  private void Start()
  {
    ui = FindObjectOfType<PlayerUI>();
    playerController = GetComponent<PlayerController>();
    player = FindObjectOfType<AudioManager>();
    levelManager = FindObjectOfType<LevelManager>();
    eventManager = FindObjectOfType<EventManager>();
    eventManager.Register<ItemSwapEvent>(OnItemSwap);
  }

  public void GainSideItem(DiceSideItem item)
  {
    items.Add(item);
  }

  public void GainDice(Dice dice)
  {
    // Create the new dice in the world under the overlay camera
    GameObject newDie = Instantiate(dice.Prefab, playerController.OverlayCamera.transform);

    DiceMovement dm = newDie.GetComponent<DiceMovement>();
    DiceController dc = newDie.GetComponent<DiceController>();

    Debug.Assert(dm != null);

    dc.DiceData = dice;
    dc.AddItems(dice.DefaultItems);
    dm.Main = playerController.FirstPersonCamera;
    dm.Overlay = playerController.OverlayCamera;
    dm.ResetPosition();

    HeldDie = dc;
  }

  public void DoDiceRollWithTarget(EnemyController enemy, Action onComplete = null)
  {
    if (attacking) return;

    attacking = true;

    TargetState targetState = HeldDie.getTargetState();
    AttackState attackState = HeldDie.getAttackState();

    player.Play(RollDiceSound);

    var seq = DOTween.Sequence();

    seq.Append(
      HeldDie.transform.DORotate(HeldDie.transform.up * 360f * 5f, .5f, RotateMode.LocalAxisAdd)
    );

    seq.OnComplete(() =>
    {
      DiceSideItem landedItem = HeldDie.FaceItems[attackState.rollResult];

      Debug.Assert(landedItem != null);

      DiceLandedPayload payload = new DiceLandedPayload()
      {
        Side = attackState.rollResult,
        Target = enemy,
        Controller = HeldDie
      };

      StartCoroutine(StartPlayerAction(landedItem, payload, onComplete));
    });
  }

  IEnumerator StartPlayerAction(DiceSideItem landedItem, DiceLandedPayload payload, Action onComplete = null)
  {
    yield return landedItem.OnLanded(payload);

    levelManager.CurrentCombat.EndCurrentTurn();

    attacking = false;
    onComplete?.Invoke();
  }

  void OnItemSwap(ItemSwapEvent ev)
  {
    if (ev.GoingToInventory())
    {
      items.Add(ev.Item);
    }
    else
    {
      items.Remove(ev.Item);
    }
  }

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.Tab))
    {
      ShowItemOptions();
    }
  }

  void ShowItemOptions()
  {
    if (levelManager.GameState == GameState.IN_COMBAT)
    {
      ui.ToggleItemOptions();
      ui.SetDiceItemOptions(CombatItemOptions.Take(OptionsShownInCombat).ToList());
    }
    else if(levelManager.GameState == GameState.EXPLORING)
    {
      ui.ToggleItemOptions();
      ui.SetDiceItemOptions(items);
    }
  }
}
