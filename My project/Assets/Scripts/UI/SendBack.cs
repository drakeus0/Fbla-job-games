using UnityEngine;
using UnityEngine.SceneManagement;

public class SendBack : MonoBehaviour
{
    [SerializeField] string scene;

    public void Return() {
        Debug.Log("fjaeofia");
        SceneManager.LoadScene(scene);
    }
    
}
