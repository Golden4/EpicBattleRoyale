using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class KillsListUI : MonoBehaviour
{
    const int MAX_KILLS = 4;
    public Image pfKills;
    public List<Image> killsList = new List<Image>();

    public void AddKillToList(CharacterBase player, CharacterBase characterKill, CharacterBase characterKilled, Weapon weapon, HitBox.HitBoxType hitBoxType)
    {
        Image goKill = SpawnKillInfo(player, characterKill, characterKilled, weapon, hitBoxType);

        killsList.Add(goKill);

        goKill.transform.DOScale(Vector3.one, .3f).ChangeStartValue(Vector3.zero);

        if (goKill.isActiveAndEnabled)
            Utility.Invoke(goKill, 10f, delegate
            {
                if (goKill != null)
                    RemoveKillFromList(goKill);
            });

        if (killsList.Count > MAX_KILLS)
        {
            RemoveKillFromList();
        }
    }

    void RemoveKillFromList(Image image = null)
    {
        Image imageGO;
        if (image == null)
        {
            imageGO = killsList[0];
            killsList.RemoveAt(0);
        }
        else
        {
            imageGO = image;
            killsList.Remove(imageGO);
        }

        imageGO.transform.GetChild(1).GetComponent<Image>().DOFade(0, .3f);
        imageGO.transform.GetChild(0).GetComponent<Text>().DOFade(0, .3f);
        imageGO.transform.GetChild(2).GetComponent<Text>().DOFade(0, .3f);

        imageGO.DOFade(0, .3f).OnComplete(delegate
        {
            Destroy(imageGO.gameObject);
        });


        // imageGO.transform.DOScale(Vector3.zero, .3f).OnComplete(delegate
        // {
        //     Destroy(imageGO.gameObject);
        // });
    }

    Image SpawnKillInfo(CharacterBase player, CharacterBase characterKill, CharacterBase characterKilled, Weapon weaponName, HitBox.HitBoxType hitBoxType)
    {
        GameObject goKill = Instantiate(pfKills.gameObject);
        goKill.gameObject.SetActive(true);
        goKill.transform.SetParent(transform, false);
        Text name1 = goKill.transform.GetChild(0).GetComponent<Text>();
        Text name2 = goKill.transform.GetChild(4).GetComponent<Text>();

        if (characterKill == null)
        {

            goKill.transform.GetChild(0).gameObject.SetActive(false);
            goKill.transform.GetChild(1).gameObject.SetActive(false);
            goKill.transform.GetChild(3).gameObject.SetActive(true);
        }
        else
        {

            goKill.transform.GetChild(0).gameObject.SetActive(true);
            goKill.transform.GetChild(1).gameObject.SetActive(true);
            goKill.transform.GetChild(3).gameObject.SetActive(false);

            Image weaponImage = goKill.transform.GetChild(1).GetComponent<Image>();
            weaponImage.material.SetColor("_FillColor", Color.white);
            weaponImage.sprite = GameAssets.Get.GetWeapon(weaponName.weaponName).sprite;

            if (player == characterKill)
            {
                name1.color = Color.yellow;
            }

            name1.text = "  " + characterKill.name + "  ";
        }

        name2.text = "  " + characterKilled.name + "  ";

        if (player == characterKilled)
        {
            name2.color = Color.yellow;
        }

        if (hitBoxType == HitBox.HitBoxType.Head)
            goKill.transform.GetChild(2).gameObject.SetActive(true);
        else
            goKill.transform.GetChild(2).gameObject.SetActive(false);

        //goKill.transform.SetAsFirstSibling();
        return goKill.GetComponent<Image>();
    }
}
