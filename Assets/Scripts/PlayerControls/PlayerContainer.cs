using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

/**************************************
 * class responsible for handling the player leaving/joining.
 * handles player death, respawn timer, and respawning.
 * 
 * component of the playerContainer prefab.
 * 
 * Pacifica Morrow
 * 03.06.2025
 * **************************************/

public class PlayerContainer : MonoBehaviour
{
    [Header("Editable Fields")]
    [SerializeField] private GameObject player;

    public bool onRespawnCooldown; // if the player can respawn or not; if false, player can respawn.
    
    // color fields
    private bool colorChosen;
    private int playerColor = 4; //defaults the player color to 4, or silver. see this.SetColorVector2() for details.

    private ColorPicker colorPicker; // reference to the colorpicker.


    private void Start()
    {
        colorPicker = GetComponent<ColorPicker>();
        DontDestroyOnLoad(gameObject);
    }

    //called by Player Input component, sends vector2 to SetColorVector.
    public void OnColorAction(InputAction.CallbackContext ctx) => SetColorVector(ctx.ReadValue<Vector2>());

    ///outdated -- string approach
    // called by PlayerInput component, sends a string containing the button path (which button is pressed) to the SetColor method
    //public void OnColorAction(InputAction.CallbackContext ctx) => SetColor(ctx.control.path);

    // Called by pressing the Start button; Start on controller, space on keyboard.
    public void OnStart(InputAction.CallbackContext ctx)
    {
        // if the color is already chosen,
        if (colorChosen)
        {
            SpawnPlayer();
        }

        // if the color hasnt been chosen,
        else
        {
            StartPlayer();

            colorChosen = true;
        }
    }

    

    // SWITCH VECTOR2 APPROACH
    private void SetColorVector(Vector2 compositeXYAB)
    {
        if (!colorChosen && compositeXYAB != Vector2.zero)
        {
            Debug.Log(compositeXYAB.ToString());

            switch (compositeXYAB.ToString())
            {
                case ("(0.00, 1.00)"):
                    // the player is the Senior's.
                    // player color is 0, aka the first one listed in the ColorPicker component.
                    playerColor = 0;
                    break;
                case ("(0.00, -1.00)"):
                    // the player is the Freshman's
                    // player color is 3, aka the fourth one listed in the ColorPicker component.
                    playerColor = 3;
                    break;
                case ("(-1.00, 0.00)"):
                    // the player is the Junior's
                    // player color is 1, aka the second one listed in the ColorPicker component.
                    playerColor = 1;
                    break;
                case ("(1.00, 0.00)"):
                    // the player is the Soph's
                    // player color is 3, aka the fourth one listed in the ColorPicker component.
                    playerColor = 2;
                    break;

                // UNEXPECTED BUTTON INPUT.
                default:
                    // playercolor is white; default.
                    Debug.LogWarning("Unexpected Input!");
                    playerColor = 4;
                    break;
            }
        }
    }

    /* BUTTON PATH STRING APPROACH
   // assigns a color based on what button was pressed. 
   // see switch cases for which button is which color.
   private void SetColor(string buttonPath)
   {
       if (!colorChosen)
       {
           Debug.Log(buttonPath);

           switch (buttonPath)
           {
               // CLASS CONTROLLER
               case "/DualShock4GamepadHID/buttonNorth":
                   // the player is the Senior's.
                   // player color is 0, aka the first one listed in the ColorPicker component.
                   playerColor = 0;
                   break;

               case "/DualShock4GamepadHID/buttonSouth":
                   // the player is the Freshman's
                   // player color is 3, aka the fourth one listed in the ColorPicker component.
                   playerColor = 3;
                   break;

               case "/DualShock4GamepadHID/buttonWest":
                   // the player is the Junior's
                   // player color is 1, aka the second one listed in the ColorPicker component.
                   playerColor = 1;
                   break;

               case "/DualShock4GamepadHID/buttonEast":
                   // the player is the Soph's
                   // player color is 3, aka the fourth one listed in the ColorPicker component.
                   playerColor = 2;
                   break;

               // UNEXPECTED BUTTON INPUT.
               default:
                   // playercolor is white; default.
                   Debug.LogWarning("Unexpected Input!");
                   playerColor = 4;
                   break;
           }
       }
   } 
   */

    /* [OUTDATED] IF STATEMENT APPROACH
    private void SetColorVector(Vector2 compositeXYAB)
    {
        if (!colorChosen)
        {
            if (compositeXYAB == Vector2.up) //if the butten pressed is north
            {
                playerColor = 0; //the color is the first one in the ColorPicker component.
            }

            else if (compositeXYAB == Vector2.down) //if the butten pressed is south
            {
                playerColor = 1; //the color is the second one in the ColorPicker component.
            }

            else if (compositeXYAB == Vector2.left) //if the button pressed is west
            {
                playerColor = 2; //the color is the third one in the ColorPicker component.
            }

            else if (compositeXYAB == Vector2.right) //if the button pressed is east
            {
                playerColor = 3; //the color is the fourth one in the ColorPicker component.
            }

            colorChosen = true;
        }
    }*/

    // enables the player without changing the color;
    // called by OnStart if player HAS already selected a color
    private void SpawnPlayer()
    {
        if (!onRespawnCooldown)
        {
            player.SetActive(true);
            Debug.Log("Player Spawned!");
        }
    }

    // enables the player and sets its color to whatever colour they selected.
    // called by OnStart if player HASN'T selected a color
    private void StartPlayer()
    {
        player.SetActive(true);

        // sets the player's color.
        Renderer renderer = player.GetComponentInChildren<Renderer>();
        renderer.material.color = GetComponent<ColorPicker>().GetColor(playerColor);
    }


}