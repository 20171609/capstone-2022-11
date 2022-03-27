using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Page : MonoBehaviour
{//��ӿ� Ŭ����
    public Button exitBtn; 
    protected bool isAlreadyInit = false;
    // Start is called before the first frame update
    void Awake()
    {
        exitBtn.onClick.AddListener(Close);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Open()
    {
        Init();

        gameObject.SetActive(true);

        Load();

    }

    virtual public void Init()
    {//�ش� ������ �̺�Ʈ������, ��ü ���

    }
    virtual public void Load()
    {//�ش� ������ �ʱ�ȭ�ϱ�

    }
    public void Close()
    {
        //�ʱ�ȭ
        Reset();
        //�ݱ�
        gameObject.SetActive(false);
    }
    virtual public void Reset()
    {//�ش� ������ �ʱ�ȭ�ϱ�

    }
}
