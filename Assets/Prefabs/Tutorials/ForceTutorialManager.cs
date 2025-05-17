using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;
using TMPro;

[RequireComponent(typeof(InputData))]
public class ForceTutorialManager : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public Transform spawnPoint;
    public float spawnRadius = 1f;

	public OnTriggerWatcher spawnTrigger;
	public OnTriggerWatcher tryPullTrigger;
	public OnTriggerWatcher tryPullPushComboTrigger;

	public TMP_Text tryForcePushText;
	public TMP_Text tryForcePullText;
	public TMP_Text tryForcePullPushComboText;
	public TMP_Text tryDefeatingAllEnnemiesText;

	public GameObject Ennemy1;
	public GameObject Ennemy2;

	private InputData _inputData;
	private TutorialState _currentState;

	public enum TutorialState {
		TryForcePush,
		TryForcePull,
		TryForcePullPushCombo,
		TryDefeatingEnnemies,
		Win
	}

	void Start()
    {
        _inputData = GetComponent<InputData>();
		_currentState = TutorialState.TryForcePush;
	}

    void Update()
    {
		_inputData._leftController.TryGetFeatureValue(CommonUsages.primaryButton, out bool buttonPressed);
        if (buttonPressed && spawnTrigger.IsTriggerEmpty) {
			SpawnForceObjects();
        }
		UpdateState();
		UpdateUI();
	}

	private void UpdateState() {
		if(_currentState == TutorialState.TryForcePush) {
			if(spawnTrigger.IsTriggerEmpty) {
				_currentState = TutorialState.TryForcePull;
			}
		}else if(_currentState == TutorialState.TryForcePull) {
			if (!tryPullTrigger.IsTriggerEmpty) {
				_currentState = TutorialState.TryForcePullPushCombo;
			}
		}else if (_currentState == TutorialState.TryForcePullPushCombo) {
			if (!tryPullPushComboTrigger.IsTriggerEmpty) {
				_currentState = TutorialState.TryDefeatingEnnemies;
			}
		}else if (_currentState == TutorialState.TryDefeatingEnnemies) {
			if (Ennemy1 == null && Ennemy2 == null) {
				_currentState = TutorialState.Win;
			}
		}else if (_currentState == TutorialState.Win) {
			//TELEPORT THE PLAYER BACK TO THE MAIN HUB
		}
	}

    private void SpawnForceObjects(){
        if (prefabToSpawn == null || spawnPoint == null)
            return;

        for (int i = 0; i < 3; i++)
        {
            Vector3 offset = Random.insideUnitSphere * spawnRadius;
            offset.y = 0;
            Instantiate(prefabToSpawn, spawnPoint.position + offset, spawnPoint.rotation);
        }
    }

	private void UpdateUI() {
		if (tryForcePushText != null) tryForcePushText.gameObject.SetActive(_currentState == TutorialState.TryForcePush);
		if (tryForcePullText != null) tryForcePullText.gameObject.SetActive(_currentState == TutorialState.TryForcePull);
		if (tryForcePullPushComboText != null) tryForcePullPushComboText.gameObject.SetActive(_currentState == TutorialState.TryForcePullPushCombo);
		if (tryDefeatingAllEnnemiesText != null) tryDefeatingAllEnnemiesText.gameObject.SetActive(_currentState == TutorialState.TryDefeatingEnnemies);
		if (Ennemy1 != null) Ennemy1.gameObject.SetActive(_currentState == TutorialState.TryDefeatingEnnemies);
		if (Ennemy2 != null) Ennemy2.gameObject.SetActive(_currentState == TutorialState.TryDefeatingEnnemies);
	}
}
