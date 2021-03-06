﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] private CircleCollider2D interactCollider;

    [Range(0f, 5f)] public float interactionRadius = 1f;

    public abstract void Interact(Transform target);


    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.transform.CompareTag("Interactor"))
            Interact(other.transform);
    }

    protected virtual void OnDrawGizmos()
    {
        interactCollider.radius = interactionRadius;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}