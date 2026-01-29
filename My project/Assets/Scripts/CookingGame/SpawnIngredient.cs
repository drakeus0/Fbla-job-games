using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnIngredient : MonoBehaviour
{
    [SerializeField] GameObject ingredientPrefab;

    [SerializeField] float spawnOffsetX = 1f;
    [SerializeField] float spawnOffsetY = 1f;

    [Header("Cooldown")]
    [SerializeField] float spawnCooldown = 1f;
    private float nextSpawnTime;

    [Header("Pop Animation")]
    [SerializeField] float popScale = 1.15f;
    [SerializeField] float popDuration = 0.15f;

    Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && Time.time >= nextSpawnTime)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform)
                {
                    InstantiateIngredient();
                    nextSpawnTime = Time.time + spawnCooldown;
                }
            }
        }
    }

    private void InstantiateIngredient()
    {
        GameObject spawnedIngredient = Instantiate(
            ingredientPrefab,
            new Vector3(
                transform.position.x + spawnOffsetX,
                transform.position.y + spawnOffsetY,
                5f
            ),
            Quaternion.identity
        );

        Rigidbody rb = spawnedIngredient.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.AddForce(Vector3.up * 2.5f, ForceMode.Impulse);
        }

        transform.DOKill(); 
        transform
            .DOScale(originalScale * popScale, popDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                transform.DOScale(originalScale, popDuration * 0.8f);
            });
    }
}
