using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Material[] colorsWagon;
    public List<WagonHolder> activeWagonHolders;
    public List<int> passagerColorIndexs;

    [SerializeField] CinemachineVirtualCamera cam;
    [SerializeField] UIManager _uiManager;

    public Level[] levels;

    public bool win;
    public bool lose;
    public int level;
    Coroutine _coroutine;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }

        Instance = this;
        level = PlayerPrefs.GetInt("level", 0);

    }
    private void Start()
    {
        
    }
    private void Init()
    {


        foreach (var item in activeWagonHolders)
        {
            Destroy(item.gameObject);
        }
        activeWagonHolders.Clear();
        activeWagonHolders = new List<WagonHolder>();
        passagerColorIndexs = new List<int>();
        if (level < levels.Length)
        {
            Grid.instance.matrix = levels[level].matrix;
        }
        else
        {
            level = 0;
        }
        win = false;
        lose = false;
    }

    public void SetPassengers()
    {
        foreach (var activWagon in activeWagonHolders)
        {
            foreach (var wagon in activWagon.wagonList)
            {

                for (int i = 0; i < wagon.wagonStat.chairsPoints.Count; i++)
                {
                    passagerColorIndexs.Add(wagon.wagonStat.wagonColorIndex);
                }
            }
        }
        passagerColorIndexs.Sort();
    }

    public void Win()
    {
        foreach (var holder in activeWagonHolders)
        {
            if (holder.wagonList.Count > 0)
                return;

        }
        _uiManager.EnableWin();
        win = true;
        level++;
        PlayerPrefs.SetInt("level", level);
        PlayerPrefs.Save();
    }

    public void Lose()
    {
        if (!win)
        {
            _uiManager.EnableLose();
            lose = true;
        }
    }

    public void Play()
    {
        Init();
        Grid.instance.Init();
        Grid.instance.SpownLevel();

        if (_coroutine != null)
            StopCoroutine(_coroutine);

        _coroutine = StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        float currentTime = 60 * (1 + (level * 0.7f));

        while (currentTime > 0 && !win)
        {
            UpdateTimerUI(currentTime);
            yield return new WaitForSeconds(1f);
            currentTime--;
        }


        Lose();
        UpdateTimerUI(0);
    }

    void UpdateTimerUI(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        _uiManager.textTimer.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    public void InitCamera()
    {
        cam.transform.position += Vector3.up * ((Grid.instance.matrix.rows + Grid.instance.matrix.rows + 2) - cam.transform.position.y);
    }

}
