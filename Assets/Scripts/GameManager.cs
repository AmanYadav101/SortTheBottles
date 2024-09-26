using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public Bottle[] bottles;
    private Bottle selectedBottle = null;
    private BottleManager bottleManager;
    public int currentLevel;
    private UIManager uIManager;
    private void Start()
    {
        bottleManager = FindObjectOfType<BottleManager>(); // Reference to BottleManager
        uIManager = FindObjectOfType<UIManager>();
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1); // Default to level 1 if not set

        if (bottles != null && bottles.Length > 0)
        {
            foreach (Bottle bottle in bottles)
            {
                AddEventTrigger(bottle.gameObject);
            }
        }
    }

    public void SetBottles(Bottle[] newBottles)
    {
        bottles = newBottles;

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
            //Debug.Log("Selected Bottle: " + selectedBottle.name);
            HighlightBottle(selectedBottle, true);
        }
        else
        {
            HighlightBottle(selectedBottle, false);
            if (selectedBottle != clickedBottle && clickedBottle.currentFillLevel < clickedBottle.maxCapacity)
            {
                //Debug.Log("Moving Liquid from " + selectedBottle.name + " to " + clickedBottle.name);
                MoveLiquid(selectedBottle, clickedBottle);
            }
            else
            {
                //Debug.Log("Cannot move liquid to the bottle: " + clickedBottle.name);
            }

            HighlightBottle(selectedBottle, false);
            selectedBottle = null;
        }
        CheckWinCondition();
    }
    
    public void CheckWinCondition()
    {
        int filledBottleCount = 0; // To track how many bottles are filled
        bool allBottlesCorrect = true; // To check if all filled bottles meet the color condition

        Debug.Log("Checking Win Condition");

        foreach (Bottle bottle in bottles)
        {
            // Count only the filled bottles
            if (bottle.currentFillLevel > 0)
            {
                filledBottleCount++;
                Debug.Log("Checking filled bottle...");

                // Check if this bottle is filled with the same color
                if (!IsBottleFilledWithSameColor(bottle))
                {
                    Debug.Log("A filled bottle has different colors.");
                    allBottlesCorrect = false; // Mark as incorrect if any filled bottle is not the same color
                    break; // No need to check further
                }
            }
        }

        // Check if the win condition is met
        if (allBottlesCorrect && filledBottleCount == (bottles.Length - 2)) // Two bottles should be empty
        {
            Debug.Log("Level Completed! Starting next level...");
            currentLevel++;
            uIManager.UpdateLevelText();
            StartNextLevel(); // Proceed to the next level
        }
    }

    private bool IsBottleFilledWithSameColor(Bottle bottle)
    {
        if (bottle.currentFillLevel == 0) return false; // Bottle is empty

        Sprite firstSprite = bottle.PeekTopImage()?.sprite;

        foreach (Image image in bottle.GetAllImages()) // Assuming Bottle has a method to get all Images
        {
            if (image.sprite != firstSprite)
            {
                return false; // If any image has a different color
            }
        }

        return true; // All images in the bottle are the same
    }

    private void StartNextLevel()
    {
        bottleManager.SetupLevel(currentLevel);

        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        PlayerPrefs.Save();
        Debug.Log("Saved Current Level: " + currentLevel);

    }

    private void HighlightBottle(Bottle bottle, bool highlight)
    {
        bottle.GetComponent<Image>().color = highlight ? Color.red : Color.white;
    }

    public void MoveLiquid(Bottle fromBottle, Bottle toBottle)
    {
        if (fromBottle == null || toBottle == null || fromBottle == toBottle) return;

        List<Image> imagesToMove = new List<Image>();
        bool canMove = false;

        // Gather consecutive images with the same color from the top of the fromBottle
        while (fromBottle.currentFillLevel > 0)
        {
            Image currentImage = fromBottle.RemoveImage();

            // If the current image is null, break the loop
            if (currentImage == null) break;

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

        // Check if we have images to move
        if (imagesToMove.Count == 0)
        {
            // No images to move, return
            return;
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
            }
            else
            {
                // If not enough space, return images back to the original bottle
                foreach (Image imageToMove in imagesToMove)
                {
                    fromBottle.AddImage(imageToMove);
                }
            }
        }
        else
        {
            // If the move is not allowed, return the images back to the original bottle
            foreach (Image imageToMove in imagesToMove)
            {
                fromBottle.AddImage(imageToMove);
            }
        }
    }
}


