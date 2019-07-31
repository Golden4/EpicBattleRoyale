using System;
using UnityEngine;

public class CharacterBase : MonoBehaviour {
	public float maxSpeed = 4;
	public float jumpForce = 500;

	public bool airControl = true;
	public LayerMask whatIsGround;
	public bool isFacingRight = true;

	Transform groundCheck;
	const float k_GroundedRadius = .2f;
	bool isGrounded;
	bool isJumping;
	Animator anim;
	Rigidbody2D rb;

	public Weapon.WeaponType weaponType;
	public bool shootingSideRight;
	public bool isShooting;
	public float move;
	bool isDead;

	public HealthSystem healthSystem;

	private void Awake ()
	{
		healthSystem = new HealthSystem (50, 0);
		healthSystem.OnHealthZero += HealthSystem_OnHealthZero;
		groundCheck = transform.Find ("GroundCheck");
		// m_CeilingCheck = transform.Find("CeilingCheck");
		anim = GetComponentInChildren<Animator> ();
		rb = GetComponent<Rigidbody2D> ();

	}

	void Update ()
	{
		if (!isJumping) {
			isJumping = Input.GetButtonDown ("Jump");
		}
	}

	private void FixedUpdate ()
	{
		isGrounded = false;
		Collider2D[] colliders = Physics2D.OverlapCircleAll (groundCheck.position, k_GroundedRadius, whatIsGround);
		for (int i = 0; i < colliders.Length; i++) {
			if (colliders [i].gameObject != gameObject)
				isGrounded = true;
		}

		anim.SetBool ("Jump", !isGrounded);

		move = Input.GetAxisRaw ("Horizontal");
		bool shootingSide = Input.mousePosition.x > Screen.width / 2; 
		Move (move, isShooting, isJumping, shootingSide);
		isJumping = false;

	}

	void HealthSystem_OnHealthZero (object sender, EventArgs e)
	{
		Die ();
	}

	public bool IsDead ()
	{
		return isDead;
	}

	public void Die ()
	{
		Debug.Log (transform.name + " is Dead!");
	}

	public void SetWeaponAnimationType (Weapon.WeaponType type)
	{
		weaponType = type;
		anim.SetInteger ("WeaponType", (int)type);
		//anim.Play ("Hold" + weaponType.ToString (), 1, );
	}

	public void PlayShootAnimation (float fireRate, bool shootingSide)
	{
		Flip (shootingSide);
		anim.SetFloat ("ShootingTime", 1f / fireRate);
		anim.Play ("Shoot" + weaponType.ToString (), 1);
		anim.SetBool ("Fire", true);
	}

	public void StopShootAnimation ()
	{
		anim.SetBool ("Fire", false);
	}

	public void PlayReloadAnimation (float reloadTime)
	{
		anim.SetFloat ("ReloadTime", 1 / reloadTime);
		anim.SetBool ("isReloading", true);
		anim.Play ("Reload" + weaponType.ToString ());
	}

	public void StopReloadAnimation ()
	{
		anim.SetBool ("isReloading", false);
		//anim.Play ("Reload" + weaponType.ToString ());
	}

	public void Move (float move, bool shooting, bool jump, bool shootingSideRight)
	{
		if (isGrounded || airControl) {
			anim.SetFloat ("Speed", Mathf.Abs (move));
			
			if (shooting) {
				if (move < 0 && shootingSideRight || move > 0 && !shootingSideRight) {
					anim.SetFloat ("LookingSide", -.7f);
					maxSpeed = 2;
				} else {
					anim.SetFloat ("LookingSide", .7f);
					maxSpeed = 2;
				}
				Flip (shootingSideRight);
			} else {
				anim.SetFloat ("LookingSide", 1);
				if (move != 0)
					Flip (move > 0);
				maxSpeed = 3.5f;
			}

			//anim.SetBool ("isWeapon", isWeapon);
				
			rb.velocity = new Vector2 (move * maxSpeed, rb.velocity.y);
				
		}
		if (isGrounded && jump && !anim.GetBool ("Jump")) {
			isGrounded = false;
			rb.AddForce (new Vector2 (0f, jumpForce));
		}
	}

	private void Flip (bool right)
	{
		isFacingRight = right;
		Vector3 theScale = transform.localScale;
		theScale.x = (right) ? 1 : -1;
		transform.localScale = theScale;
	}
}

