using Arc.Lib.Debug;
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
	LevelManager levelManager;

  private void Start()
  {
		GridMover = GetComponent<GridMovement>();
		dbg = FindObjectOfType<DebugManager>();
		eventManager = FindObjectOfType<EventManager>();
		levelManager = FindObjectOfType<LevelManager>();
  }

	public void NotifyPlayerMoved() => eventManager.Publish(new PlayerMoveEvent { NewTile = GridMover.CurrentTile });

  private void Update()
  {
		if (Input.GetKeyDown(KeyCode.Tilde))
		{
			dbg.enabled = !dbg.enabled;
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
		}

		dbg.Track("Player Tile", GridMover.CurrentTile);
  }
}
