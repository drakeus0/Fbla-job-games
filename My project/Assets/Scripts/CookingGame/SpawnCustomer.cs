using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityGLTF.Interactivity.Schema;

public class SpawnCustomer : MonoBehaviour
{
    [SerializeField] Sprite burgerSprite;
    [SerializeField] Sprite lettuceSprite;
    [SerializeField] Sprite cheeseSprite;
    [SerializeField] Sprite topBunSprite;
    [SerializeField] Sprite bottomBunSprite;

    [SerializeField] Sprite happyFace;
    [SerializeField] Sprite mediumFace;
    [SerializeField] Sprite madFace;

    [SerializeField] GameObject happinessBar;
    private Vector3 happinessBarOriginalScale;

    private Vector3 originalCustomerScale;
    [SerializeField] GameObject textBubble;
    private Vector3 textBubbleOriginalScale;
    [SerializeField] GameObject face;

    [SerializeField] GameObject customer;
    private Vector3 customerStartPosition;

    private Transform currentIngredients;

    VerticalSpriteLayout spriteUpdate;

    private List<IngredientType> ingredientsWanted;

    private static float customerHappiness;
    private float currCustomerHappiness = 3f;
    private static float currentCustomer = 0f;
    private float customerServeAmount = 8f;

    private float customerAngerTime = 85f;
    private float currCustomerTime;
    private bool angerReduced = false;
    private bool angerReduced2 = false;

    private bool customerActive = false;

    [SerializeField] StarAnimate starAnimate;

    private Dictionary<Sprite, IngredientType> spriteToType;
    private void Start()
    {
        happinessBarOriginalScale = happinessBar.transform.localScale;
        textBubbleOriginalScale = textBubble.transform.localScale;
        originalCustomerScale = customer.transform.localScale;
        customer.SetActive(false);
        customerStartPosition = customer.transform.position;
        Debug.Log(customerStartPosition);
        textBubble.SetActive(false);
        face.SetActive(false);
        happinessBar.transform.parent.gameObject.SetActive(false);

        ingredientsWanted = new List<IngredientType>();

        spriteUpdate = textBubble.GetComponent<VerticalSpriteLayout>();

        spriteToType = new Dictionary<Sprite, IngredientType>
        {
            { burgerSprite, IngredientType.WellCookedPatty },
            { lettuceSprite, IngredientType.Lettuce },
            { cheeseSprite, IngredientType.Cheese },
            { topBunSprite, IngredientType.TopBun },
            { bottomBunSprite, IngredientType.BottomBun }
        };
    }

    void Update()
    {
        if (customerActive)
        {
            currCustomerTime += Time.deltaTime;
            if (currCustomerTime >= customerAngerTime/2 && angerReduced == false)
            {
                currCustomerHappiness -= 1;
                angerReduced = true;
            }
            else if (currCustomerTime >= customerAngerTime && angerReduced2 == false)
            {
                currCustomerHappiness -= 1;
                angerReduced2 = true;
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("CookingGameIngredient")) return;

        DraggableIngredientt draggable = other.gameObject.GetComponent<DraggableIngredientt>();
        if (draggable == null) return;

        if (draggable.isDraggingItem && textBubble.activeSelf)
        {
            customer.transform.DOScale(originalCustomerScale * 1.1f, 0.2f).SetEase(Ease.OutBack);
        }

        draggable.Dropped -= CheckIngredients;
        currentIngredients = other.transform.root;
        draggable.Dropped += CheckIngredients;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("CookingGameIngredient")) return;

        DraggableIngredientt draggable = other.gameObject.GetComponent<DraggableIngredientt>();
        if (draggable == null) return;

        // Scale back to normal
        customer.transform.DOScale(originalCustomerScale, 0.2f).SetEase(Ease.InOutSine);

        draggable.Dropped -= CheckIngredients;
        currentIngredients = null;
    }


    private void CheckIngredients()
    {
        if (!currentIngredients) return;

        List<IngredientType> presentIngredients = new List<IngredientType>();
        presentIngredients.Add(currentIngredients.GetComponent<DraggableIngredientt>().ingredientType);

        AddIngredientsRecursive(currentIngredients, presentIngredients);

        //check for missing ingredients
        foreach (IngredientType wanted in ingredientsWanted)
        {
            if (!presentIngredients.Contains(wanted))
            {
                currCustomerHappiness -= 3f;
            }
        }

        //check for extra ingredints
        foreach (IngredientType present in presentIngredients)
        {
            if (!ingredientsWanted.Contains(present))
            {
                currCustomerHappiness -= 1f;
            }
        }
        Destroy(currentIngredients.gameObject);
        EndCustomer();
    }

    void AddIngredientsRecursive(Transform parent, List<IngredientType> list)
    {
        DraggableIngredientt draggable = parent.GetComponent<DraggableIngredientt>();
        if (draggable != null)
        {
            list.Add(draggable.ingredientType);
        }

        foreach (Transform child in parent)
        {
            AddIngredientsRecursive(child, list); // recurse into child
        }
    }


