using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    public int MaxHp;
    int CurrentHp;

    private EventManager eventManager;


    // Start is called before the first frame update
    void Start()
    {
        CurrentHp = MaxHp;
        eventManager = FindObjectOfType<EventManager>();
    }

    public void onDamage(DamageEvent e)
    {
        if (e.target == gameObject)
        {
            CurrentHp -= e.damage;

            if (CurrentHp <= 0)
            {
                eventManager.Publish(new DeathEvent { target = gameObject });
            }
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
