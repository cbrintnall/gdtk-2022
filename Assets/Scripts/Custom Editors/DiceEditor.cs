using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

[ExecuteInEditMode]
public class DiceEditor : OdinEditorWindow
{
  public Texture DiceTexture;

  const string TEXTURE_PARAMETER = "_OriginalTexture";

  GameObject lastSelectedgo;
  ProBuilderMesh probuildermesh;
  PreviewRenderUtility previewRenderer;
  float zoom = 0f;
  float Upscale = 1f;
  float NearClipPlane = 1f;
  Color BackgroundColor = Color.white;
  Vector2 lastMousePosition;
  float mouseSpeedScaling;
  float xrot;
  float yrot;
  bool lockRotation;
  int selectedFace = 0;
  Material previewMaterial;
  Face selectedMeshFace;
  int currentValue;
  bool lockObject;

  [MenuItem("Dice/Editor")]
  private static void OpenWindow()
  {
    var window = GetWindow<DiceEditor>();

    window.autoRepaintOnSceneChange = true;
    window.Show();
  }

  protected override void Initialize()
  {
    base.Initialize();

    if (previewRenderer != null)
      previewRenderer.Cleanup();

    previewRenderer = new PreviewRenderUtility();
    previewRenderer.cameraFieldOfView = 60;
    previewRenderer.camera.backgroundColor = BackgroundColor;
    previewRenderer.camera.clearFlags = CameraClearFlags.SolidColor;
    previewRenderer.camera.transform.position = new Vector3(0, 0, -5);
    previewRenderer.camera.farClipPlane = 1000;
    previewRenderer.camera.nearClipPlane = NearClipPlane;

    previewRenderer.lights[0].transform.rotation = FindDirectionalLights()[0].transform.rotation;
    previewRenderer.lights[0].intensity = 1;
    for (int i = 1; i < previewRenderer.lights.Length; ++i)
    {
      previewRenderer.lights[i].intensity = 0;
    }

    xrot = 0f;
    yrot = 0f;

    previewMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/DicePreview.mat");

    Debug.Log("Initialized dice editor");
  }

  private void Update()
  {
    Repaint();
  }

  private void OnDisable()
  {
    previewRenderer.Cleanup();
  }

  private Light[] FindDirectionalLights()
  {
    return GameObject.FindObjectsOfType<Light>().Where(light => light.type == LightType.Directional).ToArray();
  }

  void Draw(Rect r)
  {
    if (previewRenderer == null)
      Initialize();

    if (Selection.activeGameObject == null)
    {
      DrawError("No game object selected.");
      return;
    }

    var meshFilter = Selection.activeGameObject.GetComponent<MeshFilter>();
    var meshRenderer = Selection.activeGameObject.GetComponent<MeshRenderer>();

    if (meshRenderer == null && meshFilter == null)
    {
      DrawError("Target doesn't have MeshFilter or MeshRenderer");
      return;
    }

    if (!lockObject)
    {
      if (Selection.activeGameObject != lastSelectedgo)
      {
        probuildermesh = GeneratePBMesh();
        selectedFace = 0;
        previewMaterial.SetTexture(TEXTURE_PARAMETER, DiceTexture);

        Debug.Log("Repopulated probuilder mesh in dice editor");
      }

      lastSelectedgo = Selection.activeGameObject;
    }

    if (probuildermesh == null)
      DrawError("Couldn't create probuildermesh, try reselecting game object");

    previewRenderer.BeginPreview(r, GUIStyle.none);

    var ev = Event.current;

    if (ev.type == EventType.MouseDrag && ev.button == 0 && !lockRotation)
    {
      Vector2 mouseDir = (ev.mousePosition - lastMousePosition).normalized;
      xrot += mouseDir.x*mouseSpeedScaling;
      yrot += mouseDir.y*mouseSpeedScaling;
      lastMousePosition = Event.current.mousePosition;
    }

    previewRenderer.DrawMesh(
      meshFilter.sharedMesh,
      Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(yrot, -xrot, 0f), probuildermesh.transform.localScale * Upscale),
      previewMaterial,
      0
    );

    previewRenderer.camera.Render();
    var txt = previewRenderer.EndPreview();
    GUI.DrawTexture(r, txt);

