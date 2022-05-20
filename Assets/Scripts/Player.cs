using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour {
    [SerializeField] private GameObject flyAnimation;
    [SerializeField] private AudioSource jumpSound, starCollectedSound;

    private Rigidbody2D rb;
    public  Transform   pos;
    private bool        hitRightWall = false;
    private  int         score;

    void Start() {
        rb    = GetComponent<Rigidbody2D>();
        score = 0;
    }

    void Update() {
        if (!hitRightWall)
            transform.Translate(Vector3.right * (3 * Time.deltaTime));
        else
            transform.Translate(Vector3.left * (3 * Time.deltaTime));

        if (Input.GetMouseButtonDown(0)) {
            jumpSound.Play();
            rb.velocity = new Vector2(0, 5);
            StartCoroutine(WaitForAnimation());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("RightWall")) {
            hitRightWall         = true;
            transform.localScale = new Vector3(-0.6f, 0.6f, 0.6f);
        }

        if (collision.CompareTag("LeftWall")) {
            hitRightWall         = false;
            transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
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
}