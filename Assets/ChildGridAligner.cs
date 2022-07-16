using UnityEngine;

[ExecuteInEditMode]
public class ChildGridAligner : MonoBehaviour
{
  public Grid Grid;

  public void AlignChildrenToGrid()
  {
    for (int i = 0; i < transform.childCount; i++)
    {
      Utils.AlignToGrid(Grid, transform.GetChild(i));
    }
  }

  private void Update()
  {
    if (Application.isEditor)
    {
      AlignChildrenToGrid();
    }
  }
}
