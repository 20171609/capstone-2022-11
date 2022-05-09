using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class BuskingSpot : MonoBehaviourPun
{
    public int roomNum;
    public bool isUsed = false;

    // Title ����
    [SerializeField] private TextMeshProUGUI titleBar;
    public string title;

    private void Start()
    {
        titleBar = FindObjectOfType<Canvas>().transform.Find("TitleBar").GetComponent<TextMeshProUGUI>();
    }

    public void callChangeUsed()
    {
        photonView.RPC("changeUsed", RpcTarget.AllBuffered, null);
    }
    public void callsetTitle(string _title)
    {
        photonView.RPC("setTitle", RpcTarget.AllBuffered, _title);
    }

    [PunRPC]
    void changeUsed()
    {
        if (!isUsed)
            isUsed = true;
        else
            isUsed = false;
    }

    [PunRPC]
    void setTitle(string _title)
    {
        if (isUsed)
        {
            title = _title;
        }
        else
        {
            title = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject player = GameManager.instance.myPlayer;
        if (collision.gameObject == player && player.GetComponent<PhotonView>().IsMine)
        {
            // AgoraManager�� ����ŷ�� ���� �ֱ�
            AgoraManager.Instance.nowBuskingSpot = this;
            AgoraManager.Instance.channelName = roomNum.ToString();

            if (isUsed && !player.GetComponent<PlayerControl>().isVideoPanelShown)
            {
                titleBar.text = title;
                titleBar.gameObject.SetActive(true);

                collision.transform.GetComponent<PlayerControl>().OnVideoPanel(0);

                // Agora����
                AgoraManager.Instance.loadEngine();
                AgoraManager.Instance.callJoin(1);

            }
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject player = GameManager.instance.myPlayer;
        if (collision.gameObject == player && player.GetComponent<PhotonView>().IsMine)
        {
            titleBar.text = null;
            titleBar.gameObject.SetActive(false);

            // AgoraManager�� ����ŷ �� ���� ���� �����
            AgoraManager.Instance.nowBuskingSpot = null;
            AgoraManager.Instance.channelName = null;

            // AgoraEngine unloaded
            AgoraManager.Instance.unloadEngine();

            if (player.GetComponent<PlayerControl>().isVideoPanelShown)
                collision.transform.GetComponent<PlayerControl>().OffVideoPanel();
        }
    }


}
