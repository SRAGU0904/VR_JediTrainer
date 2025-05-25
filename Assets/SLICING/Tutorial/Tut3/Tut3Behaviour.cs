using JetBrains.Annotations;
using UnityEngine;
using Slicing;
using UnityEngine.Assertions;

namespace Slicing {
	public class Tut3Behaviour : DefaultSlicingBehaviour, ISlicingBehaviour {
		public void OnSlicingStarted() {
			UnityUtils.GetSingleton<Tut3Counter>().Success();
		}

		[CanBeNull]
		public new GameObject[] CreateHulls(GameObject objectToSlice, Vector3 planeCenter, Vector3 planeNormal) {
			Destroy(objectToSlice);
			return null;
		}
	}
}