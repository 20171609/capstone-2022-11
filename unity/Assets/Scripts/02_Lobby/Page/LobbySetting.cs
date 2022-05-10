using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LobbySetting : Page
{
    public Button gotoMainSceneBtn;

    void LogoutAndClose()
    {

        UserData.Instance.Clear();
        SceneManager.LoadScene("01_Main");

    }
    override public void Init()
    {//����â �ʱ⼼�� and ���� ���� �ҷ�����
        gotoMainSceneBtn.onClick.AddListener(LogoutAndClose);
    }

}
