using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerAreaDetector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            SwapScenes.GotoMainHub();
        }
    }

}
