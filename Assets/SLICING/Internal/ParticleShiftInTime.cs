using System;
using UnityEngine;

public class ParticleShiftInTime : MonoBehaviour {
	public float startTime;
	public float endTime;
	private ParticleSystem _particleSystem;
	private float _beginTimer;

	void Start() {
		_beginTimer = Time.time;
		_particleSystem = GetComponent<ParticleSystem>();
		_Update();
	}

	public float croppedDuration => startTime - endTime;

	private float Progress() {
		return 1 + Unity.Mathematics.math.fmod(Time.time - _beginTimer, croppedDuration) / croppedDuration;
	}


void _Update() {
	    _particleSystem.time = Mathf.Lerp(startTime, endTime, Progress());
	    Debug.Log(_particleSystem.time);
    }
    
    void Update() {
	    _Update();
    }
}
