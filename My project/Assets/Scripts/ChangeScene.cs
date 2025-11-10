using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] string scene;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);

        Debug.Log(distance);

        if (distance < 2)
        {
            text.enabled = true;
        }
        else
        {
            text.enabled = false;
        }
        if (distance < 2 && Keyboard.current.eKey.isPressed)
        {
            SceneManager.LoadScene(scene);
        }
    }
}
