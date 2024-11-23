using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDistributeObjects : MonoBehaviour
{
    [Header("Settings")]
    public int minObjects = 10;
    public int maxObjects = 30;

    public int minObjectDistance = 40;

    public GameObject objectPefab;


    Vector3[] objectPoints;

    void Start()
    {
        int numObjects = Random.Range(minObjects, maxObjects + 1);

        objectPoints = new Vector3[numObjects];

        for (int i = 0; i < numObjects; i++)
        {
            objectPoints[i] = new Vector3(Random.value * 10 - 5, Random.value * 10 - 5, Random.value * 10 - 5);
        }

        SpreadPoints(objectPoints);
        SpawnObjects(objectPoints);
    }

    void SpreadPoints(Vector3[] p)
    {
        float minDist;
        do
        {
            minDist = minObjectDistance;
            for (int i = 0; i < p.Length; i++)
            {
                for (int j = 0; j < p.Length; j++)
                {
                    if (j == i) continue;

                    float d = (p[i] - p[j]).magnitude;
                    if (d < minDist) minDist = d;

                    if (d < minObjectDistance)
                        p[i] += (p[i] - p[j]).normalized * minObjectDistance;

                }
            }
        } while (minDist < minObjectDistance);


    }

    void SpawnObjects(Vector3[] p)
    {
        foreach (Vector3 point in p)
        {
            float size = Random.value * 4 + 1;

            GameObject g = Instantiate(objectPefab, Vector3.zero, Quaternion.identity, transform);
           
            g.transform.position += point;
            g.transform.localScale = new Vector3(size, size, size);
            g.transform.rotation = Quaternion.Euler(new Vector3(Random.value * 360, Random.value * 360, Random.value * 360));
            int health = Mathf.RoundToInt(size * 10);
            g.GetComponent<Health>().totalHealth = health;
            g.SendMessage("Heal", health);
        }
    }
}
