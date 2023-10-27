using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TextCore.Text;
using TMPro;

public class PlayerImageManager : MonoBehaviour
{
    public Image[] images;
    public TextMeshProUGUI playerName;
    public string name = "";
    public int[] imageIndex = new int[] { 21, 26, 41, 165, 295, 350 };
    void Start()
    {
        UpdateImge();
        playerName.text = name;
    }

    public void UpdateImge()
    {
        for (int i = 0; i < 6; i++)
        {
            images[i].sprite = SpriteManager.instance.sprites[SpriteManager.instance.playerImageInfo[i]];
        }
        playerName.text = name;
    }
    public void UpdateImageObject()
    {
        for (int i = 0; i < 6; i++)
        {
            images[i].sprite = SpriteManager.instance.sprites[imageIndex[i]];
        }
        playerName.text = name;
    }

}
