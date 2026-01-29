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

    [SerializeField] GameObject customer;
    private Vector3 originalCustomerScale;
    [SerializeField] GameObject textBubble;
    private Vector3 textBubbleOriginalScale;
    [SerializeField] GameObject face;

    private Transform currentIngredients;

    VerticalSpriteLayout spriteUpdate;

    private List<IngredientType> ingredientsWanted;

    private float customerHappiness;
    private float currCustomerHappiness = 3f;
    private float currentCustomer = 0f;

    private float customerAngerTime = 30f;
    private float currCustomerTime;
    private bool angerReduced = false;
    private bool angerReduced2 = false;

    private bool customerActive = false;

    private Dictionary<Sprite, IngredientType> spriteToType;
    private void Start()
    {
        textBubbleOriginalScale = textBubble.transform.localScale;
        originalCustomerScale = customer.transform.localScale;
        textBubble.SetActive(false);
        customer.SetActive(true);
        face.SetActive(false);

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
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform)
                {
                    SpawnSpeachBubble();
                    customerActive = true;
                }
            }
        }
        if (customerActive)
        {
            currCustomerTime += Time.deltaTime;
            if (currCustomerTime >= customerAngerTime && angerReduced == false)
            {
                currCustomerHappiness -= 1;
                angerReduced = true;
            }
            else if (currCustomerTime >= customerAngerTime + 10 && angerReduced2 == false)
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
            SpriteRenderer sr = face.GetComponent<SpriteRenderer>();

            sr.sprite = currCustomerHappiness >= 3 ? happyFace :
                        (currCustomerHappiness > 0 ? mediumFace : madFace);
            Debug.Log(currCustomerTime);
            currCustomerTime = 0f;
            customerActive = false;
            angerReduced = false;
            angerReduced2 = false;
            //show this and remove text bubble at the same time
            PopUpFace();
            //say phrase
            AddAndRemoveTextBubble(false);

            //once popupface animation is done, play the customer remove animation
            DOVirtual.DelayedCall(1f, () => RemoveAndShowCustomer(false));
            //wait a second, then add the next customer
            DOVirtual.DelayedCall(2f, () => RemoveAndShowCustomer(true));

            //reset varfiables
            currCustomerHappiness = 3f;
            ingredientsWanted.Clear();
            foreach (Transform child in textBubble.transform)
            {
                Destroy(child.gameObject);
            }
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

    private void AddAndRemoveTextBubble(bool active)
    {
        if (active)
        {
            // Make sure it’s active
            textBubble.SetActive(true);

            // Start smaller than original
            textBubble.transform.localScale = textBubbleOriginalScale * 0.5f;

            // Animate pop to slightly bigger, then settle to original
            textBubble.transform.DOScale(textBubbleOriginalScale * 1.1f, 0.3f)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    textBubble.transform.DOScale(textBubbleOriginalScale, 0.2f)
                        .SetEase(Ease.InOutSine);
                });
        }
        else
        {
            // Start at current scale (or full size)
            textBubble.transform.localScale = textBubbleOriginalScale;

            // Animate shrink
            textBubble.transform.DOScale(textBubbleOriginalScale * 0.5f, 0.3f)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    // Deactivate after animation
                    textBubble.SetActive(false);
                });
        }
    }




    private void RemoveAndShowCustomer(bool active)
    {
        customer.SetActive(true);

        Vector3 startPos = customer.transform.position;
        Vector3 targetPos = startPos + (active ? Vector3.up * 5f : Vector3.down * 5f); // slide up or down

        customer.transform.position = active ? startPos - Vector3.up * 5f : startPos; // if showing, start below

        customer.transform.DOMove(targetPos, 0.5f).SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                if (!active) customer.SetActive(false);
            });
    }

    private void SpawnSpeachBubble()
    {
        if (!textBubble.activeSelf)
        {
            AddAndRemoveTextBubble(true);

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
        spawnedIngredientImage.transform.localScale *= 0.6f;
        spriteUpdate.RefreshStack();
    }
}
