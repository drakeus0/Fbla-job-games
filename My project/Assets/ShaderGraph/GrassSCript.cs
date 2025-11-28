using UnityEngine;

[ExecuteAlways]

public class GrassSCript : MonoBehaviour
{
    [SerializeField] private Transform grassParent;  // drag the parent here
    [SerializeField] private Material grassMaterial;

    private void Update()
    {
        if (grassParent != null && grassMaterial != null)
        {
            grassMaterial.SetVector("_ParentPos", grassParent.position);
        }
    }
}
