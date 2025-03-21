using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;
using TMPro;

/*******************************************
 * Class responsible for controling the player & administering powerups.
 * 
 * component of the Player.
 * 
 * 01.10.2025
 * Pacifica Morrow
 * ****************************************/

public class PlayerControler : MonoBehaviour
{
    [SerializeField] private float moveForceMagnitude;
    [SerializeField] private Transform focalPoint;
    [SerializeField] private Light powerUpIndicator;

    private Rigidbody rb;
    private GameObject player;
    private SphereCollider playerCollider;
    private PlayerInputActions playerInputActions;
    private Death death;
    private PlayerContainer containerClass;

    private Vector2 moveVector;
    private Vector3 startPos;    

    public bool hasPowerUp {  get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        hasPowerUp = GameManager.Singleton.debugPowerUpRepel;
        startPos = transform.position;

        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<SphereCollider>();
        powerUpIndicator = GetComponent<Light>();
        death = GetComponentInParent<Death>();
        containerClass = GetComponent<PlayerContainer>();

        playerCollider.material.bounciness = 0.4f;

        player = this.gameObject;

        player.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
    }

    // OnDisable is called when the player is disabled, aka when it "dies".
    private void OnDisable()
    {
        transform.position = startPos;
        rb.velocity = Vector3.zero;
    }


    // called by the Player Input component of the player, sets the player's moveVector to the value of the movement keys via calling SetMoveVector.
    public void OnInputAction(InputAction.CallbackContext ctx) => SetMoveVector(ctx.ReadValue<Vector2>());

    private void SetMoveVector(Vector2 value)
    {
        moveVector = value;
    }

    // assigns the player's values with those of the GameManager for the current level
    private void AssignLevelValues()
    {
        transform.localScale = GameManager.Singleton.playerScale;
        rb.mass = GameManager.Singleton.playerMass;
        rb.drag = GameManager.Singleton.playerDrag;
        moveForceMagnitude = (GameManager.Singleton.playerMoveForce) * 10;

        Debug.Log($"{rb.mass}, {rb.drag}, {moveForceMagnitude}, {focalPoint.name}");
    }

    private void Move()
    {
        if (focalPoint != null)
        {
            rb.AddForce((focalPoint.forward.normalized * moveVector.y) * moveForceMagnitude * Time.deltaTime);
            rb.AddForce((focalPoint.right.normalized * moveVector.x) * moveForceMagnitude * Time.deltaTime);
        }

        else
        {
            focalPoint = GameObject.Find("FocalPoint").transform;
            Debug.Log($"focalpoint = {focalPoint.name}");
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            AssignLevelValues();
            playerCollider.material.bounciness = GameManager.Singleton.playerBounce;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Portal"))
        {
            gameObject.layer = LayerMask.NameToLayer("Portal");
        }

        /*
        if (other.gameObject.CompareTag("PowerUp"))
        {
            PowerUpControler otherPowerUpControler = other.GetComponent<PowerUpControler>();
            float otherPowerUpCooldown = otherPowerUpControler.GetCooldown();

            
            StartCoroutine(powerUpCooldown(otherPowerUpCooldown));

            other.gameObject.SetActive(false);
        }*/

        if (other.gameObject.CompareTag("KillPlane"))
        {
            death.PlayerDeath();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Portal"))
        {
            gameObject.layer = LayerMask.NameToLayer("Player");

            // If, when the player leaves the portal, it is under the portal, IE it has gone through the portal
            if (transform.position.y < -1.0f)
            {
                string portalDestination = other.GetComponent<PortalController>().GetDestination();
                IslandManager.Singleton.SwitchLevels(portalDestination);
                
                rb.velocity = Vector3.zero;
                transform.position = Vector3.up * 25;
            }
        }
    }

    /*
    private IEnumerator powerUpCooldown(float cooldown)
    {
        hasPowerUp = true;
        powerUpIndicator.intensity = 5.0f;

        yield return new WaitForSeconds(cooldown);

        hasPowerUp = false;
        powerUpIndicator.intensity = 0.0f;
    }*/
}
