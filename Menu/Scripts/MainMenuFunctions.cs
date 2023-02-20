using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public static class MainMenuFunctions
{
    //This creates a dictionary of the all the functions the menu can call
    public static void CreateFunctionDictionary(MainMenu mainMenu)
    {
        mainMenu.functions = new Dictionary<string, System.Delegate>();

        //Add your functions here
        mainMenu.functions.Add("StartGame", new System.Action(StartGame));
        mainMenu.functions.Add("ContinueGame", new System.Action(ContinueGame));
        mainMenu.functions.Add("QuitToDesktop", new System.Action(QuitToDesktop));
        mainMenu.functions.Add("QuitToMainMenu", new System.Action(QuitToMainMenu));
        mainMenu.functions.Add("SetWindowMode", new System.Action<string>(SetWindowMode));
        mainMenu.functions.Add("ChangeResolution", new System.Action<string>(ChangeResolution));
        mainMenu.functions.Add("ActivateSubMenu", new System.Action<string>(ActivateSubMenu));
        mainMenu.functions.Add("InvertHorizontal", new System.Action<string>(InvertHorizontal));
        mainMenu.functions.Add("InvertVertical", new System.Action<string>(InvertVertical));

    }

    //This grabs all the button prefabs ready to instantiate
    public static void LoadButtons()
    {
        MainMenu mainMenu = GameObject.FindObjectOfType<MainMenu>();

        mainMenu.buttons = Resources.LoadAll<GameObject>("MenuButtons");
    }

    //This loads json file and instaniates the buttons so the menu is ready to use
    public static void LoadMenuData()
    {
        //This function grabs all the button prefabs ready to instantiate
        LoadButtons();

        //This gets the reference to the main menu (NOTE: if you have more than one main menu you will need to change this line as this line will just grab the first menu reference it can find)
        MainMenu mainMenu = GameObject.FindObjectOfType<MainMenu>();

        //This creates the function dictionary
        CreateFunctionDictionary(mainMenu);

        //This loads all the information for the menu from the Json file
        TextAsset menuItemsFile = Resources.Load("Menufiles/MainMenu") as TextAsset;
        MenuItems menuItems = JsonUtility.FromJson<MenuItems>(menuItemsFile.text);

        //This creates the menu lists ready to use
        List<string> subMenus = new List<string>();
        mainMenu.SubMenus = new List<GameObject>();

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

            if (addMenu == true & menuItem.ParentMenu != "none")
            {
                subMenus.Add(menuItem.ParentMenu);
            }

        }

        //This allows the script to log the first menu created. It will be the first loaded
        bool firstSubMenuLogged = false;

        //This creates all the sub menus
        foreach (string tempMenu in subMenus)
        {

            if (firstSubMenuLogged == false)
            {
                mainMenu.firstMenu = tempMenu;
                firstSubMenuLogged = true;
            }

            GameObject subMenu = new GameObject();
            subMenu.transform.SetParent(mainMenu.MenuContent.transform);
            mainMenu.SubMenus.Add(subMenu);
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

        //This sets the initial drop level for all menus
        mainMenu.MenuButtonPosition = new float[subMenus.Count + 1];

        for (int i = 0; i < mainMenu.MenuButtonPosition.Length; i++)
        {
            mainMenu.MenuButtonPosition[i] = 20;
        }

        bool firstButtonSelected = false;

        //This adds all the menu buttons
        foreach (MenuItem menuItem in menuItems.menuData)
        {
            //This adds the menu buttons to the left bar
            if (menuItem.ParentMenu == "none")
            {

                GameObject button = null;

                foreach (GameObject tempButton in mainMenu.buttons)
                {
                    if (tempButton.name == menuItem.Type)
                    {
                        button = GameObject.Instantiate<GameObject>(tempButton);
                    }
                }

                if (button != null)
                {

                    button.GetComponentInChildren<TextMeshProUGUI>().text = menuItem.Name;

                    button.transform.SetParent(mainMenu.MenuBar.transform);
                    button.transform.localPosition = new Vector3(20, 0, 0);
                    button.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, mainMenu.MenuButtonPosition[0], 20);
                    button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

                    if (button.GetComponent<Button>() != null & menuItem.Function != "none")
                    {

                        if (menuItem.Variable == "none")
                        {
                            button.GetComponent<Button>().onClick.AddListener(() => mainMenu.functions[menuItem.Function].DynamicInvoke());
                        }
                        else
                        {
                            button.GetComponent<Button>().onClick.AddListener(() => mainMenu.functions[menuItem.Function].DynamicInvoke(menuItem.Variable));
                        }

                    }

                    button.name = menuItem.Name;

                    mainMenu.MenuButtonPosition[0] = mainMenu.MenuButtonPosition[0] + button.GetComponent<ButtonInfo>().buttonShift;

                }

                if (firstButtonSelected == false)
                {
                    button.GetComponent<Button>().Select();
                    firstButtonSelected = true;
                }

            }
            else
            {
                //This adds the actual menu content
                int i = 1;

                foreach (GameObject subMenu in mainMenu.SubMenus)
                {

                    if (menuItem.ParentMenu + "_Settings" == subMenu.name)
                    {
                        GameObject button = null;

                        float buttonShift = 0;

                        foreach (GameObject tempButton in mainMenu.buttons)
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
                        button.GetComponent<ButtonInfo>().name.text = menuItem.Name;

                        if (button.GetComponent<ButtonInfo>().description != null)
                        {
                            button.GetComponent<ButtonInfo>().description.text = menuItem.Description;
                        }

                        button.transform.localPosition = new Vector3(20, 0, 0);
                        button.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, mainMenu.MenuButtonPosition[i], 20);
                        button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

                        if (button.GetComponent<Button>() != null & menuItem.Function != "none")
                        {

                            if (menuItem.Variable == "none")
                            {
                                button.GetComponent<Button>().onClick.AddListener(() => mainMenu.functions[menuItem.Function].DynamicInvoke());
                            }
                            else
                            {
                                button.GetComponent<Button>().onClick.AddListener(() => mainMenu.functions[menuItem.Function].DynamicInvoke(menuItem.Variable));
                            }

                        }

                        mainMenu.MenuButtonPosition[i] = mainMenu.MenuButtonPosition[i] + buttonShift;
                    }

                    i = i + 1;

                }
            }

        }

        //This turns off all the sub menus
        foreach (GameObject subMenu in mainMenu.SubMenus)
        {
            subMenu.SetActive(false);
        }

        //This loads the first menu created
        ActivateSubMenu(mainMenu.firstMenu);

    }

    //This activate a sub menu
    public static void ActivateSubMenu(string menuName)
    {

        MainMenu mainMenu = GameObject.FindObjectOfType<MainMenu>();

        int i = 1;

        foreach (GameObject subMenu in mainMenu.SubMenus)
        {

            if (subMenu.name == menuName + "_Settings")
            {
                //This activates the selected menu
                RectTransform rt = subMenu.GetComponentInParent<RectTransform>();
                rt.sizeDelta = new Vector2(0, mainMenu.MenuButtonPosition[i]);
                subMenu.SetActive(true);
                mainMenu.MenuTitle.text = menuName;

                //This selects the first button in the menu
                Button firstButton = subMenu.GetComponentInChildren<Button>();

                if (firstButton != null)
                {
                    subMenu.GetComponentInChildren<Button>().Select();
                }

            }
            else
            {
                subMenu.SetActive(false); //This sets all the menu lists to inactive
            }

            i = i + 1;

        }

        mainMenu.activeMenu = menuName;

    }

    //Move back to parent menu
    public static void ActivateParentMenu()
    {

        MainMenu mainMenu = GameObject.FindObjectOfType<MainMenu>();

        //This loads all the information for the menu from the Json file
        TextAsset menuItemsFile = Resources.Load("Menufiles/MainMenu") as TextAsset;
        MenuItems menuItems = JsonUtility.FromJson<MenuItems>(menuItemsFile.text);

        foreach (MenuItem menuItem in menuItems.menuData)
        {
            if (menuItem.Name == mainMenu.activeMenu)
            {
                if (menuItem.ParentMenu != "none")
                {
                    ActivateSubMenu(menuItem.ParentMenu);
                }
                else
                {
                    ActivateSubMenu(mainMenu.firstMenu);
                }
            }
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

        ActivateSubMenu("Video");

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

        ActivateSubMenu("Video");

    }

    //Stand in function for starting the game
    public static void StartGame()
    {

        //Your code to start the game

        Debug.Log("Game Started");

    }

    //Stand in function for continuing the game
    public static void ContinueGame()
    {

        //Your code to continue the game

        Debug.Log("Continuing Game");

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
