using UnityEngine;

[ExecuteAlways]
public class Grass : MonoBehaviour {
    [SerializeField]
    private Transform m_Character;
    [SerializeField]
    private Material m_Material;

    private void Update() {
        m_Material?.SetVector("_Character", m_Character.position);
    }
}
