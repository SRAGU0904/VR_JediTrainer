using System.Collections;
using UnityEngine;

public class Holocron : MonoBehaviour {
    
    public Transform canvasTransform;
    
    // Duration of the animation in seconds
    public float animationDuration = 1.0f;

    // Target scale when opened
    private Vector3 openScale = new Vector3(1, 1, 1);

    // Target scale when closed
    private Vector3 closedScale = new Vector3(1, 0, 1);

    // Coroutine for animating the scale
    private Coroutine scaleCoroutine;

    // Flag to track the current state
    private bool isOpen = false;

	public void Start() {
		Toggle();
	}

	// Method to open the component
	public void Open()
    {
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
        }
        scaleCoroutine = StartCoroutine(AnimateScale(openScale));
        isOpen = true;
    }

    // Method to close the component
    public void Close()
    {
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
        }
        scaleCoroutine = StartCoroutine(AnimateScale(closedScale));
        isOpen = false;
    }

    // Method to toggle the component
    public void Toggle()
    {
        if (isOpen)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    // Coroutine to animate the scale
    private IEnumerator AnimateScale(Vector3 targetScale)
    {
        float elapsedTime = 0f;
        Vector3 initialScale = canvasTransform.localScale;

        while (elapsedTime < animationDuration)
        {
            canvasTransform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / animationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasTransform.localScale = targetScale;
    }
}