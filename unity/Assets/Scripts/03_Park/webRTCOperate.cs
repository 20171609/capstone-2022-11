using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Unity.WebRTC;
using KyleDulce.SocketIo;
using LitJson;

public class webRTCOperate : Singleton<webRTCOperate>
{
    // BuskingSpot ����
    public int roomNum;
    public BuskingSpot nowBuskingSpot;

    // Socket ����
    Socket socket = null;
    string mainServer = "http://localhost:8080";

    // Webrtc ����
    private RTCPeerConnection sendPC;
    private RTCPeerConnection receivePC;

    // Stream ����
    [SerializeField] private WebCamTexture textureWebCam;
    //private VideoStreamTrack videoStream;
    private MediaStreamTrack videoStream;

    // BuskerPanel ����
    [SerializeField] private RawImage buskerRawImage;
    [SerializeField] private RawImage otherRawImageSmall;

    //��Ÿ
    bool isSocketOn = false;

    private void Update()
    {
        // input �޾ƾ߸� ��
        if (Input.GetKeyDown(KeyCode.S) && isSocketOn)
        {
            joinRoom();
        }
    }


    public void webRTCConnect()
    {
        try
        {
            socket = SocketIo.establishSocketConnection(mainServer);
            socket.open();
            isSocketOn = true;

            /**
            if (nowBuskingSpot != null) // ���߿� ���� �����ų� �ϸ� roomNum�̶� nowvBuskingSpot �ʱ�ȭ��Ű��
                nowBuskingSpot.isUsed = true; // �ϴ� �����Ϳ����¤� �� �ٲ� �ٵ� �̰� �����Ϳ��� ���ϸ� �ٷ� �ȹٲ� �� �ִٴ� �� ���
            **/

            if (nowBuskingSpot != null)
            {
                nowBuskingSpot.callChangeUsed();
            }
        }
        catch
        {
            Debug.Log("No Connection");
        }
    }

    public void setWebCamTexture(WebCamTexture texture)
    {
        textureWebCam = texture;
    }


    void joinRoom()
    {
        WebRTC.Initialize(EncoderType.Software);

        if (socket != null)
        {
            Dictionary<string, dynamic> userOption = new Dictionary<string, dynamic>();
            userOption.Add("roomNum", roomNum);
            userOption.Add("userId", socket.id);

            socket.emit("joinRoom", userOption);
            socket.on("createRoom", onCreateRoom);
            //socket.on("joinRoom", onjoinRoom);

            //socket.on("senderOffer", onSenderOffer);


            //isJoin = true;
        }
    }

