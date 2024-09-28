using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class Bottle : MonoBehaviour
{
    public int maxCapacity = 4;
    public int currentFillLevel = 0;

    private Stack<Image> liquidStack = new Stack<Image>();

    public bool AddImage(Image image)
    {
        if (currentFillLevel < maxCapacity)
        {
            liquidStack.Push(image);
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
            return liquidStack.Pop();
        }
        return null;
    }

    public Image PeekTopImage()
    {
        if (currentFillLevel > 0)
        {
            return liquidStack.Peek();
        }
        return null;
    }

    public IEnumerable<Image> GetAllImages()
    {
        return liquidStack.ToArray();
    }

    public Color GetTopImageColor()
    {
        Image topImage = PeekTopImage();
        return topImage != null ? topImage.color : Color.clear;
    }
}
