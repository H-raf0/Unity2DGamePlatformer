using UnityEngine;

public class ObstaclesScript : MonoBehaviour
{

    [SerializeField] private GameObject obstacles;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        for (int i = 0; i < obstacles.transform.childCount; i++)
        {
            Transform child = obstacles.transform.GetChild(i);

            child.name = "spike " + i;
            child.Translate(Vector2.right);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
