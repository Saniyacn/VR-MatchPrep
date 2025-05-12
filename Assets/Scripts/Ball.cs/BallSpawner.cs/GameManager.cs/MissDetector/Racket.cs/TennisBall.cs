using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Racket"))
        {
            // Add score via GameManager
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.AddScore(1);
            }

            // Destroy the ball after hitting the racket
            Destroy(gameObject);
        }
    }
}









using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int score = 0;
    public int misses = 0;
    public int maxMisses = 3;

    public TextMeshProUGUI scoreText;
    public GameObject gameOverPanel;

    void Start()
    {
        UpdateScore();
        gameOverPanel.SetActive(false);
    }

    public void AddScore(int value)
    {
        score += value;
        UpdateScore();
    }

    public void BallMissed()
    {
        misses++;
        if (misses >= maxMisses)
        {
            GameOver();
        }
    }

    void UpdateScore()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    void GameOver()
    {
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
    }
}








using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallLauncher : MonoBehaviour
{
    [Header("Ball Settings")]
    public GameObject ballPrefab;      // The tennis ball prefab
    public Transform spawnPoint;       // Where the ball appears
    public float launchForce = 10f;    // Speed of the ball
    public float upwardArc = 0.2f;     // How much upward direction

    [Header("Launch Timing")]
    public float launchInterval = 3f;  // Time between launches

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= launchInterval)
        {
            LaunchBall();
            timer = 0f;
        }
    }

    void LaunchBall()
    {
        GameObject ball = Instantiate(ballPrefab, spawnPoint.position, spawnPoint.rotation);
        Rigidbody rb = ball.GetComponent<Rigidbody>();

        // Apply forward and slight upward force
        Vector3 direction = (spawnPoint.forward + Vector3.up * upwardArc).normalized;
        rb.AddForce(direction * launchForce, ForceMode.Impulse);

        // Destroy the ball after 10 seconds
        Destroy(ball, 10f);
    }
}











using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissDetector : MonoBehaviour
{
    public GameManager gameManager;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            gameManager.BallMissed();
            Destroy(other.gameObject);
        }
    }
}








using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Racket : MonoBehaviour
{
    private Vector3 lastPosition;
    public Vector3 Velocity { get; private set; }

    void FixedUpdate()
    {
        Velocity = (transform.position - lastPosition) / Time.fixedDeltaTime;
        lastPosition = transform.position;
    }
}









using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TennisBall : MonoBehaviour
{
    private Rigidbody rb;
    public float hitForceMultiplier = 1.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        Racket racket = collision.collider.GetComponent<Racket>();
        if (racket != null)
        {
            Vector3 direction = racket.Velocity.normalized;
            float force = racket.Velocity.magnitude * hitForceMultiplier;
            rb.AddForce(direction * force, ForceMode.Impulse);
        }
    }

}
