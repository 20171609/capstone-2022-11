using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
public class UserSlot : MonoBehaviour, IPointerDownHandler
{
    public User user;
    public FollowPage.FollowSystemType type;

    public TextMeshProUGUI userName;

    public Button button;//del ��ư, �߰���ư, �Ѵ� �� �� ����. searchslot ���ο� ���� �Ǵ�

    public delegate void ClickHandler(UserSlot us);
    public event ClickHandler OnClickAddButton;
    public event ClickHandler OnClickDelButton;
    public event ClickHandler OnClickSlot;

    private bool isFollow;
    public bool Follow{
        get { return isFollow; }
        set 
        { 
            isFollow = value;
            OnOffButton(!isFollow);
        }
    }
    public void SetType(FollowPage.FollowSystemType type)
    {
        this.type = type;
        if (type == FollowPage.FollowSystemType.follower)
            button.gameObject.SetActive(false);

    }
    public void SetUser(User user)
    {
        this.user = user;
        userName.text = user.nickname + "(" + user.id + ")";
    }
    public User GetUser()
    {
        return user;
    }
    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(ClickButton);
    }
    public void OnOffButton(bool on)
    {
        button.gameObject.SetActive(on);
    }
    void ClickButton()
    {
        if (type==FollowPage.FollowSystemType.searched)
        {   //�˻��� �����̸�
            
            if (isFollow==false)
            {//�߰��Ǿ����������� �߰���ư ���
                Debug.Log(user.id + " �ȷο�");
                OnClickAddButton(this);
                Follow = true;
            }
            else
            {//�߰��Ǿ������� ��ҹ�ư ���
                Debug.Log(user.id + " �ȷο� ���");
                OnClickDelButton(this);
                Follow = false;

            }
        }
        else
        {   //�� �ȷο�, �ȷο� ��� �����̸�
            //��ҹ�ư ���
            OnClickDelButton(this);
            Follow = false;

        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        OnClickSlot(this);
    }


}
