using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class HealthSystem {
	
	public int health;
	int maxHealth = 100;
	public int armor;
	int maxArmor = 100;
	float armorTakeDamagePersent = .8f;

	public event EventHandler OnHealthChanged;
	public event EventHandler OnHealthZero;

	public int GetHealth ()
	{
		return health;
	}

	public int GetArmor ()
	{
		return armor;
	}

	public void Damage (int damage)
	{
		if (health > 0) {
			
			if (armor > 0) {
				armor -= Mathf.RoundToInt (damage * armorTakeDamagePersent);

			}

			if (armor < 0) {
				health += armor;
				health -= Mathf.RoundToInt (damage * (1 - armorTakeDamagePersent));
			} else if (armor == 0) {
					health -= damage;
				} else {
					health -= Mathf.RoundToInt (damage * (1 - armorTakeDamagePersent));
				}

			if (armor < 0) {
				armor = 0;
			}

			if (health <= 0) {
				health = 0;
				if (OnHealthZero != null)
					OnHealthZero (this, EventArgs.Empty);
			}

			if (OnHealthChanged != null)
				OnHealthChanged (this, EventArgs.Empty);
		}

		Debug.Log ("Damage " + damage + "  health" + health + "  armor " + armor);
	}

	public void HealArmor (int heal)
	{
		armor += heal;

		if (armor > maxArmor)
			armor = maxArmor;

		if (OnHealthChanged != null)
			OnHealthChanged (this, EventArgs.Empty);

		Debug.Log ("Heal " + heal + "  armor " + armor);
	}

	public void HealHealth (int heal)
	{
		if (health > 0) {
			health += heal;

			if (health > maxHealth)
				health = maxHealth;

			if (OnHealthChanged != null)
				OnHealthChanged (this, EventArgs.Empty);
		}

		Debug.Log ("Heal " + heal + "  health " + health);
	}

	public HealthSystem (int health, int armor)
	{
		this.health = health;
		this.armor = armor;
		maxHealth = 100;
		maxArmor = 100;
	}
	

}
