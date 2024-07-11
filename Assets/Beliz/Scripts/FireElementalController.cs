using System.Collections;
using UnityEngine;

public class FireElementalController : MonoBehaviour
{
    public Animator animator;
    public string isWalkingParameter = "walk";
    public float minWaitTime = 2.0f;
    public float maxWaitTime = 5.0f;
    public float speed = 3.0f;
    public string attackParameter = "attack";
    public Vector3 boundaryMin; // Minimum boundary point
    public Vector3 boundaryMax; // Maximum boundary point

    private bool isWalking = false;
    private bool isPlayerDetected = false;

    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        StartCoroutine(PlayRandomAnimation());
    }

    void Update()
    {
        if (isWalking && !isPlayerDetected)
        {
            MoveAndCheckBoundaries();
        }
    }

    IEnumerator PlayRandomAnimation()
    {
        while (true)
        {
            if (!isPlayerDetected)
            {
                isWalking = Random.value > 0.5f;
                animator.SetBool(isWalkingParameter, isWalking);

                float waitTime = Random.Range(minWaitTime, maxWaitTime);
                yield return new WaitForSeconds(waitTime);
            }
            else
            {
                yield return null;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerDetected = true;
            StartCoroutine(AttackPlayer());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerDetected = false;
            animator.SetBool(attackParameter, false);
            animator.SetBool(isWalkingParameter, false);
            StopCoroutine(AttackPlayer());
        }
    }

    IEnumerator AttackPlayer()
    {
        while (isPlayerDetected)
        {
            animator.SetBool(attackParameter, true);

            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

            animator.SetBool(attackParameter, false);
            yield return new WaitForEndOfFrame();
        }
    }

    private void MoveAndCheckBoundaries()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        Vector3 position = transform.position;

        // Check if the character is out of the boundaries
        if (position.x < boundaryMin.x || position.x > boundaryMax.x || position.z < boundaryMin.z || position.z > boundaryMax.z)
        {
            // Determine the direction back towards the center of the boundary
            Vector3 directionToCenter = (new Vector3((boundaryMin.x + boundaryMax.x) / 2, position.y, (boundaryMin.z + boundaryMax.z) / 2) - position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToCenter);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
}