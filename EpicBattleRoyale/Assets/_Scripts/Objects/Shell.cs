using UnityEngine;
using UnityEngine.UI;

public class Shell : MonoBehaviour
{
    public Sound[] shellSounds;
    static Vector2 force = new Vector2(15, 40);
    static Shell pf;

    public static void SpawnShell(Vector3 position, Vector3 rotation, Weapon.WeaponType type)
    {
        if (pf == null)
            pf = GameAssets.Get.pfShell;

        GameObject newShell = Instantiate<GameObject>(pf.gameObject);
        newShell.transform.position = position;
        newShell.transform.localEulerAngles = rotation;
        newShell.transform.localScale = GetShellSize(type);

        Rigidbody2D rb = newShell.GetComponent<Rigidbody2D>();
        float curForce = Random.Range(force.x, force.y);
        int rotForce = ((Random.Range(0, 2) == 0) ? 1 : -1);
        rb.AddForce(Vector2.up * curForce + Vector2.right * rotForce * Random.Range(0f, 5));
        rb.AddTorque(rotForce * curForce / 25);

        Shell shell = newShell.GetComponent<Shell>();

        DG.Tweening.DOVirtual.DelayedCall(.6f, delegate
        {
            AudioManager.PlaySoundAtObject(shell.shellSounds[Random.Range(0, shell.shellSounds.Length)], shell.gameObject);

            //AudioManager.PlaySound(shell.shellSounds[Random.Range(0, shell.shellSounds.Length)]); 
        });
        Destroy(newShell, 1);

    }

    static Vector3 GetShellSize(Weapon.WeaponType type)
    {
        Vector3 size = Vector3.one;
        switch (type)
        {
            case Weapon.WeaponType.Pistol:
                size = Vector3.one * .7f;
                break;
            case Weapon.WeaponType.Sniper:
                size = Vector3.one * 1.2f;
                break;
            default:
                break;
        }
        return size;
    }
}
