using UnityEngine;
using UnityEditor;

public class ShowImageWindow : EditorWindow
{
    private Texture2D image;
    private Vector2 scrollPosition;
    private float zoom = 1f;
    private Vector2 dragOffset;
    private bool isDragging;

    [MenuItem("Window/Image Viewer")]
    public static void ShowWindow()
    {
        GetWindow<ShowImageWindow>("Image Viewer");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Drag and drop an image below:", EditorStyles.boldLabel);

        // Texture selection field
        image = (Texture2D)EditorGUILayout.ObjectField(image, typeof(Texture2D), false);

        if (image != null)
        {
            HandleZoomAndPan();

            // Scrollable view for panning
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            // Draw the image with zoom applied
            Rect imageRect = new Rect(dragOffset.x, dragOffset.y, image.width * zoom, image.height * zoom);
            GUI.DrawTexture(imageRect, image, ScaleMode.StretchToFill);

            EditorGUILayout.EndScrollView();
        }
        else
        {
            EditorGUILayout.HelpBox("No image selected. Drag and drop an image into the box above.", MessageType.Info);
        }
    }

    private void HandleZoomAndPan()
    {
        Event e = Event.current;

        // Zooming with scroll wheel
        if (e.type == EventType.ScrollWheel)
        {
            float zoomDelta = -e.delta.y * 0.05f;
            zoom = Mathf.Clamp(zoom + zoomDelta, 0.1f, 5f);
            e.Use();
        }

        // Dragging with middle mouse button
        if (e.type == EventType.MouseDown && e.button == 2) // Middle mouse button
        {
            isDragging = true;
            e.Use();
        }
        else if (e.type == EventType.MouseUp && e.button == 2)
        {
            isDragging = false;
            e.Use();
        }

        if (isDragging && e.type == EventType.MouseDrag)
        {
            dragOffset += e.delta; // Move image when dragging
            e.Use();
        }
    }
}
