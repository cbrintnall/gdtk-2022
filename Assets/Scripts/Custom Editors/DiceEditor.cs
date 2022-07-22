using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class DiceEditor : OdinEditorWindow
{
  public Material DicePreviewMaterial;

  PreviewRenderUtility previewRenderer;
  float zoom = 0f;
  float Upscale = 1f;
  float NearClipPlane = 1f;
  Color BackgroundColor = Color.white;
  Vector2 lastMousePosition;
  float mouseSpeedScaling;
  float xrot;
  float yrot;
  float normalThreshold;
  int lastSelected = 0;
  List<int> selectedVertices = new();

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

    var target = Selection.activeGameObject;

    if (target == null)
    {
      DrawError("No game object selected.");
      return;
    }

    var meshFilter = target.GetComponent<MeshFilter>();
    var meshRenderer = target.GetComponent<MeshRenderer>();

    if (meshRenderer == null && meshFilter == null)
    {
      DrawError("Target doesn't have MeshFilter or MeshRenderer");
      return;
    }

    var ev = Event.current;

    if (ev.type == EventType.MouseDrag && ev.button == 0)
    {
      Vector2 mouseDir = (ev.mousePosition - lastMousePosition).normalized;
      xrot += mouseDir.x*mouseSpeedScaling;
      yrot += mouseDir.y*mouseSpeedScaling;
      lastMousePosition = Event.current.mousePosition;
    }

    previewRenderer.BeginPreview(r, GUIStyle.none);

    previewRenderer.DrawMesh(
      meshFilter.sharedMesh,
      Matrix4x4.TRS(target.transform.position, Quaternion.Euler(yrot, -xrot,0f), target.transform.localScale * Upscale),
      DicePreviewMaterial ?? meshRenderer.sharedMaterial,
      0
    );

    previewRenderer.camera.Render();
    var txt = previewRenderer.EndPreview();
    GUI.DrawTexture(r, txt);

    GUILayout.BeginHorizontal();
    GUILayout.Label("Zoom");
    zoom = EditorGUILayout.Slider(zoom, 1f, 100f);
    GUILayout.EndHorizontal();
    GUILayout.BeginHorizontal();
    GUILayout.Label("Upscale");
    Upscale = EditorGUILayout.Slider(Upscale, 1f, 100f);
    GUILayout.EndHorizontal();
    GUILayout.BeginHorizontal();
    GUILayout.Label("Near Clip Plane");
    NearClipPlane = EditorGUILayout.Slider(NearClipPlane, 0f, 100f);
    GUILayout.EndHorizontal();
    GUILayout.BeginHorizontal();
    GUILayout.Label("Rotation Scaling");
    mouseSpeedScaling = EditorGUILayout.Slider(mouseSpeedScaling, 0f, 100f);
    GUILayout.EndHorizontal();
    GUILayout.BeginHorizontal();
    GUILayout.Label("Normal threshold");
    normalThreshold = EditorGUILayout.Slider(normalThreshold, -1, 1f);
    GUILayout.EndHorizontal();
    GUILayout.BeginHorizontal();
    if (GUILayout.Button("Next Vertex"))
    {
      SelectNextVertex(meshFilter);
    }
    if (selectedVertices.Count == 1 && GUILayout.Button("Select Face"))
    {
      SelectFace(meshFilter);
    }
    else if (selectedVertices.Count > 1 && GUILayout.Button("Clear Face"))
    {
      selectedVertices.Clear();
    }
    GUILayout.EndHorizontal();
    previewRenderer.camera.transform.position = previewRenderer.camera.transform.position.normalized * zoom;
  }

  void SelectNextVertex(MeshFilter meshFilter)
  {
    if (selectedVertices.Count > 0)
    {
      selectedVertices[0] = selectedVertices[0] + 1 % meshFilter.sharedMesh.vertexCount;
    }else
    {
      selectedVertices = new List<int>() { 0 };
    }

    Debug.Log($"Selected vertex = {selectedVertices[0]}");
    meshFilter.sharedMesh.colors = new Color[meshFilter.sharedMesh.vertexCount].Select((clr, idx) => selectedVertices.Contains(idx) ? Color.green : Color.white).ToArray();
  }

  void SelectFace(MeshFilter filter)
  {
    //Vector3 currentNormal = filter.sharedMesh.normals[selectedVertex];

    //for(int i = 0; i < filter.sharedMesh.vertexCount; i++)
    //{
    //  float dot = Vector3.Dot(currentNormal, filter.sharedMesh.normals[i]);
    //}
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
