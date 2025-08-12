using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathManager))]
public class PathManagerEditor : Editor
{
    private static PathManager pathManager;
    private static bool isCreatingWaypoints = false;
    private static EWaypointType selectedWaypointType = EWaypointType.DEFAULT;
    
    void OnEnable()
    {
        pathManager = (PathManager)target;
    }
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Path Creation Tools", EditorStyles.boldLabel);

        // Waypoint type selection
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Waypoint Type:", GUILayout.Width(100));
        selectedWaypointType = (EWaypointType)EditorGUILayout.EnumPopup(selectedWaypointType);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();

        GUI.backgroundColor = isCreatingWaypoints ? Color.green : Color.white;
        if (GUILayout.Button(
            isCreatingWaypoints ?
            "Stop Creating Waypoints (Click in Scene)"
            : "Start Creating Waypoints"))
        {
            isCreatingWaypoints = !isCreatingWaypoints;
            SceneView.duringSceneGui += OnSceneGUI;
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space();
        
        if (GUILayout.Button("Clear All Waypoints"))
        {
            pathManager.ClearAllWaypoints();
        }
    }
    
    static void OnSceneGUI(SceneView sceneView)
    {
        if (!isCreatingWaypoints) return;
        
        Event e = Event.current;
        
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            // Convert mouse position to world position
            Vector3 mousePos = e.mousePosition;
            mousePos.y = sceneView.camera.pixelHeight - mousePos.y;
            
            Ray ray = sceneView.camera.ScreenPointToRay(mousePos);
            
            // Try to hit the ground plane (y = 0.5)
            float t = (0.5f - ray.origin.y) / ray.direction.y;
            if (t > 0)
            {
                Vector3 worldPos = ray.origin + ray.direction * t;
                pathManager.CreateWaypointAt(worldPos, selectedWaypointType);
                e.Use();
            }
        }
        
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
        {
            isCreatingWaypoints = false;
            SceneView.duringSceneGui -= OnSceneGUI;
            e.Use();
        }
    }
    
    void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }
}