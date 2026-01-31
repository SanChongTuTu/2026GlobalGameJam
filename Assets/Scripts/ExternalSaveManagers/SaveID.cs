using UnityEngine;

public class SaveID : MonoBehaviour
{
    [Header("Do not change this manually unless necessary")]
    public string uniqueID;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(uniqueID))
        {
            uniqueID = System.Guid.NewGuid().ToString();
        }
    }
}