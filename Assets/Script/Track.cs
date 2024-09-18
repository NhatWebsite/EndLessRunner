    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
{
    public GameObject[] obstacles;
    public Vector2 numberOfObstacles;
    public GameObject coin;
    public Vector2 numberOfCoins;
    public GameObject health;
    public Vector2 numberOfHealth;
    public GameObject Magnet;
    public Vector2 numberOfMagnet;


    public List<GameObject> newhealth = new List<GameObject>();
    public List<GameObject> newMagnet=new List<GameObject>();

    public List<GameObject> newObstacles = new List<GameObject>();
    public List<GameObject> newCoins = new List<GameObject>();
    private BoxCollider coinCollider;

    private Vector3 initSize=new Vector3();
    private Vector3 bigSize = new Vector3(7f, 0.35f, 5f);
    // Start is called before the first frame update
    void Start()
    {
        int newNumberOfObstacles = (int)Random.Range(numberOfObstacles.x, numberOfObstacles.y);
        int newNumberOfCoins = (int)Random.Range(numberOfCoins.x, numberOfCoins.y);
        int newnumberOfMagnet = (int)Random.Range(numberOfMagnet.x, numberOfMagnet.y);
        int newNumberOfHealth = (int)Random.Range(numberOfHealth.x, numberOfHealth.y);

        for (int i=0; i < newNumberOfObstacles; i++)
        {
            newObstacles.Add(Instantiate(obstacles[Random.Range(0, obstacles.Length)], transform));
            newObstacles[i].SetActive(false);
        }
        for(int i = 0; i < newNumberOfCoins; i++)
        {
            newCoins.Add(Instantiate(coin, transform));
            newCoins[i].SetActive(false);
        }
        for (int i = 0; i < newNumberOfHealth; i++)
        {
            newhealth.Add(Instantiate(health, transform));
            newhealth[i].SetActive(false);

            
        }
        for (int i = 0; i < newnumberOfMagnet; i++)
        {
            newMagnet.Add(Instantiate(Magnet, transform));
            newMagnet[i].SetActive(false);
        }
        coinCollider = coin.GetComponentInChildren<BoxCollider>();
        initSize = coinCollider.size;
        // coinCollider.size = new Vector3(coinCollider.size.x, coinCollider.size.y, sizeZ);
        //coinCollider.size = initSize;
        PositionateObstacles();
        PositionateCoin();
        PositionateHealth();

        PositionateMagnet();
    }

    private GameObject Instantiate(object p)
    {
        throw new System.NotImplementedException();
    }

    void PositionateObstacles()
    {
        for(int i=0; i < newObstacles.Count; i++)
        {
            float posZMin = (297f / newObstacles.Count) + (297f / newObstacles.Count)*i;
            float posZMax = (297f / newObstacles.Count) + (297f / newObstacles.Count) * i + 1;
            newObstacles[i].transform.localPosition = new Vector3(0, 0, Random.Range(posZMin, posZMax));
            newObstacles[i].SetActive(true);
            if (newObstacles[i].GetComponent<ChangeLane>() != null)
            {
                newObstacles[i].GetComponent<ChangeLane>().PositionLane();
            }
        }
    }
    void PositionateCoin()
    {
        float minZPos = 10f;
        
        for(int i = 0; i < newCoins.Count; i++)
        {
            float maxZPos = minZPos + 5f;
            float randomZPos = Random.Range(minZPos, maxZPos);
            newCoins[i].transform.localPosition = new Vector3(transform.position.x,transform.position.y, randomZPos);
            newCoins[i].SetActive(true);
            newCoins[i].GetComponent<ChangeLane>().PositionLane();
            minZPos = randomZPos + 1;
        }
    }
    void PositionateHealth()
    {
        float minZPos = 279f;

        for (int i = 0; i < newhealth.Count; i++)
        {
            float maxZPos = minZPos + 5f;
            float randomZPos = Random.Range(minZPos, maxZPos);
            Debug.Log(new Vector3(transform.position.x, transform.position.y, randomZPos));
            newhealth[i].transform.localPosition = new Vector3(transform.position.x, transform.position.y, randomZPos);
            newhealth[i].SetActive(true);
            newhealth[i].GetComponent<ChangeLane>().PositionLane();
            minZPos = randomZPos + 1;
        }
    }

    void PositionateMagnet()
    {
        float minZPos = 20f;

        for (int i = 0; i < newMagnet.Count; i++)
        {
            float maxZPos = minZPos + 5f;
            float randomZPos = Random.Range(minZPos, maxZPos);
            Debug.Log(new Vector3(transform.position.x, transform.position.y, randomZPos));
            newMagnet[i].transform.localPosition = new Vector3(transform.position.x, transform.position.y, randomZPos);
            newMagnet[i].SetActive(true);
            newMagnet[i].GetComponent<ChangeLane>().PositionLane();
            minZPos = randomZPos + 1;
        }
    }
    public void ChangeColliderCoin(bool isChange)
    {
        

        for (int i = 0; i < newCoins.Count; i++)
        {
            if(isChange)
            {
                newCoins[i].GetComponentInChildren<BoxCollider>().size = bigSize;
                
               
            }
            else
            {
                newCoins[i].GetComponentInChildren<BoxCollider>().size = initSize;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player"))
        {
            other.GetComponent<player>().IncreaseSpeed();
            transform.position = new Vector3(0, 0, transform.position.z + 297 * 2);
            PositionateObstacles();
            PositionateCoin();
            
        }
    }
}
