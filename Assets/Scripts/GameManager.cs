using System.Collections.Generic;
using UnityEngine;
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
        LoadLevel(currentLevel);
    }

    private void Update()
    {
        if (!isRunning)
            return;

        Debug.Log("chay");
        // Tăng tốc dần cho đến khi đạt maxSpeed
        if (currentSpeed < maxSpeed)
        {
            currentSpeed += acceleration * Time.deltaTime;
            if (currentSpeed > maxSpeed)
                currentSpeed = maxSpeed;
        }

        foreach (RectTransform frame in frames)
        {
            frame.anchoredPosition += Vector2.left * currentSpeed * Time.deltaTime;

            if (frame.anchoredPosition.x < -frameWidth / 2)
            {
                float maxX = GetRightmostX();
                frame.anchoredPosition = new Vector2(maxX + frameWidth, frame.anchoredPosition.y);
            }
        }
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
            frames.Add(child as RectTransform);
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
    }

    public void StartScrolling()
    {
        isRunning = true;
        Debug.Log("scrolling di");
        currentSpeed = 0f; // bat dau tu 0
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
}
