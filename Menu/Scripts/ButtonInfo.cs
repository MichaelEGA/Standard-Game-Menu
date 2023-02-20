using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonInfo : MonoBehaviour
{
    public TextMeshProUGUI name; //The button name
    public TextMeshProUGUI description; //The description of what the button does (if button has one)
    public float buttonShift; //How far to move before placing the next button so that it doesn't overlap with this one
}
