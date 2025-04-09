using UnityEngine;
using TMPro;

public class ExitDetector : MonoBehaviour
{
    public Transform exit; // We need to know about the exit object explicitly in order to detect it
    public float facingThreshold = 0.8f; // angle tolerance for detecting whether the player faces the exit
    public TextMeshProUGUI exitAlertText; // text to show when the player faces the exit

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // get the direction to the exit
        Vector3 directionToExit = exit.position - transform.position;

        // use dot product to get a value indicating how closely the detector is facing the exit direction
        float facing = Vector3.Dot(transform.forward, directionToExit.normalized);

        // check if player is facing the exit within the tolerance set by the threshold
        bool isFacingExit = (facing > facingThreshold);

        // enable or disable the text based on whether the player faces the exit
        exitAlertText.enabled = isFacingExit;
    }
}