    void onSenderOffer(Dictionary<string, dynamic> userOption)
    {
        try
        {
            Debug.Log("[CLIENT]get Offer");
            var offer = userOption["offer"];

            Debug.Log("getOpper: " + offer);

            receivePC.SetRemoteDescription(offer);


            /**
            receivePC
            .createAnswer({
            offerToReceiveAudio: true,
                offerToReceiveVideo: true,
            })
            .then((answer) => {
                 console.log("[CLIENT]sender Answer");
                 receivePC.setLocalDescription(new RTCSessionDescription(answer));
                 socket.emit("getReceiverAnswer", answer, userOption);
             })
            **/
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    private static RTCConfiguration GetSelectedSdpSemantics()
    {
        RTCConfiguration config = default;
        config.iceServers = new[] {
            new RTCIceServer { urls = new[] { "stun:stun.services.mozilla.com" } },
            new RTCIceServer { urls = new[] { "stun:stun.l.google.com:19302" } }
        };
        return config;
    }

    void onjoinRoom(Dictionary<string, dynamic> userOption)
    {
        Debug.Log("Join : " + userOption["userId"] + " RoomID : " + userOption["roomNum"]);

        userOption["option"] = 2;

        var config = GetSelectedSdpSemantics();
        receivePC = new RTCPeerConnection(ref config);

        receivePC.OnIceCandidate = Event => {
            if (Event.Candidate != null)
            {
                Debug.Log("[CLIENT]receive Candi");
                userOption["candidate"] = Event.Candidate;

                socket.emit("getReceiverCandidate", userOption);
            }
        };

        receivePC.OnTrack = (RTCTrackEvent e) =>
        {
            Debug.Log("OnTrack is execute");
        };

        userOption["receivePC"] = receivePC;
        socket.emit("joinRoomFromClient", userOption);

        /**
        receiveStream.OnAddTrack = e =>
        {
            if (e.Track is VideoStreamTrack track)
            {
                Texture receiveVideo = track.Texture;

                objectTarget.GetComponent<RawImage>().texture = receiveVideo;

                Debug.Log(receiveVideo);

            }

        };

        receivePC.OnTrack = (RTCTrackEvent e) =>
        {
            Debug.Log("!!!!!!!!!!!!1");

            if (e.Track.Kind == TrackKind.Video)
            {
                // Add track to MediaStream for receiver.
                // This process triggers `OnAddTrack` event of `MediaStream`.
                receiveStream.AddTrack(e.Track);
                Debug.Log("Add Track is finished");
            }
        };
        **/

    }

    // ���� ����⸸ �ϸ� server�ܿ��� senderoffer�� getsendercandidate�� ����� �۵��ؾ���
    void onCreateRoom(Dictionary<string, dynamic> userOption)
    {
        Debug.Log("Create : " + userOption["userId"] + " RoomID : " + userOption["roomNum"]);

        // MediaStreamTrack
        videoStream = new VideoStreamTrack(textureWebCam);

        userOption["option"] = 1;

        var config = GetSelectedSdpSemantics();
        sendPC = new RTCPeerConnection(ref config);

        sendPC.OnIceCandidate = Event =>
        {
            if (!string.IsNullOrEmpty(Event.Candidate)) // Candidate ������ �ٸ� > �� Json���� ����������
            {
                Debug.Log("[CLIENT]send Candi");
                
                // Json���� ����
                JsonData candidate = JsonMapper.ToJson((RTCIceCandidate)Event);
                userOption["candidate"] = candidate;

                // ������ > createoffer > oniceCandidate�ΰ� ����
                socket.emit("getSenderCandidate", userOption);
            }
        };

        // MediaStream �ֱ�
        sendPC.AddTrack(videoStream);

        // unity�� ������ rtcpeerconnection ������ �ٸ���. ���� �ʿ������� �˸� �ɰŰ�����
        //JsonData sendPCJson = JsonMapper.ToJson(sendPC); // Json���� �ִ� �� �����ε� offer�� �������� ����
        //userOption["senderPC"] = sendPC; // peerconnection ��ü�� �ִ� ���� offer�� �������� ����

        StartCoroutine(CreateOffer(userOption));

        // IceCandidateAdd
        //sendPC.OnIceCandidate = candidate => sendPC.AddIceCandidate(candidate);

    }

    private IEnumerator CreateOffer(Dictionary<string, dynamic> userOption)
    {
        var op = sendPC.CreateOffer();
        yield return op;

        if (op.IsError)
        {
            OnCreateSessionDescriptionError(op.Error);
            yield break;
        }

        try
        {
            // offer.Desc�� type�� offer�̰�, offer.Desc.sdp�� �ִ�.
            var tmp = op.Desc;

            sendPC.SetLocalDescription(ref tmp);

            Dictionary<string, dynamic> offer = new Dictionary<string, dynamic>();
            offer.Add("sdp", op.Desc.sdp);
            userOption["offer"] = offer;

            socket.emit("senderOffer", userOption);

        }
        catch (Exception e)
        {
            Debug.Log("No Sending Offer: " + e);
        }

    }

    private static void OnCreateSessionDescriptionError(RTCError error)
    {
        Debug.LogError($"Failed to create session description: {error.message}");
    }

    void getReceiverAnswer(Dictionary<string, dynamic> answer)
    {
        Debug.Log("[CLIENT]get Answer");

        //await sendPC.setRemoteDescription(answer);
        Debug.Log(answer["answer"]);
        sendPC.SetRemoteDescription(answer["answer"]);
    }

    void getCandidate(Dictionary<string, dynamic> data)
    {
        Debug.Log("[CLIENT]get Candi " + data);

        var candidate = data["candidate"];
        var option = data["option"];

        var iceCandidate = new RTCIceCandidate(candidate);

        if (option == 1)
        {
            // �ϴ� �ʿ������
            //buskerRawImage.texture = videoStream.Texture;
        }
        else
        {
            receivePC.AddIceCandidate(iceCandidate);
        }

    }


    // ---------JSON �����---------

}
