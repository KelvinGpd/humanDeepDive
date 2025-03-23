using UnityEngine;
using System.Collections;
using System.Threading;
using UnityEngine.UIElements;
using System;

public class TestOrgan : OrganInterface
{
    private Vector3 originalPosition;
    public float moveDistance = -10f;
    public float moveSpeed = 1.3f;
    public float spinSpeed = 180f;
    private Quaternion originalRotation;
    

    private void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    override
    public void MoveBack()
    {
        StopAllCoroutines();
        StartCoroutine(MoveToPosition(originalPosition));
        StartCoroutine(RotateToOriginal());
    }

    private IEnumerator RotateToOriginal()
    {
        while (Quaternion.Angle(transform.rotation, originalRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, originalRotation, Time.deltaTime * moveSpeed);
            yield return null;
        }
        transform.rotation = originalRotation; 
    }

    override
    public void MoveToSpotlight(Transform targetPosition)
    {
        Debug.Log("Brain is moving to the spotlight.");
        StopAllCoroutines();
        StartCoroutine(MoveToPosition(targetPosition.position));
        StartCoroutine(SpinContinuously());
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
            yield return null;
        }
        transform.position = targetPosition;
    }
    private IEnumerator SpinContinuously()
    {
        while (true)
        {
            transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime); // Spin around Y-axis
            yield return null; // Wait until next frame
        }
    }
}
