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
using System;
using OpenNI;
using System.Collections.Generic;
using UnityEngine;

/// @brief abstraction of the the user generator and skeleton
/// 
/// This class is responsible for abstracting the user generator and skeleton capabilities. 
/// It is responsible for all relevant initializations.
/// @ingroup OpenNIBasicObjects
public class NIUserAndSkeleton : NIWrapperContextDependant
{
    /// Used for calibration state
    public enum CalibrationState 
    { 
        notCalibrated, ///< not calibrated (i.e. should calibrate). Need to tell the user to assume the calibration position
        calibrating,   ///< in the progress of calibrating, we just have to wait (hopefully)
        calibrated     ///< the user is calibrated, we can use the skeleton.
    };

    /// This returns a list of the current users. 
    /// @note the ids provided here are the unique ids of the user
    public IList<int> Users
    {
        get {return m_usersUniqueIdList.AsReadOnly(); }
    }

    /// This gets the NI id (as defined in the user generator node itself) of the user
    /// @param uniqueID the unique id of a user
    /// @return the NI id of the user (-1 if none was found).
    public int GetNIUserId(int uniqueID)
    {
        foreach (singleUserData user in m_users)
        {
            if (user.m_userUniqueId == uniqueID)
                return user.m_userID;
        }
        return -1;
    }


    /// This gets the center of mass for a specific user
    /// @param niUserID the NI id of a user
    /// @return position of the center of mass of the user (undefined if none was found).
    public Vector3 GetUserCenterOfMass(int niUserID)
    {
        Point3D com = m_userGenerator.GetCoM(niUserID);
        Vector3 retVec=NIConvertCoordinates.ConvertPos(com);
        return retVec;
    }

    /// This gets the calibration state of a specific user
    /// @param niUserID the NI id of a user
    /// @return calibration state of the user (notCalibrated if none was found).
    public CalibrationState GetUserCalibrationState(int niUserID)
    {
        for (int i = 0; i < m_users.Count; i++)
        {
            if (m_users[i].m_userID == niUserID)
            {
                if (m_users[i].m_state == CalibrationState.calibrated && ((m_users[i].m_FrameCalibrated + 2) > Time.frameCount))
                {
                    return CalibrationState.calibrating; // this is to make sure we give a couple of frames to the calibration just to avoid initialization conditions
                }
                return m_users[i].m_state;
            }
        }
        return CalibrationState.notCalibrated;
    }


    /// the skeleton (null if invalid)
    public SkeletonCapability Skeleton
    {
        get { return Valid ? m_userGenerator.SkeletonCapability : null; }
    }

    /// Holds true if the user and skeleton is valid
    public override bool Valid
    {
        get { return base.Valid && m_userGenerator!=null; }
    }

    /// Accessor to @ref m_userGenerator
    public UserGenerator UserNode
    {
        get { return m_userGenerator; }
    }

    /// Allows us to change the smoothing factor of the skeleton (to get smoother behavior).
    public float SkeletonSmoothingFactor
    {
        set { if (Valid) Skeleton.SetSmoothing(value); }
    }


    /// Accessor to the singleton (@ref m_singleton)
    public static NIUserAndSkeleton Instance
    {
        get { return m_singleton; }
    }


