using UnityEngine;

public class CookItem : MonoBehaviour
{
    [SerializeField] private Sprite cookedBurger;
    [SerializeField] private Sprite burntBurger;
    [SerializeField] private float cookDuration = 10f;

    private CookableIngredient currentItem;
    public bool cooking;

    private void Update()
    {
        if (!cooking || currentItem == null)
            return;

        currentItem.cookTimer += Time.deltaTime;

        if (currentItem.cookTimer >= cookDuration * 2f)
        {
            if (currentItem.cookState != CookState.Burnt)
            {
                currentItem.cookState = CookState.Burnt;
                currentItem.GetComponent<SpriteRenderer>().sprite = burntBurger;
                currentItem.GetComponent<DraggableIngredientt>().ingredientType = IngredientType.BurntPatty;
            }
        }
        else if (currentItem.cookTimer >= cookDuration)
        {
            if (currentItem.cookState != CookState.Cooked)
            {
                currentItem.cookState = CookState.Cooked;
                currentItem.GetComponent<SpriteRenderer>().sprite = cookedBurger;
                currentItem.GetComponent<DraggableIngredientt>().ingredientType = IngredientType.WellCookedPatty;
            }
        }
    }

    public void CookFood(CookableIngredient item)
    {
        currentItem = item;
        cooking = true;
    }

    public void StopCooking()
    {
        cooking = false;
        currentItem = null;
    }
}
