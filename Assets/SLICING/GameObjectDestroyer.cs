using JetBrains.Annotations;
using UnityEngine;

public class GameObjectDestroyer : MonoBehaviour {
	public static void DestroyGameObject(GameObject toDestroy) {
		Destroy(toDestroy);
	}
}
