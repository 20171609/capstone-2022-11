using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Threading;


public class BuskerVideoPanel : MonoBehaviour
{
    // ī�޶� ����
    protected WebCamTexture textureWebCam = null;
    public GameObject objectTarget;
    private bool isCameraOn = false;

    // ����ũ ����
    public AudioSource micAudioSource;
    private bool isMicOn = false;

    // ī�޶� ����ũ üũ�� �̹���
    [SerializeField] private Image CameraCheck;
    [SerializeField] private Image MicCheck;

    // ����ŷ ���� ��ư
    [SerializeField] private Button StartButton;

    // ����ŷ ������ ��ư
    [SerializeField] private Button ExitButton;

    // Input Field
    [SerializeField] private TMP_InputField titleInput;

    // ī�޶�, ����ũ
    [SerializeField] private RawImage cameraImage;
    [SerializeField] private AudioSource MicSource;

    // small Video Panel
    [SerializeField] private GameObject smallVideo;

    private void OnEnable()
    {
        GameManager.instance.myPlayer.GetComponent<PlayerControl>().isMoveAble = false;
        //GameManager.instance.myPlayer.GetComponent<PlayerControl>().isUIActable = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        ExitButton.onClick.AddListener(() => { gameObject.SetActive(false); });
    }

    // Update is called once per frame
    void Update()
    {

        if (isCameraOn)
            CameraCheck.color = Color.green;
        else
            CameraCheck.color = Color.white;

        if (isMicOn)
            MicCheck.color = Color.green;
        else
            MicCheck.color = Color.white;

    }


    public void setDevice()
    {
        AgoraManager.Instance.loadEngine();
        StartButton.onClick.AddListener(StartBusking);
    }

    // ����ŷ ���ͷ�Ƽ��
    public void StartBusking()
    {
        if (titleInput.text != "")
        {
            if (AgoraManager.Instance.nowBuskingSpot != null)
            {
                AgoraManager.Instance.nowBuskingSpot.callsetTitle(titleInput.text);
            }
            AgoraManager.Instance.callJoin(0);

            // Busker ȭ�� ���ֱ�
            gameObject.SetActive(false);
            smallVideo.transform.localPosition = new Vector3(-700, 490, 0);
            smallVideo.GetComponent<Button>().enabled = false;
            smallVideo.SetActive(true);
            AgoraManager.Instance.setBuskerVideoSurface(smallVideo.GetComponent<RawImage>());
        }
    }



}
