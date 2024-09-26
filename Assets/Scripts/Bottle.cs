

//using UnityEngine;
//using UnityEngine.UI;

//public class Bottle : MonoBehaviour
//{
//    public Image[] liquidImages;
//    public int maxCapacity = 4;
//    public int currentFillLevel = 0;

//    private void Awake()
//    {
//        if (liquidImages == null || liquidImages.Length != maxCapacity)
//        {
//            liquidImages = new Image[maxCapacity];
//        }

//    }



//    public bool AddImage(Image image)
//    {
//        if (currentFillLevel < maxCapacity)
//        {
//            liquidImages[currentFillLevel] = image;
//            //liquidImages[maxCapacity - currentFillLevel -1] = image;

//            currentFillLevel++;
//            return true;
//        }
//        return false;
//    }

//    public Image RemoveImage()
//    {
//        if (currentFillLevel > 0)
//        {
//            currentFillLevel--;//removed
//            Image image = liquidImages[currentFillLevel];
//           // Image image = liquidImages[maxCapacity - currentFillLevel];
//            //liquidImages[maxCapacity - currentFillLevel] = null;
//            liquidImages[currentFillLevel] = null; // Clear the reference
//            return image;
//        }

//        return null;
//    }


//}

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

    public bool AddImage(Image image)
    {
        if (currentFillLevel < maxCapacity)
        {
            // Add image to the "top" of the bottle (end of the array in reverse)
            liquidImages[maxCapacity - currentFillLevel - 1] = image;
            currentFillLevel++;
            return true;
        }
        return false;
    }

    public Image RemoveImage()
    {
        if (currentFillLevel > 0)
        {
            // Remove image from the "top" of the bottle (end of the array in reverse)
            Image image = liquidImages[maxCapacity-currentFillLevel];
            liquidImages[maxCapacity - currentFillLevel] = null;
            currentFillLevel--;
            return image;
        }
        return null;
    }

    public Image PeekTopImage()
    {
        if (currentFillLevel > 0)
        {
            return liquidImages[maxCapacity - currentFillLevel];
        }
        return null;
    }
}
