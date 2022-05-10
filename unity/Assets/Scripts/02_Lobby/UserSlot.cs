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
        else if(type== FollowPage.FollowSystemType.searched) {
            //�˻��� ������ ��,

            //�˻��� ������ ����id�� �ȷο���ҵ� ���̵� �϶� �ȷο� falseó��
            UserData.Instance.OnDeleteFollow += SetUserFollowFalse;

            //�˻��� ������ ����id�� �ȷο�� ���̵� �϶� �ȷο� trueó��
            UserData.Instance.OnAddFollow += SetUserFollowTrue;

        }

    }
    void SetUserFollowTrue(string str)
    {
        if (str == user.id) Follow = true;
    }
    void SetUserFollowFalse(string str)
    {
        if (str == user.id) Follow = false;
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
        if (type == FollowPage.FollowSystemType.follow)
        {
            return;
        }
        button.gameObject.SetActive(on);
    }
    void ClickButton()
    {
        if (type==FollowPage.FollowSystemType.searched)
        {   //�˻��� �����̸�
            
            if (isFollow==false)
            {//�߰��Ǿ����������� �߰���ư ���
                Debug.Log(user.id + " �ȷο�");
                if(OnClickAddButton!=null)
                    OnClickAddButton(this);
                Follow = true;
            }
            else
            {//�߰��Ǿ������� ��ҹ�ư ���
                Debug.Log(user.id + " �ȷο� ���");
                if (OnClickDelButton != null)
                    OnClickDelButton(this);
                Follow = false;

            }
        }
        else
        {   //�� �ȷο�, �ȷο� ��� �����̸�
            //��ҹ�ư ���
            if (OnClickDelButton != null)
                OnClickDelButton(this);
            Follow = false;

        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (OnClickSlot != null)
            OnClickSlot(this);
    }
    private void OnDestroy()
    {
 
        UserData.Instance.OnDeleteFollow -= SetUserFollowFalse;

        UserData.Instance.OnAddFollow -= SetUserFollowTrue;
    }


}
