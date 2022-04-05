using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;

public class MusicUpload : MonoBehaviour
{ 
    string url = "http://localhost:8080/api";
    void Start()
    {
        
    }
    public void FileUpload(byte[] musicBytes, byte[] imageBytes,Music music, string fileName)
    {
        StartCoroutine(Upload(musicBytes, imageBytes, music, fileName));
    }
    IEnumerator Upload(byte[] musicBytes, byte[] imageBytes, Music music, string fileName)
    {

        string json = JsonUtility.ToJson(music);

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        formData.Add(new MultipartFormFileSection(fileName, musicBytes));
        if (imageBytes != null)
            formData.Add(new MultipartFormFileSection(music.title+".png", imageBytes));

        formData.Add(new MultipartFormDataSection("json", json));
        UnityWebRequest www = UnityWebRequest.Post(url+"/media/", formData);

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
}
