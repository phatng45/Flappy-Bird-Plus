using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour {
    [SerializeField] private GameObject  flyAnimation, waitingRoom;
    [SerializeField] private AudioSource jumpSound,    starCollectedSound;

    private Rigidbody2D rb;
    private bool        hitRightWall = false, canMove = false;
    private int         score;

    private void Awake() {
        GameManager.onGameStateChanged += GameManagerOnGameStateChanged;
    }

    private void OnDestroy() {
        GameManager.onGameStateChanged -= GameManagerOnGameStateChanged;
    }

    private void GameManagerOnGameStateChanged(GameState state) {
        if (state == GameState.Play) {
            transform.position = Vector3.zero;
            hitRightWall       = false;
            canMove            = false;
            waitingRoom.SetActive(true);
            waitingRoom.GetComponent<SpriteRenderer>().color = new Color(.9058f, .9058f, .9058f, 1);
            GetComponent<Rigidbody2D>().gravityScale         = 0.0f;
            transform.localScale                             = new Vector3(.5f, .5f, .5f);
        }
    }

    void Start() {
        rb    = GetComponent<Rigidbody2D>();
        score = 0;
    }

    void Update() {
        if (canMove) {
            if (!hitRightWall)
                transform.Translate(Vector3.right * (3 * Time.deltaTime));
            else
                transform.Translate(Vector3.left * (3 * Time.deltaTime));
        }

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) {
            if (!canMove) {
                canMove                                  = true;
                GetComponent<Rigidbody2D>().gravityScale = 1.0f;
                StartCoroutine(FadeCircle());
            }

            jumpSound.Play();
            rb.velocity = new Vector2(0, 5);
            StartCoroutine(WaitForAnimation());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("RightWall")) {
            hitRightWall         = true;
            transform.localScale = new Vector3(-.5f, .5f, .5f);
        }

        if (collision.CompareTag("LeftWall")) {
            hitRightWall         = false;
            transform.localScale = new Vector3(.5f, .5f, .5f);
        }

        if (collision.CompareTag("Obstacle") || collision.CompareTag("Spike")) {
            score = 0;
            GameManager.Instance.UpdateGameState(GameState.Lose);
        }

        if (collision.CompareTag("Star")) {
            starCollectedSound.Play();
            collision.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, .4f);
            Destroy(collision);
            if (++score == 3) {
                score = 0;
                GameManager.Instance.UpdateGameState(GameState.Win);
            }
        }
    }

    IEnumerator WaitForAnimation() {
        flyAnimation.SetActive(true);
        yield return new WaitForSeconds(.4f);
        flyAnimation.SetActive(false);
    }

    IEnumerator FadeCircle() {
        var   waitingRoomRenderer = waitingRoom.GetComponent<SpriteRenderer>();
        float alpha               = 1;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime * 1.5f) {
            Color newColor = new Color(.9058f, .9058f, .9058f, Mathf.Lerp(alpha, 0, t));
            waitingRoomRenderer.color = newColor;
            yield return null;
        }
    }
}