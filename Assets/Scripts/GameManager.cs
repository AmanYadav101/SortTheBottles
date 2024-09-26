//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.EventSystems;
//using System.Collections.Generic;

//public class GameManager : MonoBehaviour
//{
//    public Bottle[] bottles;
//    private Bottle selectedBottle = null;
//    Bottle bottle = new Bottle();
//    private void Start()
//    {
//        foreach (Bottle bottle in bottles)
//        {
//            AddEventTrigger(bottle.gameObject);
//        }
//    }

//    private void AddEventTrigger(GameObject bottleGameObject)
//    {
//        EventTrigger trigger = bottleGameObject.GetComponent<EventTrigger>();
//        if (trigger == null)
//        {
//            trigger = bottleGameObject.AddComponent<EventTrigger>();
//        }

//        EventTrigger.Entry entry = new EventTrigger.Entry
//        {
//            eventID = EventTriggerType.PointerClick
//        };
//        entry.callback.AddListener((data) => OnBottleClick(bottleGameObject));
//        trigger.triggers.Add(entry);
//    }

//    public void OnBottleClick(GameObject clickedBottleGameObject)
//    {


//        Bottle clickedBottle = clickedBottleGameObject.GetComponent<Bottle>();
//        if (clickedBottle == null) return;

//        if (selectedBottle == null)
//        {
//            selectedBottle = clickedBottle;
//            Debug.Log("Selected Bottle: " + selectedBottle.name);
//            HighlightBottle(selectedBottle, true);
//        }
//        else
//        {
//            HighlightBottle(selectedBottle, false);
//            if (selectedBottle != clickedBottle && clickedBottle.currentFillLevel < bottle.maxCapacity)
//            {
//                Debug.Log("Moving Liquid from " + selectedBottle.name + " to " + clickedBottle.name);
//                MoveLiquid(selectedBottle, clickedBottle);
//            }
//            else
//            {
//                Debug.Log("Cannot move liquid to the bottle: " + clickedBottle.name);
//            }

//            HighlightBottle(selectedBottle, false);
//            selectedBottle = null;
//        }

//    }

//    public void MoveLiquid(Bottle fromBottle, Bottle toBottle)
//    {
//        Debug.Log("Inside Move Liquid: Moving from " + fromBottle.name + " to " + toBottle.name);

//        if (fromBottle == null || toBottle == null || fromBottle == toBottle) return;

//        List<Image> imagesToMove = new List<Image>();
//        bool canMove = false;

//        // Gather consecutive images with the same color
//        while (fromBottle.currentFillLevel > 0)
//        {
//            Image currentImage = fromBottle.liquidImages[fromBottle.currentFillLevel - 1];
//            //Image CurrentImage = fromBottle.liquidImages[bottle.maxCapacty-fromBottle.currentFillLevel-1];

//            if (imagesToMove.Count == 0 || imagesToMove[0].sprite == currentImage.sprite)
//            {
//                imagesToMove.Add(currentImage);
//                fromBottle.RemoveImage(); // Remove image after adding to the list
//            }
//            else
//            {
//                break; // Stop if color changes
//            }
//        }

//        if (toBottle.currentFillLevel == 0)
//        {
//            // The destination bottle is empty, so allow the move
//            canMove = true;
//        }
//        else
//        {
//            // Check if the last image in the destination bottle matches the color of the first image to move
//            Image lastImageInToBottle = toBottle.liquidImages[toBottle.currentFillLevel - 1];

//            if (lastImageInToBottle != null && lastImageInToBottle.sprite == imagesToMove[0].sprite)
//            {
//                // The sprite of the last image matches the sprite of the images to be moved
//                canMove = true;
//            }
//        }

//        if (canMove)
//        {
//            // Check if there's enough space in the destination bottle
//            if (toBottle.currentFillLevel + imagesToMove.Count <= toBottle.maxCapacity)
//            {
//                foreach (Image imageToMove in imagesToMove)
//                {
//                    imageToMove.transform.SetParent(toBottle.transform);
//                    imageToMove.transform.localPosition = new Vector3(0, 0, 0);
//                    imageToMove.transform.localScale = new Vector3(1, 1, 1);

