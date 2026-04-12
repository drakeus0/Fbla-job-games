using UnityEngine;

public class TeleportButtons : MonoBehaviour
{
    [SerializeField] private Transform restaurantPos;
    [SerializeField] private Transform postOfficePos;

    [SerializeField] private Transform player;

    public void TeleportChef()
    {
        CharacterController cc = player.GetComponent<CharacterController>();

        cc.enabled = false;
        player.position = restaurantPos.position;
        cc.enabled = true;

        Debug.Log("Teleported");
    }

    public void TeleportDevivery()
    {
        CharacterController cc = player.GetComponent<CharacterController>();

        cc.enabled = false;
        player.position = postOfficePos.position;
        cc.enabled = true;
    }
}
