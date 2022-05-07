using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI�� ���õ� ��ũ��Ʈ �۾��� ���ؼ� �߰��� �־�� �Ѵ�. 


public class ScrollViewRect : MonoBehaviour
{
    // ��ũ�� ��� ���õ� ������ �ϱ� ���� ������ �ִ� ���� 
    RectTransform rect;
    public bool isGrid;

    void Awake () {

        rect = GetComponent<RectTransform>();
        SetContentSize();
    } 

    public void SetContentSize(float height = 30) {

        int cnt = transform.childCount;

        if (gameObject.GetComponent<GridLayoutGroup>() is GridLayoutGroup gl)
        {
            height += gl.cellSize.y * ((int)(cnt/4)+(cnt%4 !=0 ?1 :0)) + gl.spacing.y*((int)(cnt / 4) + (cnt % 4 != 0 ? 1 : 0));
        }
        else
        {
            for (int i = 0; i < cnt; i++)
            {
                height += transform.GetChild(i).gameObject.GetComponent<RectTransform>().sizeDelta.y;
               
            }
        }

        // scrollRect.content�� ���ؼ� Hierachy �信�� �ô� Viewport ���� Content ���� ������Ʈ�� ������ �� �ִ�. 
        // �׸��� sizeDelta ���� ���ؼ� Content�� ���̿� ���̸� ������ �� �ִ�. 

        rect.sizeDelta = new Vector2(rect.sizeDelta.x,height);
    }


}
