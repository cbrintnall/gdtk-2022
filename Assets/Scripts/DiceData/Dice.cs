using UnityEngine;

[CreateAssetMenu(fileName = "New Dice", menuName = "Dice/Dice")]
public class Dice : ScriptableObject
{
  public string Name;
  public string Description;
  public Texture DiceTexture;
  public GameObject Prefab;
}