using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class TutorialController : MonoBehaviour {
	public List<GameObject> Stages;
	public int CurrentStageIndex = 0;

	private static TutorialController _instance;
	[CanBeNull] private int? _newStageIndex = null;
	
	private void OnValidate() {
		TutorialController[] instances = FindObjectsByType<TutorialController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
		Assert.AreEqual(instances.Length, 1, "There should be only one TutorialController in the scene!");
		_instance = instances[0];
	}

	private void Start() {
		Stages[0].SetActive(true);
		for (int i = 1; i < Stages.Count; i++) {
			Stages[i].SetActive(false);
		}
	}

	private void _StageFinished(int stageIndex, float delay = 0f) {
		lock (_instance) {
			if (stageIndex < CurrentStageIndex) {
				return;
			}
			if (_newStageIndex != null) {
				Debug.LogError("Already changing stage!");
				return;
			}
			_newStageIndex = stageIndex + 1;
			StartCoroutine(_ChangeStageAfterDelay(delay));
		}
	}
	
	private IEnumerator _ChangeStageAfterDelay(float delay = 0f) {
		yield return new WaitForSeconds(delay);
		if (_newStageIndex > Stages.Count) {
			Debug.Log("Tutorial completed! Going back to the Hub.");
			CurrentStageIndex = (int)_newStageIndex;
			_newStageIndex = null;
			SwapScenes.GotoMainHub();
			yield break;
		}
		lock (_instance) {
			Stages[CurrentStageIndex].SetActive(false);
			Assert.IsTrue(_newStageIndex != null, "Changing stage has been aborted!?");
			Stages[(int)_newStageIndex].SetActive(true);
			CurrentStageIndex = (int)_newStageIndex;
			_newStageIndex = null;
		}
	}
	
	public static void StageFinished(int stageIndex, float delay) {
		_instance._StageFinished(stageIndex, delay);
	}
}

