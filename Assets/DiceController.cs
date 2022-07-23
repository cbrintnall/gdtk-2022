using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using System;
using UnityEditor;
using Sirenix.OdinInspector.Editor;

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
  public Dice DiceData;
  public List<BaseItem> items;
  //[OnCollectionChanged("SyncValuesToFaces")]
  public Dictionary<Transform, int> FacesToValues = new();
  [OnCollectionChanged("SyncValuesToFaces")]
  public List<DieFaceInformation> Faces = new ();
  [ReadOnly]
  public Dictionary<int, Transform> ValuesToFaces = new();

  public string diceName => DiceData.Name;
  public string diceDesc => DiceData.Description;

  [HideInInspector]
  public List<BaseItem> sortedItems;
  [HideInInspector]
  public Dictionary<string, BaseItem> itemsDic;
  [HideInInspector]
  public Dictionary<string, int> itemsCount;

  private IEnumerable Sides => Enumerable.Range(0, DiceData.NumberOfSides);

  [DisableIf("@FacesToValues.Keys.Count > 0")]
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

  [DisableIf("@FacesToValues.Keys.Count == 0")]
  [Button("Sync face data")]
  void SyncValuesToFaces()
  {
    ValuesToFaces = new Dictionary<int, Transform>();

    foreach(var key in FacesToValues.Keys)
    {
      ValuesToFaces[FacesToValues[key]] = key;
    }
  }

  [Button("Look at face")]
  void FaceLookAt(int face, Transform _)
  {
    transform.LookAt(
      GetFaceFor(face).Transform
    );
  }

  DieFaceInformation GetFaceFor(int requestedFace) => Faces.Where(face => face.Value == requestedFace).FirstOrDefault();

  public void Awake()
  {
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

  public TargetState getTargetState() {
    TargetState state = new TargetState();
    foreach(BaseItem item in sortedItems)
    {
        UnityEngine.Debug.Log(item.getPriority());
        item.updateTargetState(state);
    }
    return state;
  }

  public ProbState getProbState() {
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

  public AttackState getAttackState() {
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