//                    // Add the image to the target bottle
//                    toBottle.AddImage(imageToMove);
//                }

//                Debug.Log("Moved images to " + toBottle.name);
//            }
//            else
//            {
//                // If not enough space, return images back to the original bottle
//                foreach (Image imageToMove in imagesToMove)
//                {
//                    fromBottle.AddImage(imageToMove);
//                }
//                Debug.Log("Cannot move images to " + toBottle.name + " because there is not enough space.");
//            }
//        }
//        else
//        {
//            // If the move is not allowed, return the images back to the original bottle
//            foreach (Image imageToMove in imagesToMove)
//            {
//                fromBottle.AddImage(imageToMove);
//            }
//            Debug.Log("Cannot move images to " + toBottle.name + " because the colors do not match or the bottle is full.");
//        }
//    }



//    private void HighlightBottle(Bottle bottle, bool highlight)
//    {
//        if (highlight)
//        {
//            bottle.GetComponent<Image>().color = Color.red;
//        }
//        else
//        {
//            bottle.GetComponent<Image>().color = Color.white;
//        }

//    }
//}


using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public Bottle[] bottles;
    private Bottle selectedBottle = null;

    private void Start()
    {
        foreach (Bottle bottle in bottles)
        {
            AddEventTrigger(bottle.gameObject);
        }
    }

    private void AddEventTrigger(GameObject bottleGameObject)
    {
        EventTrigger trigger = bottleGameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = bottleGameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerClick
        };
        entry.callback.AddListener((data) => OnBottleClick(bottleGameObject));
        trigger.triggers.Add(entry);
    }

    public void OnBottleClick(GameObject clickedBottleGameObject)
    {
        Bottle clickedBottle = clickedBottleGameObject.GetComponent<Bottle>();
        if (clickedBottle == null) return;

        if (selectedBottle == null)
        {
            selectedBottle = clickedBottle;
            Debug.Log("Selected Bottle: " + selectedBottle.name);
            HighlightBottle(selectedBottle, true);
        }
        else
        {
            HighlightBottle(selectedBottle, false);
            if (selectedBottle != clickedBottle && clickedBottle.currentFillLevel < clickedBottle.maxCapacity)
            {
                Debug.Log("Moving Liquid from " + selectedBottle.name + " to " + clickedBottle.name);
                MoveLiquid(selectedBottle, clickedBottle);
            }
            else
            {
                Debug.Log("Cannot move liquid to the bottle: " + clickedBottle.name);
            }

            HighlightBottle(selectedBottle, false);
            selectedBottle = null;
        }
    }

    //public void MoveLiquid(Bottle fromBottle, Bottle toBottle)
    //{
    //    Debug.Log("Inside Move Liquid: Moving from " + fromBottle.name + " to " + toBottle.name);

    //    if (fromBottle == null || toBottle == null || fromBottle == toBottle) return;

    //    List<Image> imagesToMove = new List<Image>();
    //    bool canMove = false;

    //    // Gather consecutive images with the same color from the top of the fromBottle
    //    while (fromBottle.currentFillLevel > 0)
    //    {
    //        Image currentImage = fromBottle.RemoveImage();

    //        if (imagesToMove.Count == 0 || imagesToMove[0].sprite == currentImage.sprite)
    //        {
    //            imagesToMove.Add(currentImage);
    //        }
    //        else
    //        {
    //            fromBottle.AddImage(currentImage); // Put back the image if the colors don't match
    //            break; // Stop if color changes
    //        }
    //    }

    //    if (toBottle.currentFillLevel == 0)
    //    {
    //        // The destination bottle is empty, so allow the move
    //        canMove = true;
    //    }
    //    else
    //    {
    //        // Check if the top image in the destination bottle matches the color of the first image to move
    //        Image topImageInToBottle = toBottle.PeekTopImage();
    //        if (topImageInToBottle != null && topImageInToBottle.sprite == imagesToMove[0].sprite)
    //        {
    //            canMove = true;
    //        }
    //    }

    //    if (canMove)
    //    {
    //        // Check if there's enough space in the destination bottle
    //        if (toBottle.currentFillLevel + imagesToMove.Count <= toBottle.maxCapacity)
    //        {
    //            foreach (Image imageToMove in imagesToMove)
    //            {
    //                toBottle.AddImage(imageToMove);
    //            }
    //            Debug.Log("Moved images to " + toBottle.name);
    //        }
    //        else
    //        {
    //            // If not enough space, return images back to the original bottle
    //            foreach (Image imageToMove in imagesToMove)
    //            {
    //                fromBottle.AddImage(imageToMove);
    //            }
    //            Debug.Log("Cannot move images to " + toBottle.name + " because there is not enough space.");
    //        }
    //    }
    //    else
    //    {
    //        // If the move is not allowed, return the images back to the original bottle
    //        foreach (Image imageToMove in imagesToMove)
    //        {
    //            fromBottle.AddImage(imageToMove);
    //        }
    //        Debug.Log("Cannot move images to " + toBottle.name + " because the colors do not match or the bottle is full.");
    //    }
    //}
    public void MoveLiquid(Bottle fromBottle, Bottle toBottle)
    {
        Debug.Log("Inside Move Liquid: Moving from " + fromBottle.name + " to " + toBottle.name);

        if (fromBottle == null || toBottle == null || fromBottle == toBottle) return;

        List<Image> imagesToMove = new List<Image>();
        bool canMove = false;

        // Gather consecutive images with the same color from the top of the fromBottle
        while (fromBottle.currentFillLevel > 0)
        {
            Image currentImage = fromBottle.RemoveImage();

            if (imagesToMove.Count == 0 || imagesToMove[0].sprite == currentImage.sprite)
            {
                imagesToMove.Add(currentImage);
            }
            else
            {
                fromBottle.AddImage(currentImage); // Put back the image if the colors don't match
                break; // Stop if color changes
            }
        }

        if (toBottle.currentFillLevel == 0)
        {
            // The destination bottle is empty, so allow the move
            canMove = true;
        }
        else
        {
            // Check if the top image in the destination bottle matches the color of the first image to move
            Image topImageInToBottle = toBottle.PeekTopImage();
            if (topImageInToBottle != null && topImageInToBottle.sprite == imagesToMove[0].sprite)
            {
                canMove = true;
            }
        }

        if (canMove)
        {
            // Check if there's enough space in the destination bottle
            if (toBottle.currentFillLevel + imagesToMove.Count <= toBottle.maxCapacity)
            {
                for (int i = imagesToMove.Count - 1; i >= 0; i--)
                {
                    Image imageToMove = imagesToMove[i];

                    // Update the image's parent and reset its local position
                    imageToMove.transform.SetParent(toBottle.transform, false);
                    imageToMove.transform.localPosition = Vector3.zero;
                    imageToMove.transform.localScale = Vector3.one;

                    // Add the image to the target bottle in the correct order
                    toBottle.AddImage(imageToMove);
                }
                Debug.Log("Moved images to " + toBottle.name);
            }
            else
            {
                // If not enough space, return images back to the original bottle
                foreach (Image imageToMove in imagesToMove)
                {
                    fromBottle.AddImage(imageToMove);
                }
                Debug.Log("Cannot move images to " + toBottle.name + " because there is not enough space.");
            }
        }
        else
        {
            // If the move is not allowed, return the images back to the original bottle
            foreach (Image imageToMove in imagesToMove)
            {
                fromBottle.AddImage(imageToMove);
            }
            Debug.Log("Cannot move images to " + toBottle.name + " because the colors do not match or the bottle is full.");
        }
    }




    private void HighlightBottle(Bottle bottle, bool highlight)
    {
        bottle.GetComponent<Image>().color = highlight ? Color.red : Color.white;
    }
}
