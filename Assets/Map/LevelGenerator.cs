using UnityEngine;
using System.IO;
using System;

// IMPORTANT NOTE: THE GAME GOES IN THE Z DIRECTION
public class ObstacleGenerator : MonoBehaviour
{
    public float safeZoneLimit;
    public float endPointDistance;
    public float spaceBetweenObstacles;

    [System.Serializable]
    public class Obstacle
    {
        public string prefab;
        public float rarity;

        [System.NonSerialized]
        public GameObject prefabObject;
    }

    [System.Serializable]
    public class ObstacleList
    {
        public Obstacle[] obstacles;
    }

    private Obstacle[] obstacles;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadObstacles();
        GenerateLevel();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LoadObstacles()
    {
        string filePath = Path.Combine(Application.dataPath, "Map/obstacles.json");
        if (File.Exists(filePath))
        {
            string fileContent = File.ReadAllText(filePath);
            obstacles = JsonUtility.FromJson<ObstacleList>(fileContent).obstacles;
            string obstaclesDir = Path.Combine(Application.dataPath, "Map/Resources");
            foreach (var obstacle in obstacles)
            {
                filePath = Path.Combine(obstaclesDir, obstacle.prefab + ".prefab");
                if (File.Exists(filePath))
                {
                    obstacle.prefabObject = Resources.Load<GameObject>(obstacle.prefab);
                }
                else
                {
                    Debug.LogWarning($"File does not exist: {filePath}");
                }
            }
        }
        else
        {
            Debug.LogError("JSON file not found!");
        }
    }

    GameObject SpawnObstacle(Vector3 position)
    {
        Obstacle obstacle = PickRandomObstacleBasedOnRarity();
        return Instantiate(obstacle.prefabObject, position, Quaternion.identity);
    }

    void GenerateLevel() {
        Vector3 originalPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + safeZoneLimit);
        float z = 0f;
    
        while (originalPosition.z + z < endPointDistance) {
            var spawnedObstacle = SpawnObstacle(originalPosition + new Vector3(0f, 0f, z));
            var obstacleSize = GetCombinedBounds(spawnedObstacle).size;
            z += obstacleSize.z + spaceBetweenObstacles;
        }

        // Adjust the end point and the floor length
        endPointDistance = originalPosition.z + z;
        Transform floor = transform.Find("Floor");
        floor.localScale = new Vector3(1f, 1f, Math.Abs(endPointDistance - transform.position.z));
    }

    Obstacle PickRandomObstacleBasedOnRarity()
    {
        // Calculate the total weight (sum of all rarities)
        float totalWeight = 0;
        foreach (var item in obstacles)
        {
            totalWeight += item.rarity;
        }

        // Generate a random number between 0 and totalWeight
        float randomNumber = UnityEngine.Random.Range(0, totalWeight);

        // Select the item based on the random number
        foreach (var item in obstacles)
        {
            if (randomNumber < item.rarity)
            {
                return item;
            }
            randomNumber -= item.rarity;
        }

        // Fallback in case something goes wrong
        return obstacles[0];
    }


	Bounds GetCombinedBounds(GameObject root)
	{
		var renderers = root.GetComponentsInChildren<Renderer>();
		if (renderers.Length == 0)
			return new Bounds(root.transform.position, Vector3.zero);

		Bounds bounds = renderers[0].bounds;
		for (int i = 1; i < renderers.Length; i++)
		{
			bounds.Encapsulate(renderers[i].bounds);
		}
		return bounds;
	}
}
