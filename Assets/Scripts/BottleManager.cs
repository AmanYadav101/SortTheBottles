

using UnityEngine;
using UnityEngine.UI; // Required for UI elements

public class BottleManager : MonoBehaviour
{
    public Bottle[] bottles;
    public Sprite[] colorSprites;
    public GameObject imagePrefab; // Prefab with an Image component

    private void Start()
    {
        if (bottles.Length < 5 || colorSprites.Length < 3)
        {
            Debug.LogError("You need at least 5 bottles and 3 color sprites.");
            return;
        }

        ShuffleArray(colorSprites);

        Sprite[] spritesToFill = CreateColorListWithDuplicates(colorSprites, 4);

        ShuffleArray(spritesToFill);

        int spriteIndex = 0;
        for (int i = 0; i < bottles.Length; i++)
        {
            if (spriteIndex >= spritesToFill.Length) break;

            FillBottleWithImages(bottles[i], spritesToFill, ref spriteIndex);
        }
    }

    private Sprite[] CreateColorListWithDuplicates(Sprite[] colors, int duplicatesPerColor)
    {
        Sprite[] result = new Sprite[colors.Length * duplicatesPerColor]; // Creating result array to add sprites
        int index = 0;

        foreach (Sprite color in colors)
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
            imageObject.transform.localPosition = new Vector3(0, 0, 0);
            imageObject.transform.localScale = new Vector3(1, 1, 1);

            Image image = imageObject.GetComponent<Image>();
            if (image != null)
            {
                image.sprite = sprite;
                image.color = Color.white;
                // Add the image to the bottle
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


//B1        B4
//0R         
//1G
//2R
//3Y

//B1-B4
//   B4-B1
//0 - 3 - 0 
//1 - 2 - 1
//2 - 1 - 2
//3 - 0 - 3
//





//B1        B4
//0R        0Y         
//1G
//2R

//B1        B4
//0R         0Y         
//1G         1R

//B1        B4
//0R         0Y         
//           1R
//           2G

//B1        B4
//          0Y         
//          1R
//          2G
//          3R
