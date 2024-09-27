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
            if (selectedBottle != clickedBottle && clickedBottle.currentFillLevel < clickedBottle.maxCapacity)
            {
                MoveLiquid(selectedBottle, clickedBottle);
            }
            HighlightBottle(selectedBottle, false);
            selectedBottle = null;
        }
    }

    public void MoveLiquid(Bottle fromBottle, Bottle toBottle)
    {
        EventTrigger fromEventTrigger = fromBottle.GetComponent<EventTrigger>();

        if (fromBottle == null || toBottle == null || fromBottle == toBottle) return;

        List<Image> imagesToMove = new List<Image>();
        bool canMove = false;
        Color transferColor = Color.white;

        // Gather consecutive images with the same color from the top of the fromBottle
        while (fromBottle.currentFillLevel > 0)
        {
            Image currentImage = fromBottle.RemoveImage();
            if (currentImage == null) break;
            //Debug.Log("IMAGE TO MOVE: " + imagesToMove[imagesToMove.Count].color);


            if (imagesToMove.Count == 0 || imagesToMove[0].color == currentImage.color)
            {
                imagesToMove.Add(currentImage);
                transferColor = currentImage.color; // Using color instead of sprite
            }
            else
            {
                fromBottle.AddImage(currentImage);
                break;
            }
        }
        Debug.Log("Image to move count" + imagesToMove.Count);
        if (imagesToMove.Count == 0) return;


        // Determine if we can move images based on the destination bottle's state
        canMove = (toBottle.currentFillLevel == 0 || (toBottle.PeekTopImage().color == imagesToMove[0].color));
        Debug.Log("can move " + canMove);
        Debug.Log("Curremt fill level " + toBottle.currentFillLevel+"    " + imagesToMove.Count);
        if (canMove && (toBottle.currentFillLevel + imagesToMove.Count <= toBottle.maxCapacity))
        {
            Debug.Log("Interactable : " + bInteractable);
            if (bInteractable) 
            {
                bInteractable = false;
            }
            else
            {
                return;
            }
            
            if (fromEventTrigger != null)
            {
                fromEventTrigger.enabled = false;
            }

            Vector3 originalPosition = fromBottle.transform.position;
            Vector3 targetPosition = toBottle.transform.position + new Vector3(0, 0.5f, 0);
            gRO.enabled = false;

            // Step 1: Move the bottle to the target bottle's position
            fromBottle.transform.DOMove(targetPosition, 1f).OnComplete(() =>
            {
               
                fromBottle.transform.DORotate(new Vector3(0, 0, 45f), 1f).OnComplete(() =>
                {
                    // Start moving liquid images and animate their fill
                    ToggleLine(true, fromBottle.transform.position, toBottle.transform.position);

                    int delay = 0;
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
                    if (i ==0)
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
                    fromBottle.transform.DORotate(Vector3.zero, 1f).SetDelay(delay).OnComplete(() =>
                    {
                        // Step 5: Stop the line rendering
                        ToggleLine(false, fromBottle.transform.position, toBottle.transform.position);

                        // Step 6: Move the bottle back to its original position
                        fromBottle.transform.DOMove(originalPosition, 1f).OnComplete(() =>
                        {
                            // Enable event trigger and check win condition after completion
                            if (fromEventTrigger != null)
                            {
                                fromEventTrigger.enabled = true;
                            }
                            CheckWinCondition();
                            gRO.enabled = true;
                            bInteractable = true;
                        });
                    });

                    /*// Step 3: Use a single image to represent the transfer animation
                GameObject tempImageObj = Instantiate(imagesToMove[0].gameObject, null); // Create a temporary image object
                tempImageObj.transform.SetParent(toBottle.transform, false);
                Image tempImage = tempImageObj.GetComponent<Image>();

                // Set initial properties for the animation
                tempImage.fillMethod = Image.FillMethod.Vertical;
                tempImage.fillAmount = 0f;
                tempImage.color = transferColor; // Use the color of the liquid being transferred*/

                    // Animate the liquid transfer with a single sprite
                    /*tempImage.DOFillAmount(1f, 1f).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        // Animate Radial180 fill on the images in the fromBottle
                        foreach (Image imageToMove in imagesToMove)
                        {
                            imageToMove.fillMethod = Image.FillMethod.Radial180;
                            imageToMove.fillClockwise = true;
                            // Animate the radial fill as the liquid drains from the fromBottle
                            imageToMove.DOFillAmount(0f, 1f).SetEase(Ease.Linear).OnComplete(() =>
                            {
                                Destroy(imageToMove.gameObject);
                            });
                        }



                        // Destroy the temporary image used for the animation
                        Destroy(tempImageObj);

                        // Step 4: Return the bottle to its original rotation

                    });*/
                });
            });
        }
        else
        {
            // Return the images to the original bottle if the move can't be done
            /*foreach (Image imageToMove in imagesToMove)
            {
                fromBottle.AddImage(imageToMove);
            }*/
        }
    }


    //public void MoveLiquid(Bottle fromBottle, Bottle toBottle)
    //{
    //    EventTrigger fromEventTrigger = fromBottle.GetComponent<EventTrigger>();

    //    if (fromBottle == null || toBottle == null || fromBottle == toBottle) return;

    //    List<Image> imagesToMove = new List<Image>();
    //    bool canMove = false;
    //    Color transferColor = Color.white;

    //    // Gather consecutive images with the same color from the top of the fromBottle
    //    while (fromBottle.currentFillLevel > 0)
    //    {
    //        Image currentImage = fromBottle.RemoveImage();
    //        if (currentImage == null) break;

    //        if (imagesToMove.Count == 0 || imagesToMove[0].color == currentImage.color)
    //        {
    //            imagesToMove.Add(currentImage);
    //            transferColor = currentImage.color; // Using color instead of sprite
    //        }
    //        else
    //        {
    //            fromBottle.AddImage(currentImage);
    //            break;
    //        }
    //    }

    //    if (imagesToMove.Count == 0) return;

    //    // Determine if we can move images based on the destination bottle's state
    //    canMove = (toBottle.currentFillLevel == 0 || (toBottle.PeekTopImage()?.color == imagesToMove[0].color));

    //    if (canMove && (toBottle.currentFillLevel + imagesToMove.Count <= toBottle.maxCapacity))
    //    {
    //        if (fromEventTrigger != null)
    //        {
    //            fromEventTrigger.enabled = false;
    //        }

    //        Vector3 originalPosition = fromBottle.transform.position;
    //        Vector3 targetPosition = toBottle.transform.position + new Vector3(0, 0.5f, 0);
    //        gRO.enabled = false;

    //        // Step 1: Move the bottle to the target bottle's position
    //        fromBottle.transform.DOMove(targetPosition, 1f).OnComplete(() =>
    //        {
    //            // Step 2: Rotate the bottle by 45 degrees
    //            fromBottle.transform.DORotate(new Vector3(0, 0, 45f), 1f).OnComplete(() =>
    //            {
    //                // Start moving liquid images and animate their fill
    //                ToggleLine(true, fromBottle.transform.position, toBottle.transform.position);

    //                // Step 3: Use a single image to represent the transfer animation
    //                GameObject tempImageObj = Instantiate(imagesToMove[0].gameObject, null); // Create a temporary image object


    //                tempImageObj.transform.SetParent(toBottle.transform, false);
    //                Image tempImage = tempImageObj.GetComponent<Image>();


    //                // Set initial properties for the animation
    //                tempImage.fillMethod = Image.FillMethod.Vertical;
    //                tempImage.fillAmount = 0f;
    //                tempImage.color = transferColor; // Use the color of the liquid being transferred

    //                // Animate the liquid transfer with a single sprite
    //                tempImage.DOFillAmount(1f, 1f).SetEase(Ease.Linear).OnComplete(() =>
    //                {
    //                // After the animation completes, transfer all images to the toBottle at once
    //                for (int i = imagesToMove.Count - 1; i >= 0; i--)
    //                {
    //                    Image imageToMove = imagesToMove[i];
    //                    imageToMove.fillMethod = Image.FillMethod.Radial180;
    //                    imageToMove.fillClockwise = true;
    //                    // Destroy the original image from the fromBottle
    //                    imageToMove.DOFillAmount(0.0f, 1.0f).SetEase(Ease.Linear).OnComplete(()=>
    //                        {
    //                        Destroy(imageToMove.gameObject);

    //                    });
    //                    // Clone the image into the toBottle
    //                    GameObject newImageObj = Instantiate(imageToMove.gameObject, toBottle.transform);
    //                    Image newImage = newImageObj.GetComponent<Image>();

    //                    newImage.fillAmount = 1f; // Set fill amount directly to full since the transfer is done
    //                    newImage.color = transferColor;

    //                    // Add the new image to the toBottle
    //                    toBottle.AddImage(newImage);


    //                    }

    //                    // Destroy the temporary image used for the animation
    //                    Destroy(tempImageObj);

    //                    // Step 4: Return the bottle to its original rotation
    //                    fromBottle.transform.DORotate(Vector3.zero, 1f).OnComplete(() =>
    //                    {
    //                        // Step 5: Stop the line rendering
    //                        ToggleLine(false, fromBottle.transform.position, toBottle.transform.position);

    //                        // Step 6: Move the bottle back to its original position
    //                        fromBottle.transform.DOMove(originalPosition, 1f).OnComplete(() =>
    //                        {
    //                            // Enable event trigger and check win condition after completion
    //                            if (fromEventTrigger != null)
    //                            {
    //                                fromEventTrigger.enabled = true;
    //                            }
    //                            CheckWinCondition();
    //                            gRO.enabled = true;
    //                        });
    //                    });
    //                });
    //            });
    //        });
    //    }
    //    else
    //    {
    //        // Return the images to the original bottle if the move can't be done
    //        foreach (Image imageToMove in imagesToMove)
    //        {
    //            fromBottle.AddImage(imageToMove);
    //        }
    //    }
    //}

    /*public void MoveLiquid(Bottle fromBottle, Bottle toBottle)
    {
        EventTrigger fromEventTrigger = fromBottle.GetComponent<EventTrigger>();

        if (fromBottle == null || toBottle == null || fromBottle == toBottle) return;

        List<Image> imagesToMove = new List<Image>();
        bool canMove = false;
        Color transferColor = Color.white;

        // Gather consecutive images with the same color from the top of the fromBottle
        while (fromBottle.currentFillLevel > 0)
        {
            Image currentImage = fromBottle.RemoveImage();
            if (currentImage == null) break;

            if (imagesToMove.Count == 0 || imagesToMove[0].color == currentImage.color)
            {
                imagesToMove.Add(currentImage);
                transferColor = currentImage.color; // Using color instead of sprite
            }
            else
            {
                fromBottle.AddImage(currentImage);
                break;
            }
        }

        if (imagesToMove.Count == 0) return;

        // Determine if we can move images based on the destination bottle's state
        canMove = (toBottle.currentFillLevel == 0 || (toBottle.PeekTopImage()?.color == imagesToMove[0].color));

        if (canMove && (toBottle.currentFillLevel + imagesToMove.Count <= toBottle.maxCapacity))
        {
            if (fromEventTrigger != null)
            {
                fromEventTrigger.enabled = false;
            }
           
            Vector3 originalPosition = fromBottle.transform.position;
            Vector3 targetPosition = toBottle.transform.position + new Vector3(0, 0.5f, 0);
            gRO.enabled = false;
            // Step 1: Move to target bottle position
            fromBottle.transform.DOMove(targetPosition, 1f).OnComplete(() =>
            {
                // Step 2: Rotate the bottle by 45 degrees
                fromBottle.transform.DORotate(new Vector3(0, 0, 45f), 1f).OnComplete(() =>
                {
                    // Start moving liquid images and animate their fill
                    ToggleLine(true, fromBottle.transform.position, toBottle.transform.position);

                    // Step 3: Transfer liquid images
                    for (int i = imagesToMove.Count - 1; i >= 0; i--)
                    {
                        Image imageToMove = imagesToMove[i];
                        GameObject go = imageToMove.gameObject;

                        // Clone the image into the toBottle instead of moving it
                        GameObject newImageObj = Instantiate(go, null);
                        newImageObj.transform.SetParent(toBottle.transform, false);
                       Image newImage = newImageObj.GetComponent<Image>();

                        // Set properties of the new image
                        newImage.transform.localPosition = Vector3.zero;
                        newImage.transform.localScale = Vector3.one;

                        newImage.fillMethod = Image.FillMethod.Vertical;
                        newImage.fillAmount = 0f; // Start with no fill

                        // Animate the fill amount to 1 over 1 second
                        newImage.DOFillAmount(1f, 1f).SetEase(Ease.Linear);

                        // Update the line renderer colors
                        lineRenderer.startColor = transferColor;
                        lineRenderer.endColor = transferColor;

                        // Add the new image to the toBottle
                        toBottle.AddImage(newImage);

                        // Destroy the original image from the fromBottle after the transfer
                        Destroy(imageToMove.gameObject);
                    }

                    // Step 4: Return the bottle to its original rotation
                    fromBottle.transform.DORotate(Vector3.zero, 1f).OnComplete(() =>
                    {
                        // Step 5: Stop the line rendering
                        ToggleLine(false, fromBottle.transform.position, toBottle.transform.position);

                        // Step 6: Move the bottle back to its original position
                        fromBottle.transform.DOMove(originalPosition, 1f).OnComplete(() =>
                        {
                            // Enable event trigger and check win condition after completion
                            if (fromEventTrigger != null)
                            {
                                fromEventTrigger.enabled = true;
                            }
                            CheckWinCondition();
                            gRO.enabled = true;

                        });
                    });
                });
            });
        }
        else
        {
            // Return the images to the original bottle if the move can't be done
            foreach (Image imageToMove in imagesToMove)
            {
                fromBottle.AddImage(imageToMove);
            }
        }
    }
*/


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
