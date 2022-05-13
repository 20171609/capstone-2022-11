using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class BuskingSpot : MonoBehaviourPun
{
    // Room ����
    public int roomNum;
    public bool isUsed = false;

    // Title ����
    [SerializeField] private TextMeshProUGUI titleBar;
    public string titleText;
    public string buskerNickname;

    [SerializeField] private GameObject localuser;

    private void Start()
    {
        titleBar = FindObjectOfType<Canvas>().transform.Find("TitleBar").GetComponent<TextMeshProUGUI>();
    }

    public void callChangeUsed(string name = null, string t = null)
    {
        photonView.RPC("changeUsed", RpcTarget.AllBuffered, name, t);
    }

    [PunRPC]
    void changeUsed(string name = null, string t = null)
    {
        if (!isUsed)
        {
            isUsed = true;
            buskerNickname = name;
            titleText = t;
        }
        else
        {
            isUsed = false;
            buskerNickname = null;
            titleText = null;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject player = GameManager.instance.myPlayer;

        if (collision.gameObject == player && player.GetComponent<PhotonView>().IsMine)
        {
            localuser = player;

            // Agora�� ����ŷ�� ���� �ֱ�
            AgoraChannelPlayer.Instance.nowBuskingSpot = this;
            AgoraChannelPlayer.Instance.channelName = roomNum.ToString();

            if (isUsed && !player.GetComponent<PlayerControl>().isVideoPanelShown)
            {
                collision.transform.GetComponent<PlayerControl>().OnVideoPanel(0);

                // Agora����
                AgoraChannelPlayer.Instance.callJoin(1);

                player.GetComponent<PlayerControl>().OnInteractiveButton(2);
                //player.GetComponent<PlayerControl>().InteractiveButton.GetComponent<Button>().onClick.AddListener(); // �ȷο�

            }
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject player = GameManager.instance.myPlayer;
        if (collision.gameObject == player && player.GetComponent<PhotonView>().IsMine)
        {
            localuser = null;

            AgoraChannelPlayer.Instance.leaveChannel();

            // AgoraManager�� ����ŷ �� ���� ���� �����
            AgoraChannelPlayer.Instance.nowBuskingSpot = null;
            AgoraChannelPlayer.Instance.channelName = null;

            if (player.GetComponent<PlayerControl>().isVideoPanelShown)
                collision.transform.GetComponent<PlayerControl>().OffVideoPanel();

        }
    }

    public void callInsideUserJoin(string channelName) // localuser�� ����ȭX / Join�� ����ȭO > local�� ã�Ƽ� ����
    {
        if (channelName == AgoraChannelPlayer.Instance.channelName)
            photonView.RPC("insideUserJoin", RpcTarget.OthersBuffered, null);
    }

    [PunRPC]
    public void insideUserJoin()
    {
        if (AgoraChannelPlayer.Instance.role != "publisher" && localuser != null)
        {
            localuser.GetComponent<PlayerControl>().OnVideoPanel(0);
            AgoraChannelPlayer.Instance.callJoin(1);
            localuser.GetComponent<PlayerControl>().OnInteractiveButton(2);
        }
    }

    public void onTitleBar()
    {
        titleBar.text = buskerNickname + ": " + titleText;
        titleBar.gameObject.SetActive(true);
    }

    public void offTitleBar()
    {
        titleBar.text = null;
        titleBar.gameObject.SetActive(false);
    }



}
