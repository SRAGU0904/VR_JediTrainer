using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class OnTriggerWatcher : MonoBehaviour
{
	private HashSet<Collider> _collidersInside = new HashSet<Collider>();

	public bool IsTriggerEmpty => _collidersInside.Count == 0;

	void OnTriggerEnter(Collider other) {
		if (!other.isTrigger)
			_collidersInside.Add(other);
	}

	void OnTriggerExit(Collider other) {
		_collidersInside.Remove(other);
	}
}
