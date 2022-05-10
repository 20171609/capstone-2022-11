using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Page : MusicWebRequest
{//��ӿ� Ŭ����
    public Button exitBtn; 
    protected bool isAlreadyInit = false;
    // Start is called before the first frame update
    void Awake()
    {
        if(exitBtn!=null)
            exitBtn.onClick.AddListener(Close);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Open()
    {
        if (gameObject.activeSelf == true) return;
        Init();

        gameObject.SetActive(true);

        Load();
        MusicController.Instance.subMusicController.Reset();
    }

    virtual public void Init()
    {//�ش� ������ �̺�Ʈ������, ��ü ���

    }
    virtual public void Load()
    {//�ش� ������ �ʱ�ȭ�ϱ�

    }
    public void Close()
    {
        Init();

        //�ʱ�ȭ
        Reset();

        //�ݱ�
        MusicController.Instance.subMusicController.Reset();
        gameObject.SetActive(false);
    }
    virtual public void Reset()
    {//�ش� ������ �ʱ�ȭ�ϱ�

    }
}
