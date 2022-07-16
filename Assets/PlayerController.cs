using Arc.Lib.Debug;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveEvent : BaseEvent
{
  public Vector3Int NewTile;
}

[RequireComponent(typeof(GridMovement))]
public class PlayerController : MonoBehaviour
{
  GridMovement GridMover;
  DebugManager dbg;
	EventManager eventManager;

  private void Start()
  {
	GridMover = GetComponent<GridMovement>();
	dbg = FindObjectOfType<DebugManager>();
	eventManager = FindObjectOfType<EventManager>();
  }

  private void Update()
  {
	if (Input.GetKeyDown(KeyCode.Tilde))
	{
	  dbg.gameObject.SetActive(!dbg.gameObject.activeInHierarchy);
	}

	if (Input.GetKeyDown(KeyCode.D))
	{
	  GridMover.Rotate(true);
	}
	if (Input.GetKeyDown(KeyCode.A))
	{
	  GridMover.Rotate(false);
	}

	if (Input.GetKeyDown(KeyCode.W))
	{
	  GridMover.Move(1);
	  eventManager.Publish(new PlayerMoveEvent { NewTile = GridMover.CurrentTile });
	}

	dbg.Track("Player Tile", GridMover.CurrentTile);
  }
}
