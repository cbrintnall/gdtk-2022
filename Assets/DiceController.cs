using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using System;
using UnityEditor;
using UnityEngine.UI;

[Serializable]
public class DieFaceInformation
{
  public Transform Transform;
  [HideInInspector]
  public Face AssociatedFace;
  public int Value;
}

[ExecuteInEditMode]
public class DiceController : SerializedMonoBehaviour
{
  [Header("Audio")]
  public AudioClip SwapCanceled;

  public Dice DiceData 
  {
    get => _dice;
    set
    {
      _dice = value;

      FaceItems = new DiceSideItem[_dice.NumberOfSides];

      for(int i = 0; i < _dice.DefaultSideItems.Length;i++)
      {
        FaceItems[i] = _dice.DefaultSideItems[i]; 
      }
    }
  }
  [Header("Other")]
  public List<BaseItem> items;
  public List<DieFaceInformation> Faces = new ();
  [ReadOnly]
  public DiceSideItem[] FaceItems;
  public DiceFaceItemsController FaceItemsController => itemFaceController;
  public string diceName => DiceData.Name;
  public string diceDesc => DiceData.Description;

  [HideInInspector]
  public List<BaseItem> sortedItems;
  [HideInInspector]
  public Dictionary<string, BaseItem> itemsDic;
  [HideInInspector]
  public Dictionary<string, int> itemsCount;

  private AudioManager audio;
  private EventManager eventManager;
  private DiceFaceItemsController itemFaceController;
  private Dice _dice;

  [DisableIf("@Faces.Count > 0")]
  [Button("Generate Face Data")]
  private void GenerateFaceData()
  {
    ProBuilderMesh pbm = GetComponent<ProBuilderMesh>();

    if (pbm == null)
    {
      pbm = gameObject.AddComponent<ProBuilderMesh>();

      Mesh pbmMesh = GetComponent<MeshFilter>().sharedMesh;
      Material[] pbmMaterials = new Material[] { GetComponent<MeshRenderer>().sharedMaterial };

      MeshImporter importer = new MeshImporter(pbmMesh, pbmMaterials, pbm);
      importer.Import();

      Debug.Log($"New probuildermesh created for dice editor: {pbm}");
    }

    Faces.Clear();

    foreach(Transform child in transform)
    {
      int componentCount = child.gameObject.GetComponents(typeof(Component)).Length;

      if (componentCount == 1)
      {
        DestroyImmediate(child.gameObject, true);
      }
    }

    var validFaces = pbm.faces.Where(face => face.IsQuad()).ToList();

    List<Face> relevantFaces = validFaces
      .Select(face => Tuple.Create(face, face.ToQuad()))
      .Select(points => Tuple.Create(points.Item1, pbm.GetVertices(points.Item2).Select(vert => vert.position).ToArray()))
      .Select(vertices =>
      {
        Vector3[] verts = vertices.Item2;
        float area = UnityEngine.ProBuilder.Math.TriangleArea(verts[0], verts[1], verts[2]) +
                      UnityEngine.ProBuilder.Math.TriangleArea(verts[1], verts[2], verts[3]);

        return Tuple.Create(vertices.Item1, area);
      })
      .OrderByDescending(val => val.Item2)
      .Select(val => val.Item1)
      .Take(DiceData.NumberOfSides)
      .ToList();

    Debug.Assert(
      relevantFaces.Count == DiceData.NumberOfSides, 
      $"Generation failed to provide enough faces for the required number of sides.. {relevantFaces.Count}vs.{DiceData.NumberOfSides}"
    );

    Debug.Log($"Created a list of {relevantFaces.Count} faces for die");

    // set rotation to identity, so normals calculate correctly
    Quaternion originalRotation = transform.rotation;
    transform.rotation = Quaternion.identity;

    foreach (var face in relevantFaces)
    {
      List<Vector3> relevantVertices = new();
      List<Vector3> normals = new();
      var verts = pbm.VerticesInWorldSpace();

      for (int i = 0; i < verts.Length; i++)
      {
        if (face.indexes.Contains(i))
        {
          relevantVertices.Add(verts[i]);
          normals.Add(pbm.normals[i]);
        }
      }

      Vector3 point = UnityEngine.ProBuilder.Math.Average(relevantVertices);
      Normal normalInfo = UnityEngine.ProBuilder.Math.NormalTangentBitangent(pbm, face);

      GameObject pt = new GameObject(face.GetHashCode().ToString());
      pt.transform.SetParent(transform);
      pt.transform.localPosition = transform.InverseTransformPoint(point);
      pt.transform.forward = normalInfo.normal;

      Faces.Add(
        new DieFaceInformation()
        {
          AssociatedFace = face,
          Transform = pt.transform,
          Value = 0
        }
      );
    }

    transform.rotation = originalRotation;
  }

