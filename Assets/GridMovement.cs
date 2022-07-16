using Arc.Lib.Debug;
using UnityEngine;

public class GridMovement : MonoBehaviour
{
  public Grid Grid;
  public Vector3Int CurrentTile => Grid.WorldToCell(transform.position);

  DebugManager dbg;

  private void Start()
  {
    dbg = FindObjectOfType<DebugManager>();

    Utils.AlignToGrid(Grid, transform);
  }

  public void Move(int tiles = 1)
  {
    Utils.AlignToGrid(Grid, transform);

    Vector3 dir = new Vector3(
      Grid.cellSize.x * transform.forward.x,
      Grid.cellSize.y * transform.forward.y,
      Grid.cellSize.z * transform.forward.z
    );

    Vector3 dest = dir * tiles;

    dbg.DoDebug(() =>
    {
      Debug.DrawRay(transform.position, dest, Color.red, 1f);
    });

    if (Physics.Raycast(transform.position, dest, out RaycastHit hit))
    {
      print(hit.collider.name);
    }
    else
    {
      transform.position += new Vector3(dest.x, 0f, dest.z);
    }
  }

  public void Rotate(bool right)
  {
    float rotationAmt = right ? 90f : -90f;

    transform.rotation = Quaternion.Euler(
      transform.rotation.eulerAngles.x,
      transform.rotation.eulerAngles.y + rotationAmt,
      transform.rotation.eulerAngles.z
    );
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.cyan;
    Gizmos.DrawRay(transform.position, transform.forward);
  }
}
