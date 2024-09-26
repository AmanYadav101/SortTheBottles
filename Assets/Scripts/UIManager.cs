using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameManager gameManager;

    public TextMeshProUGUI levelText;

    
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManager.currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1); // Default to level 1 if not set

        UpdateLevelText();
    }

    public void UpdateLevelText()
    {
        levelText.text = gameManager.currentLevel.ToString();
    }
}
