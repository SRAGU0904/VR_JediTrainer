using UnityEngine;
using UnityEngine.SceneManagement;
public class SwapScenes : MonoBehaviour
{
	public static void GotoMainHub() {
		SceneManager.LoadScene("MainHubArea");
	}
	public static void GotoForce() {
		SceneManager.LoadScene("Force_tutorial");
	}

	public static void GotoJump() {
		SceneManager.LoadScene("JumpTutorial");
	}

	public static void GotoSaberThrow() {
		SceneManager.LoadScene("SaberThrow_tutorial");
	}

	public static void GotoSaberCut() { 
		SceneManager.LoadScene("SlicingTutorial");
	}
}
