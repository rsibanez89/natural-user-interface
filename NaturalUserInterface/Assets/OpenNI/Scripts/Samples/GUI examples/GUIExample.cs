/****************************************************************************
*                                                                           *
*  OpenNI Unity Toolkit                                                     *
*  Copyright (C) 2011 PrimeSense Ltd.                                       *
*                                                                           *
*                                                                           *
*  OpenNI is free software: you can redistribute it and/or modify           *
*  it under the terms of the GNU Lesser General Public License as published *
*  by the Free Software Foundation, either version 3 of the License, or     *
*  (at your option) any later version.                                      *
*                                                                           *
*  OpenNI is distributed in the hope that it will be useful,                *
*  but WITHOUT ANY WARRANTY; without even the implied warranty of           *
*  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the             *
*  GNU Lesser General Public License for more details.                      *
*                                                                           *
*  You should have received a copy of the GNU Lesser General Public License *
*  along with OpenNI. If not, see <http://www.gnu.org/licenses/>.           *
*                                                                           *
****************************************************************************/
using UnityEngine;
using System.Collections;

/// @brief This class is an example for the use of NIGUI
/// 
/// This class creates a sample use of NIGUI with various GUI elements.
/// @ingroup OpenNISpecificLogicSamples
public class GUIExample : MonoBehaviour 
{
    NIInput m_input;

    /// mono-behavior start - initializes the input
    public void Start()
    {
        m_input = FindObjectOfType(typeof(NIInput)) as NIInput;
    }

    /// mono-behavior OnGUI to show GUI elements
    public void OnGUI()
    {
        // create a regular label
        myRect.x=(Screen.width/2)-40;
        myRect.y=20;
        myRect.width=80;
        myRect.height=30;
        GUI.Label(myRect, "Example GUI");
        // place the first button

        myRect.x = 200;
        myRect.y = 50;
        myRect.width = 80;
        myRect.height = 40;
        if (NIGUI.Button(myRect, "Button1"))
        {
           lastMessage = "Button 1 was pressed at time=" + Time.time;
        }

        myRect.x = 500;
        myRect.y = 50;
        if (NIGUI.Button(myRect, "Button2"))
        {
            lastMessage = "Button 2 was pressed at time=" + Time.time;
        }


        myRect.x = 700;
        myRect.y = 50;
        if (NIGUI.RepeatButton(myRect, "Repeat"))
            lastMessage = "Repeat button was pressed at time=" + Time.time;

        myRect.x = 500;
        myRect.y = 125;
        myRect.width = 250;
        m_toggle1 = NIGUI.Toggle(myRect, m_toggle1, "Toggle example");

        myRect.x = 200;
        myRect.y = (Screen.height/2)-15;
        myRect.width = 250;
        myRect.height = 30;
        GUI.Box(myRect, lastMessage);

        myRect.x = 200;
        myRect.y = (Screen.height / 2) + 40;
        myRect.width = 250;
        myRect.height = 30;
		if(Time.time>10)			
        	GUI.Box(myRect, "value=" + m_input.GetAxis("NIGUI_CLICK"));


        myRect.x = 200;
        myRect.y = (Screen.height / 2) + 80;
        myRect.width = 350;
        myRect.height = 30;
        toolbarInt = NIGUI.Toolbar(myRect, toolbarInt, toolbarStrings);

        myRect.x = 200;
        myRect.y = (Screen.height / 2) + 120;
        myRect.width = 350;
        myRect.height = 90;
        selectionGridInt = NIGUI.SelectionGrid(myRect, selectionGridInt, toolbarStrings, 2);

        myRect.x = 700;
        myRect.y = 200;
        myRect.width = 400;
        myRect.height = 20;

        hScroll = NIGUI.HorizontalScrollbar(myRect, hScroll, 0.0f, 0.0f, 300.0f);
        myRect.x = 200;
        myRect.y = (Screen.height / 2) - 50;
        myRect.width = 250;
        myRect.height = 30;
        GUI.Box(myRect, "hScroll=" + hScroll);

        myRect.x = 680;
        myRect.y = 215;
        myRect.width = 10;
        myRect.height = 200;
        vScroll = NIGUI.VerticalScrollbar(myRect, vScroll, 40.0f, 0.0f, 140.0f);
        myRect.x = 200;
        myRect.y = (Screen.height / 2) - 90;
        myRect.width = 250;
        myRect.height = 30;
        GUI.Box(myRect, "vScroll=" + vScroll);

        
        myRect.x = 700;
        myRect.y = 215;
        myRect.width = 400;
        myRect.height = 200;
        NIGUI.BeginGroup(myRect);
        myRect.x=0;
        myRect.y=0;
        Color c = GUI.backgroundColor;
        Color almostClear = c;
        almostClear.a = 0.2f;
        GUI.backgroundColor = almostClear;
        GUI.Box(myRect, "");
        GUI.backgroundColor = c;
        if (NIGUI.Button(new Rect(150-hScroll, 50-vScroll, 300, 200), "a button to be clipped by the view"))
            lastMessage = "Internal button to group was pressed";
        NIGUI.EndGroup();



        myRect.x = 700;
        myRect.y = (Screen.height / 2) + 80;
        myRect.width = 350;
        myRect.height = 30;
        GUI.Label(myRect,"float slide=" + floatSlider);
        myRect.x = 700;
        myRect.y = (Screen.height / 2) + 110;
        myRect.width = 350;
        myRect.height = 30;
        floatSlider=NIGUI.HorizontalSlider(myRect, floatSlider, -5.0f, 5.0f);


        myRect.x = 750;
        myRect.y = (Screen.height / 2) + 120;
        myRect.width = 350;
        myRect.height = 30;
        GUI.Label(myRect, "int slide=" + Mathf.RoundToInt(intSlider));
        myRect.x = 700;
        myRect.y = (Screen.height / 2) + 110;
        myRect.width = 30;
        myRect.height = 150;
        intSlider = NIGUI.VerticalSlider(myRect, intSlider, -10.0f, 10.0f);
        if (NIGUI.changed)
            lastMessage2 = "GUI changed at frame=" + Time.frameCount;

        myRect.x = 200;
        myRect.y = (Screen.height / 2) - 130;
        myRect.width = 250;
        myRect.height = 30;
        GUI.Box(myRect, lastMessage2);

    }


    private string[] toolbarStrings = new string[] { "Toolbar1", "Toolbar1", "Toolbar3", "Toolbar4", "Toolbar5" };
    private int toolbarInt = 0;
    private int selectionGridInt = 0;

    private string lastMessage = "";
    private string lastMessage2 = "";
    private Rect myRect = new Rect(0, 0, 100, 100);
    private bool m_toggle1=false;
    private float hScroll, vScroll;
    private float floatSlider;
    private float intSlider;
}
