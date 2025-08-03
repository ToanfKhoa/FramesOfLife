using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Level Setup")]
    public LevelData[] levels;
    public LevelData currentLevel;
    public GameObject framePrefab;
    public RectTransform contentParent;

    public float speed = 0;
    public float frameWidth = 200f;

    private List<RectTransform> frames = new List<RectTransform>();

    private bool isRunning = false;
    private float currentSpeed = 0f;
    public float maxSpeed = 200f;
    public float acceleration = 50f;

    [Header("ScrollViewAnim")]
    public RectTransform contentParentAnim;
    public GameObject scrollViewAnimObj;
    public CanvasGroup scrollViewAnim;
    public float moveRange = 0;

    public float fadeDuration = 0f; // thời gian hoàn thành fade
    private float fadeTimer = 0f;

    public float rangeLerpDuration = 0f; //thoi gian giam dan bien do lac lu cua anim
    private float rangeLerpTime = 0f;

    public CanvasGroup blackPanel;

    public CutScene cutScene;

    private float offset = 0;
    private float currentRange = 0;
    private bool isStop  = false;
    private Vector2 initialPosition;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); 
    }

    private void Start()
    {
        SoundManager.Instance.PlayMainTheme();

        initialPosition = contentParent.anchoredPosition;

        if (scrollViewAnim != null)
            scrollViewAnim.alpha = 0f;

        LoadLevel(currentLevel);

    }

    private void Update()
    {
        Debug.Log("isstop: " + isStop);
        if (!isRunning)
        {
            Debug.Log("isRunning false");
            return;
        }

        // Tăng tốc dần cho đến khi đạt maxSpeed
        if (currentSpeed < maxSpeed && !isStop)
        {
            currentSpeed += acceleration * Time.deltaTime;
            if (currentSpeed > maxSpeed)
            {
                currentSpeed = maxSpeed;
            }
                
        }
        else
        if(isStop)
        {
            currentSpeed -= acceleration * Time.deltaTime;
            if (currentSpeed < 0)
            {
                currentSpeed = 0;

                //reset
                //ResetScrollingState(); tam
            }
        }

        //hien anim khi max speed
        if(currentSpeed >= maxSpeed && !isStop)
        {
            //hien len anim
            if (scrollViewAnim != null)
            {
                fadeTimer += Time.deltaTime;
                float t = Mathf.Clamp01(fadeTimer / fadeDuration);
                scrollViewAnim.alpha = Mathf.Lerp(0f, 0.8f, t);

                //giam dan hieu ung lac lu cho anim
                rangeLerpTime += Time.deltaTime;
                float v = Mathf.Clamp01(rangeLerpTime / rangeLerpDuration);
                currentRange = Mathf.Lerp(moveRange, 1f, v);
            }
            //giut giut contentanim
            offset = Mathf.PingPong(Time.time * 1000, currentRange) - currentRange / 2f;
            contentParentAnim.anchoredPosition = new Vector2(initialPosition.x + offset, initialPosition.y);
        }
        else
        if(isStop)
        {
            //lam mat anim
            if (scrollViewAnim != null)
            {
                fadeTimer += Time.deltaTime;
                float t = Mathf.Clamp01(fadeTimer / fadeDuration);
                scrollViewAnim.alpha = Mathf.Lerp(0.8f, 0f, t);

                //tang dan hieu ung lac lu cho anim
                rangeLerpTime += Time.deltaTime;
                float v = Mathf.Clamp01(rangeLerpTime / rangeLerpDuration);
                currentRange = Mathf.Lerp(1, moveRange, v);
            }
            //giut giut contentanim
            offset = Mathf.PingPong(Time.time * 1000, currentRange) - currentRange / 2f;
            contentParentAnim.anchoredPosition = new Vector2(initialPosition.x + offset, initialPosition.y);
        }


        /*foreach (RectTransform frame in frames)
        {
            frame.anchoredPosition += Vector2.left * currentSpeed * Time.deltaTime;

            if (frame.anchoredPosition.x < -frameWidth / 2)
            {
                float maxX = GetRightmostX();
                frame.anchoredPosition = new Vector2(maxX + frameWidth, frame.anchoredPosition.y);
            }
        }*/
        for (int i = frames.Count - 1; i >= 0; i--)
        {
            RectTransform frame = frames[i];
            if (frame == null)
            {
                Debug.Log("Frame null");
                frames.RemoveAt(i);
                continue;
            }
            Debug.Log("chay chay");
            frame.anchoredPosition += Vector2.left * currentSpeed * Time.deltaTime;

            if (frame.anchoredPosition.x < -frameWidth / 2)
            {
                float maxX = GetRightmostX();
                frame.anchoredPosition = new Vector2(maxX + frameWidth, frame.anchoredPosition.y);
            }
        }
        Debug.Log("update");
    }

    private float GetRightmostX()
    {
        float max = float.MinValue;
        foreach (RectTransform frame in frames)
            if (frame.anchoredPosition.x > max)
                max = frame.anchoredPosition.x;
        return max;
    }

    public void LoadLevel(LevelData data)
    {
        // Tạo danh sách chứa sprite + index ban đầu
        List<(Sprite sprite, int correctIndex)> spriteList = new List<(Sprite, int)>();
        for (int i = 0; i < data.sprites.Length; i++)
        {
            spriteList.Add((data.sprites[i], i));
        }

        // Xáo trộn danh sách
        Shuffle(spriteList);

        // Tạo frame với sprite và correctIndex tương ứng
        foreach (var entry in spriteList)
        {
            GameObject go = Instantiate(framePrefab, contentParent);
            DraggableFrame frame = go.GetComponent<DraggableFrame>();
            frame.Setup(entry.sprite, entry.correctIndex);
        }

        //luu lai cac frames
        CacheFrames();
    }
    void CacheFrames()
    {
        frames.Clear();
        foreach (Transform child in contentParent)
        {
            frames.Add(child as RectTransform);
            Debug.Log("name child: "+child.name);
        }
        Debug.Log("cacheframe");
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]); //hoan doi vi tri phan tu (tuple swap)
        }
    }

    private void ClearFrames()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        frames.Clear();
    }

    public void StartScrolling()
    {
        Debug.Log("start scrolling");
        isRunning = true;
        currentSpeed = 0f; // bat dau tu 0

        //hien len khi obj scrollview vi da an truoc do de co the keo tha
        if (!scrollViewAnimObj.activeInHierarchy)
            scrollViewAnimObj.SetActive(true);
    }

    public bool CheckPlayerOrder()
    {
        for (int i = 0; i < contentParent.childCount; i++)
        {
            DraggableFrame frame = contentParent.GetChild(i).GetComponent<DraggableFrame>();
            if (frame.correctIndex != i)
                return false;
        }
        return true;
    }

    public void StopScrolling()
    {
        // Reset lại thời gian và biên độ
        fadeTimer = 0f;
        rangeLerpTime = 0f;

        float targetRange = 20f; // tăng biên độ dao động khi dừng
        float duration = 4f;

        /*// Tween làm tăng biên độ dao động sau khi dừng
        DOTween.To(() => currentRange, x => currentRange = x, targetRange, 3)
            .SetEase(Ease.OutSine);*/

        /*// Tween làm mờ scrollViewAnim
        scrollViewAnim.DOFade(0f, duration).SetEase(Ease.InSine).OnComplete(() =>
        {
            scrollViewAnimObj.SetActive(false);
        });*/

        // Tween giảm tốc độ scroll của các frame
        isStop = true;
    }
    public void SetIsStop(bool value)
    {
        isStop = value;
    }
    public void ResetScrollingState()
    {
        isRunning = false;
        isStop = false;
        currentSpeed = 0f;

        fadeTimer = 0f;
        rangeLerpTime = 0f;
        currentRange = 0f;
        offset = 0f;

        if (scrollViewAnim != null)
        {
            scrollViewAnim.alpha = 0f;
        }

        if (scrollViewAnimObj != null && scrollViewAnimObj.activeSelf)
        {
            scrollViewAnimObj.SetActive(false);
        }

        contentParentAnim.anchoredPosition = initialPosition;

        //clear child contentparentanim
        foreach (Transform child in contentParentAnim)
        {
            Destroy(child.gameObject);
        }
    }

    public void GoToNextLevel()
    {
        //hieu ung chuyen canh
        TransitionPanel();

        int currentIndex = System.Array.IndexOf(levels, currentLevel);

        
        //cutScene
        if(currentIndex==0)
        {
            Debug.Log("start cutscene");
            cutScene.StartCutscene(cutScene.level0);
        }
        if(currentIndex==1)
        {
            cutScene.StartCutscene(cutScene.level1);
        }
        if(currentIndex==2)
        {
            cutScene.StartCutscene(cutScene.level1);
        }
        if(currentIndex==3)
        {
            SoundManager.Instance.PlayEndingTheme();
            cutScene.StartCutsceneEnd(cutScene.endLevel);
        }
        

        if (currentIndex >= 0 && currentIndex < levels.Length - 1)
        {
            currentLevel = levels[currentIndex + 1];

            // Clear old frames
            ClearFrames();

            // Reset animation & scroll states
            ResetScrollingState();

            // Load next level
            //LoadLevel(currentLevel);
            StartCoroutine(LoadLevelCoroutine());

            Debug.Log("Loaded level " + currentIndex + " → " + (currentIndex + 1));
        }
        else
        {
            Debug.Log("No more levels!");
        }
    }
    IEnumerator LoadLevelCoroutine()
    {
        yield return new WaitForEndOfFrame();
        LoadLevel(currentLevel);
    }

    public void TransitionPanel()
    {
        blackPanel.gameObject.SetActive(true);
        blackPanel.alpha = 0f;

        Sequence seq = DOTween.Sequence();
        seq.Append(blackPanel.DOFade(1f, 0f));       // Fade in
        seq.AppendInterval(0.5f);                              // Tam dung
        seq.Append(blackPanel.DOFade(0f, 1f));       // Fade out
        seq.OnComplete(() => blackPanel.gameObject.SetActive(false));
    }
}
