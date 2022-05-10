using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FollowPage : Page
{
    public TextMeshProUGUI userNameText;
    public TextMeshProUGUI followerText;
    public TextMeshProUGUI followText;
    public TextMeshProUGUI musicCntText;
    public TMP_InputField searchField;
    public Character character;

    public Button myInfoBtn;
    public Button infoFollowBtn;

    public Button[] followBtns;

    public GameObject uploadedSongScrollViewObject;
    private List<PlaySongSlot> uploadedSlots;

    public GameObject followUserScrollViewObject;
    public GameObject searchedUserScrollViewObject;


    private List<UserSlot> followUserSlots;
    private List<UserSlot> searchedUserSlots;
    private string[] currentUserId;
    private TextMeshProUGUI infoFollowText;//�ȷο� ���, �ȷο� �ϱ� text

    private FollowSystemType FL;//follow�� �����ٰ���, follower�����ٰ���
    public enum FollowSystemType
    {
        uploadList,follow,follower,searched
    }
    private void Start()
    {
        Init();
    }
    override public void Init()
    {
        if (isAlreadyInit == false)
        {

            isAlreadyInit = true;
            currentUserId = new string[2];

            uploadedSlots = new List<PlaySongSlot>(uploadedSongScrollViewObject.GetComponentsInChildren<PlaySongSlot>());
            followUserSlots = new List<UserSlot>(followUserScrollViewObject.GetComponentsInChildren<UserSlot>());
            searchedUserSlots = new List<UserSlot>(searchedUserScrollViewObject.GetComponentsInChildren<UserSlot>());

            infoFollowText = infoFollowBtn.transform.GetComponentInChildren<TextMeshProUGUI>();

            searchField.onSubmit.AddListener(delegate {
                
                GetUserListAsync(FollowSystemType.searched);
            
            });
            //�� ���� ���� ��ư
            myInfoBtn.onClick.AddListener(delegate {
                LoadUserProfile();
            });
            //�� �ȷο� ��ư
            followBtns[0].onClick.AddListener(delegate {
                followBtns[0].image.color = new Color(1f, 1f, 1f, 1f);
                followBtns[1].image.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                GetUserListAsync(FollowSystemType.follower);
  
            });
            //�� �ȷο� ��ư
            followBtns[1].onClick.AddListener(delegate {
                followBtns[1].image.color = new Color(1f, 1f, 1f, 1f);
                followBtns[0].image.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                GetUserListAsync(FollowSystemType.follow);
            });

            infoFollowBtn.onClick.AddListener(delegate{
                if (infoFollowText.text == "�ȷο�")
                {//�ȷο�
                    FollowUser(currentUserId[0], currentUserId[1]);
                }
                else
                {//�ȷο� ���
                    FollowCancelUser(currentUserId[0], currentUserId[1]);
                }
            });
            //�ȷο찡 ��ҵǾ�����
            UserData.Instance.OnDeleteFollow += SetInfoFollowTextOnDelete;

            //�ȷο찡 �߰��Ǿ��� ��
            UserData.Instance.OnAddFollow += SetInfoFollowTextOnAdd;



        }
    }
    void SetInfoFollowTextOnDelete(string str)
    {
        if (str == currentUserId[0])
            infoFollowText.text = "�ȷο�";

    }
    void SetInfoFollowTextOnAdd(string str)
    {
        if (str == currentUserId[0])
            infoFollowText.text = "�ȷο� ���";
    }
    //���� �������� �ε��ϴ� �Լ�
    async void LoadUserProfile(User user=null, bool isFollow=false)
    {
        if (user != null)
        {
            if (currentUserId[0] == user.id)
                return;
            //���� �������� ������ id�� ���� ����
            currentUserId[0] = user.id;
            currentUserId[1] = user.nickname;//�ȷο� ������

            if (isFollow == true)
            {//���� �̹� �ȷο��� �������
                infoFollowText.text = "�ȷο� ���";
                infoFollowBtn.gameObject.SetActive(true);
                
            }
            else
            {//���� �ȷο��������� �������
                infoFollowText.text = "�ȷο�";
                infoFollowBtn.gameObject.SetActive(true);
            }
        }
        else if (user == null)
        {   //null�϶� ������ �������� ǥ��.
            //���� ���� ���̵� ������ ���̵�� �ٲ�.
            currentUserId[0] = UserData.Instance.user.id;
            currentUserId[1] = UserData.Instance.user.nickname;
            user = UserData.Instance.user;
            //�ȷο� ��ư�� ��Ȱ��ȭ
            infoFollowBtn.gameObject.SetActive(false);
        }

        if (user.preferredGenres.Count == 0)
        {//������ �޾ƿ��� ���� ������ ������ �ε��� ��
            user = await GET_UserInfoAsync(user.id);
            if (user == null) return;
        }


        userNameText.text = user.GetName();
        followText.text = user.followNum + "\n�ȷο�";
        followerText.text = user.followerNum + "\n�ȷο�";
        musicCntText.text = "\n�ø� ����";

        character.ChangeSprite(user.character);
        GetuploadedMusicListAsync(user == UserData.Instance.user?null : user.id);//�����̸� null, ������ �ƴϸ� id

    }

    async void GetuploadedMusicListAsync(string userid)
    {
        MusicList ML = await GET_MusicListAsync("uploadList",false,userid);
        if (ML != null)
        {
            LoadUploadedSlots( ML.musicList);
        }
    }

    //���ε��� ��������Ʈ�� ǥ���ϰ� �ȷο�, �ȷο�, �ø����� ���� ������Ʈ�ϴ� �Լ�
    void LoadUploadedSlots(List<Music> _musics)
    {

        if (_musics != null)
        {

            musicCntText.text = _musics.Count + "\n�ø� ����";

            RemoveSlots(FollowSystemType.uploadList);
            if (_musics.Count == 0)
            {//�ø� ������ �����ϴ�.
              
            }
            GameObject _obj = null;
            PlaySongSlot slot;
            for (int i = 0; i <_musics.Count; i++)
            { 
                _obj = Instantiate(Resources.Load("Prefabs/SongSlot/SongSlotNemoPlayable") as GameObject,uploadedSongScrollViewObject.transform);

                slot = _obj.GetComponent<PlaySongSlot>();
                slot.SetMusic(_musics[i]);


                uploadedSlots.Add(slot);


            }

            uploadedSongScrollViewObject.GetComponent<ScrollViewRect>().SetContentSize();

        }
    }
    async void GetUserListAsync(FollowSystemType type)
    {
        UserList UL;
        if (type == FollowSystemType.searched)
        {
            UL = await GET_SearchUserAsync(searchField.text);
        }
        else
        {
            UL = await GET_FollowSystemUserListAsync(type);
        }

        if (UL != null)
        {
            LoadUserSlots(type,UL.userList);
        }
    }

    //���� ������ Ÿ�Ժ��� ��������Ʈ�� �˸��� ���Կ� �ε��ϴ� �Լ�
    void LoadUserSlots(FollowSystemType type, List<User> _users)
    {
        if (_users != null)
        {
            //���� ������ ����
            RemoveSlots(type);

            GameObject _obj = null;
            UserSlot slot;

            for (int i = 0; i < _users.Count; i++)
            {
                //���� ���� ������ �ε����� ����
                if (_users[i].id == UserData.Instance.id) continue;

                if(type==FollowSystemType.searched)
                    _obj = Instantiate(Resources.Load("Prefabs/SearchedUserSlot") as GameObject, searchedUserScrollViewObject.transform);
                else
                    _obj = Instantiate(Resources.Load("Prefabs/UserSlot") as GameObject, followUserScrollViewObject.transform);
            
                
                slot = _obj.GetComponent<UserSlot>();
                if(slot==null) Debug.Log(_users[i].id);
         
                slot.SetUser(_users[i]);
                slot.SetType(type);

                //�ȷο��ϰ� �ִ� ������� ���Կ��� �ȷο� ó���ϰ� �ȷο� ��ư�� ��Ȱ��ȭ ��Ŵ
                if (UserData.Instance.user.follow.Contains(_users[i].id))
                {
                    slot.Follow = true;
                }

                slot.OnClickAddButton += FollowUser;
                slot.OnClickDelButton += FollowCancelUser;
                slot.OnClickSlot += UserSlotClickHandler;


                if (type == FollowSystemType.searched)
                {
                    searchedUserSlots.Add(slot);
                }
                else
                {
                    followUserSlots.Add(slot);
                }

            }
            if (type == FollowSystemType.searched)
                searchedUserScrollViewObject.GetComponent<ScrollViewRect>().SetContentSize();
            else
                followUserScrollViewObject.GetComponent<ScrollViewRect>().SetContentSize();
        }
    }
    void FollowUser(UserSlot us)
    {
        //�������� �ȷο� ��ư Ŭ����
        CallFollowApi(us.user.id, us.user.nickname);
        UserData.Instance.AddFollow(us.user.id);

    }
    void FollowUser(string id, string nickname)
    {
        //�������� �ȷο� ��ư Ŭ����
        CallFollowApi(id, nickname);
        UserData.Instance.AddFollow(id);

    }
    void FollowCancelUser(UserSlot us)
    {
        //�������� �ȷο� ��� ��ư Ŭ����
        CallFollowApi(us.user.id, us.user.nickname, true);
        UserData.Instance.DelFollow(us.user.id);
    }
    void FollowCancelUser(string id, string nickname)
    {
        //�������� �ȷο� ��� ��ư Ŭ����
        CallFollowApi(id, nickname, true);
        UserData.Instance.DelFollow(id);
    }
    async void CallFollowApi(string userID, string userName, bool isDelete = false)
    {
        await POST_FollowUserAsync(userID, userName, isDelete);
        if (followBtns[1].image.color.r == 1.0f)
        {   //�ȷο� ����Ʈ�� ���̰� ���� ��
            //�ȷο� ����Ʈ ������Ʈ
            GetUserListAsync(FollowSystemType.follow);
        }

        if(currentUserId[0]==UserData.Instance.user.id)
            followText.text = UserData.Instance.user.follow.Count + "\n�ȷο�";

    }
    void UserSlotClickHandler(UserSlot us)
    {
        //���� ���� Ŭ����
        LoadUserProfile(us.user,us.Follow);
    }
    void RemoveSlots(FollowSystemType type)
    {
        if (type == FollowSystemType.uploadList)
        {//���ε� ����Ʈ
            for (int i = 0; i < uploadedSlots.Count; i++)
            {
                Destroy(uploadedSlots[i].gameObject);
            }
            uploadedSlots.Clear();
        }
        else if (type == FollowSystemType.searched)
        {//�˻� ����Ʈ
            for (int i = 0; i < searchedUserSlots.Count; i++)
            {
                Destroy(searchedUserSlots[i].gameObject);
            }
            searchedUserSlots.Clear();
        }
        else
        {
            //�ȷο�,�ȷο� ����Ʈ
            for (int i = 0; i < followUserSlots.Count; i++)
            {
                Destroy(followUserSlots[i].gameObject);
            }
            followUserSlots.Clear();
        }

    }
    override public void Load()
    {
        LoadUserProfile();//�� ������ �ε�
        followBtns[0].image.color = new Color(1f, 1f, 1f, 1f);
        followBtns[1].image.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        GetUserListAsync(FollowSystemType.follower);
    }
    override public void Reset()
    {
        currentUserId[0] = "";
        currentUserId[1] = "";
        RemoveSlots(FollowSystemType.searched);
        RemoveSlots(FollowSystemType.follow);
        RemoveSlots(FollowSystemType.uploadList);
    }
    private void OnDestroy()
    {

        //UserData.Instance.OnDeleteFollow -= SetInfoFollowTextOnDelete;
        //UserData.Instance.OnAddFollow -= SetInfoFollowTextOnAdd;
    }
}
