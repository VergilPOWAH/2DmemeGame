﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;
using Random = UnityEngine.Random;

// Represent visual form of itemSlot
public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Text nameField; // Item name
    [SerializeField] private Image iconField; // Item icon
    [SerializeField] private TextMeshProUGUI counterField; // Item quantity
    public ItemSlot itemSlot;

    private bool isDragging;
    private ContainerUI parentContainerUI; // Parent UI container
    private Transform prevParent; // Parent transform before dragging

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && isDragging) // While dragging adds one item to the container under cursor or drops item
        {
            ContainerUI targetContainer = OverlapContainer();
            if (targetContainer == null) // Drops
            {
                DropItems(1);
            }
            else // Adds to container
            {
                if (targetContainer.itemContainer.AddToContainer(itemSlot.item))
                    itemSlot.TakeItem(1);
            }

            UpdateCounter();
        }
    }

    // Inits new slot
    public void Init(ContainerUI draggingParent, ItemSlot slot)
    {
        parentContainerUI = draggingParent;
        nameField.text = slot.item.name;
        iconField.sprite = slot.item.uiIcon;
        itemSlot = slot;
        UpdateCounter();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        InventoryUI.draggableSlot = itemSlot;
        InventoryUI.draggableSlot = itemSlot;
        isDragging = true;
        prevParent = transform.parent;
        transform.parent = parentContainerUI.transform;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button.Equals(PointerEventData.InputButton.Left)) // Drops items slot to the container or to the ground
        {
            isDragging = false;

            ContainerUI targetContainer = OverlapContainer();
            if (targetContainer != null) // To the container
                InsertInGrid(targetContainer);
            else // To the ground
            {
                DropItems(itemSlot.count);
            }

            InventoryUI.draggableSlot = null;
        }
    }

    public void UpdateCounter() // Updates visual counter
    {
        if (itemSlot.count <= 0) // If counter is 0 or less then destroys this slot
        {
            InventoryUI.draggableSlot = null;
            DestroySlot();
        }

        if (itemSlot.count < 2)
            counterField.enabled = false;
        else
        {
            counterField.enabled = true;
            counterField.text = itemSlot.count.ToString();
        }
    }

    public void DropItems(int count) // Ground drop method
    {
        Transform inventory = parentContainerUI.itemContainer.transform;
        Vector2 velocity = new Vector2(Random.Range(20f, 25f) * inventory.transform.parent.lossyScale.x, 0);
        for (int i = 0; i < count; i++)
        {
            Item.SpawnItem(itemSlot.item, inventory.position, velocity);
        }
        itemSlot.TakeItem(count);
        UpdateCounter();
    }
    
    // Uses item if it's usable
    public void UseItem(Transform target)
    {
        if (itemSlot.item is IUsable usable)
        {
            usable.Use(target);
            itemSlot.TakeItem(1);
            UpdateCounter();
        }
    }

    private void InsertInGrid(ContainerUI targetContainer) // Container insertion method
    {
        // int closestIndex = 0;
        //
        // for (int i = 0; i < targetContainer.content.transform.childCount; i++)
        // {
        //     if (Vector3.Distance(transform.position, targetContainer.content.transform.GetChild(i).position) <
        //         Vector3.Distance(transform.position, targetContainer.content.transform.GetChild(closestIndex).position))
        //     {
        //         closestIndex = i;
        //     }
        // }
        //transform.parent = targetContainer.content.transform;
        //transform.SetSiblingIndex(closestIndex);

        if (transform.parent != targetContainer.content.transform)
        {
            for (int i = itemSlot.count; i > 0; --i)
            {
                if (targetContainer.itemContainer.AddToContainer(itemSlot.item)) // Inserts items to the new container 
                {
                    itemSlot.TakeItem(1);
                    UpdateCounter();
                }
                else // If there is no free space in target container returns the rest of items to previous container
                {
                    transform.parent = prevParent;
                    return;
                }
            }

            DestroySlot(); // Destroys slot if all transaction succeed
        }
    }

    private void DestroySlot() // Destroys this slot
    {
        parentContainerUI.itemContainer.RemoveFromContainer(itemSlot);
        Destroy(gameObject);
    }

    private ContainerUI OverlapContainer() // Checks if draggableSlot overlaps any container and returns first of it
    {
        List<RaycastResult> hited = new List<RaycastResult>();
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = Input.mousePosition;


        EventSystem.current.RaycastAll(pointer, hited);

        foreach (RaycastResult hit in hited)
        {
            if (hit.gameObject.CompareTag("Container"))
            {
                return hit.gameObject.GetComponentInParent<ContainerUI>();
            }
        }

        return null;
    }
}