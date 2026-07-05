using UnityEngine;
using System.Collections;

public class HammerController : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public Animator animator;

    public float moveSpeed = 4f;
    public float rotateSpeed = 720f;
    public int hitCount = 3;
    public string hitStateName = "Hit";

    private bool isMoving = false;
    private bool isWaitingAtB = false;
    private bool isPlayingHits = false;

    private Quaternion startLocalRotation;
    private readonly Quaternion hitLocalRotation = Quaternion.Euler(-90f, 0f, 0f);

    private void Start()
    {
        startLocalRotation = transform.localRotation;

        if (pointA != null)
        {
            transform.position = pointA.position;
        }
    }

    private void OnMouseDown()
    {
        if (isMoving || isPlayingHits)
            return;

        if (!isWaitingAtB)
        {
            StartCoroutine(MoveToPointB());
        }
        else
        {
            StartCoroutine(HitSequence());
        }
    }

    IEnumerator MoveToPointB()
    {
        if (pointA == null || pointB == null)
            yield break;

        isMoving = true;

        while (Vector3.Distance(transform.position, pointB.position) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, pointB.position, moveSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, hitLocalRotation, rotateSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = pointB.position;
        transform.localRotation = hitLocalRotation;

        isMoving = false;
        isWaitingAtB = true;
    }

    IEnumerator HitSequence()
    {
        if (animator == null)
            yield break;

        isPlayingHits = true;
        isWaitingAtB = false;

        for (int i = 0; i < hitCount; i++)
        {
            animator.ResetTrigger("Hit");
            animator.SetTrigger("Hit");

            yield return StartCoroutine(WaitForHitAnimation());

            transform.localRotation = hitLocalRotation;
        }

        yield return StartCoroutine(ReturnToPointA());
        isPlayingHits = false;
    }

    IEnumerator WaitForHitAnimation()
    {
        if (animator == null)
            yield break;

        int layerIndex = 0;
        int hitHash = Animator.StringToHash(hitStateName);

        float timeout = 2f;
        float wait = 0f;

        while (!animator.GetCurrentAnimatorStateInfo(layerIndex).IsName(hitStateName) && wait < timeout)
        {
            wait += Time.deltaTime;
            yield return null;
        }

        if (wait >= timeout)
            yield break;

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(layerIndex);
        while (state.IsName(hitStateName) && state.normalizedTime < 1f)
        {
            transform.localRotation = hitLocalRotation;
            yield return null;
            state = animator.GetCurrentAnimatorStateInfo(layerIndex);
        }
    }

    IEnumerator ReturnToPointA()
    {
        if (pointA == null)
            yield break;

        isMoving = true;

        while (Vector3.Distance(transform.position, pointA.position) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, pointA.position, moveSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, startLocalRotation, rotateSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = pointA.position;
        transform.localRotation = startLocalRotation;

        isMoving = false;
    }
}