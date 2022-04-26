using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
public class MusicController : MusicWebRequest
{
    private static MusicController instance;
    public static MusicController Instance
    {
        get
        {
            return GetInstance();
        }
    }
    public static MusicController GetInstance()
    {
        if (instance == null)
        {
            instance = (MusicController)FindObjectOfType(typeof(MusicController));

        }
        return instance;
    }

    private AudioSource audioSource;
    public SubMusicController SubMusicController;



    public Button openBtn;
    public Image[] images;
    public Button[] pauseplayBtns;
    public Button[] prevBtns;
    public Button[] nextBtns;

    public TextMeshProUGUI[] titleTexts;
    public TextMeshProUGUI[] artistTexts;

    public CustomSlider slider;

    public Toggle randomToggle;
    public Button repeatBtn;
    public TextMeshProUGUI contentText;

    private Animator animator;
    private AudioClip audioClip;

    private bool isAlreadyInit = false;
    private bool isRandomMode = false;
    private RepeatMode repeatMode;
    
    //public  List<Music> musicList;

    
    private int currentSongListIndex;
    private int currentSongIndex = 0;
    private int pastSongIndex = 0;

    private Image[] pauseplayBtnImage;
    private Image repeatBtnImage;

    private IEnumerator enumerator;
    private bool isCurrentSongFinish=false;
    PlayState playState;

    //
    //
    //
    //
    //
    //

    public Button listBtn;
    public Button lyricsBtn;
    public Button contentBtn;

    public string currentListName;
    private List<SongSlot> currentSongSlotList;
    public GameObject scrollViewObject;
    private ScrollViewRect scrollViewRect;

    private GraphicRaycaster gr;

    private enum PlayState
    {
        Play,Pause
    }
    enum RepeatMode
    {
        None,OneRepeat,AllRepeat
    }
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    public void Stop()
    {
        if (audioSource.clip != null)
            audioSource.Pause();
    }
    private void Update()
    {

        if (audioSource.clip != null)
        {
            
            if (audioSource.isPlaying == true && isCurrentSongFinish == false)
            {//��������� ��
                if ((int)audioSource.time == (int)audioClip.length)
                {//����� ������
                    
                    
                    if (RepeatMode.OneRepeat != repeatMode)
                    {
                        Debug.Log("�ڿ� ��� ��");
                        isCurrentSongFinish = true;
                        AutoPlayNextMusic();
                    }

                }
            }

            if (audioSource.isPlaying == true && playState == PlayState.Pause)
            {
                //Debug.Log("Play");
                playState = PlayState.Play;

                StartCoroutine(enumerator);

                for (int i = 0; i < 2; i++)
                {
                    pauseplayBtnImage[i].sprite = Resources.Load<Sprite>("Image/UI/pause");
                }

            }
            else if(audioSource.isPlaying ==false && playState == PlayState.Play)
            {
                //Debug.Log("Pause");
                playState = PlayState.Pause;
                StopCoroutine(enumerator);
                for (int i = 0; i < 2; i++)
                {
                    pauseplayBtnImage[i].sprite = Resources.Load<Sprite>("Image/UI/play");
                }
            }


            if (Input.GetMouseButtonDown(0))
            {
                
                var ped = new PointerEventData(null);
                ped.position = Input.mousePosition;
                List<RaycastResult> results = new List<RaycastResult>();
                gr.Raycast(ped, results);

                if (results.Count <= 0) return;
                // �̺�Ʈ ó���κ�
                if (results[0].gameObject.name != "SongSlot2(Clone)") return;
                
                SongSlot ss = results[0].gameObject.GetComponent<SongSlot>();

                int newIndex = currentSongSlotList.IndexOf(ss);

                if (newIndex == currentSongIndex) return;//���� ������� �����̸� �н�

                pastSongIndex = currentSongIndex;
                currentSongIndex = newIndex;

                currentSongSlotList[pastSongIndex].SetImage(new Color(1f, 1f, 1f));
                currentSongSlotList[currentSongIndex].SetImage(new Color(0.8f, 0.8f, 0.8f));
                StartCoroutine(GetAudioCilpUsingWebRequest(currentSongSlotList[currentSongIndex].GetMusic().locate, true));

            }

        }

        
    }
    private void LoadInfo(string type)
    {
        if (type == "lyrics")
        {
            animator.SetBool("isContentOpen", true);
            animator.SetTrigger("OpenContent");
            contentText.text = currentSongSlotList[currentSongIndex].GetMusic().lyrics;
        }
        else if(type == "info")
        {
            animator.SetBool("isContentOpen", true);
            animator.SetTrigger("OpenContent");
            contentText.text = currentSongSlotList[currentSongIndex].GetMusic().info;
        }
    }
    public void SetSongList(List<Music> _musics = null,bool play=false)
    {

        Transform[] childList = scrollViewObject.GetComponentsInChildren<Transform>();
        if (childList != null)
        {
            for (int i = 1; i < childList.Length; i++)
            {
                if (childList[i] != transform)
                {
                    Destroy(childList[i].gameObject);
                }
            }
        }
        currentSongSlotList.Clear();


        if (_musics != null)
        {
            SongSlot ss;
            GameObject _obj = null;
            for (int i=0; i< _musics.Count; i++)
            {

                _obj = Instantiate(Resources.Load("Prefabs/SongSlot/SongSlot2") as GameObject, scrollViewObject.transform);
                ss = _obj.GetComponent<SongSlot>();
                ss.SetMusic(_musics[i]);

                currentSongSlotList.Add(ss);
            }
            scrollViewRect.SetContentSize(100);
            StartCoroutine(GetAudioCilpUsingWebRequest(currentSongSlotList[currentSongIndex].GetMusic().locate, play));
        }
    }

