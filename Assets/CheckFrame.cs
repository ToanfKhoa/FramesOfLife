using System.Collections.Generic;
using UnityEngine;

public class CheckFrame: MonoBehaviour
{
    public void OnPlayButtonClick()
    {
        GameManager.Instance.StartScrolling(); //chay frame

        Transform content = GameManager.Instance.contentParent;
        int count = content.childCount;

        if (count < 3) return;

        List<int> indexes = new List<int>();
        for (int i = 1; i < count - 1; i++)
        {
            var frame = content.GetChild(i).GetComponent<DraggableFrame>();
            indexes.Add(frame.correctIndex);
        }

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


}
