using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MusicController : MonoBehaviour
{
    private AudioSource audioSource;

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
    
    public  List<Music> musicList;

    private int currentMusicList;
    private int currentMusicIndex = 0;
    public ListPageInSongPage ll;

    private Image[] pauseplayBtnImage;
    private Image repeatBtnImage;

    private IEnumerator enumerator;
    private bool isCurrentSongFinish=false;
    PlayState playState;
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //�׽�Ʈ�� �ڵ�
            musicList = ll.musicList;
            MusicWebRequest.Instance.GetAudioClip(musicList[currentMusicIndex].locate,true);
            //MusicWebRequest.Instance.GetMusicList("musicList",UserData.Instance.id);
        }
        
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
                Debug.Log("Play");
                playState = PlayState.Play;

                StartCoroutine(enumerator);

                for (int i = 0; i < 2; i++)
                {
                    pauseplayBtnImage[i].sprite = Resources.Load<Sprite>("Image/UI/pause");
                }

            }
            else if(audioSource.isPlaying ==false && playState == PlayState.Play)
            {
                Debug.Log("Pause");
                playState = PlayState.Pause;
                StopCoroutine(enumerator);
                for (int i = 0; i < 2; i++)
                {
                    pauseplayBtnImage[i].sprite = Resources.Load<Sprite>("Image/UI/play");
                }
            }

        }

        
    }
    void Init()
    {
        if (isAlreadyInit == false)
        {
            isAlreadyInit = true;
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
            MusicWebRequest.Instance.OnGetClip += SetAudioClip;
            MusicWebRequest.Instance.OnGetInitMusicList += SetMusicList;

            playState = PlayState.Pause;
            enumerator = MoveSlider();
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
        if (randomToggle.isOn == true)
        {
            //���� �̱�
            currentMusicIndex = PickRandomIndex();
        }
        else
        {
            currentMusicIndex = (currentMusicIndex - 1) % musicList.Count;
        }
        Debug.Log("currentIndex" + currentMusicIndex);
        MusicWebRequest.Instance.GetAudioClip(musicList[currentMusicIndex].locate,true);
    }
    void ClickNextButton()
    {
        if (randomToggle.isOn == true)
        {
            //���� �̱�
            currentMusicIndex = PickRandomIndex();
        }
        else
        {
            currentMusicIndex = (currentMusicIndex + 1) % musicList.Count;
        }
        Debug.Log("currentIndex" + currentMusicIndex);
        //���
        MusicWebRequest.Instance.GetAudioClip(musicList[currentMusicIndex].locate, true);
        
    }
    int PickRandomIndex()
    {
        int randomIdx = currentMusicIndex;

        while (randomIdx == currentMusicIndex)
        {
            System.Random rand = new System.Random();
            randomIdx = rand.Next(0, musicList.Count);

        }
        return randomIdx;
    }
    void AutoPlayNextMusic()
    {
        int nextIdx = PickRandomIndex();

        if (randomToggle.isOn==false)
        {//������尡 �ƴ϶��
            nextIdx = (currentMusicIndex + 1) % musicList.Count;
        }

        currentMusicIndex = nextIdx;
        Debug.Log("Autoplay" + nextIdx);
        if (repeatMode == RepeatMode.None)
        {
            if (currentMusicIndex == 0)
            {
                //�������� ���� �����Ͽ� ��� �����ϰ� �Ǿ� �������� �̵�
                if (musicList != null)
                {
                    MusicWebRequest.Instance.GetAudioClip(musicList[0].locate,false);
                    return;
                }
            }
        }
        MusicWebRequest.Instance.GetAudioClip(musicList[currentMusicIndex].locate, true);
        
    }
    void OpenCloseInfo()
    {

        animator.SetBool("isOpen", !animator.GetBool("isOpen"));
    }
    public void SetMusicList(List<Music> _musics = null)
    {
        musicList = _musics;
        if (musicList != null)
        {
            MusicWebRequest.Instance.GetAudioClip(musicList[0].locate, true);
        }
    }

    public void SetAudioClip(AudioClip ac, bool play)
    {//OnGetClip �����ʰ� ȣ��Ǹ� �Լ� ����
        audioSource.Stop();
        audioSource.time = 0;
        audioClip = ac;
        audioSource.clip = audioClip;

        slider.value = 0;
        isCurrentSongFinish = false;


        ChangeState(play);
        

        for(int i=0; i<2; i++)
        {
            titleTexts[i].text = musicList[currentMusicIndex].title;
            artistTexts[i].text = musicList[currentMusicIndex].nickname+ "("+musicList[currentMusicIndex].userID+")";
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
}
