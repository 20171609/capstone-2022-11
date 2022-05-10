using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class User
{
    public string id;
    public string password;
    public string email;
    public string nickname;
    public int character;
    public int followNum;
    public int followerNum;
    public List<string> preferredGenres=new List<string>();
    public List<string> follow = new List<string>();


    public void SetUser(string id,string email,string nickname,int character, List<string> preferredGenres=null, List<string> followIds = null)
    {
        this.id = id;
        this.email = email;
        this.nickname = nickname;
        this.character = character;
        this.preferredGenres = preferredGenres;
        this.follow = followIds;


        if (preferredGenres == null)
            this.preferredGenres = new List<string>();
        if (followIds == null)
            this.follow = new List<string>();
        
    }

    public void Clear()
    {
        this.id = "";
        this.email = "";
        this.nickname = "";
        this.character = 0;
        this.preferredGenres.Clear();
        this.follow.Clear();
    }
    public string GetName()
    {
        return nickname+"("+id+")";
    }
}
public class UserData : Singleton<UserData>
{

    public User user;
    public string Token;

    public delegate void FollowHandler();
    public event FollowHandler OnChangeFollow;

    public delegate void FollowDeleteHandler(string id);
    public event FollowDeleteHandler OnDeleteFollow;
    public string id
    {
        get { return user.id; }    // _data ��ȯ
        set { user.id = value; }   // value Ű���� ���
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    public void AddFollow(string id)
    {
        user.follow.Add(id);
        user.followNum++;
        OnChangeFollow?.Invoke();
    }
    public void DelFollow(string id)
    {
        user.follow.Remove(id);
        user.followNum--;
        OnChangeFollow?.Invoke();
        OnDeleteFollow?.Invoke(id);

    }
    public void Clear()
    {
        Token = "";
        user.Clear();
    }
}
