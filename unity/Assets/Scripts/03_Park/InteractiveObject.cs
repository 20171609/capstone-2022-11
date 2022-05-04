using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class InteractiveObject : MonoBehaviour
{
    /**
     * ��ȣ�ۿ� ����
     * 0) ����ŷ
     * 1) ������ ����Ʈ ��
     * 2) ������ ����Ʈ ��Ƽ��Ʈ
     * 3) �����̵���
     * */
    [SerializeField] protected int InteractiveType;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject player = GameManager.instance.myPlayer;
        if (player.GetComponent<PhotonView>().IsMine && collision.gameObject == player)
        {
            switch (InteractiveType)
            {
                case 0:
                    if (!GetComponentInParent<BuskingSpot>().isUsed)
                    {
                        player.GetComponent<PlayerControl>().OnInteractiveButton(InteractiveType);
                        // �ϴ� �ε�ġ�� roomNum, nowBuskingSpot ����
                        /**
                        webRTCOperate.Instance.roomNum = GetComponentInParent<BuskingSpot>().roomNum;
                        webRTCOperate.Instance.nowBuskingSpot = GetComponentInParent<BuskingSpot>();
                        **/
                        AgoraManager.Instance.nowBuskingSpot = GetComponentInParent<BuskingSpot>();

                        player.GetComponent<PlayerControl>().InteractiveButton.GetComponent<Button>().onClick.AddListener(
                            delegate { player.GetComponent<PlayerControl>().OnVideoPanel(1);});
                    }
                    break;

                default:
                    if (collision.tag == "Character")
                    {
                        collision.transform.GetComponent<PlayerControl>().OnInteractiveButton(InteractiveType);
                    }
                    break;
            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject player = GameManager.instance.myPlayer;
        if (collision.gameObject == player && player.GetComponent<PhotonView>().IsMine)
        {
            player.GetComponent<PlayerControl>().OffInteractiveButton();
        }
    }

}
