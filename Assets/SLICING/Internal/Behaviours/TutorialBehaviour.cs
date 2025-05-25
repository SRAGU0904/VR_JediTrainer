using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Slicing;
using UnityEngine.Assertions;

namespace Slicing {
	public class TutorialBehaviour : DefaultSlicingBehaviour, ISlicingBehaviour {
		public float NextStageDelay = 3f;
		public int myStage = -1;

		private void OnValidate() {
			Assert.IsTrue(myStage >= 0, "Stage index not set!");;
		}

		public void OnSlicingFinished() {
			Debug.Log("Triggered TutorialBehaviour!");
			TutorialController.StageFinished(myStage, NextStageDelay);
		}
	}
}



