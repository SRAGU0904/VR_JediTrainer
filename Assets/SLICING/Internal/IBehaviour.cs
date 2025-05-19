using UnityEngine;

namespace Slicing {
	public interface ISlicingBehaviour {
		public GameObject[] CreateHulls(GameObject objectToSlice, Vector3 planeCenter, Vector3 planeNormal);
		public void PushHulls(GameObject[] hulls, Vector3 planeNormal);
	}

}