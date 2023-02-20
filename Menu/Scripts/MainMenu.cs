using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using TMPro;

public class MainMenu: MonoBehaviour
{
    public EventSystem eventSystem;
    public Dictionary<string, System.Delegate> functions;
    public GameObject[] buttons;
    public GameObject MenuBar;
    public GameObject MenuContent;
    public TextMeshProUGUI MenuTitle;
    public TextMeshProUGUI MenuTime;
    public List<GameObject> SubMenus;
    public float[] MenuButtonPosition;
    public string firstMenu;
    public string activeMenu;
    private float pressTime;

    void Start()
    {

        //This adds the event system if it can't already find one in the game
        eventSystem = GameObject.FindObjectOfType<EventSystem>();
        InputSystemUIInputModule inputSystem = GameObject.FindObjectOfType<InputSystemUIInputModule>();

        if (eventSystem == null)
        {
            gameObject.AddComponent<EventSystem>();
        }

        if (inputSystem == null)
        {
            gameObject.AddComponent<InputSystemUIInputModule>();
        }

        //This loads the menu data. You may wish to call this function from another place
        MainMenuFunctions.LoadMenuData();

    }

    void Update()
    {
        //This outputs the day and time on the menu
        if (MenuTime != null)
        {
            MenuTime.text = System.DateTime.Now.DayOfWeek.ToString() + " " + System.DateTime.Now.ToString("hh:mm");
        }

        GoBack();

    }

    private void GoBack()
    {
        Keyboard keyboard = Keyboard.current;
        Gamepad gamepad = Gamepad.current;

        if (keyboard != null)
        {
            if (keyboard.escapeKey.isPressed == true & pressTime < Time.time)
            {
                MainMenuFunctions.ActivateParentMenu();
                pressTime = Time.time + 0.25f;
            }
        }

        if (gamepad != null)
        {
            if (gamepad.bButton.isPressed == true & pressTime < Time.time)
            {
                MainMenuFunctions.ActivateParentMenu();
                pressTime = Time.time + 0.25f;
            }

            if (gamepad.circleButton.isPressed == true & pressTime < Time.time)
            {
                MainMenuFunctions.ActivateParentMenu();
                pressTime = Time.time + 0.25f;
            }
        }
    }
    
}
