using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHandler : MonoBehaviour {
	
	public int damage;
	public float bulletSize = .5f;
	public float speed = 5;
	float destroyTime = 3;
	Vector3 direction;
	public CharacterBase cb;

	void FixedUpdate ()
	{
		destroyTime -= Time.fixedDeltaTime;
		if (destroyTime <= 0) {
			DestroyBullet ();
		}

		transform.position += direction * Time.fixedDeltaTime * speed;

		Debug.DrawRay (transform.position - direction * bulletSize, direction * (bulletSize * 2 + .02f));

		RaycastHit2D hit = Physics2D.Raycast (transform.position - (direction * bulletSize), direction, bulletSize * 2);

		if (hit.collider == null)
			return;
		Debug.Log ("Hitted " + hit.collider.name);

		CharacterBase damagable = hit.transform.GetComponent <CharacterBase> ();
		if (damagable != null)
			OnHit (hit, damagable);
	}

	public void Setup (CharacterBase cb, Vector2 dir, int damage)
	{
		this.cb = cb;
		direction = (Vector3)dir.normalized;
		this.damage = damage;
	}

	public void DestroyBullet ()
	{
		Destroy (gameObject);

	}

	public void OnHit (RaycastHit2D hit, CharacterBase damagable)
	{
		damagable.healthSystem.Damage (damage);
		Debug.Log ("Hitted " + hit.collider.name + " damage =" + damage);
	}

}