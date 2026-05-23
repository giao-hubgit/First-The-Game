using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public float moveSpeed = 1.5f;
    public float fadeDuration = 1f;
    private Color startColor;

    public void SetText(string content, Color color)
    {
        textMesh.text = content;
        textMesh.color = color;
        startColor = color;

        Destroy(gameObject, fadeDuration);
    }

    void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        float alpha = Mathf.Lerp(textMesh.color.a, 0, Time.deltaTime / fadeDuration * 10);
        textMesh.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
    }
}