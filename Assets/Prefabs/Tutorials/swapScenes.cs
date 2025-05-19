using UnityEngine;
using UnityEngine.SceneManagement;
public class SwapScenes : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

	 public static void GotoMainHub() {
		SceneManager.LoadScene(0);
	}
	public static void GotoForceTutorial() {
		SceneManager.LoadScene(2);
	}

	public static void GotoJumpTutorial() {
		SceneManager.LoadScene(1);
	}

	public static void GotoSaberThrowTutorial() {
		SceneManager.LoadScene(3);
	}

	public static void GotoSaberCutTutorial() { 
		SceneManager.LoadScene(4);
	}
}
