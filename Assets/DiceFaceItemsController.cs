using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DiceFaceItemsController : MonoBehaviour
{
  public float Offset = 1f;
  public float Speed = 10f;
  public float HoverSpeed = .5f;

  public Image DiceFaceItemTexturePrefab;
  public Canvas ItemTexturesCanvasPrefab;

  public bool ItemHovered { get; private set; }

  float time;
  DiceMovement diceMovement;
  Canvas itemCanvas;

  Vector2 fixedDiceAnchor => new Vector2(diceMovement.Anchor.x, diceMovement.Anchor.y);

  private void Awake()
  {
    diceMovement = GetComponent<DiceMovement>();
    itemCanvas = Instantiate(ItemTexturesCanvasPrefab, transform);

    for (int i = 0; i < itemCanvas.transform.childCount; i++)
    {
      var targetTransform = (itemCanvas.transform.GetChild(i).transform as RectTransform);

      targetTransform.anchorMin = fixedDiceAnchor;
      targetTransform.anchorMax = fixedDiceAnchor;
    }
  }

  public ItemButton GetButtonForSide(int side) => itemCanvas.transform.GetChild(side - 1).GetComponent<ItemButton>();
  
  public void FaceItemAdded(int side, DiceSideItem item)
  {
    var newImage = Instantiate(DiceFaceItemTexturePrefab, itemCanvas.transform);
    newImage.sprite = item.ItemTexture;
    newImage.rectTransform.anchorMin = fixedDiceAnchor;
    newImage.rectTransform.anchorMax = fixedDiceAnchor;
    newImage.transform.SetSiblingIndex(side-1);
    var itemButton = newImage.GetComponent<ItemButton>();
    itemButton.Item = item;
    itemButton.Side = side;
    var eventTrigger = newImage.GetComponent<EventTrigger>();
    eventTrigger.ListenForEvent<PointerEventData>(EventTriggerType.PointerEnter, data => CheckHoverState(true));
    eventTrigger.ListenForEvent<PointerEventData>(EventTriggerType.PointerExit, data => CheckHoverState(false));
  }

  void CheckHoverState(bool state)
  {
    for (int i = 0; i < itemCanvas.transform.childCount; i++)
    {
      var child = itemCanvas.transform.GetChild(i);
      ItemHovered = state;
      child.GetComponent<ItemButton>().ShowSide = ItemHovered;
    }
  }

  private void Update()
  {
    time += Time.deltaTime;
    int count = itemCanvas.transform.childCount;
    float speed = ItemHovered ? HoverSpeed : Speed;
    for (int i = 0; i < itemCanvas.transform.childCount; i++)
    {
      float positionOffset = ((i / (float)count) * 360f) * Mathf.Deg2Rad;
      float baseOffset = time + positionOffset;
      Vector2 basePosition = new Vector2(Mathf.Sin(baseOffset * speed), Mathf.Cos(baseOffset * speed));
      Vector2 targetAnchor = basePosition * Offset;
      (itemCanvas.transform.GetChild(i).transform as RectTransform).anchoredPosition = targetAnchor;
    }
  }
}
