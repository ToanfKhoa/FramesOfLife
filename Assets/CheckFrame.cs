using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CheckFrame: MonoBehaviour
{
    public GameObject frameAnimPrefab;
    public RectTransform contentParentAnim;
    public TextMeshProUGUI resultText;

    public Button button;
    public float fadeDuration = 1f;
    public float fadeDurationResult = 0f;

    public CanvasGroup canvasGroup; //ho tro fade cho button
    public CanvasGroup scrollBar;
    public RectTransform scrollViewRect; // Kéo ScrollRect vào Inspector

    public GameObject tryAgainButton;
    public CanvasGroup tryAgainCanvasGroup;

    private List<string> correctMessages = new List<string>
    {
        "Perfect!",
        "Well done!",
        "That’s it!",
        "Correct order!",
        "Nice work!",
        "You nailed it!"
    };

    private List<string> incorrectMessages = new List<string>
    {
        "Oops! Try again!",
        "Wrong order!",
        "Almost there!",
        "Hmm... not quite.",
        "Keep trying!",
        "Not correct yet."
    };
    public void OnPlayButtonClick()
    {
        //di chuyen tap trung vao frame
        Vector3 middlePosition = new Vector3(scrollViewRect.anchoredPosition.x, 0f, 0f);
        scrollViewRect.DOAnchorPos(middlePosition, fadeDuration*2).SetEase(Ease.InOutSine);
        //button bien mat
        button.interactable = false; // không cho bấm
        canvasGroup.DOFade(0, fadeDuration).OnComplete(() =>
        {
            gameObject.SetActive(false); // ẩn luôn sau fade
        });
        scrollBar.DOFade(0, fadeDuration).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
        //hien result
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, fadeDuration);


        GameManager.Instance.StartScrolling(); //chay frame

        Transform content = GameManager.Instance.contentParent;
        int count = content.childCount;

        if (count < 3) return;

        List<int> indexes = new List<int>();
        List<Sprite> spriteList = new List<Sprite>();

        for (int i = 1; i < count; i++)
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
        // Cập nhật kết quả vào Text
        if (IsCircularSortedOptimized(indexes))
        {
            resultText.text = GetRandomMessage(correctMessages);
            ShowWithFadeCorrect();
        }
        else
        {
            resultText.text = GetRandomMessage(incorrectMessages);
            ShowWithFadeInCorrect();
        }
    }
    public void ShowWithFadeCorrect()
    {
        resultText.gameObject.SetActive(true);
        resultText.alpha = 0f;

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(fadeDurationResult); // chờ một khoảng thời gian
        seq.Append( DOTween.To(() => resultText.alpha, x => resultText.alpha = x, 1f, fadeDuration) );

        seq.AppendInterval(5f); // giữ lại trạng thái hiển thị một lúc (tùy chỉnh thời gian)
        seq.AppendCallback(() =>
        {
            GameManager.Instance.GoToNextLevel();
            //dung click try again lam set lai bien isstop gay loi
            tryAgainButton.GetComponent<TryAgain>().OnClickTryAgain();
            GameManager.Instance.SetIsStop(false);
        });
    }
    public void ShowWithFadeInCorrect()
    {
        resultText.gameObject.SetActive(true);
        resultText.alpha = 0f;

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(fadeDurationResult); // chờ một khoảng thời gian
        seq.Append(DOTween.To(() => resultText.alpha, x => resultText.alpha = x, 1f, fadeDuration));

        //tryagain button
        tryAgainButton.SetActive(true); // Hiện đối tượng
        tryAgainButton.GetComponent<Button>().interactable = true;
        tryAgainCanvasGroup.alpha = 0f; // Đảm bảo bắt đầu từ 0
        seq.Append(tryAgainCanvasGroup.DOFade(1f, 0.5f));

    }
    private string GetRandomMessage(List<string> messages)
    {
        int rand = UnityEngine.Random.Range(0, messages.Count);
        return messages[rand];
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
            Debug.Log("CreateSimpleFrameAnimation-frame: ");
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

    public void ResetCheckButton()
    {
        gameObject.SetActive(true); // bật lên trước khi fade

        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, fadeDuration).OnComplete(() =>
        {
            button.interactable = true; // cho bấm sau khi đã hiện xong
        });

        scrollBar.alpha = 0;
        scrollBar.DOFade(1, fadeDuration);
    }
}
