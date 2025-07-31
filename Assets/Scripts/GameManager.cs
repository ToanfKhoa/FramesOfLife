using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Level Setup")]
    public LevelData[] levels;
    public LevelData currentLevel;
    public GameObject framePrefab;
    public Transform contentParent;

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

    public void LoadLevel(LevelData data)
    {
        /*currentLevel = data;
        ClearFrames();*/

        for (int i = 0; i < data.sprites.Length; i++)
        {
            GameObject go = Instantiate(framePrefab, contentParent);
            DraggableFrame frame = go.GetComponent<DraggableFrame>();
            Debug.Log("dung o day i= " + i);
            frame.Setup(data.sprites[i], i);
        }
    }

    private void ClearFrames()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
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
