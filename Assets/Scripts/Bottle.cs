

using UnityEngine;
using UnityEngine.UI;

public class Bottle : MonoBehaviour
{
    public Image[] liquidImages; 
    public int maxCapacity = 4;
    public int currentFillLevel = 0;

    private void Awake()
    {
        if (liquidImages == null || liquidImages.Length != maxCapacity)
        {
            liquidImages = new Image[maxCapacity];
        }
    }

    private void Start()
    {
        Debug.Log(this + " current fill level: " + currentFillLevel);
    }

    public bool AddImage(Image image)
    {
        if (currentFillLevel < maxCapacity)
        {
            liquidImages[currentFillLevel] = image;
            currentFillLevel++;
            return true;
        }
        return false;
    }

    public Image RemoveImage()
    {
        if (currentFillLevel > 0)
        {
            currentFillLevel--;
            Image image = liquidImages[currentFillLevel];
            liquidImages[currentFillLevel] = null; // Clear the reference
            return image;
        }
        return null;
    }
}
