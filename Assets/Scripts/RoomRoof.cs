using UnityEngine;
using System.Collections;

public class RoomRoof : MonoBehaviour
{
    public GameObject fogCover;

    public bool smoothFade = true;
    public float fadeSpeed = 3f;

    private bool isRevealed = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isRevealed)
        {
            isRevealed = true;

            if (fogCover != null)
            {
                StopAllCoroutines();

                if (smoothFade)
                {
                    StartCoroutine(FadeOutFog());
                }
                else
                {
                    fogCover.SetActive(false);
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isRevealed)
        {
            isRevealed = false;

            if (fogCover != null)
            {
                StopAllCoroutines();

                if (smoothFade)
                {
                    StartCoroutine(FadeInFog());
                }
                else
                {
                    fogCover.SetActive(true);

                    SpriteRenderer fogRenderer = fogCover.GetComponent<SpriteRenderer>();
                    if (fogRenderer != null)
                    {
                        Color c = fogRenderer.color;
                        c.a = 1f;
                        fogRenderer.color = c;
                    }
                }
            }
        }
    }

    IEnumerator FadeOutFog()
    {
        SpriteRenderer fogRenderer = fogCover.GetComponent<SpriteRenderer>();

        if (fogRenderer != null)
        {
            Color fogColor = fogRenderer.color;

            while (fogColor.a > 0f)
            {
                fogColor.a -= Time.deltaTime * fadeSpeed;
                fogRenderer.color = fogColor;
                yield return null;
            }

            fogColor.a = 0f;
            fogRenderer.color = fogColor;
        }

        fogCover.SetActive(false);
    }

    IEnumerator FadeInFog()
    {
        fogCover.SetActive(true);
        SpriteRenderer fogRenderer = fogCover.GetComponent<SpriteRenderer>();

        if (fogRenderer != null)
        {
            Color fogColor = fogRenderer.color;

            while (fogColor.a < 1f)
            {
                fogColor.a += Time.deltaTime * fadeSpeed;
                fogRenderer.color = fogColor;
                yield return null;
            }

            fogColor.a = 1f;
            fogRenderer.color = fogColor;
        }
    }
}