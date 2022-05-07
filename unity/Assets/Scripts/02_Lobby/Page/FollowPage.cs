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
    public Button InfoFollowBtn;

    public Button[] followBtns;

    public GameObject uploadedSongScrollViewObject;
    private List<PlaySongSlot> uploadedSlots;

    public GameObject followUserScrollViewObject;
    public GameObject searchedUserScrollViewObject;


    private List<UserSlot> followUserSlots;
    private List<UserSlot> searchedUserSlots;
    private User currentUser;

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

            uploadedSlots = new List<PlaySongSlot>(uploadedSongScrollViewObject.GetComponentsInChildren<PlaySongSlot>());
            followUserSlots = new List<UserSlot>(followUserScrollViewObject.GetComponentsInChildren<UserSlot>());
            searchedUserSlots = new List<UserSlot>(searchedUserScrollViewObject.GetComponentsInChildren<UserSlot>());
            
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


        }
    }
    void LoadUserProfile(User user=null)
    {
        
        if (user == null)
        {
            
            user = UserData.Instance.user;
            InfoFollowBtn.gameObject.SetActive(false);
        }
        if (currentUser == user)
            return;
        currentUser = user;
        userNameText.text = currentUser.GetName();
        followText.text = currentUser.followNum + "\n�ȷο�";
        followerText.text = currentUser.followerNum + "\n�ȷο�";
        musicCntText.text = "\n�ø� ����";

        character.ChangeSprite(currentUser.character);
        GetuploadedMusicListAsync(currentUser == UserData.Instance.user?null: currentUser.id);//�����̸� null, ������ �ƴϸ� id

    }

    async void GetuploadedMusicListAsync(string userid)
    {
        MusicList ML = await GET_MusicListAsync("uploadList",false,userid);
        if (ML != null)
        {
            LoadUploadedSlots( ML.musicList);
        }
    }
    void LoadUploadedSlots(List<Music> _musics)
    {//���ε��� ��������Ʈ�� ǥ���ϰ� �ȷο�, �ȷο�, �ø����� �� ������Ʈ

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

    void LoadUserSlots(FollowSystemType type, List<User> _users)
    {
        if (_users != null)
        {
            RemoveSlots(type);

            GameObject _obj = null;
            UserSlot slot;
            Debug.Log(_users.Count);
            for (int i = 0; i < _users.Count; i++)
            {
                if(type==FollowSystemType.searched)
                    _obj = Instantiate(Resources.Load("Prefabs/SearchedUserSlot") as GameObject, searchedUserScrollViewObject.transform);
                else
                    _obj = Instantiate(Resources.Load("Prefabs/UserSlot") as GameObject, followUserScrollViewObject.transform);
            
                
                slot = _obj.GetComponent<UserSlot>();
                if(slot==null) Debug.Log(_users[i].id);
         
                slot.SetUser(_users[i]);
                slot.SetType(type);
                if (UserData.Instance.user.followIds.Contains(_users[i].id))
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
        UserData.Instance.user.AddFollow(us.user.id);

    }
    void FollowCancelUser(UserSlot us)
    {
        //�������� �ȷο� ��� ��ư Ŭ����
        CallFollowApi(us.user.id,  us.user.nickname,true);
        UserData.Instance.user.DelFollow(us.user.id);

    }
    async void CallFollowApi(string userID, string userName, bool isDelete = false)
    {
        await POST_FollowUserAsync(userID, userName, isDelete);
        if (followBtns[1].image.color.r == 1.0f)
        {   //�ȷο� ����Ʈ�� ���̰� ���� ��
            //�ȷο� ����Ʈ ������Ʈ
            GetUserListAsync(FollowSystemType.follow);
        }
        followText.text = UserData.Instance.user.followIds.Count + "\n�ȷο�";
    }
    void UserSlotClickHandler(UserSlot us)
    {
        //���� ���� Ŭ����
        LoadUserProfile(us.user);
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
        currentUser = null;
        RemoveSlots(FollowSystemType.searched);
        RemoveSlots(FollowSystemType.follow);
        RemoveSlots(FollowSystemType.uploadList);
    }
}
