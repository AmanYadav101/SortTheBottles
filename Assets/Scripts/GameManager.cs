using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public Bottle[] bottles;
    private Bottle selectedBottle = null;
    private BottleManager bottleManager;
    public int currentLevel;
    private UIManager uIManager;
    public GridLayoutGroup gRO ;

    private bool bInteractable = true;

    private LineRenderer lineRenderer;

    private void Start()
    {
        bottleManager = FindObjectOfType<BottleManager>();
        uIManager = FindObjectOfType<UIManager>();
        lineRenderer = GameObject.Find("Line").GetComponent<LineRenderer>();
        lineRenderer.startWidth = .03f;
        lineRenderer.endWidth = .03f;
        lineRenderer.enabled = false;

        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);

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
            HighlightBottle(selectedBottle, true);
        }
        else
        {
            HighlightBottle(selectedBottle, false);
            //the selected bottle is not empty check so that the empty bottles dont become uninteractable after we select them and try to move the liquid to the clickedbottle
            //which will keep the "bInteractable" bool to false for them.
            if (selectedBottle != clickedBottle && selectedBottle.currentFillLevel != 0  && clickedBottle.currentFillLevel < clickedBottle.maxCapacity)
            {
                MoveLiquid(selectedBottle, clickedBottle);
            }
            HighlightBottle(selectedBottle, false);
            selectedBottle = null;
        }
    }

    public void MoveLiquid(Bottle fromBottle, Bottle toBottle)
    {
        if (bInteractable)
        {
            bInteractable = false;

            EventTrigger fromEventTrigger = fromBottle.GetComponent<EventTrigger>();
            EventTrigger toBottleEventTrigger = toBottle.GetComponent<EventTrigger>();

            if (fromBottle == null || toBottle == null || fromBottle == toBottle) return;

            List<Image> imagesToMove = new List<Image>();
            bool canMove = false;
            Color transferColor = Color.white;

            // Gather consecutive images with the same color from the top of the "fromBottle"
            //1. Removes the images first which is stored in the "currentImage". then checks if the image that is removes has the same color as of the image thats already in the "imagesToMove" list
            //2. if the removed image has same color, then that image is added to the "imagesToMove" list or else it added back to the "frombottle"
            while (fromBottle.currentFillLevel > 0)
            {
                Image currentImage = fromBottle.RemoveImage();
                if (currentImage == null) break;
                //Debug.Log("IMAGE TO MOVE: " + imagesToMove[imagesToMove.Count].color);


                if (imagesToMove.Count == 0 || imagesToMove[0].color == currentImage.color)
                {
                    imagesToMove.Add(currentImage);
                    transferColor = currentImage.color; //Stores the color of the image which is then later used for the line renderer
                }
                else
                {
                    fromBottle.AddImage(currentImage);
                    break;
                }
            }
            Debug.Log("Image to move count" + imagesToMove.Count);
            if (imagesToMove.Count == 0)
            {
                //can make bInteractable to true here so that if we try to transferfrom one empty bottle to another, the bInteracable dont always stay false for those bottles.
                //but we will just not let the function run if the "selectedBottle"'s capacity is 0 in the "OnBottleClicked" event.
                //bInteractable = true;
                return; //if the "fromBottle" is empty, then the imagesToMove is empty and this statement then early returns.
            }

            // Determine if we can move images based on the destination bottle's state
            canMove = (toBottle.currentFillLevel == 0 || (toBottle.PeekTopImage().color == imagesToMove[0].color));
            
            //If "canMove" is false, then sets the "bInteractable" to true, also puts back all the images that were retrieved from the "fromBottle"
            //and early returns. And we are starting the loop from the back
            //bc if start from 0 then the images are removed from bottom instead of the top.
            if(!canMove)
            {
                for(int i = imagesToMove.Count-1; i >= 0; i--)
                {
                    fromBottle.AddImage(imagesToMove[i]);
                }
                bInteractable = true;
                return;
            }

            //this is if we try to add images to the "toBottle" if the "currentFillLEvel" of the "toBottle" and "count" of "imagesToMove" exceeds the "maxCapacity" of the "toBottle"
            // and the "fromBottle" doenst go empty without moving its images to the "toBottle"
            if (toBottle.currentFillLevel + imagesToMove.Count > toBottle.maxCapacity)
            {

                for (int i = imagesToMove.Count - 1; i >= 0; i--)
                {
                    fromBottle.AddImage(imagesToMove[i]);
                }
                bInteractable = true;
                return;
            }
            Debug.Log("can move " + canMove);
            Debug.Log("Curremt fill level " + toBottle.currentFillLevel + "    " + imagesToMove.Count);
            if (canMove && (toBottle.currentFillLevel + imagesToMove.Count <= toBottle.maxCapacity))
            {
                //turn off the event triggers for the tobottle and from bottle
                //which at this point is not needed as the binteractble takes care of everything that this block of code does.
                if (fromEventTrigger != null&&toBottle!=null)
                {
 //                  toBottleEventTrigger.enabled = false;
                    fromEventTrigger.enabled = false;
                }

                Vector3 originalPosition = fromBottle.transform.position;
                Vector3 targetPosition = toBottle.transform.position + new Vector3(0, 0.5f, 0);
                gRO.enabled = false;

                //Moves to location of "toBottle"->Rotates and starts pouring the liquid at same time using the "OnPlay"
                //->moves the images ->Rotates back to 0 and MovesBack at the same time using "OnPlay" and then makes the "bIteractable to true"
                fromBottle.transform.DOMove(targetPosition, 1f).OnComplete(() =>
                {

                    //Enables the Line renderer and sets the startcolor and end color of the line renderer to the "transferColor"
                    //which was assigned earlier to the color that is in the "imagesToMove" list.
                        ToggleLine(true, fromBottle.transform.position, toBottle.transform.position);
                        lineRenderer.startColor = transferColor;
                        lineRenderer.endColor = transferColor;  

                    fromBottle.transform.DORotate(new Vector3(0, 0, 55), 2f).OnPlay(() =>
                    {
                        int delay = 0;
                        // Start moving liquid images and animate their fill
                        int count = imagesToMove.Count;

                        // After the animation completes, transfer all images to the toBottle at once
                        for (int i = imagesToMove.Count - 1; i >= 0; i--)
                        {
                            Image imageToMove = imagesToMove[i];

                            // Clone the image into the toBottle
                            GameObject newImageObj = Instantiate(imageToMove.gameObject, toBottle.transform);
                            Image newImage = newImageObj.GetComponent<Image>();

                            newImage.fillAmount = 0f;
                            newImage.color = transferColor;

                            // Add the new image to the toBottle
                            toBottle.AddImage(newImage);

                            newImage.DOFillAmount(1f, 1f).SetEase(Ease.Linear).SetDelay(delay);
                            if (i == 0)
                            {
                                Debug.Log("IN");
                                imagesToMove[imagesToMove.Count - i - 1].DOFillAmount(0f, 1f).SetEase(Ease.Linear).SetDelay(delay).OnComplete(() =>
                            {
                                for (int i = 0; i < count; i++)
                                {
                                    Destroy(imagesToMove[i].gameObject);
                                }
                            });
                            }
                            else
                            {
                                imagesToMove[imagesToMove.Count - i - 1].DOFillAmount(0f, 1f).SetEase(Ease.Linear).SetDelay(delay);
                            }

                            delay += 1;

                        }
                        fromBottle.transform.DORotate(Vector3.zero, 1f).SetDelay(delay).OnPlay(() =>
                        {
                            ToggleLine(false, fromBottle.transform.position, toBottle.transform.position);

                            fromBottle.transform.DOMove(originalPosition, 1f).OnComplete(() =>
                            {
                                // Enable event trigger and check win condition after completion
                                if (fromEventTrigger != null)
                                {
//                                    toBottleEventTrigger.enabled= true;
                                    fromEventTrigger.enabled = true;
                                }
                                gRO.enabled = true;
                                bInteractable = true;
                                CheckWinCondition();
                            });
                        });
                    });
                });
            }
        }
        else
        {
           /* // Return the images to the original bottle if the move can't be done
            *//*foreach (Image imageToMove in imagesToMove)
            {
                fromBottle.AddImage(imageToMove);
            }*/
        }
    }


    private void HighlightBottle(Bottle bottle, bool highlight)
    {
        bottle.GetComponent<Image>().color = highlight ? Color.red : Color.white;
    }

    private void ToggleLine(bool show, Vector3 startPosition, Vector3 endPosition)
    {
        lineRenderer.enabled = show;
        if (show)
        {
            lineRenderer.SetPosition(0, startPosition);
            lineRenderer.SetPosition(1, endPosition);
        }
    }

    //Increases and Checks the filledBottleCount which is the number of bottles that are filled. Number of Fully filled bottles will always be 2 less than the amount of bottles in the scene.
    //then  checks if the bottles are filled with the same color or not.
    public void CheckWinCondition()
    {
        int filledBottleCount = 0;
        bool allBottlesCorrect = true;

        foreach (Bottle bottle in bottles)
        {
            if (bottle.currentFillLevel > 0)
            {
                filledBottleCount++;
                if (!IsBottleFilledWithSameColor(bottle))
                {
                    allBottlesCorrect = false;
                    break;
                }
            }
        }

        if (allBottlesCorrect && filledBottleCount == (bottles.Length - 2))
        {
            currentLevel++;
            uIManager.UpdateLevelText();
            StartNextLevel();
        }
    }

    //Checks if the bottle is filled with the same color or not.
    //retrieves the top color of the bottle ("firstColor"), then checks if all the color of the bottle are same as of the "firstColor"
    private bool IsBottleFilledWithSameColor(Bottle bottle)
    {
        if (bottle.currentFillLevel == 0) return false;

        Color firstColor = bottle.PeekTopImage().color;

        foreach (Image image in bottle.GetAllImages())
        {
            if (image.color != firstColor)
            {
                return false;
            }
        }

        return true;
    }

    private void StartNextLevel()
    {
        bottleManager.SetupLevel(currentLevel);
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        PlayerPrefs.Save();
    }
}
