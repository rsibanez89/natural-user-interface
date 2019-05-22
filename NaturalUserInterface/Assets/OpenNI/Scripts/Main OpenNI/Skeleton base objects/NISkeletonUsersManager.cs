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
using System.Collections.Generic;
using OpenNI;

/// @brief class to Manage users and skeletons
/// 
/// This class is responsible for managing the various users and mapping them to controlled skeletons.
/// @ingroup SkeletonBaseObjects
public class NISkeletonUsersManager : MonoBehaviour
{
    /// This is a public list of skeletons to control. The user should drag & drop skeletons here.
    public NISkeletonController[] m_skeletonsControllers;


    /// the user to player mapper (to figure out the user ID for each player).
    public NIUsersToPlayerMapper m_mapper;


    /// mono-behavior awake.
    /// This will initialize the internals (m_mapper and m_skeletonsControllers if they were
    /// not specifically set by the user or if m_skeletonsControllers has null members)
    public void Awake()
    {
        if (m_mapper == null)
            m_mapper = FindObjectOfType(typeof(NIUsersToPlayerMapper)) as NIUsersToPlayerMapper;
        if (m_skeletonsControllers == null || m_skeletonsControllers.Length == 0)
        {
            m_skeletonsControllers = FindObjectsOfType(typeof(NISkeletonController)) as NISkeletonController[];
        }
        else
        {
            foreach (NISkeletonController controller in m_skeletonsControllers)
            {
                if(controller==null)
                    throw new System.Exception("There is a null controller in the skeleton user managers");
            }
        }
    }

    /// This method turns off or on the skeleton belonging to a specific player
    /// @param playerID the player ID to turn off or on
    /// @param state the state to set (on true sets to active, on false sets to inactive)
    public void SetSkeletonPlayerState(int playerID, bool state)
    {
        m_skeletonsControllers[playerID].SetSkeletonActive(state);
    }

    /// This method turns off or on the skeleton belonging to all players
    /// @param state the state to set (on true sets to active, on false sets to inactive)
    public void SetAllSkeletonPlayersToState(bool state)
    {
        foreach (NISkeletonController skel in m_skeletonsControllers)
        {
            skel.SetSkeletonActive(state);
        }
    }

	/// mono-behavior update (called once per frame)
	public void Update () 
    {
        if (m_mapper.Valid == false)
            return; // we can do nothing.
        if (m_skeletonsControllers == null)
            return; // we don't have any skeletons controllers to update
        for (int i = 0; i < m_skeletonsControllers.Length; i++)
        {
            int userID = m_mapper.GetPlayerOpenNIID(i);

            if (userID < 0)
            {
                m_skeletonsControllers[i].RotateToCalibrationPose(); // we don't have anything to work with.
                continue;
            }
            Vector3 skelPos = Vector3.zero;
            SkeletonJointTransformation skelTrans;
            if (m_mapper.GetJointInitialTransform(i, SkeletonJoint.Torso, out skelTrans))
            {
                if (skelTrans.Position.Confidence >= 0.5f)
                {
                    skelPos = NIConvertCoordinates.ConvertPos(skelTrans.Position.Position);
                }
            }
            m_skeletonsControllers[i].UpdateSkeleton(userID, m_mapper.UsersGenerator, skelPos);

        }
	}
}
