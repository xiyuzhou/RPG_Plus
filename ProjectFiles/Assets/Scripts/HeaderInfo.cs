using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class HeaderInfo : MonoBehaviourPun
{
    public TextMeshProUGUI playerNameText;
    public Image bar;
    private float maxValue;

    public void Initialize(string text, int maxVal)
    {
        playerNameText.text = text;
        maxValue = maxVal;
        bar.fillAmount = 1.0f;
    }
    [PunRPC]
    void UpdateHealthBar(int value)
    {
        bar.fillAmount = (float)value / maxValue;
    }
        // Update is called once per frame
}
