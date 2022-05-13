using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviourPunCallbacks
{

    // �÷��̾� ����
    float moveSpeed = 10f;
    public bool isMoveAble = true;
    public bool isUIActable = true;

    // ��ȣ�ۿ� ��ư
    [SerializeField] bool isInteractiveAble = false;
    public GameObject InteractiveButton;
    [SerializeField] Sprite[] buttonImages;

    // ����
    public bool isVideoPanelShown = false;
    public GameObject videoPanel;

    // �̸�Ƽ�� �ڷ�ƾ ���࿩��
    private bool isEmojiRunning = false;
    private Coroutine runningEmojiCorutine;

    // ä��
    [SerializeField] GameObject ChatPanel;

    // BusketPanel
    [SerializeField] private GameObject buskerPanel;

    // �� ��/�� �ƿ�
    private float wheelSpeed = 10;
    [SerializeField] private float cameraDistance = 10;

    // Start is called before the first frame update
    void Start()
    {
        if (this.photonView.IsMine)
        {
            InteractiveButton = FindObjectOfType<Canvas>().transform.Find("InteractiveButton").gameObject;
            videoPanel = FindObjectOfType<Canvas>().transform.Find("smallVideoPanel").gameObject;
            ChatPanel = FindObjectOfType<Canvas>().transform.Find("bigVideoPanel").gameObject.transform.Find("ChatView").gameObject;
            buskerPanel = FindObjectOfType<Canvas>().transform.Find("BuskerVideoPanel").gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoveAble)
        {
            PlyerMove();
        }

    }

    private void PlyerMove()
    {
        if (this.photonView.IsMine)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                gameObject.transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                gameObject.transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                gameObject.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                gameObject.transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
            }

            cameraDistance = Input.GetAxis("Mouse ScrollWheel") * wheelSpeed * Time.deltaTime;
            Camera.main.orthographicSize = cameraDistance;

        }
    }

    public void OnInteractiveButton(int type)
    {
        /**
         * 0) ����ŷ
         * 1) ����ŷ �׸��α�
         * 2) ����Ŀ �ȷο�
         * 3) �����̵���
         * **/

        if (!isInteractiveAble)
        {
            InteractiveButton.GetComponent<Image>().sprite = buttonImages[type];
            InteractiveButton.SetActive(true);
            isInteractiveAble = true;
        }
    }

    public void OffInteractiveButton()
    {
        if (isInteractiveAble)
        {
            InteractiveButton.GetComponent<Button>().onClick.RemoveAllListeners();
            InteractiveButton.SetActive(false);
            isInteractiveAble = false;
        }

    }

    public void changeInteractiveButton(int type)
    {
        if (isInteractiveAble)
        {
            InteractiveButton.GetComponent<Button>().onClick.RemoveAllListeners();
            InteractiveButton.GetComponent<Image>().sprite = buttonImages[type];
        }
    }

    public void OnVideoPanel(int mode)
    {
        if (!isVideoPanelShown)
        {
            switch (mode)
            {
                case 0:
                    videoPanel.GetComponent<SmallVideoPanel>();
                    videoPanel.SetActive(true);
                    isVideoPanelShown = true;
                    break;

                case 1:
                    buskerPanel.SetActive(true);
                    GameManager.instance.myPlayer.GetComponent<PlayerControl>().isMoveAble = false;
                    GameManager.instance.myPlayer.GetComponent<PlayerControl>().isUIActable = false;
                    buskerPanel.GetComponent<BuskerVideoPanel>().setDevice();
                    isVideoPanelShown = true;
                    break;

                default:
                    break;
            }
        }
    }

    public void OffVideoPanel()
    {
        if (isVideoPanelShown)
        {
            ChatPanel.GetComponent<Chat>().msgList.text = "";
            ChatPanel.GetComponent<Chat>().emojimsg = "";
            videoPanel.SetActive(false);
            isVideoPanelShown = false;
        }
    }

    // ��� �ʱ⼳��
    public void initBusking()
    {
        // ĳ���� ��ġ ��
    }

    // -------------- �̸��� ����ȭ ���� �Լ��� -------------
    public IEnumerator sendEmoji(int emojiNum)
    {
        GameObject bubble = transform.GetChild(1).gameObject;
        bubble.SetActive(true);
        bubble.GetComponent<TextMeshPro>().text = "<sprite=" + emojiNum + ">";
        isEmojiRunning = true;

        yield return new WaitForSeconds(1f);

        bubble.SetActive(false);
        isEmojiRunning = false;

    }

    [PunRPC]
    public void callEmoji(int emojiNum)
    {
        if (isEmojiRunning)
        {
            StopCoroutine(runningEmojiCorutine);
            isEmojiRunning = false;
        }

        runningEmojiCorutine = StartCoroutine(sendEmoji(emojiNum));
    }

    public void rpcEmoji(int emojiNum)
    {
        photonView.RPC("callEmoji", RpcTarget.AllBuffered, emojiNum);
    }
    //----------------------------------------------------------------

}
