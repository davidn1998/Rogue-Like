using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Player : MovingObject
{

    [SerializeField] int wallDamage = 1;
    [SerializeField] int pointsPerFood = 10;
    [SerializeField] int pointsPerSoda = 20;
    [SerializeField] float restartLevelDelay = 1f;
    [SerializeField] TextMeshProUGUI foodText;

    [SerializeField] AudioClip moveSound1;
    [SerializeField] AudioClip moveSound2;
    [SerializeField] AudioClip eatSound1;
    [SerializeField] AudioClip eatSound2;
    [SerializeField] AudioClip drinkSound1;
    [SerializeField] AudioClip drinkSound2;
    [SerializeField] AudioClip gameOverSound;

    Animator animator;
    int food;
    Vector2 touchOrigin = -Vector2.one;

    // Start is called before the first frame update
    protected override void Start()
    {
        animator = GetComponent<Animator>();
        food = GameManager.instance.GetPlayerFoodPoints();
        foodText.text = "Food: " + food;

        base.Start();
    }

    private void OnDisable()
    {
        GameManager.instance.SetPlayerFoodPoints(food);
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        if (!GameManager.instance.GetPlayersTurn())
        {
            return;
        }

        int horizontal = 0;
        int vertical = 0;

#if  UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_WEBGL

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0)
        {
            vertical = 0;
        }

#else 

        if(Input.touchCount > 0)
        {
            Touch myTouch = Input.touches[0];

            if (myTouch.phase == TouchPhase.Began)
            {
                touchOrigin = myTouch.position;
            }
            else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
            {
                Vector2 touchEnd = myTouch.position;
                float x = touchEnd.x - touchOrigin.x;
                float y = touchEnd.y - touchOrigin.y;
                touchOrigin.x = -1;
                if (Mathf.Abs(x) > Mathf.Abs(y))
                {
                    horizontal = x > 0 ? 1 : -1;
                }
                else
                {
                    vertical = y > 0 ? 1 : -1;
                }
            }
        }

#endif

        if (horizontal != 0 || vertical != 0)
        {
            AttemptMove<Wall>(horizontal, vertical);
        }
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        food--;
        foodText.text = "Food: " + food;

        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;

        if (Move(xDir, yDir, out hit))
        {
            SoundManager.instance.RandomiseSfx(moveSound1, moveSound2);
        }

        CheckIfGameOver();

        GameManager.instance.SetPlayersTurn(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Exit"))
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
        else if (other.CompareTag("Food"))
        {
            food += pointsPerFood;
            foodText.text = "+" + pointsPerFood + " Food: " + food;
            SoundManager.instance.RandomiseSfx(eatSound1, eatSound2);
            other.gameObject.SetActive(false);
        }
        else if (other.CompareTag("Soda"))
        {
            food += pointsPerSoda;
            foodText.text = "+" + pointsPerFood + " Food: " + food;
            SoundManager.instance.RandomiseSfx(drinkSound1, drinkSound2);
            other.gameObject.SetActive(false);
        }
    }

    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        animator.SetTrigger("playerChop");
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");
        foodText.text = "-" + loss + " Food: " + food;
        food -= loss;
        CheckIfGameOver();
    }


    void CheckIfGameOver()
    {
        if (food <= 0)
        {
            GameManager.instance.GameOver();
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.GetMusicSource().Stop();
            Debug.Log("Game Over");
        }
    }
}
