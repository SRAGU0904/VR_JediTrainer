using JetBrains.Annotations;
using UnityEngine;

public class Timer {
	public float timeStart { private set; get; }
	public float duration { private set; get; }
	
	public static Timer CreateInstance(float duration) {
		Timer result = new Timer();
		result.timeStart = Time.time;
		result.duration = duration;
		return result;
	}
	
	public bool Check() {
		return Time.time - timeStart >= duration;
	}
	
	public static bool CheckWithNull([CanBeNull] Timer timer) {
		return timer == null || timer.Check();
	}
	
}
