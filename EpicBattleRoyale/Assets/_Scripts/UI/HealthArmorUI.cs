using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthArmorUI : MonoBehaviour {

	public Text healthText;
	HealthSystem healthSystem;

	public void Setup (HealthSystem hs)
	{
		healthSystem = hs;
		SetHealth (hs.GetHealth (), hs.GetArmor ());
		healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
	}

	void HealthSystem_OnHealthChanged (object sender, System.EventArgs e)
	{
		SetHealth (healthSystem.GetHealth (), healthSystem.GetArmor ());
	}

	void SetHealth (int health, int armor)
	{
		healthText.text = "Health: " + health + "\nArmor: " + armor;
	}

	/*	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.O)) {
			healthSystem.Damage (Random.Range (5, 100));
		}
		if (Input.GetKeyDown (KeyCode.P)) {
			healthSystem.HealHealth (Random.Range (5, 100));
		}

		if (Input.GetKeyDown (KeyCode.I)) {
			healthSystem.HealArmor (Random.Range (5, 100));
		}
	}*/

}
