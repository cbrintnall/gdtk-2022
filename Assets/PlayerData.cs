using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public GameObject dicePrefab;
    public DiceData dice1;
    public DiceData dice2;
    public DiceData activeDice;


    public void Start()
    {
        GameObject o1 = Instantiate(dicePrefab);
        dice1 = o1.GetComponent<DiceData>();
        dice1.id = "dice1";
        dice1.description = "d6";
        dice1.numSides = 6;

        GameObject o2 = Instantiate(dicePrefab);
        dice2 = o2.GetComponent<DiceData>();
        dice2.id = "dice2";
        dice2.description = "d12";
        dice2.numSides = 12;

        activeDice = dice1;
    }
}
