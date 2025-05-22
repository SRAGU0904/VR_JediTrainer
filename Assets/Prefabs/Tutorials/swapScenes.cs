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
		SceneManager.LoadScene("Force_tutorial");
	}

	public static void GotoJumpTutorial() {
		SceneManager.LoadScene("JumpTutorial");
	}

	public static void GotoSaberThrowTutorial() {
		SceneManager.LoadScene("SaberThrow_tutorial");
	}

	public static void GotoSaberCutTutorial() { 
		SceneManager.LoadScene("SlicingTutorial");
	}
}
