using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviour
{
    [Header("Physics Settings")]
    private Rigidbody2D rb;
    [SerializeField] private float bounceVelocity;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.TryGetComponent(out PlayerController playerController))
        {
            Bounce(collision.GetContact(0).normal);
        }
    }
    private void Bounce(Vector2 normal)
    {
        rb.velocity = normal*bounceVelocity;
    }
}
