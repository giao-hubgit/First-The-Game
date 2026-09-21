using UnityEngine;
using UnityEngine.InputSystem;

public class UICrosshair : MonoBehaviour
{
    [SerializeField] RectTransform crosshairUI;

    void Awake()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        if (crosshairUI != null || Mouse.current != null)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            crosshairUI.position = mousePos;
        }
    }

    private void OnDisable()
    {
        Cursor.visible = true;
    }
}