    private void EndCustomer()
    {
        if (textBubble.activeSelf)
        {
            happinessBar.transform.parent.gameObject.SetActive(false);

            SpriteRenderer sr = face.GetComponent<SpriteRenderer>();

            sr.sprite = currCustomerHappiness >= 3 ? happyFace :
                        (currCustomerHappiness > 0 ? mediumFace : madFace);
            currCustomerTime = 0f;
            customerActive = false;
            angerReduced = false;
            angerReduced2 = false;
            PopUpFace();
            AddAndRemoveSpriteSizeMethod(textBubble, textBubbleOriginalScale);
            customerHappiness += currCustomerHappiness;
            currCustomerHappiness = 3f;
            currentCustomer += 1;
            ingredientsWanted.Clear();
            foreach (Transform child in textBubble.transform)
            {
                Destroy(child.gameObject);
            }
            if (currentCustomer >= customerServeAmount)
            {
                float happinessPercentage = customerHappiness / (customerServeAmount * 3) * 100f;

                float stars;
                if (happinessPercentage >= 85f)
                    stars = 3;
                else if (happinessPercentage >= 75f)
                    stars = 2;
                else if (happinessPercentage >= 65f)
                    stars = 1;
                else
                    stars = 0;

                starAnimate.ShowUI(stars);
                Debug.Log(happinessPercentage);
                Debug.Log(customerHappiness);
            }
            DOVirtual.DelayedCall(1f, () => RemoveAndShowCustomer(false));

            if (currentCustomer >= customerServeAmount - 2) return;

            DOVirtual.DelayedCall(Random.Range(8,15), () => RemoveAndShowCustomer(true));
        }
    }

    private void PopUpFace()
    {
        face.SetActive(true);
        SpriteRenderer faceRenderer = face.GetComponent<SpriteRenderer>();
        faceRenderer.color = new Color(1, 1, 1, 0); // start transparent

        Sequence faceSeq = DOTween.Sequence();
        faceSeq.Append(faceRenderer.DOFade(1f, 0.3f));   // fade in
        faceSeq.AppendInterval(0.5f);                     // hold
        faceSeq.Append(faceRenderer.DOFade(0f, 0.3f));   // fade out
        faceSeq.OnComplete(() => face.SetActive(false));
        faceSeq.Play();
    }

    private void AddAndRemoveSpriteSizeMethod(GameObject s, Vector3 os)
    {
        if (!s.activeSelf)
        {
            // Make sure it’s active
            s.SetActive(true);

            // Start smaller than original
            s.transform.localScale = os * 0.5f;

            // Animate pop to slightly bigger, then settle to original
            s.transform.DOScale(os * 1.1f, 0.3f)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    s.transform.DOScale(os, 0.2f)
                        .SetEase(Ease.InOutSine);
                });
        }
        else
        {
            // Start at current scale (or full size)
            s.transform.localScale = os;

            // Animate shrink
            s.transform.DOScale(os * 0.5f, 0.3f)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    // Deactivate after animation
                    s.SetActive(false);
                });
        }
    }




    public void RemoveAndShowCustomer(bool active)
    {
        // Ensure the customer is active so it can move
        customer.SetActive(true);

        // Kill any previous tweens on the customer
        customer.transform.DOKill();
        Vector3 startPos = customerStartPosition; // Assign this at initialization
        float exitDistance = 5f; // How far he moves down when exiting

        if (active)
        {
            // Start below the start position
            customer.transform.position = startPos - Vector3.up * exitDistance;

            // Move up to start position
            customer.transform.DOMove(startPos, 0.5f)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    // Trigger speech bubble and bar after reaching start position
                    SpawnSpeachBubble();
                    AnimateBar();
                });

            customerActive = true;
        }
        else
        {
            // Move customer down by exitDistance
            Vector3 targetPos = customer.transform.position - Vector3.up * exitDistance;

            customer.transform.DOMove(targetPos, 0.5f)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    customer.SetActive(false);
                });

            customerActive = false;
        }
    }



    private void AnimateBar()
    {
        happinessBar.transform.parent.gameObject.SetActive(true);
        // Reset scale & color
        happinessBar.transform.localScale = happinessBarOriginalScale;
        SpriteRenderer sr = happinessBar.GetComponent<SpriteRenderer>();
        sr.color = Color.green;

        // Tween the scale Y to 0
        happinessBar.transform.DOScaleY(0, customerAngerTime)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                float t = happinessBar.transform.localScale.y / happinessBarOriginalScale.y; // 1 = full, 0 = empty

                // Change color based on normalized value
                if (t > 0.5f)
                {
                    sr.color = Color.Lerp(Color.orange, Color.green, (t - 0.5f) * 2f);
                }
                else
                {
                    sr.color = Color.Lerp(Color.red, Color.orange, t * 2f);
                }
                if (!customerActive)
                {
                    happinessBar.transform.DOKill();
                }
            });
    }

    private void SpawnSpeachBubble()
    {
        if (!textBubble.activeSelf)
        {
            AddAndRemoveSpriteSizeMethod(textBubble, textBubbleOriginalScale);

            SpawnIngredientDialogue(topBunSprite);

            bool lettuce = Random.Range(0, 2) == 1;
            if (lettuce) SpawnIngredientDialogue(lettuceSprite);

            bool cheese = Random.Range(0, 2) == 1;
            if (cheese) SpawnIngredientDialogue(cheeseSprite);

            SpawnIngredientDialogue(burgerSprite);

            SpawnIngredientDialogue(bottomBunSprite);
        }
    }

    private void SpawnIngredientDialogue(Sprite ingredientImage)
    {
        if (spriteToType.TryGetValue(ingredientImage, out IngredientType type))
        {
            ingredientsWanted.Add(type);
        }

        GameObject spawnedIngredientImage = new GameObject("IngredientUI");
        SpriteRenderer spriteRenderer = spawnedIngredientImage.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = ingredientImage;
        spawnedIngredientImage.transform.parent = textBubble.transform;
        spriteRenderer.sortingOrder = 2;
        spawnedIngredientImage.transform.localScale *= 0.2f;
        spriteUpdate.RefreshStack();
    }
}
