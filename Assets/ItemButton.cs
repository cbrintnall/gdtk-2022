using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ItemSwapEvent : BaseEvent
{
  public int? Side;
  public DiceSideItem Item;

  public bool GoingToInventory() => !Side.HasValue;
}

[RequireComponent(typeof(Toggleable))]
[RequireComponent(typeof(Image))]
public class ItemButton : MonoBehaviour
{
  public static List<ItemButton> ItemButtons = new();

  [Header("Audio")]
  public AudioClip RejectSound;

  [Header("Text")]
  public TMPro.TextMeshProUGUI SideText;
  public bool ShowSide
  {
    get => _showSide;
    set
    {
      _showSide = value;
      SideText.enabled = value;
    }
  }

  [Header("State")]
  public Toggleable Toggle;
  public int? Side
  {
    get => _side;
    set
    {
      _side = value;
      line.enabled = _side.HasValue;

      if (value.HasValue)
      {
        SideText.enabled = true;
        SideText.text = value.Value.ToString();
      }
      else
      {
        SideText.enabled = false;
      }
    }
  }
  public DiceSideItem Item
  {
    get => _item;
    set
    {
      _item = value;

      if (_item != null)
      {
        _image.sprite = _item.ItemTexture;
        tooltip.Title.text = _item.Name;
        tooltip.Paragraph.text = _item.Description;
      }
    }
  }

  [SerializeField]
  Tooltip tooltip;
  AudioManager audio;
  EventManager eventManager;
  DiceSideItem _item;
  Image _image;
  LineRenderer line;
  int? _side;
  DieFaceInformation attachedFace;
  bool _showSide;

  private void Awake()
  {
    line = GetComponent<LineRenderer>();
    line.SetPositions(new Vector3[] { Vector3.zero, Vector3.zero });
    line.enabled = _side.HasValue;
    eventManager = FindObjectOfType<EventManager>();
    _image = GetComponent<Image>();
    ItemButtons.Add(this);
  }

  private void Start()
  {
    audio = FindObjectOfType<AudioManager>();

    Toggle = GetComponent<Toggleable>();
    Toggle.Changed.AddListener(OnToggle);
    ShowSide = false;
  }

  void SwapItems(ItemButton swapTo)
  {
    DiceSideItem temp = swapTo.Item;

    swapTo.Item = Item;
    Item = temp;

    Toggle.Toggled = false;
    swapTo.Toggle.Toggled = false;

    eventManager.Publish(
      new ItemSwapEvent()
      {
        Side = Side,
        Item = Item,
      }
    );

    eventManager.Publish(
      new ItemSwapEvent()
      {
        Side = swapTo.Side,
        Item = swapTo.Item,
      }
    );
  }

  void OnToggle(bool state)
  {
    if (state)
    {
      ItemButton[] selected = ItemButtons
        .Where(btn => btn != this && btn.Toggle.Toggled)
        .ToArray();

      Debug.Assert(selected.Length <= 1, "More than one button chosen for swap, we should never have more than two toggled and the initiator should be filtered out.");

      if (selected.Length > 0)
      {
       ItemButton swapTo = selected.FirstOrDefault();

        if (this.Side == null && swapTo.Side == null)
        {
          audio.PlayRandomPitch(RejectSound, 1f, new Vector2(.9f, 1.1f));

          Toggle.Toggled = false;
          swapTo.Toggle.Toggled = false;

          return;
        }

        SwapItems(swapTo);
      }
    }
  }
}
