using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

[RequireComponent(typeof(InputData))]
public class ForceTutorialManager : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public Transform spawnPoint;
    public float spawnRadius = 1f;

	public OnTriggerWatcher spawnTrigger;
	public OnTriggerWatcher tryPullTrigger;
	public OnTriggerWatcher tryPullPushComboTrigger;

	public GameObject tryForcePushHolo;
	public GameObject tryForcePullHolo;
	public GameObject tryForcePullPushComboHolo;
	public GameObject tryDefeatingAllEnnemiesHolo;

	public List<GameObject> enemies;

	private InputData _inputData;
	private TutorialState _currentState;
	private int _numberOfWaves = 0;

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
		_inputData._leftController.TryGetFeatureValue(CommonUsages.primaryButton, out bool leftButtonPressed);
		_inputData._rightController.TryGetFeatureValue(CommonUsages.primaryButton, out bool rightButtonPressed);
		if ((leftButtonPressed || rightButtonPressed) && spawnTrigger.IsTriggerEmpty) {
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
			int nextIndex = _numberOfWaves <= 3 ? (1 << _numberOfWaves) : (_numberOfWaves - 2)*8;
			bool currentWaveDefeated = true;
			for (int i = 0; i < nextIndex && i < enemies.Count; i++) {
				if (enemies[i] != null) {
					currentWaveDefeated = false;
					break;
				}
			}
			if (currentWaveDefeated) {
				_numberOfWaves += 1;
			}
			if (enemies.TrueForAll(enemy => enemy == null)) {
				_currentState = TutorialState.Win;
			}
		}else if (_currentState == TutorialState.Win) {
			SwapScenes.GotoMainHub();
		}
	}

    private void SpawnForceObjects(){
        if (prefabToSpawn == null || spawnPoint == null)
            return;

        for (int i = 0; i < 3; i++){
            Vector3 offset = Random.insideUnitSphere * spawnRadius;
            offset.y = 0;
            Instantiate(prefabToSpawn, spawnPoint.position + offset, spawnPoint.rotation);
        }
    }

	private void UpdateUI() {
		if (tryForcePushHolo != null) tryForcePushHolo.SetActive(_currentState == TutorialState.TryForcePush);
		if (tryForcePullHolo != null) tryForcePullHolo.SetActive(_currentState == TutorialState.TryForcePull);
		if (tryForcePullPushComboHolo != null) tryForcePullPushComboHolo.SetActive(_currentState == TutorialState.TryForcePullPushCombo);
		if (tryDefeatingAllEnnemiesHolo != null) tryDefeatingAllEnnemiesHolo.SetActive(_currentState == TutorialState.TryDefeatingEnnemies);
		int nextIndex = _numberOfWaves <= 3 ? (1 << _numberOfWaves) : (_numberOfWaves - 2) * 8;
		for (int i = 0; i < enemies.Count; i++) {
			GameObject enemy = enemies[i];
			if (enemy != null) {
				enemy.SetActive(_currentState == TutorialState.TryDefeatingEnnemies && i < nextIndex);
			}
		}
	}
}