  DieFaceInformation GetFaceFor(int requestedFace) => Faces.Where(face => face.Value == requestedFace).FirstOrDefault();

  public void Awake()
  {
    itemFaceController = GetComponent<DiceFaceItemsController>();
    sortedItems = new List<BaseItem>();
    itemsDic = new Dictionary<string, BaseItem>();
    itemsCount = new Dictionary<string, int>();

    foreach(BaseItem item in items)
    {
        addItem(item);
    }
  }

  public void Start()
  {
    // TODO add back in
    //Debug.Assert(FacesToValues.Keys.Count == DiceData.NumberOfSides, $"Mismatch with amount of faces and number of sides in {name}");

    ForEachSide(
      side =>
      {
        int idx = side - 1;

        FaceItems[idx] = DiceData.DefaultSideItems[idx];
        itemFaceController.FaceItemAdded(side, FaceItems[idx]);
      }
    );

    eventManager = FindObjectOfType<EventManager>();
    eventManager.Register<ItemSwapEvent>(OnItemSwap);
    audio = FindObjectOfType<AudioManager>();
  }

  void OnItemSwap(ItemSwapEvent ev)
  {
    if (ev.Side.HasValue)
    {
      FaceItems[ev.Side.Value-1] = ev.Item;
    }
  }

  public void AddItems(params BaseItem[] items) => items.ToList().ForEach(addItem);

  public void addItem(BaseItem item)
  {
    // TODO: Alex this is how this is supposed to work right? names were originally used as unique keys?
    // we can use type instead here.
    string n = item.GetType().ToString();

    if (itemsDic.ContainsKey(n))
    {
        itemsCount[n]++;
    } else
    {
        itemsDic[n] = item;
        itemsCount[n] = 1;

        sortedItems.Add(item);
        sortedItems = sortedItems.OrderBy(item => item.getPriority()).ToList();
    }
  }

  public TargetState getTargetState() 
  {
    TargetState state = new TargetState();
    foreach(BaseItem item in sortedItems)
    {
        UnityEngine.Debug.Log(item.getPriority());
        item.updateTargetState(state);
    }
    return state;
  }

  public ProbState getProbState() 
  {
    ProbState state = new ProbState();

    foreach(BaseItem item in sortedItems)
    {
        item.updateProbState(state);
    }
    return state;
  }

  public int weightedSample(List<double> probs)
  {
    List<double> cumSum = new List<double>();
    double x = 0;
    foreach (double p in probs)
    {
        x += p;
        cumSum.Add(x);
    }
    UnityEngine.Debug.Log(string.Join("; ", cumSum));

    int i;
    double r = UnityEngine.Random.Range(0.0f, (float)x);
    for (i = 0; i < cumSum.Count; i++)
    {
        if (r <= cumSum[i])
        {
            break;
        }
    }

    return i;
  }

  public AttackState getAttackState() 
  {
    ProbState probState = getProbState();

    int result = weightedSample(probState.probs) + 1;

    AttackState attackState = new AttackState();
    attackState.rollResult = result;

    foreach (BaseItem item in sortedItems)
    {
        item.updateAttackState(attackState);
    }

    return attackState;
  }

  // note, the past in integer does NOT correspond to an index, but rather a side.
  // ex: index 0 will get value 1 passed in.. subtract 1 if you need an index!
  private void ForEachSide(Action<int> frs)
  {
    for(int i = 0; i < DiceData.NumberOfSides; i++)
    {
      frs(i+1);
    }
  }

  private void OnDrawGizmos()
  {
    //foreach(Transform child in transform)
    //{
    //  Gizmos.color = Selection.activeGameObject == child.gameObject ? Color.blue : Color.white;
    //  Gizmos.DrawSphere(child.transform.position, .01f);
    //  Gizmos.DrawRay(child.transform.position, child.transform.forward);
    //  Handles.Label(child.transform.position+Vector3.up*.01f, $"{child.name} - {FacesToValues[child]}");
    //}

    foreach (DieFaceInformation info in Faces)
    {
      Gizmos.DrawSphere(info.Transform.position, .01f);
      Gizmos.DrawRay(info.Transform.position, info.Transform.forward);
      Handles.Label(info.Transform.position + Vector3.up * .01f, $"{info.Transform.name} - {info.Value}");
    }
  }
}
