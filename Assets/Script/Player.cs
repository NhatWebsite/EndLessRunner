using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float jumpHeight;
    public float jumpLenght;
    public float speed;
    public float laneSpeed;
    public float slideLenght;
    public int maxLife = 3;
    public float minSpeed = 10f;
    public float maxSpeed = 30f;
    public int invincibleTime;
    public GameObject model;

    private float slideStart;
    private Animator anim;
    private BoxCollider boxCollider;
    private Vector3 verticalTagetPosition;
    private int currentLane = 1;
    private Rigidbody rb;
    private bool jumping=false;
    private float jumpStart;
    private bool sliding = false;
    private Vector3 boxColliderSize;
    private bool isSwipping = false;
    private Vector2 startingTouch;
    private int currentLife;
    private bool invincible = false;
    static int blinkingValue;
    private UIManager uiManager;
    private int coins;
    private float score;
    // Start is called before the first frame update
    void Start()
    {

      
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
       
        boxCollider = GetComponent<BoxCollider>();
        boxColliderSize = boxCollider.size;
        anim.Play("runStart");
        currentLife = maxLife;
        speed = minSpeed;
        blinkingValue = Shader.PropertyToID("_blinkingValue");
        uiManager = FindObjectOfType<UIManager>();

    }

    // Update is called once per frame
    void Update()
    {
        score += Time.deltaTime * speed;
        uiManager.UpdateScore((int)score);
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeLane(-1);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeLane(1);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Slide();
        }
        if (jumping)
        {
            float ratio = (transform.position.z - jumpStart) / jumpLenght;
            if (ratio >= 1f)
            {
                jumping = false;
                anim.SetBool("Jumping", false);
            }
            else
            {
                verticalTagetPosition.y = Mathf.Sin(ratio * Mathf.PI) * jumpHeight;
            }
        }
        else
        {
            verticalTagetPosition.y = Mathf.MoveTowards(verticalTagetPosition.y, 0, 5 * Time.deltaTime);
        }
        if (sliding)
        {
            float ratio = (transform.position.z - slideStart) / slideLenght;
            if (ratio >= 1f)
            {
                sliding = false;
                anim.SetBool("Sliding", false);
                boxCollider.size = boxColliderSize;
            }
        }
        if(Input.touchCount == 1)
        {
            if (isSwipping)
            {
                Vector2 diff = Input.GetTouch(0).position-startingTouch;
                diff = new Vector2(diff.x / Screen.width, diff.y / Screen.width);
                if (diff.magnitude > 0.01f)
                {
                    if (Mathf.Abs( diff.y) > Mathf.Abs(diff.x))
                    {
                        if (diff.y < 0)
                        {
                            Slide();
                        }
                        else
                        {
                            Jump();
                        }
                    }
                    else
                    {
                        if (diff.x < 0)
                        {
                            ChangeLane(-1);

                        }
                        else
                        {
                            ChangeLane(1);
                        }
                    }
                    isSwipping = false;
                }
            }
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                startingTouch = Input.GetTouch(0).position;
                isSwipping = true;
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                isSwipping = false;
            }
        }
        
        Vector3 targetPosition = new Vector3(verticalTagetPosition.x, verticalTagetPosition.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, laneSpeed * Time.deltaTime);
        
    }

    private void FixedUpdate()
    {
        rb.velocity = Vector3.forward * speed;
    }
    void ChangeLane(int direction)
    {
        
        int targetLane = currentLane + direction;
        if (targetLane < 0 || targetLane > 2)

            return;
        currentLane = targetLane;
        verticalTagetPosition = new Vector3((currentLane - 1), 0, 0);

    }
    void Jump()
    {
        if (!jumping)
        {
            jumpStart = transform.position.z;
            anim.SetFloat("JumpSpeed", speed / jumpLenght);
            anim.SetBool("Jumping", true);
            jumping = true;
        }
    }
    void Slide()
    {

        if(!jumping && !sliding)
        {
         
            slideStart = transform.position.z;
            anim.SetFloat("JumpSpeed", speed / slideLenght);
            anim.SetBool("Sliding", true);
            Vector3 newSize = boxCollider.size;
            newSize.y = newSize.y / 2;
            boxCollider.size = newSize;
            sliding = true;
            //boxCollider.size.y = 2;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            coins++;
            uiManager.UpdateCoins(coins);
            other.transform.parent.gameObject.SetActive(false);
        }
        if (invincible)
        {
            return;
        }
        if (other.CompareTag("Obstacle"))
        {
            currentLife--;
            uiManager.UpdateHeart(currentLife);
            anim.SetTrigger("Hit");
            speed = 0;
            if(currentLife <= 0)
            {
                speed = 0;
                anim.SetBool("Dead", true);
                uiManager.gameOverPanel.SetActive(true);
                Invoke("CallMenu", 2f);
            }
            else
            {
                StartCoroutine(Blinking(invincibleTime));
            }
        }
    }
    IEnumerator Blinking(float time)
    {
        invincible = true;
        float timer = 0;
        float currentBlink = 1f;
        float lastBlink = 0;
        float blinkPeriod = 0.1f;
        bool enable = false;
        yield return new WaitForSeconds(1f);
        speed = minSpeed;
        while(timer <time && invincible)
        {
            model.SetActive(enable);
            //Shader.SetGlobalFloat(blinkingValue, currentBlink);
            yield return null;
            timer += Time.deltaTime;
            lastBlink += Time.deltaTime;
           
            if (blinkPeriod < lastBlink)
            {
                lastBlink = 0;
                currentBlink = 1f - currentBlink;
                enable = !enable;
            }
        }
        model.SetActive(true);
        //Shader.SetGlobalFloat(blinkingValue, 0);
        invincible = false;
    }
    void CallMenu()
    {
        GameManager.gm.EmdRun();
    }

    public  void IncreaseSpeed()
    {
        speed *= 1.5f;
        if (speed >= maxSpeed)
            speed = maxSpeed;
            
        
    }
}
