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
using System.Collections.Generic;
using OpenNI;

/// class to Manage users and skeletons
/// 
/// @brief a class to map between players and NIUsers.
/// 
/// This class is mapping players to users, i.e. it provides access to "player X" where is is the Xth player.
/// It assumes the following logic: The first user to calibrate is player 0, the second is player 1 etc.
/// if a player becomes invalid (user lost) then the next player to calibrate will be that player. If more
/// than one player becomes available, the first index is filled first.
/// @note Players are mapped from player 0 and onwards and @b NOT from player 1!
/// @ingroup SkeletonBaseObjects
public class NIUsersToPlayerMapper : MonoBehaviour 
{
    /// This is the context we get the data from.
    public OpenNISettingsManager m_contextManager;

    /// true if the mapper is valid, false otherwise.
    public virtual bool Valid
    {
        get { return m_contextManager.Valid && m_contextManager.UserSkeletonValid; }
    }


    /// This method finds the unique ID of player "playerID" (running index).
    /// @param playerID a running number of the player ID (player 1 is 0, player 2 is 1 etc.)
    /// @return The unique id of the user for that player (-1 if no active user exists for that player). 
    public int GetPlayerUniqueID(int playerID)
    {
        if (playerID < 0 || playerID >= m_players.Count)
            return -1;
        return m_players[playerID].UniqueID;
    }

    /// accessor to the user generator (null if invalid)
    public NIUserAndSkeleton UsersGenerator
    {
        get { return Valid ? m_contextManager.UserGenrator : null; }
    }

    /// This method finds the OpenNI ID of player "playerID" (running index).
    /// @param playerID a running number of the player ID (player 1 is 0, player 2 is 1 etc.)
    /// @return The OpenNI id of the user for that player (-1 if no active user exists for that player). 
    public int GetPlayerOpenNIID(int playerID)
    {
        if (playerID < 0 || playerID >= m_players.Count)
            return -1;
        return m_players[playerID].UserID;
    }

    /// accessor to the number of active players (i.e. those with calibrated users).
    public int NumActivePlayers
    {
        get
        {
            int numPlayers=0;
            foreach (InternalSkeleton player in m_players)
            {
                if (player.UniqueID >= 0)
                    numPlayers++;
            }
            return numPlayers;
        }
    }

    /// returns the skeleton transformation that was set when the user was initially added to the list
    /// of users for a specific joint.
    /// @param playerID a running number of the player ID (player 1 is 0, player 2 is 1 etc.)
    /// @param joint the joint we want the transform of this.
    /// @param[out] skelTransform the skeleton transformation
    /// @return will return true if we found the joint for that player and false otherwise.
    public bool GetJointInitialTransform(int playerID, SkeletonJoint joint, out SkeletonJointTransformation skelTransform)
    {
        skelTransform = m_internalTransformation;
        if (playerID < 0 || playerID >= m_players.Count)
            return false;
        InternalSkeleton player=m_players[playerID];
        if (player.UniqueID < 0)
            return false;
        List<SkeletonJoint> jointList = player.JointsIDs;
        if (jointList == null)
            return false; // no joint!
        for (int i = 0; i < jointList.Count; i++)
        {
            if (jointList[i] == joint)
            {
                skelTransform = player.JointTransforms[i];
                return true;
            }
        }
        return false;
    }

    /// recalculates the initial transforms of all joints. This should be done when some (or all)
    /// initial transforms are irrelevant (e.g. the initial calibration had no leg information but
    /// we want them so we will recalc everything accordingly).
    /// @param playerID a running number of the player ID (player 1 is 0, player 2 is 1 etc.)
    public void RecalcAllInitialTransforms(int playerID)
    {
        if (playerID < 0 || playerID >= m_players.Count)
            return; // no such player
        InternalSkeleton player = m_players[playerID];
        if (player.UniqueID < 0)
            return; // no info for the player
        m_players[playerID].InitInternalSkeleton(m_contextManager.UserGenrator, player.UniqueID);
    }

 
    /// mono-behavior start (called on initialization)
	public void Awake () 
    {
        m_players = new List<InternalSkeleton>();
        if (m_contextManager == null)
            m_contextManager = FindObjectOfType(typeof(OpenNISettingsManager)) as OpenNISettingsManager;

	}
	
	/// mono-behavior update (called once per frame)
	public void Update () 
    {
        if (m_contextManager.Valid == false || m_contextManager.UserSkeletonValid == false)
            return; // we can do nothing.
        // we go over all existing players and make sure they are still legal. 
        // If it is illegal (i.e. the user no longer exists) we mark it as such.
        // when this loop finishes m_players will have only legal players
        foreach (InternalSkeleton player in m_players)
        {
            if(player.UniqueID<0)
                continue; // already illegal
            int userID = m_contextManager.UserGenrator.GetNIUserId(player.UniqueID);
            if (userID < 0 || userID != player.UserID)
            {
                Debug.Log("Lose one");
                player.UniqueID = -1; // invalidate!
            }
        }
        // now we need to make sure all users are translated to players. We will go over all users
        // in the user generator and try to make sure we have a player for them.
        
        // get the list of users
        IList<int> userList=m_contextManager.UserGenrator.Users;

        // for each user we try to find if it is already in (i.e. valid) and if not we will position it
        foreach (int uniqueID in userList)
        {
            if(uniqueID<0)
                continue; // an invalid user
            InternalSkeleton playerToAdd=null; // holds the first free player (this is where we will add the new user if it does not exist)
            bool foundUser=false; // will be changed to true if we find the user in the player list
            foreach (InternalSkeleton player in m_players)
            {
                if (player.UniqueID == uniqueID)
                {
                    foundUser = true;
                    break;
                }
                if (playerToAdd == null && player.UniqueID < 0)
                    playerToAdd = player;
            }
            if(foundUser)
                continue; // the user exists, no need to do anything.
            if (playerToAdd == null)
            {
                playerToAdd = m_nextSkeleton;
                // note InternalSkeleton is a class so we can change it directly
                if (playerToAdd.InitInternalSkeleton(m_contextManager.UserGenrator, uniqueID))
                {
                    Debug.Log("adding a new player. Count=" + m_players.Count);
                    m_players.Add(playerToAdd);
                    m_nextSkeleton = new InternalSkeleton();
                }
            }
            else playerToAdd.InitInternalSkeleton(m_contextManager.UserGenrator, uniqueID);
        }
    }

