using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.WebRTC;
using KyleDulce.SocketIo;
using System.IO;
using System;

public class BuskerVideoPanel : MonoBehaviour
{
    // Stream
    Socket socket;
    string mainServer = "http://localhost:8080";
    public MediaStream sourceStream;

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

    // webRTC script
    [SerializeField] public WebRTC webrtc;

    // ����ŷ ���� ��ư
    [SerializeField] private Button StartButton;

    // player
    GameObject player;

    //TCP
    /**
    TcpClient tcpsocket;
    NetworkStream stream;
    StreamWriter writer;
    StreamReader reader;
    **/

    // ī�޶�, ����ũ
    [SerializeField] private RawImage cameraImage;
    [SerializeField] private AudioSource MicSource;

    private RTCConfiguration config = new RTCConfiguration
    {
        iceServers = new[] { new RTCIceServer { urls = new[] { "stun:stun.services.mozilla.com", "stun:stun.l.google.com:19302" } } }
    };


    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.instance.myPlayer;

    }

    void test(Dictionary<string, dynamic> t)
    {
        Debug.Log(t["hey"]);
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


        if (Input.GetKeyDown(KeyCode.S))
        {
            joinRoom();
        }

    }

    /**
    void socketConnect()
    {

        try
        {
            tcpsocket = new TcpClient("http://localhost", 8080);
            stream = tcpsocket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            //socketReady = true;
        }
        catch (Exception e)
        {
            Debug.Log($"���Ͽ��� : {e.Message}");
        }
    }
    **/


    private void cameraConnect()
    {

        // ī�޶�
        WebCamDevice[] devices = WebCamTexture.devices;

        int selectedCameraIndex = -1;
        for (int i = 0; i < devices.Length; i++)
        {
            // ��� ������ ī�޶� �α�
            Debug.Log("Available Webcam: " + devices[i].name + ((devices[i].isFrontFacing) ? "(Front)" : "(Back)"));

            // �ĸ� ī�޶����� üũ
            if (devices[i].isFrontFacing)
            {
                // �ش� ī�޶� ����
                selectedCameraIndex = i;
                break;
            }
        }

        // WebCamTexture ����
        if (selectedCameraIndex >= 0)
        {
            // ���õ� ī�޶� ���� ���ο� WebCamTexture�� ����
            textureWebCam = new WebCamTexture(devices[selectedCameraIndex].name);

            // ���ϴ� FPS�� ����
            if (textureWebCam != null)
            {
                textureWebCam.requestedFPS = 60;
            }
        }

        // objectTarget���� ī�޶� ǥ�õǵ��� ����
        if (textureWebCam != null)
        {
            // ī�޶�
            objectTarget.GetComponent<RawImage>().texture = textureWebCam;
            textureWebCam.Play();

            // ī�޶� �����ٰ� ǥ��
            isCameraOn = true;

            cameraImage = objectTarget.GetComponent<RawImage>();


            //MediaStreamTrack 


        }
        else // ī�޶� �ؽ��� ����
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

            // ����ũ �����ٰ� ǥ��
            isMicOn = true;
        }
        catch
        {
            Debug.Log("No mic");
        }
    }

    public void setDevice()
    {
        cameraConnect();
        micConnect();

        StartButton.onClick.AddListener(StartBusking);
    }

    // ����ŷ ���ͷ�Ƽ��
    public void StartBusking()
    {
        if (isMicOn && isCameraOn)
        {
            player.GetComponent<PlayerControl>().OffInteractiveButton();
            webRTCConnect();
        }

    }

    public void webRTCConnect()
    {
        try
        {
            socket = SocketIo.establishSocketConnection(mainServer);
            socket.open();

        }
        catch 
        {
            Debug.Log("No Connection");
        }

    }



    void joinRoom()
    {
        int roomNum = 1; // �ϴ� ����

        if (socket != null)
        {
            Dictionary<string, dynamic> userOption = new Dictionary<string, dynamic>();
            userOption.Add("roomNum", roomNum);
            userOption.Add("userId", socket.id);

            socket.emit("joinRoom", userOption);

            socket.on("createRoom", onCreateRoom);

            //isJoin = true;
        }
    }

    void callCreateRoom()
    {
        if (socket != null)
        {
            socket.on("createRoom", onCreateRoom);
        }
    }

    // on�� �ϱ� ���ؼ��� emit�ȿ� �־�� �ϴ� �� ���� why..?
    void onCreateRoom(Dictionary<string, dynamic> userOption)
    {

        var sendPC = new RTCPeerConnection(ref config);
        Debug.Log("!!!!!!!!");
        Debug.Log(sendPC);

        sendPC.OnIceCandidate = candidate => sendPC.AddIceCandidate(candidate);

        var opOffer = sendPC.CreateOffer();

        if (opOffer != null)
        {
            RTCSessionDescription desc = new RTCSessionDescription();
            desc = opOffer.Desc;
            sendPC.SetLocalDescription(ref desc);

            //Dictionary<string, object> tmp = (Dictionary<string, object>) userOption;
            //Debug.Log("Tmp : " + tmp);
            //tmp["offer"] = opOffer;
           
            //socket.emit("senderOffer", userOption);
        }

        //user option = 1
        try
        {
            //var sendPc = new RTCPeerConnection(ref config);

            /**
            sendPc.OnTrack = e =>
            {
                if (e.Track is VideoStreamTrack videoTrack)
                {
                    videoTrack.OnVideoReceived += tex =>
                    {
                        cameraImage.texture = tex;

                        //??????
                        sourceStream.AddTrack(videoTrack);
                    };
                }

                if (e.Track is AudioStreamTrack audioTrack)
                {
                    MicSource.SetTrack(audioTrack);
                    MicSource.loop = true;
                    MicSource.Play();
                }
            };

            sendPc.OnIceCandidate = candidate => sendPc.AddIceCandidate(candidate);


            foreach (var track in sourceStream.GetTracks())
            {
                sendPc.AddTrack(track, sourceStream);
            }

            var offer = sendPc.CreateOffer();

            var offerDesc = offer.Desc;
            sendPc.SetLocalDescription(ref offerDesc);

            //socket.emit("senderOffer", "TESt");
            **/
        }
        catch
        {

        }
    }



}
