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

/// @brief A small utility which prompts users to calibrate.
/// 
/// Used mainly for debugging.
/// @ingroup OpenNIDebugUtilities
public class NISkeletonCalibrationMessageUtility : MonoBehaviour 
{
    /// this holds the manager which holds which players need to be calibrated.
    public NISkeletonUsersManager m_manager;

    /// this holds the player manager. @note this is only relevant if the skeleton manager is not
    /// found or defined.
    public NIUsersToPlayerMapper m_playerMapper;
    /// the image of the calibration pose.
    public Texture2D m_Image;
    /// mono-behavior initialization
	void Start () 
    {
	    if(m_manager==null)
            m_manager = FindObjectOfType(typeof(NISkeletonUsersManager)) as NISkeletonUsersManager;

        if(m_playerMapper==null)
            m_playerMapper = FindObjectOfType(typeof(NIUsersToPlayerMapper)) as NIUsersToPlayerMapper;

        m_basePos=new Rect(0, Screen.height/2,200,30);
	}

    /// mono-behavior GUI drawing
    void OnGUI()
    {
        Rect curPos = m_basePos;
        if (m_manager != null)
        {
            // being here means we have a skeleton manager. So we will see for each controller
            // if they need calibration.
            int numMessages = 0; // number of messages we show
            for (int i = 0; i < m_manager.m_skeletonsControllers.Length; i++)
            {
                int userID = m_manager.m_mapper.GetPlayerOpenNIID(i);

                if (userID < 0)
                {
                    // not calibrated, we need to show a message.
                    GUI.Box(curPos, "waiting for player " + i + " to calibrate");
                    numMessages++;
                }
                curPos.y += 35;
            }
            if (numMessages == 0)
                return; // we have no messages (i.e. everyone is calibrated) so no need to draw the image...
        }
        else
        {
            if (m_playerMapper != null && m_playerMapper.Valid && m_playerMapper.NumActivePlayers == 0)
            {
                // this means we have a valid player mapper with no calibrated users, we need to show a message...
                GUI.Box(curPos, "waiting for a player to calibrate");
            }
            else
                return; // since we don't need to do a calibration message, we don't need to show the image either
        }
        if (m_Image==null)
            return; // no image to draw.
        curPos.y +=35;
        curPos.width = 128;
        curPos.height = 128;
        GUI.Box(curPos, m_Image);
    }

    /// the base position for the first GUI drawing.
    protected Rect m_basePos;
    
}
