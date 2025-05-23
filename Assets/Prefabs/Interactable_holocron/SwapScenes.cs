using UnityEngine;
using UnityEngine.SceneManagement;
public class SwapScenes : MonoBehaviour
{
	public static void GotoMainHub() {
		SceneManager.LoadScene(0);
	}
	public static void GotoForce() {
		SceneManager.LoadScene(2);
	}

	public static void GotoJump() {
		SceneManager.LoadScene(1);
	}

	public static void GotoSaberThrow() {
		SceneManager.LoadScene(3);
	}

	public static void GotoSaberCut() { 
		SceneManager.LoadScene(4);
	}
}
