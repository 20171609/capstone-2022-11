using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System;
using KyleDulce.SocketIo;

public class BuskingSpot : MonoBehaviourPun, IPunObservable
{
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isUsed);
        }
        else
        {
            this.isUsed = (bool)stream.ReceiveNext();
        }
    }

    // ī�޶� ����
    protected WebCamTexture textureWebCam = null;
    [SerializeField] private GameObject objectTarget;

    // ����ũ ����
    [SerializeField] private AudioSource micAudioSource;

    public bool isUsed = false;

    Socket socket;

    private void Update()
    {
        if (isUsed)
        {
            this.GetComponent<SpriteRenderer>().color = Color.blue;
        }
        else
        {
            this.GetComponent<SpriteRenderer>().color = new Color32(202, 162, 48, 250);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject player = GameManager.instance.myPlayer;
        if (collision.gameObject == player && player.GetComponent<PhotonView>().IsMine)
        {
            if (isUsed && !player.GetComponent<PlayerControl>().isVideoPanelShown)
                collision.transform.GetComponent<PlayerControl>().OnVideoPanel(0);
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject player = GameManager.instance.myPlayer;
        if (collision.gameObject == player && player.GetComponent<PhotonView>().IsMine)
        {
            if (player.GetComponent<PlayerControl>().isVideoPanelShown)
                collision.transform.GetComponent<PlayerControl>().OffVideoPanel();
        }
    }


    // ����ŷ ���ͷ�Ƽ��
    public void StartBusking()
    {
        isUsed = true;

        GameObject player = GameManager.instance.myPlayer;

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
            player.GetComponent<PlayerControl>().OnVideoPanel(1);
            textureWebCam.Play();

            // �Ҹ�
            try
            {
                string mic = Microphone.devices[0];
                micAudioSource.clip = Microphone.Start(mic, true, 10, 44100);
                while (!(Microphone.GetPosition(mic) > 0)) { } // Wait until the recording has started
                micAudioSource.Play(); // Play the audio source!
                player.GetComponent<PlayerControl>().OffInteractiveButton();
                webRTCConnect();
            }
            catch
            {
                webRTCConnect();
                Debug.Log("No mic");
            }

        }
        else // ī�޶� �ؽ��� ����
        {
            webRTCConnect();
            Debug.Log("No Camera Texture");
        }

    }

    void webRTCConnect()
    {
        try
        {
            socket = SocketIo.establishSocketConnection("http://localhost:8080");
            socket.connect();

            Debug.Log(socket);
            Debug.Log("Connect Success!");
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }

    }

}