     /// this holds a list of the players.
    protected List<InternalSkeleton> m_players;

    /// used internally so we don't have to do a "new" every time
    static protected SkeletonJointTransformation m_internalTransformation = new SkeletonJointTransformation();

    /// holds the next (already constructed) internal skeleton. The idea is to use this and create a new one
    /// whenever needed (in order to have a constructed one to try and initialize).
    static protected InternalSkeleton m_nextSkeleton = new InternalSkeleton();

    /// @brief an internal class to hold skeleton information
    /// 
    /// This is an internal class to hold skeleton information. Each player will have this which
    /// holds the ids of the user and the joint information on initialization.
    protected class InternalSkeleton
    {
        /// accessor to @ref m_uniqueID
        public int UniqueID
        {
            get { return m_uniqueID; }
            set { m_uniqueID=value; }
        }

        /// accessor to get the user ID (returns -1 if the object invalid)
        public int UserID
        {
            get { return m_uniqueID>=0 ? m_userID : -1; }
        }

        /// accessor to get the joint id list (returns null if the object invalid)
        public List<SkeletonJoint> JointsIDs
        {
            get { return m_uniqueID >= 0 ? m_jointsIDs : null; }
        }

        /// accessor to get the skeleton transforms list (returns null if the object invalid)
        public List<SkeletonJointTransformation> JointTransforms
        {
            get { return m_uniqueID >= 0 ? m_jointsTrans : null; }
        }


        protected int m_uniqueID; ///< the unique id of the user as it appears in the user generator. If this is smaller than 0 it is invalid
        protected int m_userID;   ///< the internal OpenNI user ID used for OpenNI constructs
        protected List<SkeletonJoint> m_jointsIDs; ///< a list of the joints
        protected List<SkeletonJointTransformation> m_jointsTrans; ///< a list of the skeleton transformation as they were when initialized.


        /// constructor. The class is invalid (m_uniqueID<0) until @ref InitInternalSkeleton is called successfully.
        public InternalSkeleton()
        {
            m_uniqueID = -1; // making this invalid!
            NIOpenNICheckVersion.Instance.ValidatePrerequisite();
            m_jointsIDs = new List<SkeletonJoint>();
            m_jointsTrans = new List<SkeletonJointTransformation>();
        }

        /// @brief initializes the class with a new user.
        /// 
        /// initializes the class with a new user (The class is invalid (m_uniqueID<0) until this is done successfully)
        /// @param usersGenerator the users generator which holds the users.
        /// @param uniqueID the uniqueID of the user we should initialize to.
        /// @return true on success and false on failure (making the object invalid, i.e. m_uniqueID<0).
        public bool InitInternalSkeleton(NIUserAndSkeleton usersGenerator, int uniqueID)
        {
            m_uniqueID = -1; // to make sure we are invalid until we are sure we are valid.
            if (usersGenerator.Valid == false)
                return false; // must have a valid users generator to do anything
            m_userID = usersGenerator.GetNIUserId(uniqueID);
            if (m_userID < 0)
                return false; // means the uniqueID was illegal
            if (usersGenerator.GetUserCalibrationState(m_userID) != NIUserAndSkeleton.CalibrationState.calibrated)
                return false; // we only initialize calibrated users as we need the skeleton.           
            int numJoints = 0; // holds the place to position the next joint.
            foreach (SkeletonJoint joint in Enum.GetValues(typeof(SkeletonJoint)))
            {
                if (usersGenerator.Skeleton.IsJointAvailable(joint)==false)
                    continue; // an irrelevant joint.
                // we need to add the joint but we don't want to create a new one unless we need to.
                if (numJoints < m_jointsIDs.Count)
                {
                    m_jointsIDs[numJoints] = joint;
                }
                else
                {
                    m_jointsIDs.Add(joint);
                }

                // we need to add the joint transformation but we don't want to create a new one unless we need to.
                if(numJoints<m_jointsTrans.Count)
                {
                    m_jointsTrans[numJoints]=usersGenerator.Skeleton.GetSkeletonJoint(m_userID, joint);
                }
                else
                {
                    SkeletonJointTransformation skelTrans = usersGenerator.Skeleton.GetSkeletonJoint(m_userID, joint);
                    m_jointsTrans.Add(skelTrans);
                }
                numJoints++;
            }
            if (numJoints < m_jointsIDs.Count)
            {
                for (; numJoints < m_jointsIDs.Count; numJoints++)
                    m_jointsIDs[numJoints] = SkeletonJoint.Invalid;
            }
            m_uniqueID = uniqueID;
            return true;
        }
    }


}
