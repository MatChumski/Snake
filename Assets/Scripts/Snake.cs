using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Snake : MonoBehaviour
{

    // Definir los límites del tamaño del entorno de juego
    public int xSize, ySize;

    public Camera camara;

    public AudioSource soundEffect;
    public AudioClip deathAudio;
    public AudioClip eatAudio;

    // Bloque base
    public GameObject block;

    // Cabeza y dirección
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

    // Velocidad a la que la serpiente va a cambiar de posición
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

        // Establece la acción para el botón de restart
        retry.onClick.AddListener(RestartGame);

        // Crea la única instancia de comida que se va a mover
        food = Instantiate(block);
        food.GetComponent<MeshRenderer>().material = foodMaterial;
        MoveFood();

        UpdateSpeed();

        // Hace que la función MoveHead() se llame en intervalos según gameSpeed
        InvokeRepeating("MoveHead", 0, gameSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        float headPositionX = head.GetComponent<Transform>().position.x;
        float headPositionY = head.GetComponent<Transform>().position.y;

        float foodPositionX = food.GetComponent<Transform>().position.x;
        float foodPositionY = food.GetComponent<Transform>().position.y;

        // Si la cabeza está fuera de los límites, muere el jugador
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

        // Si la posición de la cabeza coincide con la comida, mueve la comida y aumenta el tamaño
        if (headPositionX == foodPositionX && headPositionY == foodPositionY)
        {
            MoveFood();
            EatFood();
        }

        /* 
        Cambiar de dirección
        Si no tiene cola, puede moverse en cualquier dirección, pero si la tiene, no puede ir
        en la dirección contraria a la actual
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

        // Según la dirección actual, va a cambiar su posición
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

        // Tras mover la cabeza, mueve la cola. Envía la posición en la que estaba ANTES de moverse
        MoveTail(headPositionX, headPositionY);
    }

    // Movimiento de la cola
    private void MoveTail(float lastX, float lastY)
    {
        // Recorre la cola
        foreach (GameObject part in tail)
        {
            // Establece como nueva posición la del bloque anterior
            // Inicialmente, es la posición en la que estaba la cabeza previamente
            float newX = lastX;
            float newY = lastY;

            // Establece como posición anterior la ubicación actual del bloque que se va a mover, para
            // pasarla a la siguiente interación
            lastX = part.GetComponent<Transform>().position.x;
            lastY = part.GetComponent<Transform>().position.y;

            // Mueve el bloque actual a la posición nueva, que corresponde a la del bloque anterior
            part.GetComponent<Transform>().position = new Vector3(newX, newY, 0);
        }
    }

    // Mover Comida
    private void MoveFood()
    {
        // Nuevas posiciones
        float newX;
        float newY;

        // Genera una posición nueva aleatoria según un tamaño máximo
        static float RandomPosition(int maxSize)
        {
            float randomPos = Random.Range(maxSize / 2 - 1, -maxSize / 2 + 1);

            return randomPos;
        }

        // Genera las nuevas posiciones
        newX = RandomPosition(xSize);
        newY = RandomPosition(ySize);

        // Mientras que la posición no sea válida, se va a repetir el ciclo
        bool valid = false;
        while (!valid)
        {
            bool found = false; // ¿Las posiciones chocan?
            foreach (GameObject part in tail)
            {
                if (part.GetComponent<Transform>().position.x == newX && part.GetComponent<Transform>().position.y == newY)
                {
                    found = true;
                    continue;
                }
            }

            // Si chocan, genera una nueva posición
            if (found)
            {
                valid = false;
                newX = RandomPosition(xSize);
                newY = RandomPosition(ySize);
            }
            else // Si no, es válido
            {
                valid = true;
            }

        }

        // Cambia la posición de la comida
        food.GetComponent<Transform>().position = new Vector3(newX, newY, 0);
    }

    // Comer
    private void EatFood()
    {
        // Última posición de la cola
        float lastPositionX;
        float lastPositionY;

        // Si no hay cola, guarda la posición de la cabeza
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

        // Añade un elemento a la cola
        tail.Add(Instantiate(block));
        tail[tail.Count - 1].GetComponent<MeshRenderer>().material = tailMaterial;

        //tail[tail.Count - 1].GetComponent<Transform>().position = new Vector3(lastPositionX, lastPositionY, 0);
                
        // Según la dirección en la que esté yendo, añade este bloque a la posición anterior al último bloque de la cola,
        // que se guardó anteriormente
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

        // Si la el tiempo de actualización está por encima de 0.11 ms, aumenta la velocidad del juego (disminuye gameSpeed)
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

    // Se crean todos los límites en las coordenadas que corresponden a los bordes
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
