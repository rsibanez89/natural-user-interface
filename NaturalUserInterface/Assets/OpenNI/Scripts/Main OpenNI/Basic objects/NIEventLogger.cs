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
using System;


/// @brief An internal logger
///
/// This class is responsible logging events internal to OpenNi wrapper and filter them
/// @note it is mono-behavior so we can drag & drop it.
/// @ingroup OpenNIBasicObjects
public class NIEventLogger : MonoBehaviour
{
    /// Defines legal categories
    public enum Categories
    {
        Initialization, ///< initialization related issues
        Callbacks,      ///< callback related issues
        Errors,         ///< errors
    }

    /// used to control the categories we want to show 
    public bool[] m_categoriesToShow = new bool[Enum.GetNames(typeof(Categories)).Length];

    /// Defines the sources of the messages
    public enum Sources
    {
        BaseObjects, ///< relates to basic objects (context etc.)
        Image,       ///< image related issues
        Skeleton,    ///< skeleton and users related issues
        Hands,       ///< hand tracking and session manager related issues
        Input,       ///< Input object related issues
        GUI          ///< GUI and controls related issues
    }


    /// used to control the sources we want to show 
    public bool[] m_sourcesToShow = new bool[Enum.GetNames(typeof(Sources)).Length];


    /// does the logging.
    /// @param str the string to log
    /// @param category the category of the log
    /// @param source the source of the log
    public virtual void Log(string str, Categories category, Sources source)
    {
        if(m_categoriesToShow[(int)category] && m_sourcesToShow[(int)source])
            Debug.Log(str);
    }
}
