using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SimpleSpriteAnimator : MonoBehaviour
{
    private Image targetImage;
    private Sprite[] frames;
    private float frameRate;

    public void StartAnimation(Image image, Sprite[] sprites, float rate)
    {
        targetImage = image;
        frames = sprites;
        frameRate = rate;
        StartCoroutine(PlayAnimation());
    }

    private IEnumerator PlayAnimation()
    {
        int index = 0;
        while (true)
        {
            targetImage.sprite = frames[index];
            index = (index + 1) % frames.Length;
            yield return new WaitForSeconds(1f / frameRate);
        }
    }
}
