using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpPool : MonoBehaviour
{

  public int MaxHp;
  int CurrentHp;
    // Start is called before the first frame update
  void Start()
  {
    CurrentHp = MaxHp;
  }

  // Update is called once per frame
  void Update()
  {

  }
}
