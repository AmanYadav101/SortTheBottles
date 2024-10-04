//using UnityEngine;
//using UnityEngine.UI;
//using System.IO;
//using System.Collections.Generic;

//[System.Serializable]
//public class LevelData
//{
//    public int level;
//    public int bottles;
//    public int colors;
//}

//[System.Serializable]
//public class GameData
//{
//    public List<LevelData> levels;
//}

//public class BottleManager : MonoBehaviour
//{
//    public GameObject bottlePrefab; // Prefab for bottle with a Bottle script
//    public GameObject imagePrefab; // Prefab with an Image component
//    public Transform bottleParent; // Parent object to hold all bottles
//    public Sprite whiteSprite; // A white sprite that is used for all images
//    public Color[] allColors;// Array of colors to be used

//    private List<Bottle> bottles = new List<Bottle>();
//    private GameData gameData;

//    public GameManager gameManager; // Reference to GameManager

//    private void Start()
//    {
//        LoadGameData();
//        SetupLevel(gameManager.currentLevel); // Example: Starting with level 1
//    }

//    private void LoadGameData()
//    {
//        string jsonPath = Path.Combine(Application.streamingAssetsPath, "gamedata.json");

//        if (File.Exists(jsonPath))
//        {
//            string jsonData = File.ReadAllText(jsonPath);
//            gameData = JsonUtility.FromJson<GameData>(jsonData);
//        }
//        else
//        {
//            Debug.LogError("Game data file not found!");
//        }
//    }

//    public void SetupLevel(int level)
//    {
//        LevelData levelData = gameData.levels.Find(l => l.level == level);
//        if (levelData == null)
//        {
//            Debug.LogError("Level data not found!");
//            return;
//        }

//        foreach (Transform child in bottleParent)
//        {
//            Destroy(child.gameObject);
//        }
//        bottles.Clear();

//        for (int i = 0; i < levelData.bottles; i++)
//        {
//            GameObject newBottleObject = Instantiate(bottlePrefab, bottleParent);
//            Bottle newBottle = newBottleObject.GetComponent<Bottle>();
//            newBottle.name = "bottle_" + i.ToString();
//            bottles.Add(newBottle);
//        }

//        // Assign bottles to the GameManager
//        if (gameManager != null)
//        {
//            gameManager.SetBottles(bottles.ToArray());
//        }

//        // Create the color list based on the number of colors
//        Color[] colorsToFill = CreateColorListWithDuplicates(levelData.colors, 4);
//        ShuffleArray(colorsToFill);

//        int colorIndex = 0;
//        foreach (Bottle bottle in bottles)
//        {
//            FillBottleWithImages(bottle, colorsToFill, ref colorIndex);
//        }
//    }

//    private Color[] CreateColorListWithDuplicates(int colorCount, int duplicatesPerColor)
//    {
//        Color[] result = new Color[colorCount * duplicatesPerColor];
//        Color[] selectedColors = new Color[colorCount];

//        for (int i = 0; i < colorCount; i++)
//        {
//            selectedColors[i] = allColors[i];
//        }

//        int index = 0;
//        foreach (Color color in selectedColors)
//        {
//            for (int i = 0; i < duplicatesPerColor; i++)
//            {
//                result[index] = color;
//                index++;
//            }
//        }

//        return result;
//    }

//    private void FillBottleWithImages(Bottle bottle, Color[] colors, ref int colorIndex)
//    {
//        for (int i = 0; i < bottle.maxCapacity; i++)
//        {
//            if (colorIndex >= colors.Length) break;

//            Color color = colors[colorIndex];
//            colorIndex++;

//            // Create a new GameObject with an Image component
//            GameObject imageObject = Instantiate(imagePrefab);
//            imageObject.transform.SetParent(bottle.transform);
//            imageObject.transform.localPosition = Vector3.zero;
//            imageObject.transform.localScale = Vector3.one;

//            Image image = imageObject.GetComponent<Image>();
//            if (image != null)
//            {
//                image.sprite = whiteSprite; // Set the sprite to a white sprite
//                image.color = color; // Set the color to the desired color
//                bottle.AddImage(image);
//            }
//            else
//            {
//                Debug.LogError("Image component not found on the prefab!");
//            }
//        }
//    }

//    private void ShuffleArray(Color[] array)
//    {
//        for (int i = array.Length - 1; i > 0; i--)
//        {
//            int j = Random.Range(0, i + 1);
//            Color temp = array[i];
//            array[i] = array[j];
//            array[j] = temp;
//        }
//    }
//}

using UnityEngine;
using UnityEngine.UI;
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
    public Transform bottleParent; // Parent object to hold all bottles
    public Sprite whiteSprite; // A white sprite that is used for all images
    public Color[] allColors; // Array of colors to be used

    public TextAsset jsonFile; // Reference to JSON file in the Inspector

    private List<Bottle> bottles = new List<Bottle>();
    private GameData gameData;

    public GameManager gameManager; // Reference to GameManager

    private void Start()
    {
        LoadGameData();
        SetupLevel(gameManager.currentLevel); // Example: Starting with level 1
    }

    private void LoadGameData()
    {
        if (jsonFile != null)
        {
            string jsonData = jsonFile.text;
            gameData = JsonUtility.FromJson<GameData>(jsonData);
        }
        else
        {
            Debug.LogError("JSON file not assigned in the Inspector!");
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
            newBottle.name = "bottle_" + i.ToString();
            bottles.Add(newBottle);
        }

        // Assign bottles to the GameManager
        if (gameManager != null)
        {
            gameManager.SetBottles(bottles.ToArray());
        }

        // Create the color list based on the number of colors
        Color[] colorsToFill = CreateColorListWithDuplicates(levelData.colors, 4);
        ShuffleArray(colorsToFill);

        int colorIndex = 0;
        foreach (Bottle bottle in bottles)
        {
            FillBottleWithImages(bottle, colorsToFill, ref colorIndex);
        }
    }

    private Color[] CreateColorListWithDuplicates(int colorCount, int duplicatesPerColor)
    {
        Color[] result = new Color[colorCount * duplicatesPerColor];
        Color[] selectedColors = new Color[colorCount];

        for (int i = 0; i < colorCount; i++)
        {
            selectedColors[i] = allColors[i];
        }

        int index = 0;
        foreach (Color color in selectedColors)
        {
            for (int i = 0; i < duplicatesPerColor; i++)
            {
                result[index] = color;
                index++;
            }
        }

        return result;
    }

    private void FillBottleWithImages(Bottle bottle, Color[] colors, ref int colorIndex)
    {
        for (int i = 0; i < bottle.maxCapacity; i++)
        {
            if (colorIndex >= colors.Length) break;

            Color color = colors[colorIndex];
            colorIndex++;

            // Create a new GameObject with an Image component
            GameObject imageObject = Instantiate(imagePrefab);
            imageObject.transform.SetParent(bottle.transform);
            imageObject.transform.localPosition = Vector3.zero;
            imageObject.transform.localScale = Vector3.one;

            Image image = imageObject.GetComponent<Image>();
            if (image != null)
            {
                image.sprite = whiteSprite; // Set the sprite to a white sprite
                image.color = color; // Set the color to the desired color
                bottle.AddImage(image);
            }
            else
            {
                Debug.LogError("Image component not found on the prefab!");
            }
        }
    }

    private void ShuffleArray(Color[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            Color temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }
}
