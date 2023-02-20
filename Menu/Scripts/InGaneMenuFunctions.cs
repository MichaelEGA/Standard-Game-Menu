using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public static class InGameMenuFunctions
{
    //This creates a dictionary of the all the functions the menu can call
    public static void CreateFunctionDictionary(InGameMenu inGameMenu)
    {
        inGameMenu.functions = new Dictionary<string, System.Delegate>();

        //Add your functions here
        inGameMenu.functions.Add("QuitToDesktop", new System.Action(QuitToDesktop));
        inGameMenu.functions.Add("QuitToMainMenu", new System.Action(QuitToMainMenu));
        inGameMenu.functions.Add("SetWindowMode", new System.Action<string>(SetWindowMode));
        inGameMenu.functions.Add("ChangeResolution", new System.Action<string>(ChangeResolution));
        inGameMenu.functions.Add("ActivateSubMenu", new System.Action<string>(ActivateSubMenu));
        inGameMenu.functions.Add("InvertHorizontal", new System.Action<string>(InvertHorizontal));
        inGameMenu.functions.Add("InvertVertical", new System.Action<string>(InvertVertical));

    }

    //This grabs all the button prefabs ready to instantiate
    public static void LoadButtons()
    {
        InGameMenu inGameMenu = GameObject.FindObjectOfType<InGameMenu>();

        inGameMenu.buttons = Resources.LoadAll<GameObject>("MenuButtons");
    }

    //This loads json file and instaniates the buttons so the menu is ready to use
    public static void LoadMenuData()
    {
        //This function grabs all the button prefabs ready to instantiate
        LoadButtons();

        //This gets the reference to the ingame menu (NOTE: if you have more than one ingame menu you will need to change this line as this line will just grab the first menu reference it can find)
        InGameMenu inGameMenu = GameObject.FindObjectOfType<InGameMenu>();

        //This creates the function dictionary
        CreateFunctionDictionary(inGameMenu);

        //This loads all the information for the menu from the Json file
        TextAsset menuItemsFile = Resources.Load("Menufiles/InGameMenu") as TextAsset;
        MenuItems menuItems = JsonUtility.FromJson<MenuItems>(menuItemsFile.text);

        //This creates the menu lists ready to use
        List<string> mainMenus = new List<string>();
        inGameMenu.SubMenus = new List<GameObject>();

        List<string> subMenus = new List<string>();
        inGameMenu.SubMenus = new List<GameObject>();

        //This creates a list of all the sub-main menus
        foreach (MenuItem menuItem in menuItems.menuData)
        {

            if (menuItem.ParentMenu == "none")
            {
                mainMenus.Add(menuItem.Name);
            }

        }

        //This allows the menu to log the first menu created. This will be the first one loaded.
        bool firstMenuLogged = false;

        //This creates all the sub-main menus
        foreach (string tempMenu in mainMenus)
        {

            if (firstMenuLogged == false)
            {
                inGameMenu.firstMenu = tempMenu;
                firstMenuLogged = true;
            }

            GameObject mainMenu = new GameObject();
            mainMenu.transform.SetParent(inGameMenu.MenuBarLeft.transform);
            inGameMenu.MainMenus.Add(mainMenu);
            mainMenu.AddComponent<RectTransform>();
            RectTransform tempRect = mainMenu.GetComponent<RectTransform>();
            tempRect.anchorMin = new Vector2(0, 1);
            tempRect.anchorMax = new Vector2(0, 1);
            tempRect.pivot = new Vector2(0, 1);
            tempRect.pivot = new Vector2(0, 1);
            mainMenu.transform.localPosition = new Vector3(0, 0, 0);
            mainMenu.transform.localScale = new Vector3(1, 1, 1);
            mainMenu.name = tempMenu + "_Menu";
        }

        //This creates a list of all the sub menus
        foreach (MenuItem menuItem in menuItems.menuData)
        {

            bool addMenu = true;

            foreach (string menuItem2 in subMenus)
            {
                if (menuItem.ParentMenu == menuItem2)
                {
                    addMenu = false;
                }
            }

            foreach (string menuItem2 in mainMenus)
            {
                if (menuItem.ParentMenu == menuItem2)
                {
                    addMenu = false;
                }
            }

            if (addMenu == true & menuItem.ParentMenu != "none")
            {
                subMenus.Add(menuItem.ParentMenu);
            }

        }

        //This creates all the sub menus
        foreach (string tempMenu in subMenus)
        {
            GameObject subMenu = new GameObject();
            subMenu.transform.SetParent(inGameMenu.MenuContent.transform);
            inGameMenu.SubMenus.Add(subMenu);
            subMenu.AddComponent<RectTransform>();
            RectTransform tempRect = subMenu.GetComponent<RectTransform>();
            tempRect.anchorMin = new Vector2(0, 1);
            tempRect.anchorMax = new Vector2(0, 1);
            tempRect.pivot = new Vector2(0, 1);
            tempRect.pivot = new Vector2(0, 1);
            subMenu.transform.localPosition = new Vector3(0, 0, 0);
            subMenu.transform.localScale = new Vector3(1, 1, 1);
            subMenu.name = tempMenu + "_Settings";
        }

        //This sets the initial drop level for all main menus
        inGameMenu.MainMenusButtonPos = new float[mainMenus.Count];

        for (int i = 0; i < inGameMenu.MainMenusButtonPos.Length; i++)
        {
            inGameMenu.MainMenusButtonPos[i] = 20;
        }

        //This sets the initial drop level for all sub menus
        inGameMenu.SubMenusButtonPos = new float[subMenus.Count];

        for (int i = 0; i < inGameMenu.SubMenusButtonPos.Length; i++)
        {
            inGameMenu.SubMenusButtonPos[i] = 20;
        }

        float topBarButtonPos = 0;
        bool firstButtonSelected = false;

        //This adds all the menu buttons
        foreach (MenuItem menuItem in menuItems.menuData)
        {
            //This adds menu buttons for the top bar
            if (menuItem.ParentMenu == "none")
            {

                GameObject button = null;

                foreach (GameObject tempButton in inGameMenu.buttons)
                {
                    if (tempButton.name == menuItem.Type)
                    {
                        button = GameObject.Instantiate<GameObject>(tempButton);
                    }
                }

                button.GetComponentInChildren<TextMeshProUGUI>().text = menuItem.Name;

                button.transform.SetParent(inGameMenu.MenuBarTop.transform);
                button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                button.GetComponent<RectTransform>().anchoredPosition = new Vector3(topBarButtonPos, 0, 0);

                if (button.GetComponent<Button>() != null & menuItem.Function != "none")
                {

                    if (menuItem.Variable == "none")
                    {
                        button.GetComponent<Button>().onClick.AddListener(() => inGameMenu.functions[menuItem.Function].DynamicInvoke());
                    }
                    else
                    {
                        button.GetComponent<Button>().onClick.AddListener(() => inGameMenu.functions[menuItem.Function].DynamicInvoke(menuItem.Variable));
                    }

                }

                button.name = menuItem.Name;

                topBarButtonPos = topBarButtonPos + button.GetComponent<ButtonInfo>().buttonShift;
                
                //This ensures the first button is selected for gamepads
                if (firstButtonSelected == false)
                {
                    button.GetComponent<Button>().Select();
                    firstButtonSelected = true;
                }

                //This adds the top bar buttons to a list which can be cycled through by shoulder buttons on game pads
                inGameMenu.TopBarButtons.Add(button);
            
            }
            else 
            {
                //This adds buttons for left bar
                int i = 0;

                foreach (GameObject mainMenu in inGameMenu.MainMenus)
                {

                    if (menuItem.ParentMenu + "_Menu" == mainMenu.name)
                    {
                        GameObject button = null;

                        foreach (GameObject tempButton in inGameMenu.buttons)
                        {
                            if (tempButton.name == menuItem.Type)
                            {
                                button = GameObject.Instantiate<GameObject>(tempButton);
                            }
                        }

                        if (button != null)
                        {

                            button.GetComponentInChildren<TextMeshProUGUI>().text = menuItem.Name;

                            button.transform.SetParent(mainMenu.transform);
                            button.transform.localPosition = new Vector3(20, 0, 0);
                            button.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, inGameMenu.MainMenusButtonPos[i], 20);
                            button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

                            if (button.GetComponent<Button>() != null & menuItem.Function != "none")
                            {

                                if (menuItem.Variable == "none")
                                {
                                    button.GetComponent<Button>().onClick.AddListener(() => inGameMenu.functions[menuItem.Function].DynamicInvoke());
                                }
                                else
                                {
                                    button.GetComponent<Button>().onClick.AddListener(() => inGameMenu.functions[menuItem.Function].DynamicInvoke(menuItem.Variable));
                                }

                            }

                            button.name = menuItem.Name;

                            inGameMenu.MainMenusButtonPos[i] = inGameMenu.MainMenusButtonPos[i] + button.GetComponent<ButtonInfo>().buttonShift;

                        }
                    }

                    i = i + 1;

                }

                i = 0;

                //This adds the actual menu content
                foreach (GameObject subMenu in inGameMenu.SubMenus)
                {

                    if (menuItem.ParentMenu + "_Settings" == subMenu.name)
                    {
                        GameObject button = null;

                        float buttonShift = 0;

                        foreach (GameObject tempButton in inGameMenu.buttons)
                        {
                            if (tempButton.name == menuItem.Type)
                            {
                                button = GameObject.Instantiate<GameObject>(tempButton);
                            }
                        }

                        if (button.GetComponent<ButtonInfo>() != null)
                        {
                            buttonShift = button.GetComponent<ButtonInfo>().buttonShift;
                        }
                        else
                        {
                            buttonShift = 60;
                        }

                        button.transform.SetParent(subMenu.transform);

                        if (button.GetComponent<ButtonInfo>().name != null)
                        {
                            button.GetComponent<ButtonInfo>().name.text = menuItem.Name;
                        }

                        if (button.GetComponent<ButtonInfo>().description != null)
                        {
                            button.GetComponent<ButtonInfo>().description.text = menuItem.Description;
                        }

                        button.transform.localPosition = new Vector3(20, 0, 0);
                        button.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, inGameMenu.SubMenusButtonPos[i], 20);
                        button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

                        if (button.GetComponent<Button>() != null & menuItem.Function != "none")
                        {

                            if (menuItem.Variable == "none")
                            {
                                button.GetComponent<Button>().onClick.AddListener(() => inGameMenu.functions[menuItem.Function].DynamicInvoke());
                            }
                            else
                            {
                                button.GetComponent<Button>().onClick.AddListener(() => inGameMenu.functions[menuItem.Function].DynamicInvoke(menuItem.Variable));
                            }

                        }

                        inGameMenu.SubMenusButtonPos[i] = inGameMenu.SubMenusButtonPos[i] + buttonShift;
                    }

                    i = i + 1;

                }
            }

        }

        inGameMenu.MenuBarTop.GetComponent<RectTransform>().sizeDelta = new Vector2(topBarButtonPos, 0);

        //This turns off all the main menus
        foreach (GameObject mainMenu in inGameMenu.MainMenus)
        {
            mainMenu.SetActive(false);
        }

        //This turns off all the sub menus
        foreach (GameObject subMenu in inGameMenu.SubMenus)
        {
            subMenu.SetActive(false);
        }

        //This activates the first menu on load
        ActivateSubMenu(inGameMenu.firstMenu);

    }


    //This activate a sub menu
    public static void ActivateSubMenu(string menuName)
    {

        InGameMenu inGameMenu = GameObject.FindObjectOfType<InGameMenu>();

        bool isMainMenu = false;

        foreach (GameObject mainMenu in inGameMenu.MainMenus)
        {
            if (mainMenu.name == menuName + "_Menu")
            {
                isMainMenu = true;
            }
        }

        int i = 0;

        if (isMainMenu == true)
        {

            foreach (GameObject mainMenu in inGameMenu.MainMenus)
            {

                if (mainMenu.name == menuName + "_Menu")
                {
                    //This activates the selected menu
                    RectTransform rt = mainMenu.GetComponentInParent<RectTransform>();
                    rt.sizeDelta = new Vector2(0, inGameMenu.MainMenusButtonPos[i]);
                    mainMenu.SetActive(true);

                    //This sets all the menu lists to inactive
                    foreach (GameObject subMenu in inGameMenu.SubMenus)
                    {
                        subMenu.SetActive(false);
                    }

                    //This selects the first button in the menu
                    Button firstButton = mainMenu.GetComponentInChildren<Button>();

                    if (firstButton != null)
                    {
                        mainMenu.GetComponentInChildren<Button>().Select();
                    }

                }
                else
                {
                    mainMenu.SetActive(false);
                }

                i = i + 1;

            }

        }
        else
        {

            foreach (GameObject subMenu in inGameMenu.SubMenus)
            {

                if (subMenu.name == menuName + "_Settings")
                {
                    RectTransform rt = subMenu.GetComponentInParent<RectTransform>();
                    rt.sizeDelta = new Vector2(0, inGameMenu.SubMenusButtonPos[i]);
                    subMenu.SetActive(true);

                    //This selects the first button in the menu
                    Button firstButton = subMenu.GetComponentInChildren<Button>();

                    if (firstButton != null)
                    {
                        subMenu.GetComponentInChildren<Button>().Select();
                    }

                }
                else
                {
                    subMenu.SetActive(false);
                }

                i = i + 1;

            }

        }

        inGameMenu.activeMenu = menuName;

    }

    //Move back to parent menu
    public static void ActivateParentMenu()
    {

        InGameMenu inGameMenu = GameObject.FindObjectOfType<InGameMenu>();

        //This loads all the information for the menu from the Json file
        TextAsset menuItemsFile = Resources.Load("Menufiles/MainMenu") as TextAsset;
        MenuItems menuItems = JsonUtility.FromJson<MenuItems>(menuItemsFile.text);

        bool menuFound = false;

        foreach (MenuItem menuItem in menuItems.menuData)
        {
            if (menuItem.Name == inGameMenu.activeMenu)
            {
                if (menuItem.ParentMenu != "none")
                {
                    ActivateSubMenu(menuItem.ParentMenu);
                }
                else
                {
                    ActivateSubMenu(inGameMenu.firstMenu);
                }

                menuFound = true;

            }
            
        }

        if (menuFound == false)
        {
            ActivateSubMenu(inGameMenu.firstMenu);
        }

    }

    //This sets the screen resolution
    public static void ChangeResolution(string resolution)
    {

        FullScreenMode screenMode = Screen.fullScreenMode;

        if (resolution == "Detect Screen Resolution")
        {
            int width = Display.main.systemWidth;
            int height = Display.main.systemHeight;
            Screen.SetResolution(width, height, screenMode);
        }
        else if (resolution == "640 x 480 (4:3)")
        {
            Screen.SetResolution(640, 480, screenMode);
        }
        else if (resolution == "640 x 360 (16:9)")
        {
            Screen.SetResolution(640, 360, screenMode);
        }
        else if (resolution == "640 x 400 (16:10)")
        {
            Screen.SetResolution(640, 400, screenMode);
        }
        else if (resolution == "800 x 600 (4:3)")
        {
            Screen.SetResolution(800, 600, screenMode);
        }
        else if (resolution == "848 x 450 (16:9)")
        {
            Screen.SetResolution(848, 450, screenMode);
        }
        else if (resolution == "960 x 600 (16:10)")
        {
            Screen.SetResolution(960, 600, screenMode);
        }
        else if (resolution == "1024 x 768 (4:3)")
        {
            Screen.SetResolution(1024, 768, screenMode);
        }
        else if (resolution == "1024 x 576 (16:9)")
        {
            Screen.SetResolution(1024, 576, screenMode);
        }
        else if (resolution == "1024 x 640 (16:10)")
        {
            Screen.SetResolution(1024, 640, screenMode);
        }
        else if (resolution == "1220 x 915 (4:3)")
        {
            Screen.SetResolution(1220, 915, screenMode);
        }
        else if (resolution == "1220 x 720 (16:9)")
        {
            Screen.SetResolution(1220, 720, screenMode);
        }
        else if (resolution == "1280 x 800 (16:10)")
        {
            Screen.SetResolution(1280, 800, screenMode);
        }
        else if (resolution == "1680 × 1260 (4:3)")
        {
            Screen.SetResolution(1680, 1260, screenMode);
        }
        else if (resolution == "1680 × 945 (16:9)")
        {
            Screen.SetResolution(1680, 945, screenMode);
        }
        else if (resolution == "1680 × 1050 (16:10)")
        {
            Screen.SetResolution(1680, 1050, screenMode);
        }
        else if (resolution == "1920 x 1440 (4:3)")
        {
            Screen.SetResolution(1920, 1440, screenMode);
        }
        else if (resolution == "1920 x 1080 (16:9)")
        {
            Screen.SetResolution(1920, 1080, screenMode);
        }
        else if (resolution == "1920 x 1200 (16:10)")
        {
            Screen.SetResolution(1920, 1200, screenMode);
        }
        else if (resolution == "2560 x 1920 (4:3)")
        {
            Screen.SetResolution(2560, 1920, screenMode);
        }
        else if (resolution == "2560 x 1440 (16:9)")
        {
            Screen.SetResolution(2560, 1440, screenMode);
        }
        else if (resolution == "2560 x 1600 (16:10)")
        {
            Screen.SetResolution(2560, 1600, screenMode);
        }
        else if (resolution == "3840 x 2880 (4:3)")
        {
            Screen.SetResolution(3840, 2880, screenMode);
        }
        else if (resolution == "3840 x 2160 (16:9)")
        {
            Screen.SetResolution(3840, 2160, screenMode);
        }
        else if (resolution == "3840 x 2400 (16:10)")
        {
            Screen.SetResolution(3840, 2400, screenMode);
        }
        else if (resolution == "7680 x 5760 (4:3)")
        {
            Screen.SetResolution(7680, 5760, screenMode);
        }
        else if (resolution == "7680 x 4320 (16:9)")
        {
            Screen.SetResolution(7680, 4320, screenMode);
        }
        else if (resolution == "7680 x 4800 (16:10)")
        {
            Screen.SetResolution(7680, 4800, screenMode);
        }

        ActivateSubMenu("Settings");

    }

    //This sets the window mode
    public static void SetWindowMode(string windowMode)
    {

        int widthRes = Screen.currentResolution.width;
        int heightRes = Screen.currentResolution.height;

        if (windowMode == "Windowed")
        {
            Screen.SetResolution(widthRes, heightRes, FullScreenMode.Windowed);
        }
        else if (windowMode == "MaximizedWindow")
        {
            Screen.SetResolution(widthRes, heightRes, FullScreenMode.MaximizedWindow);
        }
        else if (windowMode == "FullScreenWindow")
        {
            Screen.SetResolution(widthRes, heightRes, FullScreenMode.FullScreenWindow);
        }
        else if (windowMode == "ExclusiveFullScreen")
        {
            Screen.SetResolution(widthRes, heightRes, FullScreenMode.ExclusiveFullScreen);
        }

        ActivateSubMenu("Settings");

    }

    //Stand in function for inverting the horizontal view
    public static void InvertHorizontal(string choice)
    {
        
        if (choice == "true")
        {
            //Your code to invert the horizontal
            Debug.Log("Horizontal Inverted");
        }
        else
        {
            //Your code to set the horizontal to normal
            Debug.Log("Horizontal Normal");
        }

        ActivateSubMenu("Controls");

    }

    //Stand in function for inverting the vertical view
    public static void InvertVertical(string choice)
    {
        if (choice == "true")
        {
            //Your code to invert the horizontal
            Debug.Log("Vertical Inverted");
        }
        else
        {
            //Your code to set the horizontal to normal
            Debug.Log("Vertical Normal");
        }

        ActivateSubMenu("Controls");

    }

    //Stand in funciton for returning to the main menu
    public static void QuitToMainMenu()
    {
        //Close game and return to main menu

        Debug.Log("Return to Main Menu");

    }

    //This quits the application and return you to the desk. NOTE: doesn't work in editor (obviously) only build
    public static void QuitToDesktop()
    {
        Debug.Log("Quitting the Game - NOTE: Only works in build");

        Application.Quit();
    }

    //This allows you to pause the game when the menu is active
    public static void PauseGame(bool isPaused)
    {

        if (isPaused == true)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }

    }

    //This coroutine can be used to fade the canvas group in
    public static IEnumerator FadeInCanvas(CanvasGroup canvasGroup, float duration)
    {

        //This sets the starting alpha value to 0
        float alpha1 = 0;

        //This fades in the canvas
        while (alpha1 < 1)
        {
            alpha1 = alpha1 + (1f / (60f * duration));
            canvasGroup.alpha = alpha1;
            yield return new WaitForSecondsRealtime(0.016f);
        }


    }

    //This coroutine can be used to fade the canvas group out
    public static IEnumerator FadeOutCanvas(CanvasGroup canvasGroup, float duration)
    {

        //This sets the starting alpha value to 1
        float alpha2 = 1;

        //This fades the canvas out
        while (alpha2 > 0)
        {
            alpha2 = alpha2 - (1f / (60f * duration));
            canvasGroup.alpha = alpha2;
            yield return new WaitForSecondsRealtime(0.016f);
        }

    }

}
