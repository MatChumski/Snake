using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Snake : MonoBehaviour
{

    // Definir los l�mites del tama�o del entorno de juego
    public int xSize, ySize;

    public Camera camara;

    public AudioSource soundEffect;
    public AudioClip deathAudio;
    public AudioClip eatAudio;

    // Bloque base
    public GameObject block;

    // Cabeza y direcci�n
    GameObject head;
    string headDirection = "right";

    // Cuerpo 
    List<GameObject> tail;

    // Comida
    GameObject food;

    // Materiales
    public Material headMaterial;
    public Material tailMaterial;
    public Material wallMaterial;
    public Material foodMaterial;
    public Material deadBodyMaterial;
    public Material deadHeadMaterial;
    public Material backgroundMaterial;

    // Velocidad a la que la serpiente va a cambiar de posici�n
    public float gameSpeed = 0.7f;

    // Interfaz
    // InGame
    public GameObject UI;
    public Text scoreText;  // Puntaje
    public float score = 0;
    public Text speedText; // Velocidad
    // GameOver
    public GameObject gameOver;
    public Button retry;

    // Start is called before the first frame update
    void Start()
    {
        xSize = GenerateEven(10, 30);
        ySize = GenerateEven(10, 30);

        CreateGrid();
        CreatePlayer();

        camara.GetComponent<Camera>().orthographicSize = Mathf.Max(xSize, ySize)/2 + 1;
        
        // Oculta el mensaje de Game Over
        gameOver.SetActive(false);

        // Establece la acci�n para el bot�n de restart
        retry.onClick.AddListener(RestartGame);

        // Crea la �nica instancia de comida que se va a mover
        food = Instantiate(block);
        food.GetComponent<MeshRenderer>().material = foodMaterial;
        MoveFood();

        UpdateSpeed();

        // Hace que la funci�n MoveHead() se llame en intervalos seg�n gameSpeed
        InvokeRepeating("MoveHead", 0, gameSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        float headPositionX = head.GetComponent<Transform>().position.x;
        float headPositionY = head.GetComponent<Transform>().position.y;

        float foodPositionX = food.GetComponent<Transform>().position.x;
        float foodPositionY = food.GetComponent<Transform>().position.y;

        // Si la cabeza est� fuera de los l�mites, muere el jugador
        if (headPositionX >= xSize / 2 || headPositionX <= -xSize / 2 || headPositionY >= ySize / 2 || headPositionY <= -ySize / 2)
        {
            KillPlayer();
        }

        // Si la cabeza toca alguna parte de su cola, muere el jugador
        foreach (GameObject part in tail)
        {
            if (headPositionX == part.GetComponent<Transform>().position.x && headPositionY == part.GetComponent<Transform>().position.y)
            {
                KillPlayer();
                break;
            }
        }

        // Si la posici�n de la cabeza coincide con la comida, mueve la comida y aumenta el tama�o
        if (headPositionX == foodPositionX && headPositionY == foodPositionY)
        {
            MoveFood();
            EatFood();
        }

        /* 
        Cambiar de direcci�n
        Si no tiene cola, puede moverse en cualquier direcci�n, pero si la tiene, no puede ir
        en la direcci�n contraria a la actual
        */

        if (Input.GetKeyDown("down"))
        {
            if (tail.Count > 0 && headDirection != "up")
            {
                headDirection = "down";
            }
            else if (tail.Count <= 0)
            {
                headDirection = "down";
            }
        }
        if (Input.GetKeyDown("up"))
        {
            if (tail.Count > 0 && headDirection != "down")
            {
                headDirection = "up";
            }
            else if (tail.Count <= 0)
            {
                headDirection = "up";
            }
        }
        if (Input.GetKeyDown("left"))
        {
            if (tail.Count > 0 && headDirection != "right")
            {
                headDirection = "left";
            }
            else if (tail.Count <= 0)
            {
                headDirection = "left";
            }
        }
        if (Input.GetKeyDown("right"))
        {
            if (tail.Count > 0 && headDirection != "left")
            {
                headDirection = "right";
            }
            else if (tail.Count <= 0)
            {
                headDirection = "right";
            }
        }

    }

    private int GenerateEven(int min, int max)
    {
        bool isEven = false;
        int num = 0;
        while (!isEven)
        {
            num = Random.Range(min, max);
            if (num % 2 == 0)
            {
                isEven = true;
            }
        }

        return num;
    }

    // Crear Jugador
    private void CreatePlayer()
    {
        head = Instantiate(block);
        head.GetComponent<MeshRenderer>().material = headMaterial;
        head.GetComponent<Transform>().position = new Vector3(0, 0, 0);
        tail = new List<GameObject>();
    }
    
    // Matar Jugador
    private void KillPlayer()
    {
        CancelInvoke(); // Detiene el ciclo de movimiento
        head.GetComponent<MeshRenderer>().material = deadHeadMaterial;

        foreach (GameObject part in tail)
        {
            part.GetComponent<MeshRenderer>().material = deadBodyMaterial;
        }

        soundEffect.GetComponent<AudioSource>().clip = deathAudio;
        soundEffect.Play();

        Destroy(head);

        // Muestra el mensaje de Game Over
        gameOver.SetActive(true);
    }

    // Reiniciar Juego
    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    // Actualizar puntaje en pantalla
    private void UpdateScore()
    {
        score += 1;
        scoreText.text = "Score: " + score.ToString();
    }

    // Actualiza velocidad en pantalla
    private void UpdateSpeed()
    {
        speedText.GetComponent<Text>().text = "Speed: " + System.Math.Round((1 / gameSpeed), 2).ToString() + "m/s";
    }

    // Movimiento de la cabeza
    private void MoveHead()
    {
        float headPositionX = head.GetComponent<Transform>().position.x;
        float headPositionY = head.GetComponent<Transform>().position.y;

        // Seg�n la direcci�n actual, va a cambiar su posici�n
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

        // Tras mover la cabeza, mueve la cola. Env�a la posici�n en la que estaba ANTES de moverse
        MoveTail(headPositionX, headPositionY);
    }

    // Movimiento de la cola
    private void MoveTail(float lastX, float lastY)
    {
        // Recorre la cola
        foreach (GameObject part in tail)
        {
            // Establece como nueva posici�n la del bloque anterior
            // Inicialmente, es la posici�n en la que estaba la cabeza previamente
            float newX = lastX;
            float newY = lastY;

            // Establece como posici�n anterior la ubicaci�n actual del bloque que se va a mover, para
            // pasarla a la siguiente interaci�n
            lastX = part.GetComponent<Transform>().position.x;
            lastY = part.GetComponent<Transform>().position.y;

            // Mueve el bloque actual a la posici�n nueva, que corresponde a la del bloque anterior
            part.GetComponent<Transform>().position = new Vector3(newX, newY, 0);
        }
    }

    // Mover Comida
    private void MoveFood()
    {
        // Nuevas posiciones
        float newX;
        float newY;

        // Genera una posici�n nueva aleatoria seg�n un tama�o m�ximo
        static float RandomPosition(int maxSize)
        {
            float randomPos = Random.Range(maxSize / 2 - 1, -maxSize / 2 + 1);

            return randomPos;
        }

        // Genera las nuevas posiciones
        newX = RandomPosition(xSize);
        newY = RandomPosition(ySize);

        // Mientras que la posici�n no sea v�lida, se va a repetir el ciclo
        bool valid = false;
        while (!valid)
        {
            bool found = false; // �Las posiciones chocan?
            foreach (GameObject part in tail)
            {
                if (part.GetComponent<Transform>().position.x == newX && part.GetComponent<Transform>().position.y == newY)
                {
                    found = true;
                    continue;
                }
            }

            // Si chocan, genera una nueva posici�n
            if (found)
            {
                valid = false;
                newX = RandomPosition(xSize);
                newY = RandomPosition(ySize);
            }
            else // Si no, es v�lido
            {
                valid = true;
            }

        }

        // Cambia la posici�n de la comida
        food.GetComponent<Transform>().position = new Vector3(newX, newY, 0);
    }

    // Comer
    private void EatFood()
    {
        // �ltima posici�n de la cola
        float lastPositionX;
        float lastPositionY;

        // Si no hay cola, guarda la posici�n de la cabeza
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

        // A�ade un elemento a la cola
        tail.Add(Instantiate(block));
        tail[tail.Count - 1].GetComponent<MeshRenderer>().material = tailMaterial;

        //tail[tail.Count - 1].GetComponent<Transform>().position = new Vector3(lastPositionX, lastPositionY, 0);
                
        // Seg�n la direcci�n en la que est� yendo, a�ade este bloque a la posici�n anterior al �ltimo bloque de la cola,
        // que se guard� anteriormente
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
        
        // Actualiza el puntaje
        UpdateScore();

        // Si la el tiempo de actualizaci�n est� por encima de 0.11 ms, aumenta la velocidad del juego (disminuye gameSpeed)
        if (gameSpeed > 0.11f)
        {
            gameSpeed -= 0.01f;
            UpdateSpeed();
            CancelInvoke();
            InvokeRepeating("MoveHead", gameSpeed, gameSpeed);
        }

        soundEffect.GetComponent<AudioSource>().clip = eatAudio;
        soundEffect.Play();

    }

    // Se crean todos los l�mites en las coordenadas que corresponden a los bordes
    private void CreateGrid()
    {
        for (int x = 0; x <= xSize; x++)
        {
            // Se crea una instancia del objecto block para borderBottom
            GameObject borderBottom = Instantiate(block);
            borderBottom.GetComponent<MeshRenderer>().material = wallMaterial;
            borderBottom.GetComponent<Transform>().position = new Vector3(x - (xSize / 2), -ySize / 2, -0.2f);

            GameObject borderTop = Instantiate(block);
            borderTop.GetComponent<MeshRenderer>().material = wallMaterial;
            borderTop.GetComponent<Transform>().position = new Vector3(x - (xSize / 2), ySize / 2, -0.2f);
        }

        for (int y = 0; y <= ySize; y++)
        {
            GameObject borderLeft = Instantiate(block);
            borderLeft.GetComponent<MeshRenderer>().material = wallMaterial;
            borderLeft.GetComponent<Transform>().position = new Vector3(-xSize / 2, y - (ySize / 2), -0.2f);

            GameObject borderRight = Instantiate(block);
            borderRight.GetComponent<MeshRenderer>().material = wallMaterial;
            borderRight.GetComponent<Transform>().position = new Vector3(xSize / 2, y - (ySize / 2), -0.2f);
        }

        GameObject background = Instantiate(block);
        background.GetComponent<MeshRenderer>().material = backgroundMaterial;
        background.GetComponent<Transform>().position = new Vector3(0, 0, 0.2f);
        background.GetComponent<Transform>().localScale = new Vector3(xSize, ySize, 1);
    }
}
