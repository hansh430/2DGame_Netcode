using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviour
{
    [Header("Physics Settings")]
    [SerializeField] private float bounceVelocity;

    [Header("Events")]
    public static Action OnHit;
    public static Action OnFellInWater;

    private Rigidbody2D rb;
    private bool isAlive = true;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isAlive)
            return;

        if (collision.collider.TryGetComponent(out PlayerController playerController))
        {
            Bounce(collision.GetContact(0).normal);
            OnHit?.Invoke();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isAlive)
            return;

        if (collision.CompareTag("Water"))
        {
            OnFellInWater?.Invoke();
            isAlive = false;
        }
    }
    private void Bounce(Vector2 normal)
    {
        rb.velocity = normal*bounceVelocity;
    }
}
