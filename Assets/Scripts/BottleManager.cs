using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class LevelData
{
    public int level;
    public int bottles;
    public int colors;
}

[System.Serializable]
public class GameData
{
    public List<LevelData> levels;
}

public class BottleManager : MonoBehaviour
{
    public GameObject bottlePrefab; // Prefab for bottle with a Bottle script
    public GameObject imagePrefab; // Prefab with an Image component
    public Sprite[] allColorSprites; // All available sprites for colors
    public Transform bottleParent; // Parent object to hold all bottles

    private List<Bottle> bottles = new List<Bottle>();
    private GameData gameData;

    public GameManager gameManager; // Reference to GameManager

    private void Start()
    {
        LoadGameData();
        SetupLevel(1); // Example: Starting with level 1
    }

    private void LoadGameData()
    {
        string jsonPath = Path.Combine(Application.streamingAssetsPath, "gamedata.json");

        if (File.Exists(jsonPath))
        {
            string jsonData = File.ReadAllText(jsonPath);
            gameData = JsonUtility.FromJson<GameData>(jsonData);
        }
        else
        {
            Debug.LogError("Game data file not found!");
        }
    }

    public void SetupLevel(int level)
    {
        LevelData levelData = gameData.levels.Find(l => l.level == level);
        if (levelData == null)
        {
            Debug.LogError("Level data not found!");
            return;
        }

        foreach (Transform child in bottleParent)
        {
            Destroy(child.gameObject);
        }
        bottles.Clear();

        for (int i = 0; i < levelData.bottles; i++)
        {
            GameObject newBottleObject = Instantiate(bottlePrefab, bottleParent);
            Bottle newBottle = newBottleObject.GetComponent<Bottle>();
            bottles.Add(newBottle);
        }

        // Assign bottles to the GameManager
        if (gameManager != null)
        {
            gameManager.SetBottles(bottles.ToArray());
        }

        Sprite[] spritesToFill = CreateColorListWithDuplicates(levelData.colors, 4);
        ShuffleArray(spritesToFill);

        int spriteIndex = 0;
        foreach (Bottle bottle in bottles)
        {
            FillBottleWithImages(bottle, spritesToFill, ref spriteIndex);
        }
    }
   
    private Sprite[] CreateColorListWithDuplicates(int colorCount, int duplicatesPerColor)
    {
        Sprite[] result = new Sprite[colorCount * duplicatesPerColor];
        Sprite[] selectedColors = new Sprite[colorCount];

        for (int i = 0; i < colorCount; i++)
        {
            selectedColors[i] = allColorSprites[i];
        }

        int index = 0;
        foreach (Sprite color in selectedColors)
        {
            for (int i = 0; i < duplicatesPerColor; i++)
            {
                result[index] = color;
                index++;
            }
        }

        return result;
    }

    private void FillBottleWithImages(Bottle bottle, Sprite[] sprites, ref int spriteIndex)
    {
        for (int i = 0; i < bottle.maxCapacity; i++)
        {
            if (spriteIndex >= sprites.Length) break;

            Sprite sprite = sprites[spriteIndex];
            spriteIndex++;

            // Create a new GameObject with an Image component
            GameObject imageObject = Instantiate(imagePrefab);
            imageObject.transform.SetParent(bottle.transform);
            imageObject.transform.localPosition = Vector3.zero;
            imageObject.transform.localScale = Vector3.one;

            Image image = imageObject.GetComponent<Image>();
            if (image != null)
            {
                image.sprite = sprite;
                image.color = Color.white;
                bottle.AddImage(image);
            }
            else
            {
                Debug.LogError("Image component not found on the prefab!");
            }
        }
    }

    private void ShuffleArray(Sprite[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            Sprite temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }
}
