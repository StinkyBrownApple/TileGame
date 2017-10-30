using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierTest : MonoBehaviour {

    [SerializeField]
    Transform PosA;
    [SerializeField]
    Transform PosB;
    [SerializeField]
    Transform PosC;

    Vector2 posA;
    Vector2 posB;
    Vector2 posC;

    public float multiplier = 5f;
    public float t = 0f;

    void Start () 
	{
        transform.position = posA;
    }
	
	void Update () 
	{
        posA = PosA.position;
        posB = PosB.position;
        posC = PosC.position;

        if (t < 1)
        {
            transform.position = Vector2.Lerp(posA, Vector2.Lerp(Vector2.Lerp(posA, posB, t), Vector2.Lerp(posB, posC, t), t), t);
            t += Time.deltaTime;
        }
        else
        {
            t = 0;
            transform.position = posA;
        }
    }
}
