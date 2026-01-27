using System.Collections.Generic;
using UnityEngine;

public class PlayerMaskManager : MonoBehaviour
{
    [System.Serializable]
    public class MaskEntry
    {
        public string maskName;
        public GameObject maskVisual;
        public bool isUnlocked;
        public Monster.FactionType faction;
    }

    [Header("Mask Inventory")]
    public List<MaskEntry> masks = new List<MaskEntry>();
    
    private int _currentMaskIndex = -1; 
    
    public Monster.FactionType CurrentFaction
    {
        get
        {
            if (_currentMaskIndex >= 0 && _currentMaskIndex < masks.Count)
            {
                return masks[_currentMaskIndex].faction;
            }
            return (Monster.FactionType)999; 
        }
    }

    void Start()
    {
        foreach (var mask in masks)
        {
            if(mask.maskVisual != null) mask.maskVisual.SetActive(false);
        }
        
        if (masks.Count > 0 && masks[0].isUnlocked)
            EquipMask(0);
    }

    void Update()
    {
        HandleNumberInput();
        HandleScrollInput();
    }
    
    void HandleNumberInput()
    {
        for (int i = 0; i < masks.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                EquipMask(i);
            }
        }
    }
    
    void HandleScrollInput()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (scroll > 0f)
            {
                CycleMasks(1);
            }
            else if (scroll < 0f)
            {
                CycleMasks(-1);
            }
        }
    }

    void CycleMasks(int direction)
    {
        if (masks.Count == 0) return;

        int newIndex = _currentMaskIndex + direction;
        
        if (newIndex >= masks.Count) newIndex = 0;
        if (newIndex < 0) newIndex = masks.Count - 1;

        EquipMask(newIndex);
    }

    public void EquipMask(int index)
    {
        if (index < 0 || index >= masks.Count) return;
        
        if (!masks[index].isUnlocked)
        {
            Debug.Log($"Mask '{masks[index].maskName}' is locked!");
            return;
        }
        
        if (_currentMaskIndex >= 0 && _currentMaskIndex < masks.Count)
        {
            if (masks[_currentMaskIndex].maskVisual != null)
                masks[_currentMaskIndex].maskVisual.SetActive(false);
        }
        
        _currentMaskIndex = index;
        
        if (masks[_currentMaskIndex].maskVisual != null)
            masks[_currentMaskIndex].maskVisual.SetActive(true);

        Debug.Log($"Equipped: {masks[_currentMaskIndex].maskName}");
    }
    
    public void UnlockMask(int index)
    {
        if (index >= 0 && index < masks.Count)
        {
            if (!masks[index].isUnlocked)
            {
                masks[index].isUnlocked = true;
                Debug.Log($"Unlocked Mask: {masks[index].maskName}");
            }
        }
    }
}