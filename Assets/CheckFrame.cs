using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CheckFrame: MonoBehaviour
{
    public GameObject frameAnimPrefab;
    public RectTransform contentParentAnim;

    public void OnPlayButtonClick()
    {
        GameManager.Instance.StartScrolling(); //chay frame

        Transform content = GameManager.Instance.contentParent;
        int count = content.childCount;

        if (count < 3) return;

        List<int> indexes = new List<int>();
        List<Sprite> spriteList = new List<Sprite>();

        for (int i = 1; i < count - 1; i++)
        {
            var frame = content.GetChild(i).GetComponent<DraggableFrame>();
            indexes.Add(frame.correctIndex);

            //tong hop anh da xep thanh mang de tao animation
            spriteList.Add(content.GetChild(i).GetComponent<DraggableFrame>().GetSprite());
        }

        //tao 6 anim dua vao scrollview anim
        for (int i=1; i<=5; i++)
        {
            Sprite[] shifted = RotateSprites(spriteList.ToArray(), i);
            CreateSimpleFrameAnimation(shifted);
        }    

        //check
        if (IsCircularSortedOptimized(indexes))
            Debug.Log("Hợp lệ");
        else
            Debug.Log("Không hợp lệ");
    }

    private bool IsCircularSortedOptimized(List<int> list)
    {
        int incBreak = 0;
        int decBreak = 0;
        int n = list.Count;

        for (int i = 0; i < n; i++)
        {
            int curr = list[i];
            int next = list[(i + 1) % n];

            if (curr > next) incBreak++;
            if (curr < next) decBreak++;

            if (incBreak > 1 && decBreak > 1) return false; // sớm dừng
        }

        return incBreak <= 1 || decBreak <= 1;
    }

    private void CreateSimpleFrameAnimation(Sprite[] sprites)
    {
        GameObject obj = Instantiate(frameAnimPrefab, contentParentAnim);

        // Lấy image từ con của prefab
        Image img = null;
        foreach (Transform child in obj.transform)
        {
            img = child.GetComponent<Image>();
            if (img != null) break;
        }

        obj.AddComponent<SimpleSpriteAnimator>().StartAnimation(img, sprites, 12f);
    }
    private Sprite[] RotateSprites(Sprite[] original, int offset)
    {
        int len = original.Length;
        Sprite[] rotated = new Sprite[len];
        for (int i = 0; i < len; i++)
        {
            rotated[i] = original[(i + offset) % len];
        }
        return rotated;
    }
}
