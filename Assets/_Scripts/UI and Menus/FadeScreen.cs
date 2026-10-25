using UnityEngine;
using UnityEngine.SceneManagement;

// Lives on Canvas/FadeScreen, which starts inactive so it doesn't block raycasts
// for the rest of the level. Animation Events can only call methods on a script
// attached to the same GameObject as the Animator, so the actual scene load is
// kicked off from here once the Fade Out clip's end-of-clip event fires, instead
// of on a fixed timer racing the animation.
//
// Starting inactive means Awake() never runs until something activates this
// GameObject, so callers must look this up with FindObjectOfType<FadeScreen>(true)
// (includeInactive) rather than a static instance set in Awake.
public class FadeScreen : MonoBehaviour
{
    private Animator anim;
    private string pendingLevel;

    public void PlayFadeOut(string levelToLoad)
    {
        if (anim == null) anim = GetComponent<Animator>();

        pendingLevel = levelToLoad;
        gameObject.SetActive(true);
        anim.SetTrigger("Fade Out");
    }

    // Wired to the animation event at the end of Fade Out.anim.
    public void OnFadeOutComplete()
    {
        SceneManager.LoadScene(pendingLevel);
    }
}
