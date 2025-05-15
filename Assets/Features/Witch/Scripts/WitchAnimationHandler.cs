using UnityEngine;

public class WitchAnimationHandler : MonoBehaviour
{
    [Header("Animators")]
    public Animator faceAnimator;
    public Animator eyesAnimator;
    public Animator headAnimator;

    string defaultFaceState = "FaceIdle";
    string defaultEyesState = "eyesIdle 1";
    string defaultHeadState = "Hover";

    void Start()
    {
        headAnimator.Play(defaultHeadState);
        PlayFaceIdle();
        eyesAnimator.enabled = false;
    }

    public void EnterBargain()
    {
        eyesAnimator.enabled = true; 
        PlayEyesIdle();
    }

    public void ExitBargain()
    {
        eyesAnimator.enabled = false;
    }

    public void PlayEyesIdle()
    {
        eyesAnimator.Play(defaultEyesState);
    }

    public void PlayFaceIdle()
    {
        faceAnimator.Play(defaultFaceState);
    }

    public void PlayTalking()
    {
        faceAnimator.SetTrigger("Talking");
    }

    public void PlayCrazed()
    {
        faceAnimator.SetTrigger("Crazed");
    }

    public void PlayCrazedEyes()
    {
        eyesAnimator.SetTrigger("CrazedEyes");
    }

    public void PlaySmirk()
    {
        faceAnimator.SetTrigger("Smirk");
    }

    public void ResetToIdle()
    {
        faceAnimator.Play(defaultFaceState);

        if (eyesAnimator.enabled)
        {
            eyesAnimator.Play(defaultEyesState);
        }
    }
}
