using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
public Bottle[] bottles;
private Bottle selectedBottle = null;
Bottle bottle = new Bottle();
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
        if (selectedBottle != clickedBottle && clickedBottle.currentFillLevel < bottle.maxCapacity)
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

  
    public void MoveLiquid(Bottle fromBottle, Bottle toBottle)
    {
        Debug.Log("Inside Move Liquid: Moving from " + fromBottle.name + " to " + toBottle.name);

        if (fromBottle == null || toBottle == null || fromBottle == toBottle) return;

        // Remove the image from the 'fromBottle'
        Image imageToMove = fromBottle.RemoveImage();

        if (imageToMove != null)
        {
            // Set the image's parent to the target bottle
            imageToMove.transform.SetParent(toBottle.transform);
            imageToMove.transform.localPosition = new Vector3(0, 0, 0);
            imageToMove.transform.localScale = new Vector3(1, 1, 1);

            // Add the image to the target bottle
            bool success = toBottle.AddImage(imageToMove);

            if (success)
            {
                Debug.Log("Moved image to " + toBottle.name);
            }
            else
            {
                Debug.Log("Cannot add image to " + toBottle.name);
            }
        }
        else
        {
            Debug.Log("No image to move from " + fromBottle.name);
        }
    }


    private void HighlightBottle(Bottle bottle, bool highlight)
{
    if (highlight)
    {
        bottle.GetComponent<Image>().color = Color.red;
    }
    else
    {
        bottle.GetComponent <Image>().color = Color.white;
    }

}
}
