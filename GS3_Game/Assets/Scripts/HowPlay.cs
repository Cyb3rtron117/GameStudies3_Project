using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class HowPlay : MonoBehaviour
{
    public Animator anim;
    private int slideIndex = 0;
    public GameObject[] Slides;
    public static bool TutisActive = false;
    public TextMeshProUGUI SlideCountText;
    public GameObject Prompt;
    private SoundManager soundScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        soundScript = GetComponent<SoundManager>();
        Prompt.SetActive(false);
        TutisActive = false;
        slideIndex = 0;
        foreach(GameObject slide in Slides)
        {
            slide.SetActive(false);
        }
        SlideCountText.SetText($"Page {slideIndex + 1} out of {Slides.Length}");
    }

    public void NextSlide()
    {
        if(TutisActive)
        {
            soundScript.PlayClick();
            Slides[slideIndex].SetActive(false);
            slideIndex++;
            if (slideIndex < Slides.Length)
            {
                Slides[slideIndex].SetActive(true);
                SlideCountText.SetText($"Page {slideIndex + 1} out of {Slides.Length}");
            }
            else
            {
                HideSlides();
            }
        }
        

    }
    private void PreviousSlide()
    {
        if (TutisActive)
        {
            if (slideIndex > 0)
            {
                Slides[slideIndex].SetActive(false);
                slideIndex--;
                Slides[slideIndex].SetActive(true);
                SlideCountText.SetText($"Page {slideIndex + 1} out of {Slides.Length}");
                soundScript.PlayClick();
            }
            else
            {
                soundScript.PlayError();
            }
        }
    }
    public void ChangeSlideInput(float direction)
    {
        if(!TutisActive)
        {
            return;
        }

        if (direction > 0.5f)
        {
            NextSlide();
        }
        else if (direction < -0.5f)
        {
            PreviousSlide();
        }
    }

    public void ShowSlides()
    {
        if(TutisActive)
        {
            soundScript.PlayError();
            return;
        }
        soundScript.PlayClick();
        TutisActive = true;
        slideIndex = 0;
        foreach (GameObject slide in Slides)
        {
            slide.SetActive(false);
        }
        Slides[slideIndex].SetActive(true);
        anim.SetBool("Show", true);
    }
    public void HideSlides()
    {
        TutisActive = false;
        anim.SetBool("Show", false);
    }

    public void ShowPrompt()
    {
        Prompt.SetActive(true);
        anim.SetTrigger("ShowPrompt");
    }
}
