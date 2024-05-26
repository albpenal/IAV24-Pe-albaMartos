/*    
   Copyright (C) 2020-2023 Federico Peinado
   http://www.federicopeinado.com
   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).
   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UCM.IAV.Navegacion;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UCM.IAV.Movimiento
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance = null;

        // Textos UI
        Text label;
        Text label2;
        Text life;
        string mapName = "Map1";

        private int frameRate = 60;
        TheseusGraph theseusGraph;

        // Variables de timer de framerate
        int m_frameCounter = 0;
        float m_timeCounter = 0.0f;
        float m_lastFramerate = 0.0f;
        float m_refreshTime = 0.5f;

        private bool cameraPerspective = true;

        GameObject player = null;
        GameObject exitSlab = null;
        GameObject startSlab = null;
        Image GameOverBack;
        Image victory;

        GameObject exit = null;
        GameObject temporalExit = null;

        public bool IA = false;

        public bool gameOver;

        private void Awake()
        {
            // Hacemos que el gestor del juego sea un Ejemplar Único
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        private void Start()
        {
            Application.targetFrameRate = frameRate;

            FindGO();
            gameOver = false;
            IA = false;
        }

        // Lo primero que se llama al activarse (tras el Awake)
        void OnEnable()
        {

            // No necesito este delegado
            //SceneManager.activeSceneChanged += OnSceneWasSwitched;

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        // Delegado para hacer cosas cuando una escena termina de cargar (no necesariamente cuando ha cambiado/switched)
        // Antiguamente se usaba un método del SceneManager llamado OnLevelWasLoaded(int level), ahora obsoleto
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            FindGO();

        }


        // Se llama cuando el juego ha terminado
        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }


        // Update is called once per frame
        void Update()
        {
            if (!gameOver)
            {
                // Timer para mostrar el frameRate a intervalos
                if (m_timeCounter < m_refreshTime)
                {
                    m_timeCounter += Time.deltaTime;
                    m_frameCounter++;
                }
                else
                {
                    m_lastFramerate = (float)m_frameCounter / m_timeCounter;
                    m_frameCounter = 0;
                    m_timeCounter = 0.0f;
                }

                if (player != null && (player.transform.position - exit.transform.position).magnitude < 0.5f)
                    Victory();
                if (life != null)
                    life.text = "Vida: " + ((int)player.GetComponent<PlayerLife>().getLife()).ToString();

                //Input
                if (Input.GetKeyDown(KeyCode.R))
                    RestartScene();
            }
        }

        private void FindGO()
        {
            if (SceneManager.GetActiveScene().name == "Menu") // Nombre de escena que habría que llevar a una constante
            {
                label = GameObject.FindGameObjectWithTag("DDLabel").GetComponent<Text>();
                label2 = GameObject.FindGameObjectWithTag("IA").GetComponent<Text>();
            }
            else if (SceneManager.GetActiveScene().name == "Refugiate") // Nombre de escena que habría que llevar a una constante
            {
                theseusGraph = GameObject.FindGameObjectWithTag("TesterGraph").GetComponent<TheseusGraph>();
                exitSlab = GameObject.FindGameObjectWithTag("Exit");
                startSlab = GameObject.FindGameObjectWithTag("Start");
                player = GameObject.Find("Avatar");
                life = GameObject.FindGameObjectWithTag("Life").GetComponent<Text>();
                GameOverBack = GameObject.FindGameObjectWithTag("GameOverBackground").GetComponent<Image>();
                GameOverBack.enabled = false;
                victory = GameObject.FindGameObjectWithTag("victory").GetComponent<Image>();
                victory.enabled = false;
            }
        }

        public GameObject GetPlayer()
        {
            if (player == null) player = GameObject.Find("Avatar");
            return player;
        }

        public void RestartScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }


        public void setIA()
        {
            IA = (label2.text == "IA");
        }

        public void goToScene(string scene)
        {
            SceneManager.LoadScene(scene);
        }

        public GameObject GetExitNode()
        {
            return temporalExit;
        }

        public void SetExit(int i, int j, float size)
        {
            exit = new GameObject(); exit.name = "Exit";
            exit.transform.position = new Vector3(i * size, 0, j * size);
            exitSlab.transform.position = new Vector3(i * size, 0.3f, j * size);
            temporalExit = new GameObject();
            temporalExit.transform.position = new Vector3(i * size, 0, j * size);
            temporalExit.transform.position = new Vector3(i * size, 0.3f, j * size);
        }

        public void SetStart(int i, int j, float size)
        {
            player.transform.position = new Vector3(i * size, 0.2f, j * size);
            startSlab.transform.position = new Vector3(i * size, 0.2f, j * size);
        }

        private void ChangeFrameRate()
        {
            if (frameRate == 30)
            {
                frameRate = 60;
                Application.targetFrameRate = 60;
            }
            else
            {
                frameRate = 30;
                Application.targetFrameRate = 30;
            }
        }

        public void ChangeName()
        {
            mapName = label.text;
        }
        public string getName()
        {
            return mapName;
        }

        public void setExit(Vector3 pos)
        {
            temporalExit.transform.position = pos;
        }
        private void Victory()
        {
            victory.enabled = true;
            //life.enabled = false;
            player.SetActive(false);
            gameOver = true;
            GameObject.FindGameObjectWithTag("Left").SetActive(false);
            GameObject.FindGameObjectWithTag("Right").SetActive(false);

            StartCoroutine(WaitAndLoadMenu());
        }
        public void GameOver()
        {
            GameOverBack.enabled = true;
            //life.enabled = false;
            player.SetActive(false);
            gameOver = true;
            GameObject.FindGameObjectWithTag("Left").SetActive(false);
            GameObject.FindGameObjectWithTag("Right").SetActive(false);

            StartCoroutine(WaitAndLoadMenu());
        }

        private IEnumerator WaitAndLoadMenu()
        {
            yield return new WaitForSeconds(3);
            gameOver = false;
            IA = false;
            goToScene("Menu");
        }
    }
}