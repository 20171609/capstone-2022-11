using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using LitJson;
using NAudio;
using NAudio.Wave;

public class ModifiedChar
{
    public string id;
    public int value;
}
public class MusicTitle
{
    public string title;
}
public class UserID
{
    public string id;
}
[System.Serializable]
public class Music
{
    public string locate;
    public string imagelocate;
    public string title;
    public string id;
    public string userID;
    public string nickname;
    public string category;
}
public class MusicWebRequest : MonoBehaviour
{
    protected string url = "http://localhost:8080/api";


    protected delegate void SongListHandler(List<Music> musics, bool play=false);
    protected event SongListHandler OnGetSongList;


    protected delegate void MusicHandler(AudioClip audioClip, bool play);//play- �ٷ� ����Ұ�����
    protected event MusicHandler OnGetClip;


    protected delegate void CharacterHandler(int character);//play- �ٷ� ����Ұ�����
    protected event CharacterHandler ModifyCharacter;

    protected IEnumerator POST_ModifiedChar(string _id, int _character)
    {
        ModifiedChar mo = new ModifiedChar//���� inputfield�� �ۼ��� �� Ŭ������ ��ȯ
        {
            id = _id,
            value = _character
        };
     


        string json = JsonUtility.ToJson(mo);
        using (UnityWebRequest request = UnityWebRequest.Post(url + "/auth/modifiedChar", json))
        {// ���� �ּҿ� ������ �Է�

            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("token", UserData.Instance.Token);
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();//��� ������ �� ������ ��ٸ���


            if (request.error == null)
            {

                Debug.Log(request.downloadHandler.text);

                ModifyCharacter(_character);

            }
            else
            {
                Debug.Log(request.error.ToString());

            }
        }
    }

    protected IEnumerator Upload(byte[] musicBytes, byte[] imageBytes, Music music, string fileName)
    {

        string json = JsonUtility.ToJson(music);

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        formData.Add(new MultipartFormFileSection(fileName, musicBytes));
        if (imageBytes != null)
            formData.Add(new MultipartFormFileSection(music.title + ".png", imageBytes));

        formData.Add(new MultipartFormDataSection("json", json));
        UnityWebRequest www = UnityWebRequest.Post(url + "/media/", formData);

        //���� �ε� �ִϸ��̼��߰� 
        yield return www.SendWebRequest();


        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            //�ε��ִϸ��̼� ����
            Debug.Log("Form upload complete!");
        }
    }
    protected IEnumerator GET_MusicList(string listName, string _userid, bool play=false)
    {
        UserID userID= new UserID();
        userID.id = _userid;

        string json = JsonUtility.ToJson(userID);
        Debug.Log(listName+" ����Ʈ: " + json);

        using (UnityWebRequest www = UnityWebRequest.Get(url + "/auth/"+listName))
        {
            www.SetRequestHeader("token",  UserData.Instance.Token);
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("accept", "text/plain");
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));

            yield return www.SendWebRequest();

            List<Music> musics = new List<Music>();
            if (www.error == null)
            {
                if (www.isDone)
                {
                    string jsonResult = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                    Debug.Log("��� " + jsonResult);
                    
                    JsonData jsonData2 = JsonToObject(jsonResult);
                    JsonData jsonData = jsonData2[listName];

                    for (int i = 0; i < jsonData.Count; i++)
                    {
                        Music music = new Music();

                        music.title = (string)jsonData[i]["title"];
                        music.id = (string)jsonData[i]["id"];
                        music.locate = (string)jsonData[i]["locate"];
                        music.userID = (string)jsonData[i]["userID"];
                        music.category = (string)jsonData[i]["category"];
                        music.imagelocate = (string)jsonData[i]["imagelocate"];
                        music.nickname = (string)jsonData[i]["nickname"];

                        musics.Add(music);

                    }
                }
                OnGetSongList(musics, play);
                Debug.Log("done");

            }
            else
            {
                Debug.Log(www.error.ToString());
            }
        }



    }
    protected IEnumerator GetAudioCilpUsingWebRequest(string _filePath, bool play)
    {
        AudioType audioType = AudioType.MPEG;

        string type = _filePath.Substring(_filePath.Length - 3);
        if (type == "wav")
        {
            audioType = AudioType.WAV;
        }
        else if (type == "mp3")
        {

            audioType = AudioType.MPEG;
        }
        else if (type == "ogg")
        {
            audioType = AudioType.OGGVORBIS;
        }
        Debug.Log("get audio " +_filePath+audioType.ToString());
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url + _filePath, audioType))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
            }
            else
            {
                OnGetClip(DownloadHandlerAudioClip.GetContent(www),play);
            }
        }
    }
    protected IEnumerator GET_SearchMusicTitle(string _title)
    {
        MusicTitle musicTitle = new MusicTitle();
        musicTitle.title = _title;

        string json = JsonUtility.ToJson(musicTitle);
        Debug.Log("�� �˻� json: " + json);

        using (UnityWebRequest www = UnityWebRequest.Get(url + "/music/"))
        {

            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("accept", "text/plain");
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));

            yield return www.SendWebRequest();

            List<Music> musics = new List<Music>();
            if (www.error == null)
            {
                if (www.isDone)
                {
                    string jsonResult = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                    Debug.Log("��� " + jsonResult);
                    JsonData jsonData = JsonToObject(jsonResult);


                    for (int i = 0; i < jsonData.Count; i++)
                    {
                        Music music = new Music();

                        music.title = (string)jsonData[i]["title"];
                        music.id = (string)jsonData[i]["id"];
                        music.locate = (string)jsonData[i]["locate"];
                        music.userID = (string)jsonData[i]["userID"];
                        music.category = (string)jsonData[i]["category"];

                        musics.Add(music);

                    }
                }
                OnGetSongList(musics);
                Debug.Log("done");

            }
            else
            {
                Debug.Log(www.error.ToString());
            }
        }



    }
    JsonData JsonToObject(string json)
    {
        return JsonMapper.ToObject(json);
    }
}



