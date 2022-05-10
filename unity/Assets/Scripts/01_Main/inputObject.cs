using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;//����ǥ����
using LitJson;
using UnityEngine.Networking;


public class inputObject : MonoBehaviour
{



    public int type;//0:�ߺ�üũ   1:regex üũ(no ��ư)   2:regexüũ(��ư o)   3: toggle ����
    public string key;
    public string name_content;
    public string btn_content;
    public string under_content;
    public string regex_str;
    public string[] strs;

    public TextMeshProUGUI nameText;
    public Button btn;
    public TMP_InputField inputField;

    public TextMeshProUGUI underText;


    public inputObject sub_obj;

    private Regex regex;

    public bool isOkay = false;

    public delegate void OkayHandler(string str);
    public event OkayHandler OnClickButton_;

    private TextMeshProUGUI btn_text;
    private Toggle[] toggles;
    private TextMeshProUGUI[] toggleTexts;
    // Start is called before the first frame update
    void Awake()
    {

        
        if (inputField != null)
        {
            inputField.onValueChanged.AddListener(delegate { checkRegex(); });
            

            if (btn_content == "")
            {
                btn.gameObject.SetActive(false);
            }
            else
            {
                btn_text = (btn.GetComponentInChildren<TextMeshProUGUI>());
                btn_text.text = btn_content;
                btn.onClick.AddListener(onClickButton);
            }
        }
        if (key=="fav")
        {
            
            toggles = GetComponentsInChildren<Toggle>();
            toggleTexts = new TextMeshProUGUI[toggles.Length];
            for (int i=0; i<toggles.Length; i++)
            {
                toggleTexts[i] = toggles[i].GetComponentInChildren<TextMeshProUGUI>();
                toggleTexts[i].text = GlobalData.Genre[i + 1];
                toggles[i].isOn = false;
            }

            isOkay = true;
        }

        nameText.text = name_content;


    }
    public void setRegex_str(string str)
    {
        regex_str = str;
        regex = new Regex(regex_str);
    }
    public void reset()
    {
  
        if (key=="password2" || key=="email2" )
        {
            setRegex_str("");
        }
        else
        {
            setRegex_str(regex_str);
        }

        if (inputField != null)
        {
            inputField.text = "";
            checkRegex();
        }

        isOkay = false;

        if (toggles != null)
        {
            for (int i = 0; i < toggles.Length; i++)
            {
                toggles[i].isOn = false;
            }
            isOkay = true;
        }
        

    }
    void changeUnderTextColor()
    {//�Ϸ� ��Ȳ�� ���� �� �ٲٴ� �Լ�
        if (isOkay)
        { 
            underText.text = "�Ϸ�";
            underText.color = new Color(0f, 255f, 0f);
        }
        else
        {
            
            underText.color = new Color(255f, 0f, 0f);
        }

    }
    bool checkRegex()
    {
        isOkay = false;
        //������ Ȯ���ϰ� �˸��� �޽����� �����ֵ��� �ϴ� �Լ�
        //������ �ӽ÷� �⺻ �޽����� �������� ��
        if (inputField != null)
        {
            if (key == "email")
            {
                btn_text.text = btn_content;
            }
            if (regex.IsMatch(inputField.text))
            {
                if (underText != null)
                {
                    underText.text = "";
                }
                if (type == 1 && regex_str!="")
                {
                    isOkay = true;

                    if (sub_obj != null)
                    {
                        sub_obj.setRegex_str(inputField.text);
                        sub_obj.checkRegex();
                    }
                    changeUnderTextColor();
                }
            }
            else
            {
                if (underText != null)
                {
                    underText.text = under_content;
                }
                changeUnderTextColor();

                return false;
            }
        }

        return true;
    }



    void onClickButton()
    {
        if (inputField != null && type!=2)
        {
            if (underText.text == under_content) return;
        }

        switch (type)
        {
                //0:�ߺ�üũ   1:regex üũ(no ��ư)   2:regexüũ(��ư o)   3: toggle ����
                case 0:

                    if (key == "email" && btn_text.text!=btn_content)//������ �ܰ���
                    {
                        EmailAuth();
                    }
                    else
                    {
                        StartCoroutine(GET_Check());
                    }
                    
                    return;

                case 2:
                    //Ȯ�� �˾����� regexüũ
                    if (regex_str != "")
                    {
                        if (checkRegex())
                        {
                            isOkay = true;
                        }
                    }
                    break;

                default:
                    break;

        }
        if (strs.Length > 0)
        {//��� �˾��� �ߴ� ������Ʈ���
            OnClickButton_(isOkay ? strs[1] : strs[0]);
        }

        changeUnderTextColor();
    }
    private void EmailAuth()
    {
        if (key != "email") return;

        //�̸��� �����ϰ�
        var characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var Charsarr = new char[6];
        var random = new System.Random();

        for (int i = 0; i < Charsarr.Length; i++)
        {
            Charsarr[i] = characters[random.Next(characters.Length)];
        }

        var resultString = new string(Charsarr);

        //����
        StartCoroutine(POST_EmailCode(inputField.text, resultString));

        sub_obj.setRegex_str("^" + resultString + "$");
        sub_obj.checkRegex();

        
        
        
    }
    public List<string> GetPreferredGenres()
    {
        List<string> list = new List<string>();
        for (int i = 0; i<toggles.Length; i++)
        {
            if (toggles[i].isOn == true)
            {
                list.Add(toggleTexts[i].text);
            }
        }
        return list;
    }
    public string GetText()
    {
        return inputField.text;
    }
    IEnumerator GET_Check()
    {

        IdEmail check = new IdEmail();
        if (key == "id")
            check.id = inputField.text;
        else if (key == "email")
            check.email = inputField.text;

        string json = JsonUtility.ToJson(check);

        Debug.Log(json);
        using (UnityWebRequest www = UnityWebRequest.Get(GlobalData.url + "/user/check"))
        {

            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("accept", "text/plain");
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));

            yield return www.SendWebRequest();

            if (www.error == null)
            {
                if (www.isDone)
                {
                    if (key == "email")
                    {                       
                        btn_text.text = "���� �ڵ�";
                    }
                    else if (key == "id")
                    {
                        isOkay = true;
                        changeUnderTextColor();
                    }
                    OnClickButton_(strs[1]);
                }
                else
                {
                    isOkay = false;
                }                    
                
            }
            else
            {
                if (www.responseCode == 400) {//�ߺ�
                    isOkay = false;
                    if (key == "email") {
                        
                    }
                    else if (key == "id")
                    {
                        changeUnderTextColor();
                    }
                    OnClickButton_(strs[0]);
                }
                Debug.Log(www.error.ToString());
            }
        }



    }
    IEnumerator POST_EmailCode(string email, string key)
    {
        EmailKey emailKey = new EmailKey();
        emailKey.email = email;
        emailKey.key = key;
        string json = JsonUtility.ToJson(emailKey);
        using (UnityWebRequest request = UnityWebRequest.Post(GlobalData.url + "/auth/email", json))
        {// ���� �ּҿ� ������ �Է�
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();//���� ��ٸ���

            if (request.error == null)//���� ����
            {
                isOkay = true;
                //������ ����

            }
            else//������ ����
            {
                isOkay = false;
                Debug.Log(request.error.ToString());

            }
            OnClickButton_(isOkay ? strs[3] : strs[2]);
            changeUnderTextColor();
        }

    }
    JsonData JsonToObject(string json)
    {
        return JsonMapper.ToObject(json);
    }
}
