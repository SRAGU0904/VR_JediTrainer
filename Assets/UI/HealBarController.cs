using UnityEngine;
using UnityEngine.UIElements;

public class HealthBarController : MonoBehaviour
{

	public UIDocument uIDocument;
	private VisualElement barFill;

	[Range(0f, 100f)]
	public float health = 50f;

	private void OnEnable()
	{
		var root = uIDocument.rootVisualElement;
		barFill = root.Q<VisualElement>("health-fill");
		UpdateHealthBar();
	}

	void Update() {
		UpdateHealthBar();
	}

	public void SetHealth(float newHealth)
	{
		health = newHealth;
		UpdateHealthBar();
	}

	public void TakeDamage(float damageAmount) {
		health-= damageAmount;
		UpdateHealthBar();	
	}

	private void UpdateHealthBar()
	{
		if (barFill != null)
		{
			barFill.style.width = Length.Percent(health);
		}
	}
}
