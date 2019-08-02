using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(CharacterBase))]
public class Platformer2DUserControl : MonoBehaviour
{

    private CharacterBase m_Character;
    private bool m_Jump;

    private void Awake()
    {
        m_Character = GetComponent<CharacterBase>();
    }


    private void Update()
    {
        if (!m_Jump)
        {
            m_Jump = Input.GetButtonDown("Jump");
        }
    }

    private void FixedUpdate()
    {
        // bool shooting = Input.GetButton ("Fire1");
        // float h = Input.GetAxisRaw ("Horizontal");
        // bool shootingSide = Input.mousePosition.x > Screen.width / 2; 
        // m_Character.Move (h, shooting, m_Jump, shootingSide);
        // m_Jump = false;
    }
}

