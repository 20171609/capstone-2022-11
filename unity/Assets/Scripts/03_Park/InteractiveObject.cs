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

    // ���ͷ�Ƽ�� ��ư
    [SerializeField] GameObject interactiveButton;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        PhotonView player = collision.transform.gameObject.GetPhotonView();
        if (player.IsMine)
        {
            switch (InteractiveType)
            {
                case 0:
                    if (!this.GetComponent<BuskingSpot>().isUsed && collision.tag == "Character")
                    {
                        collision.transform.GetComponent<PlayerControl>().OnInteractiveButton(InteractiveType);
                        interactiveButton.GetComponent<Button>().onClick.AddListener(StartBusking);
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
        if (collision.tag == "Character")
        {
            collision.transform.GetComponent<PlayerControl>().OffInteractiveButton();
        }
    }

    // ����ŷ ���ͷ�Ƽ��
    void StartBusking()
    {

    }

}
