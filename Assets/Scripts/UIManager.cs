using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameManager gameManager;
    BottleManager bottleManager;
    public TextMeshProUGUI levelText;
    public GameObject SettingsPannel;

    public Image soundOnOffImg;
    public Sprite[] soundSprites;

    public Image musicOnOffImg;
    public Sprite[] musicSprites;
    
    bool clicked = false;
    int sound = 0;
    int music = 0;

    // Start is called before the first frame update
    void Start()
    {
        SettingsPannel.SetActive(false);
        bottleManager = FindObjectOfType<BottleManager>();
        gameManager = FindObjectOfType<GameManager>();
        gameManager.currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1); // Default to level 1 if not set
        music = PlayerPrefs.GetInt("music", 0);
        sound = PlayerPrefs.GetInt("sound", 0);
        CheckMusic();
        CheckSound();
        UpdateLevelText();
    }

    public void UpdateLevelText()
    {
        levelText.text ="Level: " + gameManager.currentLevel.ToString();
    }

    public void ResetBottles()
    {
        if (gameManager.bInteractable)
        {
        bottleManager.SetupLevel(gameManager.currentLevel);
        }
    }

    public void GameSettings()
    {
        if (!clicked)
        {
            SettingsPannel?.SetActive(true);
        }
        else
        {
            SettingsPannel?.SetActive(false);
        }
            clicked = !clicked;
    }

    private void CheckMusic()
    {
        if (music == 0)
        {
            musicOnOffImg.sprite = musicSprites[0];
        }
        else
        {
            musicOnOffImg.sprite = musicSprites[1];

        }
    }


    public void MusicOnOff()
    {
        if (music == 0)
        {
            musicOnOffImg.sprite = musicSprites[1];
            music = 1;
            
        }
        else
        {
            musicOnOffImg.sprite = musicSprites[0];
            music = 0;
        }
        PlayerPrefs.SetInt("music", music);
    }
    
    private void CheckSound()
    {
        if (sound == 0)
        {
            soundOnOffImg.sprite = soundSprites[0];
        }
        else
        {
            soundOnOffImg.sprite = soundSprites[1];
        }
    }
    public void SoundOnOff()
    {
        if (sound == 0)
        {
            soundOnOffImg.sprite = soundSprites[1];
            sound = 1;
        }
        else
        {
            soundOnOffImg.sprite = soundSprites[0];
            sound = 0;
        }
        PlayerPrefs.SetInt("sound", sound);

    }
}
