using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class SpriteManager : MonoBehaviour
{
    public Sprite[] sprites;
    [HideInInspector]
    public int[,] _index;

    public Image[] images;
    public int[] playerImageInfo;
    public static SpriteManager instance;
    public Menu menu;
    private ExitGames.Client.Photon.Hashtable _CustomProperties = new ExitGames.Client.Photon.Hashtable();
    void Awake()
    {
        instance = this;

    }
    void Start()
    {
        _index = new int[6, 2] { { 1, 21 }, { 22, 41 }, { 42, 161 }, { 162, 277 }, { 278, 337 } ,{ 338,447 } };

        playerImageInfo = new int[] { 21, 26, 41, 165, 295, 350 };
        _CustomProperties.Add("plyerImageIndex", playerImageInfo);
        PhotonNetwork.LocalPlayer.SetCustomProperties(_CustomProperties);
    }
    public void OnReset()
    {
        playerImageInfo = new int[] { 21, 26, 41, 165, 295, 350 };
        for (int i = 0; i < 6; i++)
        {
            images[i].sprite = sprites[playerImageInfo[i]];
        }
        UpdateCustomProperties();
    }
    public void OnRandom(int i)
    {
        int result = Random.Range(_index[i,0], _index[i, 1]);
        //Debug.Log(result);
        images[i].sprite = sprites[result];
        playerImageInfo[i] = result;
        UpdateCustomProperties();
    }

    void UpdateCustomProperties()
    {
        Debug.Log(playerImageInfo[0]);
        _CustomProperties["plyerImageIndex"] = playerImageInfo;
        PhotonNetwork.LocalPlayer.SetCustomProperties(_CustomProperties);
        Invoke("UpdateMenuImage", 0.5f);
    }
    void UpdateMenuImage()
    {
        menu.onPlayerChangeImage();
    }

}
