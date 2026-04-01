using UnityEngine;
using TMPro;

public class DashUI : MonoBehaviour
{
    public TMP_Text dashText;
    public PlayerMovement player;

    void Update()
    {
        if (dashText == null || player == null)
            return;

        if (GameController.CurrentProgress >= player.dashUnlockRequirement)
        {
            dashText.text = "DASH READY";
        }
        else
        {
            int remaining = player.dashUnlockRequirement - GameController.CurrentProgress;
            dashText.text = "Dash locked: " + remaining;
        }
    }
}