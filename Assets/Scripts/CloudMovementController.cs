using UnityEngine;
using System.Collections;
 
public class CloudMovementController : MonoBehaviour {

    private GameObject island;
   
    IEnumerator Start() {

        island = GameObject.FindWithTag("Island");

        float halfWorldDimension = (island.GetComponent<Renderer>().bounds.size.x) / 2;

        Vector3 pointA = transform.position;
        Vector3 pointB;
        int direction = Random.Range(0,2);
        if(direction == 1) {
            pointB = new Vector3(halfWorldDimension, pointA.y, pointA.z);
        } else {
            pointB = new Vector3(-halfWorldDimension, pointA.y, pointA.z);
        }
        while(true)
        {
            float time = Random.Range(250.0f, 750.0f);
            yield return StartCoroutine(MoveObject(transform, pointA, pointB, time));
            yield return StartCoroutine(MoveObject(transform, pointB, pointA, time));
            pointA = new Vector3(-halfWorldDimension, pointB.y, pointB.z);
        }
    }
   
    IEnumerator MoveObject(Transform thisTransform, Vector3 startPos, Vector3 endPos, float time) {
        var i= 0.0f;
        var rate= 1.0f/time;
        while(i < 1.0f)
        {
            i += Time.deltaTime * rate;
            thisTransform.position = Vector3.Lerp(startPos, endPos, i);
            yield return null;
        }
    }
}