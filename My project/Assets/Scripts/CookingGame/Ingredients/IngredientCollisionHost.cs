using System.Runtime.CompilerServices;
using UnityEngine;

public class IngredientCollisionHost : MonoBehaviour
{
    private DraggableIngredientt ingredient;

    private void Awake()
    {
        ingredient = GetComponent<DraggableIngredientt>();
    }

    private void OnCollisionEnter(Collision collision)
    {
       ingredient.NotifyCollisionEnter(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
       ingredient.NotifyCollisionExit(collision);
    }
}


