using Arc.Lib.Debug;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class GridMovement : MonoBehaviour
{
  public Grid Grid;
  public Vector2Int CurrentTile => Utils.xy(Grid.WorldToCell(transform.position));
  public UnityEvent OnMovementComplete;

  [Header("Move Speeds")]
  public float RotationSpeedSeconds = .2f;
  public float TileMoveSpeedSeconds = .1f;

  [Header("Audio")]
  public AudioClip[] MoveSounds;

  private AudioSource audioplayer;
  public float targetRotation = 0;
  DebugManager dbg;
  Tweener rotationTween;
  Tweener moveTween;

  bool firstTween = true;

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

    Debug.Log($"Setting initial rotation to {transform.rotation.eulerAngles.y}");
    targetRotation = transform.rotation.eulerAngles.y;
  }

  public void Move(int tiles = 1)
  {
    // without ignoring, you can glitch collision or the grid
    if (moveTween.IsPlaying() || rotationTween.IsPlaying()) return;

    Utils.AlignToGrid(Grid, transform);

    Vector3 dir = new Vector3(
      Grid.cellSize.x * transform.forward.x,
      Grid.cellSize.y * transform.forward.y,
      Grid.cellSize.z * transform.forward.z
    );

    Vector3 dest = dir * tiles;

    if (!Physics.Raycast(transform.position, dest.normalized, out RaycastHit hit, Grid.cellSize.Average(), ~0, QueryTriggerInteraction.Ignore))
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
    // without ignoring, you can glitch collision or the grid
    if (moveTween.IsPlaying() || rotationTween.IsPlaying()) return;

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
    if (firstTween)
    {
      firstTween = false;
      return;
    }

    if (MoveSounds.Length > 0)
    {
      audioplayer?.PlayOneShot(MoveSounds.Random());
      audioplayer.pitch = Random.Range(.9f, 1.1f);
    }
    OnMovementComplete?.Invoke();
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.cyan;
    Gizmos.DrawRay(transform.position, transform.forward);
  }
}
