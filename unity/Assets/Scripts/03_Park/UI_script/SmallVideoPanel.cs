using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SmallVideoPanel : MonoBehaviourPun
{

    // mode = 0 ��û�� mode = 1 ����Ŀ
    public int mode;

    private void OnEnable()
    {
        GameManager.instance.myPlayer.GetComponent<PlayerControl>().isMoveAble = true;
        GameManager.instance.myPlayer.GetComponent<PlayerControl>().isUIActable = true;

        if (mode == 1)
        {
            this.gameObject.AddComponent<PhotonView>();
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
