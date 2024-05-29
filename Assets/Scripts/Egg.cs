using System;
using System.Collections;
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
    private float gravityScale;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        isAlive = true;
        gravityScale = rb.gravityScale;
        rb.gravityScale = 0;

        StartCoroutine(WaitAndFall());
    }
    private IEnumerator WaitAndFall()
    {
        yield return new WaitForSeconds(2);
        rb.gravityScale = gravityScale;
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
            isAlive = false;
            OnFellInWater?.Invoke();
        }
    }
    private void Bounce(Vector2 normal)
    {
        rb.velocity = normal * bounceVelocity;
    }

    public void Reuse()
    {
        transform.position = Vector2.up * 5;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
        rb.gravityScale = 0;
        isAlive = true;
        StartCoroutine(WaitAndFall());
    }
}
