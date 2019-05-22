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

/// @brief utility class to track hand position in the x/y axes using @ref NIInput.
/// 
/// This class is a utility which should be attached to an invisible game object. It 
/// will draw (using OnGUI box) the position of the hand it is attached to on the
/// screen.
/// It assumes there are a horizontal and vertical axes in the @ref NIInput inspector.
/// @ingroup OpenNISpecificLogicSamples
public class NIHandPositionInputPositionOnly : MonoBehaviour 
{
	
    /// the input which controls us
    public NIInput m_input;

    /// the initialization (mono-behavior)
    public void Start()
    {
        m_input = FindObjectOfType(typeof(NIInput)) as NIInput;
    }

    /// the mono-behavior update
    public void OnGUI()
    {       
        float x = m_input.GetAxis("Horizontal");
        float y = -m_input.GetAxis("Vertical");
        x += 0.5f;
        x *= (Screen.width-20);
        y += 0.5f;
        y *= (Screen.height-20);
        GUI.Box(new Rect(x,y,20,20),"");
    }
}
