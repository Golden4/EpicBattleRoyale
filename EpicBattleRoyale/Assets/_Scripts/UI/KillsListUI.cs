﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class KillsListUI : MonoBehaviour
{
    const int MAX_KILLS = 4;
    public Image pfKills;
    public List<Image> killsList = new List<Image>();

    public void AddKillToList(CharacterBase player, CharacterBase characterKillName, CharacterBase characterKilledName, GameAssets.WeaponsList weaponName)
    {
        Image goKill = SpawnKillInfo(player, characterKillName, characterKilledName, weaponName);

        killsList.Add(goKill);

        goKill.transform.DOScale(Vector3.one, .3f).ChangeStartValue(Vector3.zero);

        DOVirtual.DelayedCall(5f, delegate
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
            killsList.Remove(image);
        }

        imageGO.transform.DOScale(Vector3.zero, .3f).OnComplete(delegate
        {
            Destroy(imageGO.gameObject);
        });
    }

    Image SpawnKillInfo(CharacterBase player, CharacterBase characterKillName, CharacterBase characterKilledName, GameAssets.WeaponsList weaponName)
    {
        GameObject goKill = Instantiate(pfKills.gameObject);
        goKill.gameObject.SetActive(true);
        goKill.transform.SetParent(transform, false);
        Text name1 = goKill.transform.GetChild(0).GetComponent<Text>();
        Text name2 = goKill.transform.GetChild(2).GetComponent<Text>();

        name1.text = "  " + characterKillName.name + "  ";
        name2.text = "  " + characterKilledName.name + "  ";

        if (player == characterKillName)
        {
            name1.color = Color.yellow;
        }

        if (player == characterKilledName)
        {
            name2.color = Color.yellow;
        }

        goKill.transform.GetChild(1).GetComponent<Image>().sprite = GameAssets.Get.GetWeapon(weaponName).sprite;
        goKill.transform.SetAsFirstSibling();
        return goKill.GetComponent<Image>();
    }
}
