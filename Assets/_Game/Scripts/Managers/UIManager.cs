using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] List<CanvasGroup> _btnsHome;
    [SerializeField] List<CanvasGroup> _Pages;
    [SerializeField] CanvasGroup _btns;
    [SerializeField] CanvasGroup _backGround;
    [SerializeField] CanvasGroup _playButton;
    [SerializeField] CanvasGroup _gamePlay;
    [SerializeField] CanvasGroup _settings;
    [SerializeField] CanvasGroup _win;
    [SerializeField] CanvasGroup _lose;
    [SerializeField] TMP_Text[] _levelText;

    public TMP_Text textTimer;

    public Vector3 pos;
    private void Start()
    {
        Init();
    }

    void Init()
    {
        ResetPos(0);
        UpdateLevelText();
    }

    public void ResetPos(int _index)
    {
        if (_btnsHome.Count > _index)
        {
            for (int i = 0; i < _btnsHome.Count; i++)
            {
                if (i != _index)
                {
                    _btnsHome[i].transform.DOLocalMoveY(-25f, 0.1f);
                    _Pages[i].transform.DOLocalMoveX(750f, 0.1f);
                    _Pages[i].gameObject.SetActive(false);


                }
                else
                {
                    _btnsHome[i].transform.DOLocalMoveY(25, 0.1f);
                    _Pages[i].transform.DOLocalMoveX(0f, 0.1f);
                    _Pages[i].gameObject.SetActive(true);
                }
            }
            _btns.gameObject.SetActive(true);
            _playButton.gameObject.SetActive(true);
            _backGround.gameObject.SetActive(true);
            _gamePlay.gameObject.SetActive(false);
            _win.gameObject.SetActive(false);
            _lose.gameObject.SetActive(false);
            UpdateLevelText();
        }
        else
        {
            _btns.gameObject.SetActive(false);
            _playButton.gameObject.SetActive(false);
            _backGround.gameObject.SetActive(false);
            _gamePlay.gameObject.SetActive(true);
            _settings.gameObject.SetActive(false);
            _win.gameObject.SetActive(false);
            _lose.gameObject.SetActive(false);
        }
    }

    void UpdateLevelText()
    {
        for (int i = 0; i < _levelText.Length; i++)
        {
            _levelText[i].text = (GameManager.Instance.level + i + 1).ToString();
        }
    }
    public void Settings(bool state)
    {
        _settings.gameObject.SetActive(state);
    }

    public void EnableWin()
    {
        _win.gameObject.SetActive(true);
    }

    public void EnableLose()
    {
        _lose.gameObject.SetActive(true);
    }
    public void HidePlayUi()
    {
        GameManager.Instance.Play();
    }
}
