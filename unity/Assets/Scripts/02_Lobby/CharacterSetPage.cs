using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CharacterSetPage : Page
{

    public Transform sampleObj;
    public Button setBtn;

    [SerializeField]
    private int partsCount;
    [SerializeField]
    private int[] partsIdxs;
    private List<Image> characterPartsImages;
    private List<CharacterPartsSlot> characterPartsSlots;


    public delegate void CharacterHandler();
    public event CharacterHandler OnChangeCharacter;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }
    void SetCharacter()
    {
        //character�ѹ��� ��ȯ
        int tmp = 0;
        for (int i = 0; i < partsCount; i++)
        {
             tmp += partsIdxs[i] * (int)Mathf.Pow(10, (i) * 2);
        }
        UserData.Instance.user.character = tmp;
        OnChangeCharacter();
        //DB�� �ٲ� char �� ���ε�
        //
        //

        Close();

    }
    public override void Init()
    {
        if (isAlreadyInit==false)
        {//�ʱ� ������ �ȵǾ��ִٸ�
            partsCount = sampleObj.childCount;
            partsIdxs = new int[partsCount];

            characterPartsImages = new List<Image>();
            for (int i = 0; i < partsCount; i++)
            {
                characterPartsImages.Add(sampleObj.GetChild(i).gameObject.GetComponent<Image>());
            }

            characterPartsSlots = new List<CharacterPartsSlot>(GetComponentsInChildren<CharacterPartsSlot>());
            for (int i = 0; i < partsCount; i++)
            {
                characterPartsSlots[i].SetNum(i);
                characterPartsSlots[i].OnChangeSlotImage += LoadParts;
            }
            setBtn.onClick.AddListener(SetCharacter);
            isAlreadyInit = true;
        }
    }
    override public void Load()
    {//ĳ���� �ε�
        Init();

        int tmp = UserData.Instance.user.character;
        for(int i=0; i < partsCount; i++)
        {
            partsIdxs[i] = (tmp % (int)Mathf.Pow(10, (i+1)*2))/ (int)Mathf.Pow(10, i*2);
            characterPartsSlots[i].cur = partsIdxs[i];
        }
    }
    void LoadParts(int partsNum, int idx, Sprite sprite)
    {
        characterPartsImages[partsNum].sprite = sprite;
        partsIdxs[partsNum] = idx;
    }
}
