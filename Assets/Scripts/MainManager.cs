using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public GameObject GameOverText;

    public Text bestPlayerName;
    public Text currentPlayerName;

    private bool m_Started = false;
    private int m_Points;
    
    private bool m_GameOver = false;

    private static int bestScore;
    private static string bestPlayer;


    private void Awake()
    {
        LoadGameRank();
        
    }
    // Start is called before the first frame update
    void Start()
    {

        
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }

        currentPlayerName.text = DataManager.Instance.playerName;
        SetBestPlayer();
      
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        
        SetBestPlayer();


    }

    void AddPoint(int point)
    {
        m_Points += point;
        DataManager.Instance.score = m_Points;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;
        CheckBestPlayer();
        GameOverText.SetActive(true);
        
    }

    private void CheckBestPlayer()
    {
        int currentScore = DataManager.Instance.score;

        if(currentScore>bestScore)
        {
            bestScore = DataManager.Instance.score;
            bestPlayer = DataManager.Instance.playerName;

            bestPlayerName.text = "Best Score: " + bestPlayer + " : " + bestScore;
        }
        SaveGameRank(bestPlayer, bestScore);
    }

    public void SetBestPlayer()
    {
        if(bestPlayer==null && bestScore==0)
        {
            bestPlayerName.text = "";
        }
        else
        {
            bestPlayerName.text = "Best Score: " + bestPlayer + " : " + bestScore;
        }
    }

    public void SaveGameRank(string bestP_Name,int bestP_Score)
    {
        SaveData data = new SaveData();

        data.theBestPlayer = bestP_Name;
        data.theHighiestScore = bestP_Score;

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadGameRank()
    {
        string path = Application.persistentDataPath + "/savefile.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            bestPlayer = data.theBestPlayer;
            bestScore = data.theHighiestScore;
        }
    }

    [System.Serializable]
    class SaveData
    {
        public int theHighiestScore;
        public string theBestPlayer;
    }
}
