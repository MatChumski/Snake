using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Snake : MonoBehaviour
{

    // Definir los límites del tamaño del entorno de juego
    public int xSize, ySize;

    public GameObject block;

    GameObject head;
    string headDirection = "right";

    List<GameObject> tail;

    GameObject food;

    public Material headMaterial;
    public Material tailMaterial;
    public Material wallMaterial;
    public Material foodMaterial;
    public Material deadBodyMaterial;
    public Material deadHeadMaterial;
    public Material backgroundMaterial;

    public float gameSpeed = 0.7f;

    public Text scoreText;
    public float score = 0;

    public Text speedText;

    // Start is called before the first frame update
    void Start()
    {
        CreateGrid();
        CreatePlayer();

        food = Instantiate(block);
        food.GetComponent<MeshRenderer>().material = foodMaterial;
        MoveFood();

        UpdateSpeed();

        InvokeRepeating("MoveHead", 0, gameSpeed); //calls moveHead() every 2 secs
    }

    // Update is called once per frame
    void Update()
    {
        float headPositionX = head.GetComponent<Transform>().position.x;
        float headPositionY = head.GetComponent<Transform>().position.y;

        float foodPositionX = food.GetComponent<Transform>().position.x;
        float foodPositionY = food.GetComponent<Transform>().position.y;


        if (headPositionX >= xSize / 2 || headPositionX <= -xSize / 2 || headPositionY >= ySize / 2 || headPositionY <= -ySize / 2)
        {
            KillPlayer();
        }

        foreach(GameObject part in tail)
        {
            if (headPositionX == part.GetComponent<Transform>().position.x && headPositionY == part.GetComponent<Transform>().position.y)
            {
                KillPlayer();
                break;
            }
        }

        if (headPositionX == foodPositionX && headPositionY == foodPositionY)
        {
            MoveFood();
            EatFood();
        }

        if (Input.GetKeyDown("down"))
        {
            if (headDirection != "up")
            {
                headDirection = "down";
            }
        }
        if (Input.GetKeyDown("up"))
            if (headDirection != "down")
            {
                headDirection = "up";
            }
        {
        }
        if (Input.GetKeyDown("left"))
        {
            if (headDirection != "right")
            {
                headDirection = "left";
            }
        }
        if (Input.GetKeyDown("right"))
        {
            if (headDirection != "left")
            {
                headDirection = "right";
            }
        }

    }

    private void CreatePlayer()
    {
        head = Instantiate(block);
        head.GetComponent<MeshRenderer>().material = headMaterial;
        head.GetComponent<Transform>().position = new Vector3(0, 0, 0);
        tail = new List<GameObject>();
    }

    private void KillPlayer()
    {
        CancelInvoke();
        head.GetComponent<MeshRenderer>().material = deadHeadMaterial;

        foreach (GameObject part in tail)
        {
            part.GetComponent<MeshRenderer>().material = deadBodyMaterial;
        }
    }

    private void UpdateScore()
    {
        score += 1;
        scoreText.text = "Score: " + score.ToString();
    }

    private void UpdateSpeed()
    {
        speedText.GetComponent<Text>().text = "Speed: " + Mathf.Round(1 / gameSpeed).ToString() + "m/s";
    }

    private void MoveHead()
    {
        float headPositionX = head.GetComponent<Transform>().position.x;
        float headPositionY = head.GetComponent<Transform>().position.y;

        if (headDirection == "right")
        {
            head.GetComponent<Transform>().position = new Vector3(headPositionX + 1, headPositionY, 0);
        }
        if (headDirection == "left")
        {
            head.GetComponent<Transform>().position = new Vector3(headPositionX - 1, headPositionY, 0);
        }
        if (headDirection == "up")
        {
            head.GetComponent<Transform>().position = new Vector3(headPositionX, headPositionY + 1, 0);
        }
        if (headDirection == "down")
        {
            head.GetComponent<Transform>().position = new Vector3(headPositionX, headPositionY - 1, 0);
        }

        MoveTail(headPositionX, headPositionY);
    }

    private void MoveTail(float lastX, float lastY)
    {
        foreach (GameObject part in tail)
        {
            float newX = lastX;
            float newY = lastY;

            lastX = part.GetComponent<Transform>().position.x;
            lastY = part.GetComponent<Transform>().position.y;

            part.GetComponent<Transform>().position = new Vector3(newX, newY, 0);
        }
    }

    private void MoveFood()
    {
        float newX;
        float newY;

        static float RandomPosition(int maxSize)
        {
            float randomPos = Random.Range(maxSize / 2 - 1, -maxSize / 2 + 1);

            return randomPos;
        }


        newX = RandomPosition(xSize);
        newY = RandomPosition(ySize);

        bool valid = false;
        while (!valid)
        {
            bool found = false;
            foreach (GameObject part in tail)
            {
                if (part.GetComponent<Transform>().position.x == newX && part.GetComponent<Transform>().position.y == newY)
                {
                    found = true;
                    continue;
                }
            }
            if (found)
            {
                valid = false;
                newX = RandomPosition(xSize);
                newY = RandomPosition(ySize);
            } else
            {
                valid = true;
            }
            
        }

        food.GetComponent<Transform>().position = new Vector3(newX, newY, 0);
    }

    private void EatFood()
    {
        float lastPositionX;
        float lastPositionY;

        if (tail.Count == 0)
        {
            lastPositionX = head.GetComponent<Transform>().position.x;
            lastPositionY = head.GetComponent<Transform>().position.y;
        }
        else
        {
            lastPositionX = tail[tail.Count - 1].GetComponent<Transform>().position.x;
            lastPositionY = tail[tail.Count - 1].GetComponent<Transform>().position.y;

        }

        tail.Add(Instantiate(block));
        tail[tail.Count - 1].GetComponent<MeshRenderer>().material = tailMaterial;

        //tail[tail.Count - 1].GetComponent<Transform>().position = new Vector3(lastPositionX, lastPositionY, 0);

        if (headDirection == "right")
        {
            tail[tail.Count - 1].GetComponent<Transform>().position = new Vector3(lastPositionX - 1, lastPositionY, 0);
        }
        if (headDirection == "left")
        {
            tail[tail.Count - 1].GetComponent<Transform>().position = new Vector3(lastPositionX + 1, lastPositionY, 0);
        }
        if (headDirection == "up")
        {
            tail[tail.Count - 1].GetComponent<Transform>().position = new Vector3(lastPositionX, lastPositionY - 1, 0);
        }
        if (headDirection == "down")
        {
            tail[tail.Count - 1].GetComponent<Transform>().position = new Vector3(lastPositionX, lastPositionY + 1, 0);
        }

        UpdateScore();

        if (score % 5 == 0 && gameSpeed > 0.11f)
        {
            gameSpeed -= 0.05f;
            UpdateSpeed();
            CancelInvoke();
            InvokeRepeating("MoveHead", gameSpeed, gameSpeed);
        }


    }

    // Se crean todos los límites en las coordenadas que corresponden a los bordes
    private void CreateGrid()
    {
        for (int x = 0; x <= xSize; x++)
        {
            // Se crea una instancia del objecto block para borderBottom
            GameObject borderBottom = Instantiate(block);
            borderBottom.GetComponent<MeshRenderer>().material = wallMaterial;
            borderBottom.GetComponent<Transform>().position = new Vector3(x - (xSize / 2), -ySize / 2, 0.1f);

            GameObject borderTop = Instantiate(block);
            borderTop.GetComponent<MeshRenderer>().material = wallMaterial;
            borderTop.GetComponent<Transform>().position = new Vector3(x - (xSize / 2), ySize / 2, 0.1f);
        }

        for (int y = 0; y <= ySize; y++)
        {
            GameObject borderLeft = Instantiate(block);
            borderLeft.GetComponent<MeshRenderer>().material = wallMaterial;
            borderLeft.GetComponent<Transform>().position = new Vector3(-xSize / 2, y - (ySize / 2), 0.1f);

            GameObject borderRight = Instantiate(block);
            borderRight.GetComponent<MeshRenderer>().material = wallMaterial;
            borderRight.GetComponent<Transform>().position = new Vector3(xSize / 2, y - (ySize / 2), 0.1f);
        }

        GameObject background = Instantiate(block);
        background.GetComponent<MeshRenderer>().material = backgroundMaterial;
        background.GetComponent<Transform>().position = new Vector3(0, 0, 0.2f);
        background.GetComponent<Transform>().localScale = new Vector3(xSize, ySize, 1);
    }
}
