using Arc.Lib.Debug;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class GridMovement : MonoBehaviour
{
  public Grid Grid;
  public Vector3Int CurrentTile => Grid.WorldToCell(transform.position);
  public UnityEvent OnMovementComplete;

  [Header("Move Speeds")]
  public float RotationSpeedSeconds = .2f;
  public float TileMoveSpeedSeconds = .1f;

  [Header("Audio")]
  public AudioClip[] MoveSounds;

  private AudioSource audioplayer;
  public float targetRotation = 90f;
  DebugManager dbg;
  Tweener rotationTween;
  Tweener moveTween;

  private void Start()
  {
    audioplayer = GetComponent<AudioSource>();
    dbg = FindObjectOfType<DebugManager>();

    Utils.AlignToGrid(Grid, transform);

    rotationTween = transform
      .DORotateQuaternion(transform.rotation, RotationSpeedSeconds)
      .SetAutoKill(false)
      .SetRecyclable(true)
      .SetEase(Ease.InOutCubic);

    moveTween = transform
      .DOMove(transform.position, TileMoveSpeedSeconds)
      .SetAutoKill(false)
      .SetRecyclable(true)
      .OnComplete(OnMoveTweenComplete)
      .SetEase(Ease.InOutCubic);
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

      if (!Physics.Raycast(transform.position, dest.normalized, out RaycastHit hit, Grid.cellSize.Average()))
      {
        moveTween.ChangeEndValue(transform.position + new Vector3(dest.x, 0f, dest.z), true);

        if (!moveTween.IsPlaying())
        {
          moveTween.Play();
        }
      }
    }

    public Vector2Int GetTargetTile(int tiles = 1)
    {
        Vector3 dir = new Vector3(
          Grid.cellSize.x * transform.forward.x,
          0,
          Grid.cellSize.z * transform.forward.z
        );

        Vector3 dest = dir * tiles + transform.position;

        Vector3Int cellPos = Grid.WorldToCell(dest);

        return new Vector2Int(cellPos.x, cellPos.z);
    }

    public void Rotate(bool right)
  {
    targetRotation += right ? 90f : -90f;

    Quaternion target = Quaternion.Euler(
      transform.rotation.eulerAngles.x,
      targetRotation,
      transform.rotation.eulerAngles.z
    );

    rotationTween.ChangeEndValue(target, true);

    if (!rotationTween.IsPlaying())
    {
      rotationTween.Play();
    }
  }

  void OnMoveTweenComplete()
  {
    audioplayer?.PlayOneShot(MoveSounds.Random());
    OnMovementComplete?.Invoke();
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.cyan;
    Gizmos.DrawRay(transform.position, transform.forward);
  }
}
