using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class HealthSystem : MonoBehaviour {
    // Max health = 100f
    public float maxHealth = 100f;
    public float healthPerSecond = 3;
    public float vignetteTransitionTime = 0.5f;

    private float health;
    private Vignette vignette;
    private Coroutine transitionCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        health = maxHealth;
        Camera.main.GetComponent<PostProcessVolume>().profile.TryGetSettings(out vignette);
    }

    // Update is called once per frame
    void Update() {
        // TODO: update the vignette
        health = Math.Max(maxHealth, health + Time.deltaTime * healthPerSecond);
    }

    public void TakeDamage(float damageAmount) {
        Debug.Log("take damage");
        health -= damageAmount;
        UpdateDisplayedHealth();
    }

    private void UpdateDisplayedHealth() {
        if (transitionCoroutine != null) StopCoroutine(transitionCoroutine);
        transitionCoroutine = StartCoroutine(TransitionVignetteIntensity(vignette.intensity.value, 1f - health / maxHealth, vignetteTransitionTime));
    }

    IEnumerator TransitionVignetteIntensity(float startIntensity, float endIntensity, float duration) {
        float elapsedTime = 0f;

        while (elapsedTime < duration) {
            // Calculate the current intensity using linear interpolation
            float currentIntensity = Mathf.Lerp(startIntensity, endIntensity, elapsedTime / duration);
            vignette.intensity.value = currentIntensity;

            // Increment the elapsed time
            elapsedTime += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        // Ensure the final intensity is set correctly
        vignette.intensity.value = endIntensity;
    }
}
