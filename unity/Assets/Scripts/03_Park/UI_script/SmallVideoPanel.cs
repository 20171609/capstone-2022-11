using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class SmallVideoPanel : MonoBehaviour
{
    // ���� ����ȭ �õ��غ���

    // mode = 0 ��û�� mode = 1 ����Ŀ
    public int mode;

    private void OnEnable()
    {
        GameManager.instance.myPlayer.GetComponent<PlayerControl>().isMoveAble = true;
        GameManager.instance.myPlayer.GetComponent<PlayerControl>().isUIActable = true;

        if (mode == 0)
        {
        }
    }


    public void ScaleUpPanel()
    {
        if (mode == 0)
        {
            this.gameObject.SetActive(false);
            FindObjectOfType<Canvas>().transform.Find("bigVideoPanel").gameObject.SetActive(true);
        }
    }
    
}
