using System;
using UnityEngine;

public class Camera2DFollow : MonoBehaviour
{
    public CharacterBase target;
    public float damping = 1;
    public float cameraOffset;

    private float m_OffsetZ;
    private Vector3 m_CurrentVelocity;

    // Use this for initialization
    private void Awake()
    {
        World.OnPlayerSpawn += World_OnPlayerSpawn;
    }

    void World_OnPlayerSpawn(Player player)
    {
        target = player.characterBase;
        m_OffsetZ = (transform.position - target.transform.position).z;
        transform.parent = null;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        /*		// only update lookahead pos if accelerating or changed direction
                float xMoveDelta = (target.transform.position - m_LastTargetPosition).x;

                bool updateLookAheadTarget = Mathf.Abs (xMoveDelta) > lookAheadMoveThreshold;

                if (updateLookAheadTarget) {
                    m_LookAheadPos = lookAheadFactor * Vector3.right * Mathf.Sign (xMoveDelta);
                } else {
                    m_LookAheadPos = Vector3.MoveTowards (m_LookAheadPos, Vector3.zero, Time.deltaTime * lookAheadReturnSpeed);
                }*/
        if (target == null)
            return;

        Vector3 aheadTargetPos = (Vector3)((Vector2)target.worldPosition + Vector2.right * cameraOffset * ((target.isFacingRight) ? 1 : -1) - Vector2.up * target.pivotOffset) + Vector3.forward * m_OffsetZ;
        Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref m_CurrentVelocity, damping);
        // newPos.y = 0;

        float screenSizeX = Vector2.Distance(Camera.main.ScreenToWorldPoint(new Vector2(0, 0)), Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0))) * 0.5f;
        float screenSizeY = Vector2.Distance(Camera.main.ScreenToWorldPoint(new Vector2(0, 0)), Camera.main.ScreenToWorldPoint(new Vector2(Screen.height, 0))) * 0.5f;

        newPos.x = Mathf.Clamp(newPos.x, MapsController.Ins.GetCurrentWorldEndPoints().x + screenSizeX, MapsController.Ins.GetCurrentWorldEndPoints().y - screenSizeX);

        newPos.y = Mathf.Clamp(newPos.y, MapsController.Ins.GetCurrentScreenUpDownEndPoints().x + screenSizeY, MapsController.Ins.GetCurrentScreenUpDownEndPoints().y - screenSizeY);

        transform.position = newPos;
    }

    void OnDestroy()
    {
        World.OnPlayerSpawn -= World_OnPlayerSpawn;
    }
}

