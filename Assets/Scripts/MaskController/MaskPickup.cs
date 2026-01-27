using UnityEngine;

public class MaskPickup : MonoBehaviour
{
    [Tooltip("The index of the mask in the Player's Inspector list (0 = 1st mask, 1 = 2nd mask...)")]
    public int maskIndexToUnlock = 0;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMaskManager manager = other.GetComponent<PlayerMaskManager>();
            
            if (manager != null)
            {
                manager.UnlockMask(maskIndexToUnlock);
                
                Destroy(gameObject);
            }
        }
    }
}