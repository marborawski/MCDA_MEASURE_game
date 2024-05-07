using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attacking the opponent through the tower (EnemyAttack module is attached to the tower)
public class EnemyAttack : MonoBehaviour
{

// Opponent's tag
    public string waveTag;

    private GameObject tracking;

    private float timer;

// Time in seconds after which the enemy will stop being shot at if he moves outside the area fired by the tower
    public float endTrackingTime = 0.5f;

// The missile fired by the turret
    public GameObject bullet;

// Tower rate of fire
    public float rateOfFire = 0.5f;

    private float timerFire;

// Distance from the tower at which the missile starts its flight. Protects the tower from being hit
    public float shiftY;

// The force with which the bullet is fired (determines the range of the shot)
    public float force = 100;

    private TowerCharacter towerCharacter;

    private void EndTracking()
    {
        tracking = null;
        timerFire = 0;
    }

    private void StartTracking(GameObject ob)
    {
        tracking = ob;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (tracking == null && other.tag == waveTag)
        {
            StartTracking(other.gameObject);
            timer = endTrackingTime;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (tracking == null && other.tag == waveTag)
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

    private void Fire()
    {
        if (bullet != null)
        {
            Vector3 v = transform.position + new Vector3(0, shiftY, 0);
            GameObject go = Instantiate(bullet, v, transform.rotation);
            go.transform.parent = null;
            go.SetActive(true);
            BulletAtack bulletAtack = go.GetComponent<BulletAtack>();
            if (bulletAtack != null)
                bulletAtack.AddTowerCharacter(towerCharacter);
            Rigidbody rigidbody = go.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.AddForce(transform.forward * force);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        towerCharacter = GetComponent<TowerCharacter>();
    }

    // Update is called once per frame
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
            timerFire -= Time.deltaTime;
            if (timerFire < 0)
            {
                Fire();
                timerFire += rateOfFire;
            }
        }
    }
}
