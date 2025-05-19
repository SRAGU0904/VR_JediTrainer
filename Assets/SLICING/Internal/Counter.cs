using Unity.VisualScripting;
using UnityEngine;

public class SliceCounter : MonoBehaviour {
	public int sliceCount = 0;

	public static int GetSliceCount(GameObject go) {
		return go.GetOrAddComponent<SliceCounter>().sliceCount;
	}
	
	public static void SetSliceCount(GameObject go, int newValue) {
		go.GetOrAddComponent<SliceCounter>().sliceCount = newValue;
	}
}
