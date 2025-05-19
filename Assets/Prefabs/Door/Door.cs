using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    public GameObject leftDoor;
    public GameObject rightDoor;
    public float translationLength = 2.75f;
    public float translationDuration = 1;

    private bool ready = true;
    private bool opened = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Open();
    }

    private IEnumerator TranslateOverTime(GameObject obj, Vector3 translation, float duration)
    {
        if (gameObject != null) {
            ready = false;
            // Calculate the target position
            Vector3 startPosition = obj.transform.position;
            Vector3 targetPosition = startPosition + translation;
            float elapsedTime = 0;

            while (elapsedTime < duration)
            {
                // Update the object's position
                obj.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure the object reaches the exact target position
            obj.transform.position = targetPosition;
            ready = true;
        }
    }

    public bool DoorsReady() {
        return ready;
    }

    public bool Opened() {
        return opened;
    }

    public void Open() {
        if (!ready || opened) return;
        opened = true;
        StartCoroutine(TranslateOverTime(leftDoor, -Vector3.left * translationLength, translationDuration));   
        StartCoroutine(TranslateOverTime(rightDoor, Vector3.left * translationLength, translationDuration));
        leftDoor.GetComponent<AudioSource>().Play();
        rightDoor.GetComponent<AudioSource>().Play();
    }

    public void Close() {
        if (!ready || !opened) return;
        opened = false;
        StartCoroutine(TranslateOverTime(leftDoor, Vector3.left * translationLength, translationDuration));   
        StartCoroutine(TranslateOverTime(rightDoor, -Vector3.left * translationLength, translationDuration));  
        leftDoor.GetComponent<AudioSource>().Play();
        rightDoor.GetComponent<AudioSource>().Play();    
    }

    public void Toggle() {
        Debug.Log("toggle");
        if (opened) {
            Close();
        } else {
            Open();
        }
    }
}
