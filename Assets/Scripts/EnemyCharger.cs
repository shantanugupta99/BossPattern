using UnityEngine;
using System.Collections;

public class EnemyCharger : MonoBehaviour
{
    public float detectionRange = 5.0f;
    public float chargeSpeed = 10.0f;
    public float waitTime = 1.5f;
    public float cooldownTime = 5.0f; // Duration of cooldown in seconds.
    private float lastChargeTime;


    private bool isCharging = false;
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Debug.Log("Player");
    }

    private void Update()
    {
        if (!isCharging && Time.time - lastChargeTime >= cooldownTime)
        {
            float distanceToPlayer = Vector3.Distance(player.position, transform.position);

            if (distanceToPlayer <= detectionRange)
            {
                StartCoroutine(ChargePlayer());
            }
        }
    }


    private IEnumerator ChargePlayer()
    {
        isCharging = true;

        yield return new WaitForSeconds(waitTime); // Wait before charging.

        Vector3 chargeDirection = (player.position - transform.position).normalized;

        // Charge towards the player until we hit something or the player moves out of range.
        while (Vector3.Distance(player.position, transform.position) > 0.5f)
        {
            transform.position += chargeDirection * chargeSpeed * Time.deltaTime;
            yield return null;
        }

        lastChargeTime = Time.time;
        isCharging = false;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (isCharging)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                // Deduct HP from the player.
                collision.gameObject.GetComponent<ThirdPersonMovement>().TakeDamage(1);
            }

            if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Pillar"))
            {
                StopCoroutine(ChargePlayer()); // Stop the charge.
                isCharging = false;
                // Can add more behavior here like a stunned state or play a sound.
            }
        }
    }
}
