using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageEvent : BaseEvent
{
    public GameObject target;
    public int damage;
}

public class HealEvent : BaseEvent
{
    public GameObject target;
    public int healing;
}

public class DeathEvent : BaseEvent
{
    public GameObject target;
}

public class HpPool : MonoBehaviour
{
  public float NormalizedHP => (float)CurrentHp/(float)MaxHp;
  public int MaxHp;
  [PropertyRange(0, "MaxHp")]
  public int CurrentHp;

  public UnityEvent Hurt;
  public UnityEvent Healed;
  public UnityEvent Died;

  private EventManager eventManager;
  bool dead;

  // Start is called before the first frame update
  void Start()
  {
    CurrentHp = MaxHp;
    eventManager = FindObjectOfType<EventManager>();
  }

  public void Die()
  {
    dead = true;
    Died?.Invoke();
    eventManager.Publish(
      new DeathEvent()
      {
        target = gameObject
      }
    );
  }

  public void Damage(int dmg) => Adjust(dmg);
  public void Heal(int amt) => Adjust(-amt);

  private void Adjust(int dmg)
  {
    if (dead) return;

    if (Mathf.Sign(dmg) >= 0)
    {
      Hurt?.Invoke();
    }
    else
    {
      Healed?.Invoke();
    }

    CurrentHp -= dmg;

    if (CurrentHp  <= 0)
    {
      Die();
    }
  }

  public void onHeal(HealEvent e)
  {
    if (e.target == gameObject)
    {
      CurrentHp += e.healing;
      if (CurrentHp > MaxHp)
      {
        CurrentHp = MaxHp;
      }
    }
  }
}
