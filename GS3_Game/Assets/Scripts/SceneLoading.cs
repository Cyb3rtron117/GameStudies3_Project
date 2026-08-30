using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoading : MonoBehaviour
{
    public Animator transition;
    private void Awake()
    {
        if (transition == null)
        {
            transition = GetComponent<Animator>();
        }
    }

    public float transitionTime = 1f;
    public void loadMainMenu() //start scene
    {
        StartCoroutine(LoadNextLevel(0));
    }
    public void loadLevel() //level
    {
        StartCoroutine(LoadNextLevel(1));
    }

    public void Exit()
    {
        Application.Quit();
    }

    IEnumerator LoadNextLevel(int LevelIndex)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(LevelIndex);
    }
}
