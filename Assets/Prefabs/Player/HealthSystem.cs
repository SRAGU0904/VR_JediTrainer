using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class HealthSystem : MonoBehaviour {
    // Max health = 100f
    public float maxHealth = 100f;
    public float healthPerSecond = 3;
    public float spawnEffectTransitionTime = 5f;
    public float vignetteTransitionTime = 0.5f;

    private float health;
    private Vignette vignette;
    private DepthOfField depthOfField;
    private Coroutine transitionCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        health = maxHealth;
        Camera.main.GetComponent<PostProcessVolume>().profile.TryGetSettings(out vignette);
        Camera.main.GetComponent<PostProcessVolume>().profile.TryGetSettings(out depthOfField);

        SpawnEffect();
    }

    // Update is called once per frame
    void Update() {
        // TODO: update the vignette
        health = Math.Max(maxHealth, health + Time.deltaTime * healthPerSecond);
    }

    public void TakeDamage(float damageAmount) {
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

    public void SpawnEffect() {
        StartCoroutine(SpawnEffect(spawnEffectTransitionTime));
    }

    IEnumerator SpawnEffect(float duration) {
        float elapsedTime = 0f;
        float startIntensity = 0;
        float endIntensity = 7f;

        depthOfField.active = true;
        while (elapsedTime < duration) {
            // Calculate the current intensity using linear interpolation
            float currentIntensity = Mathf.Lerp(startIntensity, endIntensity, elapsedTime / duration);
            depthOfField.aperture.value = currentIntensity;

            // Increment the elapsed time
            elapsedTime += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        // Ensure the final intensity is set correctly
        depthOfField.aperture.value = endIntensity;
        depthOfField.active = false;
    }
}
