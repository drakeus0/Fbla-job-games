using UnityEngine;
using UnityEngine.UI;

public class CookItem : MonoBehaviour
{
    [SerializeField] private Sprite cookedBurger; 
    [SerializeField] private Sprite burntBurger; 

    [SerializeField] private float cookTimer = 10f; 
    private GameObject currCookedItem;
    public bool cooking;

    private float timer = 0f;

    void Update()
    {
        if (cooking)
        {
            timer += Time.deltaTime;

            if (timer >= (cookTimer + cookTimer))
            {
                currCookedItem.GetComponent<SpriteRenderer>().sprite = burntBurger;
            } else if (timer >= cookTimer)
            {
                currCookedItem.GetComponent<SpriteRenderer>().sprite = cookedBurger;
                currCookedItem.GetComponent<DraggableIngredientt>().canPickUp = true;
            }   
        }
    }

    public void CookFood(GameObject burger)
    {
        cooking = true;
        currCookedItem = burger;
    }

    public void StopCooking()
    {
        cooking = false;
        timer = 0f;
    }
}
