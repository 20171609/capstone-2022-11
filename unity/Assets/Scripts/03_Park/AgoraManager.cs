using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using agora_gaming_rtc;

public class AgoraManager : Singleton<AgoraManager>
{
    [Header("Agora Properties")]

    // *** ADD YOUR APP ID HERE BEFORE GETTING STARTED *** //
    [SerializeField] private string appID = "ADD YOUR APP ID HERE";
    //[SerializeField] private string channel = "unity3d";
    [SerializeField] private string token = "ADD Your APP TOKEN HERE";
    [SerializeField] private AgoraChannel nowChannel;
    private IRtcEngine mRtcEngine = null;
    private uint myUID = 0;

    [Header("GameObjects")]
    [SerializeField] private RawImage buskerVideo;

    // Start is called before the first frame update
    void Start()
    {
    }

    void setChannel(int roomNum)
    {
        // channel�� ������ �����
        // channel �̸��� �� �̸�����
        if (mRtcEngine != null)
        {
            nowChannel = mRtcEngine.CreateChannel(roomNum.ToString());

            // SetClientRole >> Host
            nowChannel.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);

        }
    }

    public void loadEngine()
    {
        if (mRtcEngine != null)
        {
            mRtcEngine = null;
            IRtcEngine.Destroy();
        }


        mRtcEngine = IRtcEngine.GetEngine(appID);
        //mRtcEngine = IRtcEngine.getEngine(appID);
        //RtcEngineConfig config = new RtcEngineConfig(appID, config: new LogConfig());
        //mRtcEngine = IRtcEngine.GetEngine(config);

        // ���� ä�� ����� ���
        mRtcEngine.SetMultiChannelWant(true);

    }

    public void setBuskerAgora(int roomNum)
    {
        // channel ����
        setChannel(roomNum);

        // Video setup
        VideoEncoderConfiguration config = new VideoEncoderConfiguration();
        config.dimensions.width = 480;
        config.dimensions.height = 480;
        config.frameRate = FRAME_RATE.FRAME_RATE_FPS_15;
        config.bitrate = 800;
        config.degradationPreference = DEGRADATION_PREFERENCE.MAINTAIN_QUALITY;
        mRtcEngine.SetVideoEncoderConfiguration(config);

        // SetUp Callbacks
        nowChannel.ChannelOnJoinChannelSuccess 
            = OnBuskerJoinChannelSuccessHandler;
        // mRtcEngine.OnUserJoined = OnAudienceJoinedHandler; // �ٸ� ���� ���Դ��� �� �ʿ� ����
        mRtcEngine.OnLeaveChannel = OnBuskerLeaveChannelHandler;
        // mRtcEngine.OnUserOffline = OnAudienceOfflineHandler; // �ٸ� ���� �������� �� �ʿ� ����

        // Video �θ���
        mRtcEngine.EnableVideo();
        mRtcEngine.EnableVideoObserver();

        // By setting our UID to "0" the Agora Engine creates a unique UID and returns it in the OnJoinChannelSuccess callback. 
        nowChannel.JoinChannel(token, "", 0, new ChannelMediaOptions(true, true));
    }

    #region Agora Callbacks
    // Busker Join Success Handler
    private void OnBuskerJoinChannelSuccessHandler(string channelName, uint uid, int elapsed)
    {
        myUID = uid;

        CreateUserVideoSurface(uid, true);
    }

    // Audience Join Success Handler
    private void OnAudienceJoinedHandler(uint uid, int elapsed)
    {

    }

    // Busker�� ������ ��
    private void OnBuskerLeaveChannelHandler(RtcStats stats)
    {
        if (mRtcEngine != null)
        {
            // ä�� �ı��ϱ�
            nowChannel.LeaveChannel();
            nowChannel.ReleaseChannel();

            // mRTCEngine video �ȵǰ�
            mRtcEngine.DisableVideoObserver();
            mRtcEngine.DisableVideo();
        }
    }

    #endregion

    // Create new image plane to display users in party.
    private void CreateUserVideoSurface(uint uid, bool isLocalUser)
    {
        // BuskerVideo�� VideoSurface �ޱ�
        if (buskerVideo == null)
        {
            Debug.LogError("CreateUserVideoSurface() - newUserVideoIsNull");
            return;
        }

        // Update our VideoSurface to reflect new users
        VideoSurface newVideoSurface = buskerVideo.GetComponent<VideoSurface>();
        if (newVideoSurface == null)
        {
            Debug.LogError("CreateUserVideoSurface() - VideoSurface component is null on newly joined user");
            return;
        }

        /**
        if (isLocalUser == false)
        {
            newVideoSurface.SetForUser(uid);
        }
        newVideoSurface.SetGameFps(30);
        **/
    }


    private void OnApplicationQuit()
    {
        TerminateAgoraEngine();
    }

    private void TerminateAgoraEngine()
    {
        if (mRtcEngine != null)
        {
            mRtcEngine.LeaveChannel();
            mRtcEngine = null;
            IRtcEngine.Destroy();
        }
    }



}