    void Init()
    {
        if (isAlreadyInit == false)
        {
            isAlreadyInit = true;

            //info ������ ������Ʈ
            scrollViewRect = scrollViewObject.GetComponent<ScrollViewRect>();
            currentSongSlotList = new List<SongSlot>();
            gr = GetComponent<GraphicRaycaster>();

            //��
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            audioSource.loop = false;

            pauseplayBtnImage = new Image[2];

            int[] num = { 0, 1 };
            for (int i = 0; i < num.Length; i++)
            {
                pauseplayBtns[i].onClick.AddListener(delegate {
                    if (audioSource.clip != null) {
                        ChangeState(!audioSource.isPlaying);
                    }
                });
                pauseplayBtnImage[i] = (pauseplayBtns[i].gameObject).GetComponent<Image>();


                prevBtns[i].onClick.AddListener(ClickPrevButton);
                nextBtns[i].onClick.AddListener(ClickNextButton);

            }
            repeatBtnImage = (repeatBtn.gameObject).GetComponent<Image>();
            repeatBtn.onClick.AddListener(SetRepeatMode);


            slider.OnPointUp += OnValueChange;
            slider.OnPointDown += StopSlider;

            openBtn.onClick.AddListener(OpenCloseInfo);

            lyricsBtn.onClick.AddListener(delegate { LoadInfo("lyrics"); });
            contentBtn.onClick.AddListener(delegate { LoadInfo("info"); });
            listBtn.onClick.AddListener(delegate { animator.SetBool("isContentOpen",false); });

            playState = PlayState.Pause;
            enumerator = MoveSlider();

            //������
            OnGetClip += SetAudioClip;
            OnGetSongList += SetSongList;


            StartCoroutine(GET_MusicList("myList", UserData.Instance.id, false));
        }
    }

