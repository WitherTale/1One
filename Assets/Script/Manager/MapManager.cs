using UnityEngine;

public class MapManager : MonoBehaviour
{
    public Transform player;
    public GameObject[] maps;
    public float mapLength = 50f;      
    public float viewDistance = 100f;   

    void Start()
    {
        
        for (int i = 0; i < maps.Length; i++)
        {
          
            maps[i].transform.position = new Vector3(0, 0, i * mapLength);
        }
    }

    void Update()
    {
       
        foreach (GameObject map in maps)
        {
            float dist = Vector3.Distance(player.position, map.transform.position);
            map.SetActive(dist < viewDistance);
        }
    }
}