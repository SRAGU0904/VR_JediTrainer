// using System;
// using UnityEngine;
//
// [Serializable]
// public class Bounce {
// 	public float acceleration = 3f;
// 	public float drag = 0.5f;
// 	public float zeroThreshold = 0.1f;
//
// 	public float TimeStep(float dt, float velocity) {
// 		acceleration -= drag * velocity * dt;
// 		if (acceleration < zeroThreshold) {
// 			acceleration = 0;
// 		}
// 		return acceleration * dt;
// 	}
// }