using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerDiceController : MonoBehaviour
{

    private PlayerData playerData;
    private EventManager eventManager;

    public GameObject item1Prefab;

    // Start is called before the first frame update
    void Start()
    {
        playerData = gameObject.GetComponent<PlayerData>();
        eventManager = FindObjectOfType<EventManager>();
        eventManager.Register<OnRollEvent>(onRoll);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            playerData.activeDice = playerData.dice1;
            PrintActiveDice();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            playerData.activeDice = playerData.dice2;
            PrintActiveDice();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            playerData.activeDice.roll();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            BaseItem.assignToDice(item1Prefab, playerData.activeDice);
        }

        if (Input.GetKeyDown(KeyCode.X)) { }
    }

    void onRoll(OnRollEvent e)
    {
        UnityEngine.Debug.Log($"Rolling the active dice: {e.dice.id}");
    }

    void PrintActiveDice()
    {
        DiceData d = playerData.activeDice;
        UnityEngine.Debug.Log($"Setting active dice to '{d.id}', '{d.description}'");
        UnityEngine.Debug.Log("The assigned items are: ");
        foreach (BaseItem item in d.items)
        {
            UnityEngine.Debug.Log($" - {item.getCount()}x{item.getName()}");
        }
    }
}
