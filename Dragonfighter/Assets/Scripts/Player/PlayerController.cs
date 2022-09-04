using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float rotationSpeed;
    public Animator dragonAnimator;
    public GameObject cameraObject;
    bool actionRunning = false;
    float translation, rotation;

    private bool isFlying;

    float health = 100f;
    [SerializeField]
    private Image healthStats;
    void Update()
    {
        if (actionRunning == false)
        {
            translation = Input.GetAxisRaw("Vertical") * speed;
            rotation = Input.GetAxisRaw("Horizontal") * rotationSpeed;

        }

        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;

        transform.Translate(0, 0, translation);

        transform.Rotate(0, rotation, 0);
        AnimationInputControls();
        healthStats.fillAmount = health / 100;

        if(Input.GetKey(KeyCode.F))
        {
            isFlying = true;
            dragonAnimator.SetBool("Fly", true);
            this.gameObject.GetComponent<Rigidbody>().useGravity = false;
            transform.Translate(0, 0.01f, 0);
        }
        //if (!Input.GetKey(KeyCode.F))
        //{
        //    dragonAnimator.SetBool("Fly", false);
        //    isFlying = true;
        //    transform.Translate(0, 0, 0);
        //}
        if (Input.GetKey(KeyCode.E))
        {
            transform.Translate(0, -0.01f, 0);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 0.2f))
            {
                if(hit.collider.gameObject.tag == "Ground")
                {
                    isFlying = false;
                    dragonAnimator.SetBool("Fly", false);
                    this.gameObject.GetComponent<Rigidbody>().useGravity = true;
                }
            }
        }
        if(!Input.anyKey)
        {
            this.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        Die();
    }

    private void OnCollisionEnter(Collision target)
    {
        if(target.gameObject.tag == "Ground")
        {
            dragonAnimator.SetTrigger("Grounded");
        }
    }
    void AnimationInputControls()
    {
        if (translation > 0f && Input.GetKey(KeyCode.LeftShift))
        {
            dragonAnimator.SetBool("Run", true);
            dragonAnimator.SetBool("Walk", false);
            speed = 5f;
        }
        else if (translation > 0f && Input.GetKeyUp(KeyCode.LeftShift))
        {
            dragonAnimator.SetBool("Walk", true);
            dragonAnimator.SetBool("Run", false);
            speed = 2f;
        }
        else if (translation > 0f || translation < 0f)
        {
            dragonAnimator.SetBool("Walk", true);
            dragonAnimator.SetBool("Run", false);
            speed = 2f;
        }
        else
        {
            dragonAnimator.SetBool("Walk", false);
            dragonAnimator.SetBool("Run", false);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            dragonAnimator.SetTrigger("Jump");
        }

        if (isFlying && Input.GetKey(KeyCode.LeftShift) && translation > 0)
        {
            dragonAnimator.SetBool("FlyForward", true);
            dragonAnimator.SetBool("FlyBack", false);
            dragonAnimator.SetBool("Fly", false);
        }
        else if (isFlying && !Input.GetKey(KeyCode.LeftShift))
        {
            dragonAnimator.SetBool("FlyForward", false);
            dragonAnimator.SetBool("FlyBack", false);
        }
        if(isFlying && translation < 0)
        {
            dragonAnimator.SetBool("FlyBack", true);
        }
        if (isFlying && translation >= 0)
        {
            dragonAnimator.SetBool("FlyBack", false);
        }
    }
    IEnumerator WaitForAction(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        actionRunning = false;
    }



    public void ReduceHealth(float _damage)
    {
        health -= _damage;
    }
    public void RecoverHealth(float _damage)
    {
        health += _damage;
    }
    public void Die()
    {
        if (health <= 0)
        {
            healthStats.fillAmount = health / 100;
            StartCoroutine(DeathAnime());
        }
    }
    IEnumerator DeathAnime()
    {
        cameraObject.transform.SetParent(null);
        dragonAnimator.SetTrigger("DeathAnime");
        //gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
        //gameObject.transform.localRotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, 70f);
        gameObject.GetComponent<PlayerController>().enabled = false;
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
    }

    public bool CheckFlying()
    {
        return isFlying;
    }
}