    /// @brief Initialize the user and skeleton information
    /// 
    /// This method initializes the user and skeleton information. It assumes the 
    /// context is already valid (otherwise we fail).
    /// @note - Since we use a singleton pattern, multiple initializations will simply delete 
    /// the old information and create new one!
    /// @note - It is the responsibility of the initializer to call @ref Dispose.
    /// @param context the context used
    /// @param logger the logger object we will enter logs into
    /// @return true on success, false on failure. 
    public bool Init(NIEventLogger logger, NIContext context)
    {
        NIOpenNICheckVersion.Instance.ValidatePrerequisite();
        Dispose(); // to make sure we are not initialized
        if (InitWithContext(logger, context) == false)
            return false;
        m_userGenerator = context.CreateNode(NodeType.User) as UserGenerator;
        if (m_userGenerator == null || m_userGenerator.SkeletonCapability==null)
        {
            Log("Failed to create proper user generator.", NIEventLogger.Categories.Errors, NIEventLogger.Sources.Skeleton);
            // we either don't have a user generator or the user generator we received does not have a skeleton
            Dispose();
            return false;
        }
        // skeleton heuristics tries to handle the skeleton when the confidence is low. It can
        // have two values: 0 (no heuristics) or 255 (use heuristics).
        m_userGenerator.SetIntProperty("SkeletonHeuristics", 255);
        if (m_userGenerator.SkeletonCapability.DoesNeedPoseForCalibration && m_userGenerator.PoseDetectionCapability == null)
        {
            Log("skeleton requires calibration pose but user generator has no calibration pose.", NIEventLogger.Categories.Errors, NIEventLogger.Sources.Skeleton);
            // we need a calibration pose but we do not support it.
            Dispose();
            return false;
        }
        // makes sure we use all joints
        m_userGenerator.SkeletonCapability.SetSkeletonProfile(SkeletonProfile.All);
        m_userGenerator.NewUser += new EventHandler<NewUserEventArgs>(NewUserCallback);
        m_userGenerator.LostUser += new EventHandler<UserLostEventArgs>(LostUserCallback);
        if (m_userGenerator.SkeletonCapability.DoesNeedPoseForCalibration)
            m_userGenerator.PoseDetectionCapability.PoseDetected += new EventHandler<PoseDetectedEventArgs>(PoseDetectedCallback);
        m_userGenerator.SkeletonCapability.CalibrationComplete += new EventHandler<CalibrationProgressEventArgs>(CalibrationEndCallback);
        
        m_lastUserUniqueId = 0;
        m_users.Clear();
        return true;
    }

    /// @brief Release a previously initialize image node.
    /// 
    /// This method releases a previously initialized image node.
    /// @note - Since we use a singleton pattern, only one place should do a release, otherwise the
    /// result could become problematic for other objects
    /// @note - It is the responsibility of the whoever called @ref Init to do the release. 
    /// @note - The release should be called BEFORE releasing the context. If the context is invalid, the result is
    /// undefined.
    public override void Dispose()
    {
        if (m_context == null)
            return;
        m_users.Clear();
        m_usersUniqueIdList.Clear();
        m_lastUserUniqueId = 0;
        if (m_userGenerator != null && m_context != null)
        {
            m_context.ReleaseNode(m_userGenerator); // we call this even if the context is invalid because it is a singleton which will release stuff
        }
        m_userGenerator = null;
        Log("after nulling the node", NIEventLogger.Categories.Errors, NIEventLogger.Sources.Skeleton);
        base.Dispose();
    }


    // protected members

    /// An internal object using the the OpenNI basic user generator node.
    protected UserGenerator m_userGenerator;
    /// @brief The singleton itself
    /// 
    /// NIImage uses a singleton pattern. There is just ONE NIUserSkeleton object which is used by all.
    protected static NIUserAndSkeleton m_singleton = new NIUserAndSkeleton();

    /// @brief The list of tracked users
    /// 
    /// This list manages the list of users
    protected List<singleUserData> m_users;

    /// @brief a list of users by their unique id
    /// 
    /// This list manages a list of users by their unique id.
    protected List<int> m_usersUniqueIdList=new List<int>();

    /// Definition of data for a specific user
    protected struct singleUserData
    {
        // user ID
        public int m_userID;                ///< Holds the user id (this is the id provided by NI internals)
        public int m_userUniqueId;          ///< This is unique number for each user. Externally we use this number
        public CalibrationState m_state;    ///< The calibration state for this specific user
        public int m_FrameCalibrated;       ///< This holds the frame when the state was changed to calibrated (undefined if not calibrated).
    }

    /// Holds the next unique id to use.
    /// @note unique ids restart numbering whenever we initialize!
    protected int m_lastUserUniqueId;

    /// @brief helper function to update the calibration state of a user
    /// 
    /// This method is a helper function which updates the calibration state of a user
    /// @param userID the NI user ID of the user to start calibrating
    /// @param newState the new calibration state.
    protected void UpdateCalibrationState(int userID, CalibrationState newState)
    {
        for (int i = 0; i < m_users.Count; i++)
        {
            if (m_users[i].m_userID == userID)
            {
                singleUserData tmpUser = m_users[i];
                tmpUser.m_state = newState;
                tmpUser.m_FrameCalibrated = Time.frameCount;
                m_users[i] = tmpUser;
                return;
            }
        }

    }


    // private methods

    /// @brief private constructor
    /// 
    /// This is part of the singleton pattern, a protected constructor cannot be called externally (although
    /// it can be inherited for extensions. In which case the extender should also replace the singleton!
    private NIUserAndSkeleton()
    {
        m_userGenerator = null;
        m_users = new List<singleUserData>();
    }

