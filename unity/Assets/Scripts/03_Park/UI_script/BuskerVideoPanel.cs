using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using agora_gaming_rtc;
using Photon.Pun;

public class BuskerVideoPanel : MonoBehaviour
{
    // 카메라 관련
    protected WebCamTexture textureWebCam = null;
    public GameObject objectTarget;
    private bool isCameraOn = false;

    // 마이크 관련
    public AudioSource micAudioSource;
    private bool isMicOn = false;

    // 카메라 마이크 체크할 이미지
    [SerializeField] private Image CameraCheck;
    [SerializeField] private Image MicCheck;

    // 버스킹 시작 버튼
    [SerializeField] private Button StartButton;

    // 버스킹 나가기 버튼
    [SerializeField] private Button ExitButton;

    // Input Field
    [SerializeField] private TMP_InputField titleInput;

    // small Video Panel
    [SerializeField] private GameObject smallVideo;

    // Start is called before the first frame update
    void Start()
    {
        ExitButton.onClick.AddListener(exitPanel);
    }

    // Update is called once per frame
    void Update()
    {
        if (isCameraOn)
            CameraCheck.color = Color.green;
        else
            CameraCheck.color = Color.red;

        if (isMicOn)
            MicCheck.color = Color.green;
        else
            MicCheck.color = Color.red;
    }

    private void cameraConnect()
    {

        // 카메라
        WebCamDevice[] devices = WebCamTexture.devices;

        int selectedCameraIndex = -1;
        for (int i = 0; i < devices.Length; i++)
        {
            // 사용 가능한 카메라 로그
            Debug.Log("Available Webcam: " + devices[i].name + ((devices[i].isFrontFacing) ? "(Front)" : "(Back)"));

            // 후면 카메라인지 체크
            if (devices[i].isFrontFacing)
            {
                // 해당 카메라 선택
                selectedCameraIndex = i;
                break;
            }
        }

        // WebCamTexture 생성
        if (selectedCameraIndex >= 0)
        {
            // 선택된 카메라에 대한 새로운 WebCamTexture를 생성
            textureWebCam = new WebCamTexture(devices[selectedCameraIndex].name);

            // 원하는 FPS를 설정
            if (textureWebCam != null)
            {
                textureWebCam.requestedFPS = 60;
            }
        }

        // objectTarget으로 카메라가 표시되도록 설정
        if (textureWebCam != null)
        {
            // 카메라
            objectTarget.GetComponent<RawImage>().texture = textureWebCam;
            textureWebCam.Play();

            // 카메라 켜졌다고 표시
            isCameraOn = true;

        }
        else // 카메라 텍스쳐 없음
        {
            Debug.Log("No Camera Texture");
        }
    }

    private void micConnect()
    {
        try
        {
            string mic = Microphone.devices[0];
            micAudioSource.clip = Microphone.Start(mic, true, 10, 44100);
            while (!(Microphone.GetPosition(mic) > 0)) { } // Wait until the recording has started
            micAudioSource.Play(); // Play the audio source!

            // 마이크 켜졌다고 표시
            isMicOn = true;

        }
        catch
        {
            Debug.Log("No mic");
        }
    }


    private void exitPanel()
    {
        GameManager.instance.myPlayer.GetComponent<PlayerControl>().OffVideoPanel();
        GameManager.instance.myPlayer.GetComponent<PlayerControl>().isMoveAble = true;
        GameManager.instance.myPlayer.GetComponent<PlayerControl>().isUIActable = true;
    }

    public void setDevice()
    {
        cameraConnect();
        micConnect();

        if (isMicOn && isCameraOn)
        {
            StartButton.onClick.RemoveAllListeners(); // 지워주고 해야함
            StartButton.onClick.AddListener(StartBusking);
        }

    }

    // 버스킹 인터렉티브
    public void StartBusking()
    {
        if (titleInput.text != "" && titleInput.text != null)
        {
            AgoraChannelPlayer.Instance.callJoin(0, PhotonNetwork.LocalPlayer.NickName, titleInput.text);

            gameObject.SetActive(false);

            /**
            // Busker 화면 없애기
            gameObject.SetActive(false);
            smallVideo.SetActive(true);
            AgoraChannelPlayer.Instance.setBuskerVideoSurface(smallVideo.GetComponent<RawImage>());
            GameManager.instance.myPlayer.GetComponent<PlayerControl>().isUIActable = true;
            **/

            // 그만두기 버튼 설정
            PlayerControl player = GameManager.instance.myPlayer.GetComponent<PlayerControl>();
            player.changeInteractiveButton(1);
            player.InteractiveButton.GetComponent<Button>().onClick.AddListener(delegate { player.changeInteractiveButton(0); }); // 버스킹 그만두기 버튼 삭제 or 그대로 트리거 안에 있으니까 시작하기로 다시 바꾸기

            AgoraChannelPlayer.Instance.nowBuskingSpot.callInsideUserJoin(AgoraChannelPlayer.Instance.channelName);

        }
    }

}
