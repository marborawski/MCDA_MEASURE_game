using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Create a health bar above the enemy
public class EnemyGetHealtBar : MonoBehaviour
{
// Pointer to the template object from which the health level bar will be cloned
    public HealthBarInterface healthBarInterface;

    private HealthBarInterface healthBar;

// The height of the life bar above the enemy
    public float shiftY = 0;

    void Start()
    {
        if (healthBarInterface != null && healthBar == null)
        {
            GameObject go = Instantiate(healthBarInterface.gameObject);
            go.transform.parent = transform;
            go.transform.localPosition = new Vector3(0, shiftY, 0);
            go.SetActive(true);
            healthBar = go.GetComponent<HealthBarInterface>();
        }
    }
}