    /// @brief callback for updating structures when a user is lost
    /// 
    /// This callback is called when a user is lost. It removes the user from all structures...
    /// @param sender who called the callback
    /// @param e the arguments of the event.
    private void LostUserCallback(object sender, UserLostEventArgs e)
    {
        int uniqueID=-1;
        for (int i = 0; i < m_users.Count; i++)
        {
            if (m_users[i].m_userID == e.ID)
            {
                uniqueID=m_users[i].m_userUniqueId;
                m_users[i] = m_users[m_users.Count - 1];
                m_users.RemoveAt(m_users.Count - 1);
                break;
            }
        }
        Log("lost user, uniqueID="+uniqueID+" userID="+e.ID, NIEventLogger.Categories.Callbacks, NIEventLogger.Sources.Skeleton);
        if(uniqueID<0)
            return; // we couldn't find anything to remove...
        for (int i = 0; i < m_usersUniqueIdList.Count; i++)
        {
            if (m_usersUniqueIdList[i] == uniqueID)
            {
                m_usersUniqueIdList[i] = m_usersUniqueIdList[m_usersUniqueIdList.Count - 1];
                m_usersUniqueIdList.RemoveAt(m_usersUniqueIdList.Count - 1);
            }
        }
    }



    /// @brief callback for updating structures when a user is found
    /// 
    /// This callback is called when a user is found. It adds the user to the data structures (including
    /// a new unique ID).
    /// @param sender who called the callback
    /// @param e the arguments of the event.
    private void NewUserCallback(object sender, NewUserEventArgs e)
    {
        Log("found new user, uniqueID=" + m_lastUserUniqueId+" userID="+e.ID, NIEventLogger.Categories.Callbacks, NIEventLogger.Sources.Skeleton);
        singleUserData tmpUser;  // the new data
        tmpUser.m_userID=e.ID;
        tmpUser.m_userUniqueId=m_lastUserUniqueId++; // the unique ID simply grows all the time...
        tmpUser.m_state=CalibrationState.notCalibrated;
        tmpUser.m_FrameCalibrated = -1;
        m_users.Add(tmpUser);
        m_usersUniqueIdList.Add(tmpUser.m_userUniqueId);
        StartCalibrationProcess(e.ID);
    }

    /// @brief helper function to start the calibration process
    /// 
    /// This method is a helper function which starts the calibration process.
    /// @param userID the NI user ID of the user to start calibrating
    private void StartCalibrationProcess(int userID)
    {
        Log("started calibration process for user=" + userID, NIEventLogger.Categories.Callbacks, NIEventLogger.Sources.Skeleton);
        if (Skeleton.DoesNeedPoseForCalibration)
        {
            string calibrationPose = Skeleton.CalibrationPose;
            m_userGenerator.PoseDetectionCapability.StartPoseDetection(calibrationPose, userID);
            UpdateCalibrationState(userID, CalibrationState.notCalibrated);
        }
        else
        {
            Skeleton.RequestCalibration(userID, true);
            UpdateCalibrationState(userID, CalibrationState.calibrating);
        }

    }


    /// @brief callback for updating structures when calibration ends
    /// 
    /// This callback is called when a calibration process ends. If the calibration succeeded
    /// then the user is in a calibrated state, otherwise it starts the calibration process from
    /// scratch.
    /// @param sender who called the callback
    /// @param e the arguments of the event.
    private void CalibrationEndCallback(object sender, CalibrationProgressEventArgs e)
    {
        Log("finished calibration for user=" + e.ID +" status="+e.Status, NIEventLogger.Categories.Callbacks, NIEventLogger.Sources.Skeleton);
        if (e.Status == CalibrationStatus.OK)
        {
            Skeleton.StartTracking(e.ID);
            UpdateCalibrationState(e.ID, CalibrationState.calibrated);
        }
        else
        {
            StartCalibrationProcess(e.ID);
        }

    }


    /// @brief callback for updating structures when the calibration pose is detected
    /// 
    /// This callback is called when a the calibration pose is detected (and we can start
    /// the calibration process).
    /// @param sender who called the callback
    /// @param e the arguments of the event.
    private void PoseDetectedCallback(object sender, PoseDetectedEventArgs e)
    {
        Log("found calibration pose for user=" + e.ID, NIEventLogger.Categories.Callbacks, NIEventLogger.Sources.Skeleton);
        m_userGenerator.PoseDetectionCapability.StopPoseDetection(e.ID);
        Skeleton.RequestCalibration(e.ID, true);
        UpdateCalibrationState(e.ID, CalibrationState.calibrating);
    }
}
