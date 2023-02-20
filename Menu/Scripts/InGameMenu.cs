using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using TMPro;

public class InGameMenu: MonoBehaviour
{
    public Dictionary<string, System.Delegate> functions;
    public GameObject[] buttons;
    public GameObject MenuBarTop;
    public GameObject MenuBarLeft;
    public GameObject MenuContent;
    public TextMeshProUGUI MenuTime;
    public List<GameObject> MainMenus;
    public List<GameObject> SubMenus;
    public List<GameObject> TopBarButtons;
    public float[] SubMenusButtonPos;
    public float[] MainMenusButtonPos;
    public string firstMenu;
    public string activeMenu;

    private Gamepad gamepad;
    private Keyboard keyboard;

    void Start()
    {

        //This adds the event system if it can't already find one in the game
        EventSystem eventSystem = GameObject.FindObjectOfType<EventSystem>();
        InputSystemUIInputModule inputSystem = GameObject.FindObjectOfType<InputSystemUIInputModule>();

        if (eventSystem == null)
        {
            gameObject.AddComponent<EventSystem>();
        }

        if (inputSystem == null)
        {
            gameObject.AddComponent<InputSystemUIInputModule>();
        }

        //This gets references to the keyboard and gamepad
        keyboard = Keyboard.current;
        gamepad = Gamepad.current;

        //This loads the menu data. You may wish to call this function from another place
        InGameMenuFunctions.LoadMenuData();

    }

    void Update()
    {
       
        //This outputs the day and time on the menu
        if (MenuTime != null)
        {
            MenuTime.text = System.DateTime.Now.DayOfWeek.ToString() + " " + System.DateTime.Now.ToString("hh:mm");
        }

        //This function allows you to move back up menu tree to the first menu
        GoBack();

        //This function allows you to select the top bar button with the shoulder pad
        CycleTopBarButtons();

    }

    //This function allows you to move back up menu tree to the first menu
    private float pressTime;
 
    private void GoBack()
    {
        
        if (keyboard != null)
        {
            if (keyboard.escapeKey.isPressed == true & pressTime < Time.time)
            {
                InGameMenuFunctions.ActivateParentMenu();
                pressTime = Time.time + 0.25f;
            }
        }

        if (gamepad != null)
        {
            if (gamepad.bButton.isPressed == true & pressTime < Time.time)
            {
                InGameMenuFunctions.ActivateParentMenu();
                pressTime = Time.time + 0.25f;
            }

            if (gamepad.circleButton.isPressed == true & pressTime < Time.time)
            {
                InGameMenuFunctions.ActivateParentMenu();
                pressTime = Time.time + 0.25f;
            }

        }
    }


    //This function allows you to select the top bar button with the shoulder pad
    private int buttonNo;

    private void CycleTopBarButtons()
    {

        if (gamepad != null)
        {
            if (gamepad.leftShoulder.isPressed == true & pressTime < Time.time)
            {

                if (buttonNo > 0)
                {
                    buttonNo = buttonNo - 1;
                }
                else
                {
                    buttonNo = TopBarButtons.Count - 1;
                }

                TopBarButtons[buttonNo].GetComponent<Button>().Select();
                pressTime = Time.time + 0.25f;

            }
            else if (gamepad.rightShoulder.isPressed == true & pressTime < Time.time)
            {

                if (buttonNo < TopBarButtons.Count - 1)
                {
                    buttonNo = buttonNo + 1;
                }
                else
                {
                    buttonNo = 0;
                }

                TopBarButtons[buttonNo].GetComponent<Button>().Select();
                pressTime = Time.time + 0.25f;

            }
        }
    }

}
