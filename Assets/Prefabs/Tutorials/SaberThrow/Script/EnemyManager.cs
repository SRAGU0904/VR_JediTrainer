using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
	public HolocronV3 holocron;
	public List<GameObject> wave1Enemies;
	public List<GameObject> wave2Enemies;
	public List<GameObject> wave3Enemies;

	private int currentWave = 0;
	private int aliveEnemies = 0;

	void Start()
	{
		HideEnemies(wave1Enemies);
		HideEnemies(wave2Enemies);
		HideEnemies(wave3Enemies);

		StartWave1();

	}

	public void StartWave1()
	{
		currentWave = 1;
		ShowEnemies(wave1Enemies);
	}

	public void StartWave2()
	{
		currentWave = 2;
		ShowEnemies(wave2Enemies);
		holocron.DisplayMessage(1);
	}

	public void StartWave3()
	{
		currentWave = 3;
		ShowEnemies(wave3Enemies);
		holocron.DisplayMessage(2);
	}

	public void OnEnemyKilled()
	{
		aliveEnemies--;

		if (aliveEnemies <= 0)
		{
			Debug.Log($"[ENEMY MANAGER] Wave {currentWave} cleared.");

			if (currentWave == 1)
			{
				StartWave2();
			}
			else if (currentWave == 2)
			{
				StartWave3();
			}
			else if (currentWave == 3)
			{Debug.Log($"[ENEMY MANAGER] Showing {aliveEnemies} enemies in wave {currentWave}");
				holocron.DisplayMessage(3);
				Invoke(nameof(ReturnToMainHub), 10f);
			}
		}
	}

	private void ShowEnemies(List<GameObject> enemies)
	{
		aliveEnemies = 0;

		foreach (var enemy in enemies)
		{
			if (enemy == null) continue;

			enemy.SetActive(true);

			var ctrl = enemy.GetComponent<ThrowEnemyController>();
			if (ctrl != null)
			{
				ctrl.manager = this;
				aliveEnemies++;
			}

			var targeting = enemy.GetComponent<TargetingSystem>();
			if (targeting != null)
			{
				targeting.enabled = true;

				targeting.Invoke("Shoot", Random.Range(0.5f, 1.5f));
			}
		}
	}

	private void HideEnemies(List<GameObject> enemies)
	{
		foreach (var enemy in enemies)
		{
			if (enemy == null) continue;

			var targeting = enemy.GetComponent<TargetingSystem>();
			if (targeting != null)
			{
				targeting.CancelInvoke("Shoot"); 
			}

			enemy.SetActive(false); 
		}
	}

	
	private void ReturnToMainHub()
	{
		SwapScenes.GotoMainHub();
	}
}