    GUILayout.BeginHorizontal();
    LogStat("Quad Count", probuildermesh.faces.Where(face => face.IsQuad()).Count());
    LogStat("Is Quad", selectedMeshFace.IsQuad());
    GUILayout.Label("Lock Object");
    lockObject = EditorGUILayout.Toggle(lockObject);
    GUILayout.EndHorizontal();
    GUILayout.BeginHorizontal();
    GUILayout.Label("Zoom");
    zoom = EditorGUILayout.Slider(zoom, 1f, 100f);
    GUILayout.EndHorizontal();
    GUILayout.BeginHorizontal();
    GUILayout.Label("Upscale");
    Upscale = EditorGUILayout.Slider(Upscale, 1f, 100f);
    GUILayout.EndHorizontal();
    GUILayout.BeginHorizontal();
    GUILayout.Label("Rotation Scaling");
    mouseSpeedScaling = EditorGUILayout.Slider(mouseSpeedScaling, .25f, 5f);
    GUILayout.Label("Lock Rotation");
    lockRotation = EditorGUILayout.Toggle(lockRotation);
    GUILayout.EndHorizontal();
    GUILayout.BeginHorizontal();
    GUILayout.Label("Selected Face");
    selectedFace = Mathf.RoundToInt(EditorGUILayout.Slider(selectedFace, 0, probuildermesh.faceCount-1));
    selectedMeshFace = probuildermesh.faces[selectedFace];

    meshFilter.sharedMesh.colors = new Color[meshFilter.sharedMesh.vertexCount]
      .Select((clr, idx) => selectedMeshFace.distinctIndexes.Contains(idx) ? Color.blue : Color.white)
      .ToArray();
    GUILayout.EndHorizontal();
    previewRenderer.camera.transform.position = previewRenderer.camera.transform.position.normalized * zoom;

    CheckAndAssignValue(Selection.activeGameObject.GetComponent<DiceController>());
  }

  void CheckAndAssignValue(DiceController dc)
  {
    GUILayout.BeginHorizontal();
    GUILayout.Label("Value:");
    MeshFilter mesh = Selection.activeGameObject.GetComponent<MeshFilter>();
    currentValue = Mathf.RoundToInt(EditorGUILayout.Slider(currentValue, 1, dc.DiceData.NumberOfSides));
    if (GUILayout.Button("Assign"))
    {
      List<Vector3> relevantVertices = new();
      var verts = probuildermesh.VerticesInWorldSpace();

      for (int i = 0; i < verts.Length; i++)
      {
        if (selectedMeshFace.indexes.Contains(i))
        {
          relevantVertices.Add(verts[i]);
        }
      }

      Vector3 point = Math.Average(relevantVertices);

      //if (dc.DiceData.Faces == null)
      //  dc.DiceData.Faces = new Vector3[dc.DiceData.NumberOfSides];

      ////Vector3 newFaceData = dc.DiceData.Faces.CopyTo

      //dc.DiceData.Faces = new Vector3[dc.DiceData.NumberOfSides];
      //dc.DiceData.Faces[currentValue - 1] = point;
    }
    GUILayout.EndHorizontal();
  }

  void LogStat(string name, object stat)
  {
    GUILayout.BeginVertical();
    GUILayout.Label(name);
    GUILayout.Label(stat.ToString());
    GUILayout.EndVertical();
  }

  ProBuilderMesh GeneratePBMesh()
  {
    if (probuildermesh != null)
    {
      DestroyImmediate(probuildermesh.gameObject);
    }

    GameObject copy = Instantiate(Selection.activeGameObject);

    // this check is an assumption we're debugging, looking 
    // at an already created mesh
    if(copy.GetComponent<ProBuilderMesh>() != null)
    {
      return probuildermesh;
    }

    ProBuilderMesh pbm = copy.AddComponent<ProBuilderMesh>();

    Debug.Log($"New probuildermesh created for dice editor: {pbm}");

    MeshFilter mf = Selection.activeGameObject.GetComponent<MeshFilter>();
    MeshRenderer mr = Selection.activeGameObject.GetComponent<MeshRenderer>();

    MeshImporter importer = new MeshImporter(mf.sharedMesh, new Material[] { mr.sharedMaterial }, pbm);
    importer.Import();

    return pbm;
  }

  void DrawError(string txt)
  {
    EditorGUILayout.LabelField(txt);
  }

  protected override void OnGUI()
  {
    base.OnGUI();

    Rect r = new Rect(0.0f, 0.0f, position.width, position.height);

    Draw(r);
  }
}
