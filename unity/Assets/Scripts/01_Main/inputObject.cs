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
    string url = "http://localhost:8080";



    public int type;//0:�ߺ�üũ   1:regex üũ(no ��ư)  2: �̸��� ����   3:regexüũ(��ư o)   4: toggle 3�� ����
    public string key;
    public string name_content;
    public string btn_content;
    public string under_content;
    public string regex_str;
    public string[] result_strs;

    public TextMeshProUGUI nameText;
    public Button btn;
    public TMP_InputField inputField;

    public TextMeshProUGUI underText;


    public inputObject sub_obj;

    private Regex regex;

    public bool isOkay = false;

    public delegate void OkayHandler(string str);
    public event OkayHandler OnClickButton_;
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
                (btn.GetComponentInChildren<TextMeshProUGUI>()).text = btn_content;
                btn.onClick.AddListener(onClickButton);
            }
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
        if (checkRegex()==true)
        {
            switch (type)
            {
                //0:�ߺ�üũ   1:regex üũ(no ��ư)   2:regexüũ(��ư o)   3: toggle 3�� ����
                case 0:
                    
                    StartCoroutine(GET_Check());
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
            if (result_strs.Length > 0)
            {//��� �˾��� �ߴ� ������Ʈ���
                OnClickButton_(isOkay ? result_strs[1] : result_strs[0]);
            }
        }
        else
        {
            isOkay = false;
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


        sub_obj.setRegex_str("^" + resultString + "$");
        sub_obj.checkRegex();
        isOkay = true;

        changeUnderTextColor();
        if (result_strs.Length > 0)
        {//��� �˾��� �ߴ� ������Ʈ���
            OnClickButton_(isOkay ? result_strs[1] : result_strs[0]);
        }
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
        using (UnityWebRequest www = UnityWebRequest.Get(url + "/api/user/check"))
        {

            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("accept", "text/plain");
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));

            yield return www.SendWebRequest();

            if (www.error == null)
            {
                if (www.isDone)
                {
                    Debug.Log(key+"�ߺ� ���");
                    if (key == "email") EmailAuth();
                    else if(key=="id")
                    {
                        isOkay = true;
                        
                    }
                }
                else
                {
                    isOkay = false;
                }                    
                
            }
            else
            {
                if (www.responseCode == 400) {
                    isOkay = false;
                }
                Debug.Log(www.error.ToString());
            }
            changeUnderTextColor();
            if (result_strs.Length > 0)
            {//��� �˾��� �ߴ� ������Ʈ���
                OnClickButton_(isOkay ? result_strs[1] : result_strs[0]);
            }
        }



    }
    JsonData JsonToObject(string json)
    {
        return JsonMapper.ToObject(json);
    }
}
