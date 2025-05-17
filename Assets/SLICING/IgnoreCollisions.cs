using UnityEngine;

public class IgnoreCollisions : MonoBehaviour
{
    void Start()
    {
	    Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Hulls"), LayerMask.NameToLayer("Hulls"));
    }

}
