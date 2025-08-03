using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TryAgain : MonoBehaviour
{
    public RectTransform scrollViewRect;
    public RectTransform originalScrollViewPosition;
    public Button buttonTryAgain;
    public CanvasGroup canvasGroupButtonTryAgain;
    public TextMeshProUGUI resultText;
    public float fadeDuration;

    [Header("Check")]
    public Button buttonCheck;
    public CanvasGroup canvasGroupButtonCheck;
    public CanvasGroup scrollBar;


    public void OnClickTryAgain()
    {
        GameManager.Instance.StopScrolling();

        //di chuyen scrollview ve vi tri cu
        scrollViewRect.DOAnchorPos(originalScrollViewPosition.anchoredPosition, 2f).SetEase(Ease.InOutSine);

        //button bien mat
        buttonTryAgain.interactable = false;
        canvasGroupButtonTryAgain.DOFade(0, fadeDuration).OnComplete(() =>
        {
            gameObject.SetActive(false); 
        });

        //xoa result
        HideWithFadeResult();


        //hien button check, scrollbar
        //button bien mat
        buttonCheck.interactable = true; 
        canvasGroupButtonCheck.DOFade(1, fadeDuration).OnComplete(() =>
        {
            canvasGroupButtonCheck.gameObject.SetActive(true);
        });
        scrollBar.DOFade(1, fadeDuration).OnComplete(() =>
        {
            scrollBar.gameObject.SetActive(true);
        });

    }
    public void HideWithFadeResult()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(DOTween.To(() => resultText.alpha, x => resultText.alpha = x, 0f, fadeDuration));
        seq.OnComplete(() =>
        {
            resultText.gameObject.SetActive(false); //an sau khi lam mo
        });

    }
}
