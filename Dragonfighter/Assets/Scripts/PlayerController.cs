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
    public GameObject attack_Point;
    float health = 100f;
    [SerializeField]
    //private Image healthStats;
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
        //healthStats.fillAmount = health / 100;
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
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            dragonAnimator.SetTrigger("Strike");
            actionRunning = true;
            StartCoroutine(WaitForAction(1.5f));
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            dragonAnimator.ResetTrigger("Strike");

        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            dragonAnimator.SetTrigger("Roar");
            //SoundManager.Instance.PlaySound(3);
            actionRunning = true;
            StartCoroutine(WaitForAction(3f));
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            dragonAnimator.ResetTrigger("Roar");
        }
    }
    IEnumerator WaitForAction(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        actionRunning = false;
    }

    private void OnTriggerEnter(Collider target)
    {
        if (target.CompareTag("Home"))
        {
            //SoundManager.Instance.PlaySound(1);
            //GameManager.Instance.ShowGameMenuAtWin();
            StartCoroutine(WinAnimation());
        }
    }

    IEnumerator WinAnimation()
    {
        cameraObject.transform.SetParent(null);
        gameObject.GetComponent<PlayerController>().enabled = false;
        dragonAnimator.SetBool("Walk", false);
        dragonAnimator.SetBool("Run", false);
        yield return new WaitForSeconds(3);
        dragonAnimator.SetTrigger("Roar");
        //SoundManager.Instance.PlaySound(3);
    }
    void Turn_On_AttackPoint()
    {
        attack_Point.SetActive(true);
    }

    void Turn_Off_AttackPoint()
    {
        if (attack_Point.activeInHierarchy)
        {
            attack_Point.SetActive(false);
        }
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
            //healthStats.fillAmount = health / 100;
            StartCoroutine(DeathAnime());
        }
    }
    IEnumerator DeathAnime()
    {
        cameraObject.transform.SetParent(null);
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
        gameObject.transform.localRotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, 70f);
        gameObject.GetComponent<PlayerController>().enabled = false;
        //SoundManager.Instance.StopAllSounds();
        //GameManager.Instance.ShowGameMenuAtDeath();
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
        //Destroy(gameObject);
    }
}
