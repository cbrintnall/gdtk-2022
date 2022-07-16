using Arc.Lib.Debug;
using UnityEditor;
using UnityEngine;
using DG.Tweening;

public class GridMovement : MonoBehaviour
{
  public Grid Grid;
  public Vector3Int CurrentTile => Grid.WorldToCell(transform.position);

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
      .OnComplete(() => audioplayer?.PlayOneShot(MoveSounds.Random()))
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

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.cyan;
    Gizmos.DrawRay(transform.position, transform.forward);
  }
}
