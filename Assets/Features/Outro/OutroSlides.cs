using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SlideshowCutscene : MonoBehaviour
{
    public Image cutsceneImage;
    public TextMeshProUGUI dialogueText;
    public Sprite[] images;
    public string[] texts;
    public float delayBetweenSlides = 2.0f;
    public float fadeDuration = 0.35f;
    private int currentIndex = 0;
    private CanvasGroup canvasGroup;

    void Start()
    {
        Debug.Log("Slideshow starting...");
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            Debug.Log("didn't find canvas group");
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        StartCoroutine(PlayCutscene());
    }

    IEnumerator PlayCutscene()
    {
        while (currentIndex < images.Length)
        Debug.Log("Current index: " + currentIndex);
        {
            ShowSlide(currentIndex);
            yield return StartCoroutine(FadeIn());
            yield return new WaitForSeconds(delayBetweenSlides);
            yield return StartCoroutine(FadeOut());
            currentIndex++;
        }

        EndCutscene();
    }

    void ShowSlide(int index)
    {
        cutsceneImage.sprite = images[index];
        dialogueText.text = texts[index];
    }

    IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    IEnumerator FadeOut()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }

    void EndCutscene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
}
