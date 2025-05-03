using UnityEngine;
using UnityEngine.Assertions;

public class LevelManager : MonoBehaviour
{
    public int numberOfLevels = 5;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateMap() {
        Vector3 startPoint = transform.position;
        
        GameObject level = Resources.Load<GameObject>("Level");
        
    }
}
