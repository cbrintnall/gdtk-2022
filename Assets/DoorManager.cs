using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  public void OpenDoor(int doorIndex)
  {
    string doorStr = "Door" + doorIndex;
    var door = GameObject.Find(doorStr);
    door?.SetActive(false);
  }
}
