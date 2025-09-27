using UnityEngine;

public class DeadZone : MonoBehaviour
{
    public Transform respawner;
    public FadeManager fadeManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterController cc = other.GetComponent<CharacterController>();
            PlayerMovement pm = other.GetComponent<PlayerMovement>();

            // Ação de respawn encapsulada
            System.Action respawnAction = () =>
            {
                if (cc != null)
                {
                    cc.enabled = false;
                    other.transform.position = respawner.position;
                    cc.enabled = true;
                }
                else
                {
                    other.transform.position = respawner.position;
                }

                if (pm != null)
                    pm.ResetPlayer();
            };

            if (fadeManager != null)
            {
                fadeManager.FadeOutInCinematic(respawnAction);
            }
            else
            {
                // Fallback sem fade
                respawnAction.Invoke();
            }
        }
    }
}