using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;	// UnityWebRequest����� ���ؼ� �����ش�.
using LitJson;

[System.Serializable]
public class IdEmail
{
    public string id;
    public string email;
}
[System.Serializable]
public class Auth
{
    public string id;
    public string password;
}
public class Main : MonoBehaviour
{
    private string url = GlobalData.url;

    public Button joinBtn;
    public Button loginBtn;
    public TMP_InputField id_input;
    public TMP_InputField password_input;
    public Join join;

    public GameObject wrong_obj;

    private Animator animator;
    private TextMeshProUGUI wrongText;

    IEnumerator timewaitter;
    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        joinBtn.onClick.AddListener(delegate { join.OpenJoinPanel(); });
        loginBtn.onClick.AddListener(OnClickLoginButton);
        wrong_obj.SetActive(false);
        wrongText = wrong_obj.GetComponentInChildren<TextMeshProUGUI>();
        join.OnClickJoinButton_ += PostJoin;
        timewaitter = waitTime(5);
        
    }
    void PostJoin(User user)
    {
        if (timewaitter != null)
        {
            StopCoroutine(timewaitter);
            StartCoroutine(timewaitter);
        }
        StartCoroutine(Join_UnityWebRequestPOST(user));
    }
    void PostEmailCode(string email, string code)
    {
        StartCoroutine(POST_EmailCode(email,code));
    }
    void OnClickLoginButton()
    {//�α��� ��ư�� ������ �� 
        if (timewaitter != null)
        {
            StopCoroutine(timewaitter);
            StartCoroutine(timewaitter);
        }
        StartCoroutine(Login_UnityWebRequestPOST());     
        
    }
    IEnumerator waitTime(float value)
    {
        yield return new WaitForSeconds(value);
        StopAllCoroutines();
    }
    IEnumerator Login_UnityWebRequestPOST()
    {
        animator.SetBool("isLoading", true);
        Auth auth = new Auth//���� inputfield�� �ۼ��� �� Ŭ������ ��ȯ
        {
            id = id_input.text,
            password = password_input.text
        };
        Debug.Log("�α��� �õ� : " + id_input.text + " " + password_input.text);


        string json = JsonUtility.ToJson(auth);
        using (UnityWebRequest request = UnityWebRequest.Post(url + "/auth", json))
        {// ���� �ּҿ� ������ �Է�
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();//��� ������ �� ������ ��ٸ���


            if (timewaitter != null)
            {
                StopCoroutine(timewaitter);
            }
            animator.SetBool("isLoading", false);

            if (request.error == null)//�α��� ����
            {
                if (request.isDone)
                {   
                    string jsonResult = System.Text.Encoding.UTF8.GetString(request.downloadHandler.data);


                    JsonData jsonData = JsonToObject(jsonResult);
                    Debug.Log("��� " + jsonData[1]);
                    User user = new User();
             
                    user.SetUser((string)(jsonData[1]["id"]),
                            (string)(jsonData[1]["email"]),
                            (string)(jsonData[1]["nickname"]),
                            (int)(jsonData[1]["character"])
                            );

                    UserData.Instance.Token = (string)jsonData[0];                    
                    UserData.Instance.user = user;
                }
                Debug.Log(request.downloadHandler.text);

                SceneManager.LoadScene("02_Lobby");
            }
            else//�α��� ����
            {
                if (request.responseCode == 400)
                {
                    string jsonResult = System.Text.Encoding.UTF8.GetString(request.downloadHandler.data);
                    JsonData jsonData = JsonToObject(jsonResult);
                    if (jsonData["msg"] != null)
                    {
                        wrongText.text = (string)jsonData["msg"];
                    }
                    wrong_obj.SetActive(true);
                }
                Debug.Log(request.error);
            }
        }
       
        
    }
    IEnumerator Join_UnityWebRequestPOST(User user)
    {
        //join �ε� �ִϸ��̼�
        join.LoadingJoin();

        string json = JsonUtility.ToJson(user);
        using (UnityWebRequest request = UnityWebRequest.Post(url + "/user", json))
        {// ���� �ּҿ� ������ �Է�
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();//���� ��ٸ���
            if (timewaitter != null)
            {
                StopCoroutine(timewaitter);
            }

            if (request.error == null)//���� ����
            {

                Debug.Log(request.downloadHandler.text);
                //join ����
                join.SuccessJoin();
            }
            else//�α��� ����
            {

                Debug.Log(request.error.ToString());
                join.FailJoin();
            }
        }


    }
    
    IEnumerator POST_EmailCode(string email, string key)
    {
        EmailKey emailKey = new EmailKey();
        emailKey.email = email;
        emailKey.key = key;
        string json = JsonUtility.ToJson(emailKey);
        using (UnityWebRequest request = UnityWebRequest.Post(url + "/auth/email", json))
        {// ���� �ּҿ� ������ �Է�
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();//���� ��ٸ���

            if (request.error == null)//���� ����
            {

                //join ����

            }
            else//�α��� ����
            {

                Debug.Log(request.error.ToString());

            }
        }


    }
    JsonData JsonToObject(string json)
    {
        return JsonMapper.ToObject(json);
    }

}
public class EmailKey
{
    public string email;
    public string key;
}