    void SetRepeatMode()
    {
        repeatMode = (RepeatMode)(((int)repeatMode + 1) % 3);
        switch (repeatMode)
        {
            case RepeatMode.None:

                break;
            case RepeatMode.OneRepeat:
                audioSource.loop = true;
                break;
            case RepeatMode.AllRepeat:
                audioSource.loop = false;
                break;
        }
        repeatBtnImage.sprite = Resources.Load<Sprite>("Image/UI/repeat" + (int)repeatMode);

    }
    void ClickPrevButton()
    {
        pastSongIndex = currentSongIndex;
        if (randomToggle.isOn == true)
        {
            //���� �̱�
            currentSongIndex = PickRandomIndex();
        }
        else
        {
            currentSongIndex = (currentSongIndex - 1) % currentSongSlotList.Count;
        }

        Debug.Log("currentIndex" + currentSongIndex);
        StartCoroutine(GetAudioCilpUsingWebRequest(currentSongSlotList[currentSongIndex].GetMusic().locate, true));
    }
    void ClickNextButton()
    {
        pastSongIndex = currentSongIndex;
        if (randomToggle.isOn == true)
        {
            //���� �̱�
            currentSongIndex = PickRandomIndex();
        }
        else
        {
            currentSongIndex = (currentSongIndex + 1) % currentSongSlotList.Count;
        }
        Debug.Log("currentIndex" + currentSongIndex);
        //���
        StartCoroutine(GetAudioCilpUsingWebRequest(currentSongSlotList[currentSongIndex].GetMusic().locate, true));

    }
    int PickRandomIndex()
    {
        int randomIdx = currentSongIndex;

        while (randomIdx == currentSongIndex)
        {
            System.Random rand = new System.Random();
            randomIdx = rand.Next(0, currentSongSlotList.Count);

        }
        return randomIdx;
    }
    void AutoPlayNextMusic()
    {
        int nextIdx = PickRandomIndex();

        if (randomToggle.isOn==false)
        {//������尡 �ƴ϶��
            nextIdx = (currentSongIndex + 1) % currentSongSlotList.Count;
        }

        pastSongIndex = currentSongIndex;
        currentSongIndex = nextIdx;
        Debug.Log("Autoplay" + nextIdx);
        if (repeatMode == RepeatMode.None)
        {
            if (currentSongIndex == 0)
            {
                //�������� ���� �����Ͽ� ��� �����ϰ� �Ǿ� �������� �̵�
                if (currentSongSlotList != null)
                {
                    StartCoroutine(GetAudioCilpUsingWebRequest(currentSongSlotList[0].GetMusic().locate, false));
                    return;
                }
            }
        }
        StartCoroutine(GetAudioCilpUsingWebRequest(currentSongSlotList[currentSongIndex].GetMusic().locate, true));

    }
    void OpenCloseInfo()
    {
        animator.SetBool("isOpen", !animator.GetBool("isOpen"));
        if (animator.GetBool("isOpen")==false)
        {
            animator.SetBool("isContentOpen", false);
        }

    }


    public void SetAudioClip(AudioClip ac, bool play)
    {//OnGetClip �����ʰ� ȣ��Ǹ� �Լ� ����
        Debug.Log("����� ��ü");
        audioSource.Stop();
        audioSource.time = 0;
        audioClip = ac;
        audioSource.clip = audioClip;

        slider.value = 0;
        isCurrentSongFinish = false;


        ChangeState(play);


        currentSongSlotList[pastSongIndex].SetImage(new Color(1f, 1f, 1f));
        currentSongSlotList[currentSongIndex].SetImage(new Color(0.8f, 0.8f, 0.8f));
        Music music = currentSongSlotList[currentSongIndex].GetMusic();

        LoadImage(music.imageLocate);

        for (int i=0; i<2; i++)
        {
            titleTexts[i].text = music.title;
            artistTexts[i].text = music.GetArtistName();
        }
    }

    void OnValueChange(float value)
    {
        if (audioSource == null) return;

        audioSource.time = Mathf.Max(Mathf.Min(audioClip.length *value, audioClip.length), 0);

        if (audioSource.isPlaying == true)
            StartCoroutine(enumerator);
    }
    void StopSlider(float value)
    {
        if (audioSource == null) return;

        if (audioSource.isPlaying == true)
        {
            Debug.Log("stop!!!!!");
            StopCoroutine(enumerator);
        }
    }
    void ChangeState(bool isPlay)
    {
        if (audioSource.clip != null)
        {           
            if (isPlay==false && audioSource.isPlaying ==true)
            {
                audioSource.Pause();
            }
            else if(isPlay==true && audioSource.isPlaying==false)
            {
                audioSource.Play();
            }
        }
    }
    IEnumerator MoveSlider()
    {
        while (audioSource.isPlaying)
        {
            slider.value = audioSource.time / audioClip.length;
            yield return new WaitForSeconds(0.1f);
        }

    }
    private void LoadImage( string filePath)
    {
        if (filePath == null || filePath == "")
        {
            for(int i=0; i<images.Length; i++)
                images[i].sprite = Resources.Load<Sprite>("Image/none");
        }
        else
        {
            StartCoroutine(GetTexture(filePath));
        }
    }

    IEnumerator GetTexture(string _path)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture("https://"+_path);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            for (int i = 0; i < images.Length; i++)
                images[i].sprite = Resources.Load<Sprite>("Image/none");
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            Rect rect = new Rect(0, 0, texture.width, texture.height);
            for (int i = 0; i < images.Length; i++)
                images[i].sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
        }
    }

    
}
