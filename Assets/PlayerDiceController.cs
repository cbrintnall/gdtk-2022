using System.Collections;
using System.Collections.Generic;
using UnityEngine;



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


public class PlayerDiceController : MonoBehaviour
{
    public GameObject dice1Prefab;
    public GameObject dice2Prefab;

    private DiceController dice1;
    private DiceController dice2;
    private DiceController activeDice;

    // Start is called before the first frame update
    void Start()
    {
        GameObject o1 = Instantiate(dice1Prefab);
        dice1 = o1.GetComponent<DiceController>();

        GameObject o2 = Instantiate(dice2Prefab);
        dice2 = o2.GetComponent<DiceController>();

        activeDice = dice1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            activeDice = dice1;
            PrintActiveDice();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            activeDice = dice2;
            PrintActiveDice();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            TargetState targetState = activeDice.getTargetState();
            string targetStr = "TargetPositions: ";
            foreach(Vector2Int v in targetState.relativeTargets)
            {
                targetStr += $"({v.x}, {v.y}) ";
            }
            UnityEngine.Debug.Log(targetStr);

            AttackState attackState = activeDice.getAttackState();
        }
    }

    void PrintActiveDice()
    {
        UnityEngine.Debug.Log($"Setting active dice to '{activeDice.diceName}', '{activeDice.diceDesc}'");
        UnityEngine.Debug.Log("The assigned items are: ");
        foreach (KeyValuePair<string, int> entry in activeDice.itemsCount)
        {
            UnityEngine.Debug.Log($" - {entry.Value}x{entry.Key}");
        }
    }
}
