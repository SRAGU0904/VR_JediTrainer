using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class TutorialController : MonoBehaviour {
	public List<GameObject> Stages;
	private int _currentStageIndex = 0;
	[CanBeNull] private int? _newStageIndex = null;
	public float defaultDelay = 3f;
	public float initialDelay = 5f;

	private static TutorialController _instance;

	private static void FindInstances() {
		TutorialController[] instances = FindObjectsByType<TutorialController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
		Assert.AreEqual(instances.Length, 1, "There should be only one TutorialController in the scene!");
		_instance = instances[0];
	}
	
	private void OnValidate() {
		FindInstances();
	}

	private IEnumerator AsyncStart() {
		yield return new WaitForSecondsRealtime(initialDelay);
		ExecuteStageChange(_currentStageIndex);
		StartCoroutine(Worker(defaultDelay));
	}
	
	private void Start() {
		FindInstances();
		_instance.StartCoroutine(AsyncStart());
	}

	private bool ExecuteStageChange(int new_) {
		if (new_ >= Stages.Count) {
			// This object should get destroyed and Worker coroutine interrupted,
			// but just in case exit cleanly on our own.
			SwapScenes.GotoMainHub();
			return true;
		}

		for (int i = 0; i < Stages.Count; i++) {
			if (i != new_) {
				Stages[i].SetActive(false);
			}
		}
		_currentStageIndex = new_;
		_newStageIndex = null;
		Stages[new_].SetActive(true);
		return false;
	}

	private IEnumerator Worker(float delay) {
		while (true) {
			yield return new WaitForFixedUpdate();
			// Freeze the values so that no external change can break things
			int currentStageIndex;
			int newStageIndex;
			lock (_instance) {
				currentStageIndex = _currentStageIndex;
				newStageIndex = _newStageIndex.GetValueOrDefault(currentStageIndex);
			}
			if (_currentStageIndex == newStageIndex) {
				continue;
			}
			yield return new WaitForSeconds(delay);
			lock (_instance) {
				if (ExecuteStageChange(newStageIndex)) {
					yield break;
				}
			}
		}
		
	}
	
	public static void StageFinished(int stageIndex) {
		lock (_instance) {
			if (stageIndex < _instance._currentStageIndex) {
				Debug.LogWarning("Tutorial stage already finished!");
				return;
			}
			_instance._newStageIndex = stageIndex + 1;
		}
	}

	// [CanBeNull]
	// public static GameObject GetMyStage(GameObject caller) {
	// 	while (caller != null) {
	// 		if (_instance.Stages.Contains(caller)) {
	// 			return caller;
	// 		}
	// 		caller = caller.transform.parent.gameObject;
	// 	}
	//
	// 	return caller;
	// }
}

