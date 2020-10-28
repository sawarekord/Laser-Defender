using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 10f;

    [SerializeField]
    float laserSpeed = 20f;

    [SerializeField]
    float health = 500;

    [SerializeField]
    GameObject laserPrefab;

    [SerializeField]
    float projectileFiringPeriod = 0.05f;

    Coroutine firingCoroutine;

    float xMin;
    float xMax;
    float yMin;
    float yMax;

    void Start()
    {
        SetUpMoveBoundaries();
    }

    private void SetUpMoveBoundaries()
    {
        Camera gameCamera = Camera.main;
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + 0.519f;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - 0.519f;
        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + 0.40f;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - 0.40f;
    }

    void Update()
    {
        Move();
        Fire();
    }

    private void Fire()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            firingCoroutine = StartCoroutine(FireContinuously());
        }
        if (Input.GetButtonUp("Fire1"))
        {
            StopCoroutine(firingCoroutine);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }
        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();

        damageDealer.Hit();

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator FireContinuously()
    {
        while (true)
        {
            GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.identity);
            laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, laserSpeed);

            yield return new WaitForSeconds(projectileFiringPeriod);
        }
    }

    private void Move()
    {
        var newXPos = Mathf.Clamp(transform.position.x + Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime, xMin,xMax);
        var newYPos = Mathf.Clamp(transform.position.y + Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime, yMin, yMax);

        transform.position = new Vector2(newXPos, newYPos);
    }
}
