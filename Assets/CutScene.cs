using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CutScene : MonoBehaviour
{
    public Image imageTarget;               // Component Image UI để hiển thị ảnh
    public TextMeshProUGUI cutSceneText;

    //Cac danh sách sprite sẽ chạy
    public List<Sprite> level0;            
    public List<Sprite> level1;
    public List<Sprite> endLevel;

    public int loopCount = 3;                   // Số vòng lặp cutscene

    public float delayBetweenSprites = 2f;  // Thời gian giữa các ảnh

    public GameObject panelToShow;              // Panel sẽ hiện sau cutscene
    public float panelDuration = 3f;            // Thời gian hiện panel

    private void Start()
    {

    }

    public void StartCutscene(List<Sprite> sprites)
    {
        this.gameObject.SetActive(true);
        if (sprites == null || sprites.Count == 0 || imageTarget == null)
        {
            Debug.LogWarning("Thiếu dữ liệu cutscene!");
            return;
        }

        StartCoroutine(PlayCutscene(sprites));
    }

    private IEnumerator PlayCutscene(List<Sprite> sprites)
    {
        /*foreach (var sprite in sprites)
        {
            imageTarget.sprite = sprite;
            yield return new WaitForSeconds(delayBetweenSprites);
        }*/
        for (int loop = 0; loop < loopCount; loop++)
        {
            foreach (var sprite in sprites)
            {
                imageTarget.sprite = sprite;
                yield return new WaitForSeconds(delayBetweenSprites);
            }
        }

        // Hiện panel nếu có
        if (panelToShow != null)
        {
            panelToShow.SetActive(true);
            yield return new WaitForSeconds(panelDuration);
            panelToShow.SetActive(false); // tuỳ ý, có thể giữ nguyên
        }

        // Kết thúc cutscene, có thể ẩn hoặc load scene khác ở đây nếu muốn
        this.gameObject.SetActive(false);
        Debug.Log("Cutscene kết thúc.");
    }

    public void SetText(string s)
    {
        cutSceneText.text = s;
    }
}
