using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    public int tutorialIndex = 0;

    private bool IsAPressed = false;
    private bool IsDPressed = false;
    private bool WasClickedOnAD = false;
    private bool WasClickedOnRCM = false;
    private bool WasClickedOnLCM = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Update()
    {
        if (GameManager.Instance.IsTutorialPassed)
        {
            Destroy(this);
        }
        else if (tutorialIndex == 0 && UIManager.Instance.WasStartedFirstLevel)
        {
            tutorialIndex++;
            UIManager.Instance.ShowHint(tutorialIndex);
        }
        else if (tutorialIndex == 1)
        {
            if (!WasClickedOnAD)
            {
                if (IsAPressed && IsDPressed)
                {
                    WasClickedOnAD = true;
                }
                else
                {
                    if (Input.GetKeyUp(KeyCode.A))
                    {
                        IsAPressed = true;
                    }
                    else if (Input.GetKeyUp(KeyCode.D))
                    {
                        IsDPressed = true;
                    }
                }
            }
            else
            {
                tutorialIndex++;
                UIManager.Instance.ShowHint(tutorialIndex);
            }
        }
        else if (tutorialIndex == 2)
        {
            if (!WasClickedOnRCM)
            {
                if (Input.GetKeyUp(KeyCode.Mouse1))
                {
                    WasClickedOnRCM = true;
                }
            }
            else
            {
                tutorialIndex++;
                UIManager.Instance.ShowHint(tutorialIndex);
            }
        }
        else if (tutorialIndex == 3)
        {
            if (!WasClickedOnLCM)
            {
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    WasClickedOnLCM = true;
                }
            }
            else
            {
                tutorialIndex++;
                UIManager.Instance.ShowHint(tutorialIndex);
            }
        }
        else if (tutorialIndex >= 4)
        {
            if (UIManager.Instance.WasClosedShop) 
            {
                GameManager.Instance.IsTutorialPassed = true;
                YandexGame.savesData.IsTutorialPassed = true;

                YandexGame.SaveProgress();
                UIManager.Instance.tutorialPanel.SetActive(false);
            }
        }

    }
    internal void StartTutorial()
    {
        UIManager.Instance.tutorialPanel.SetActive(true);
        UIManager.Instance.ShowHint(tutorialIndex);
    }
}
