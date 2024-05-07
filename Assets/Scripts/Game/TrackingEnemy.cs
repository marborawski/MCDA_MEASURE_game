using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Tracking the opponent through the tower. The module rotates the tower towards the opponent
public class TrackingEnemy : MonoBehaviour
{
// Tag przeciwnika
    public string enemyTag;

    private GameObject tracking;

    private float timer;

// Time after which the tower will stop tracking the enemy if he moves outside the tower's area of interest
    public float endTrackingTime = 0.5f;

// Tower rotation speed
    public float speed = 1;

    private void EndTracking()
    {
        tracking = null;
    }

    private void StartTracking(GameObject ob)
    {
        tracking = ob;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (tracking == null && other.tag == enemyTag)
        {
            StartTracking(other.gameObject);
            timer = endTrackingTime;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (tracking == null && other.tag == enemyTag)
        {
            StartTracking(other.gameObject);
            timer = endTrackingTime;
        }
        if (tracking != null && other.tag == tracking.tag)
        {
            timer = endTrackingTime;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (tracking != null && other.tag == tracking.tag)
        {
            EndTracking();
        }
    }

    void Update()
    {
        if (timer >= 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                EndTracking();
            }
        }
        if (tracking != null)
        {
// Determine the quaternion representing the target direction
            Vector3 v = tracking.transform.position - transform.position;
            v.y = transform.position.y;
            Quaternion q = Quaternion.LookRotation(v);
// Determine the quaternion specifying how much the object should be rotated in the target direction
            Quaternion q2 = Quaternion.RotateTowards(transform.rotation, q, speed * Time.deltaTime);
// Assigning the designated quaternion as the transformation of the object's rotation 
            transform.rotation = q2;
        }
    }

}
