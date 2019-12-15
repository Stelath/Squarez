using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public Animator animator;

    private int sceneToLoad = 0;

    public void FadeToScene(int scene)
    {
        animator.SetTrigger("FadeOut");
        sceneToLoad = scene;
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
