using UnityEngine;
using TMPro;

public class DashUI : MonoBehaviour
{
    public TMP_Text dashText;
    public PlayerMovement player;

    private bool dashReadySoundPlayed = false;

    void Update()
    {
        if (dashText == null || player == null)
            return;

        bool isDashReady = GameController.CurrentProgress >= player.dashUnlockRequirement;

        if (isDashReady)
        {
            dashText.text = "DASH READY";

            if (!dashReadySoundPlayed)
            {
                dashReadySoundPlayed = true;

                if (SoundManager.Instance != null)
                    SoundManager.Instance.PlaySound3D("DashReady", player.transform.position);
            }
        }
        else
        {
            int remaining = player.dashUnlockRequirement - GameController.CurrentProgress;
            dashText.text = "Dash locked: " + remaining;

            dashReadySoundPlayed = false;
        }
    }
}