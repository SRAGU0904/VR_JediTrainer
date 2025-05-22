using UnityEngine;
using System;

public class HolocroneTeleportInfo : MonoBehaviour
{
	public Action TeleportAction;

    public void TriggerTeleport() {
		TeleportAction?.Invoke();
	}
}
