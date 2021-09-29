using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public float lerpSpeed = 5.0f;
    public GameObject player;
    public Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void LateUpdate()
    {
        LerpCamera();
    }

    void LerpCamera()
    {
        Vector3 desiredPos = player.transform.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * lerpSpeed);
    }
}
