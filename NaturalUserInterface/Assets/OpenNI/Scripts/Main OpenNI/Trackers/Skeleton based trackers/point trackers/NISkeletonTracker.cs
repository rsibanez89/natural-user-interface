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
using OpenNI;

/// @brief A point tracker which follows a single joint from a skeleton for a specific player.
/// 
/// @ingroup OpenNIPointTrackers
public class NISkeletonTracker : NIPointTracker 
{
    /// the player number for the player whose hands we will follow
    public int m_playerNum;
    /// a list of legal hands to track

    /// which joint to track 
    public SkeletonJoint m_jointToUse;

    /// the mapper we are working with
    public NIUsersToPlayerMapper m_mapper;


    /// returns a unique name for the tracker type.
    /// @note this is what is used to identify the tracker
    /// @return the unique name.
    public override string GetTrackerType()
    {
        return "skeleton tracker for player " + m_playerNum + " tracking joint " + m_jointToUse;
    }


    /// returns the initial position of the point.
    /// @note this is the position of the relevant hand when the player was identified!
    public override Vector3 StartingPos
    {
        get
        {
            // The position we count from is the initial calibration pose.
            int userID;
            SkeletonCapability skel = GetSkeletonCapability(out userID);
            if(skel==null)
                return Vector3.zero; // no good skeleton capability. i.e. it is not calibrated or the player was not found.
            SkeletonJointTransformation skelTrans;
            if (m_mapper.GetJointInitialTransform(m_playerNum, m_jointToUse, out skelTrans) == false)
                return Vector3.zero; // we don't have the transform
            Point3D pos = skelTrans.Position.Position;
            Vector3 res = NIConvertCoordinates.ConvertPos(pos);
            if (skelTrans.Position.Confidence < 0.5f || float.IsNaN(res.magnitude) || res.z>=0)
            {
                // this means the value is irrelevant.
                m_mapper.RecalcAllInitialTransforms(m_playerNum);
                float tmpConfidence;
                pos = GetSkelPoint(out tmpConfidence);
                res = NIConvertCoordinates.ConvertPos(pos);
            }
            return res;
        }
    }

    /// return the delta of the current position (preferably from the current frame...) from 
    /// the last frame's position.
    /// @note Do not use CurDeltaPos during LateUpdate, this is because during LateUpdate 
    /// the previous point might have already been updated making CurDeltaPos return 0! 
    public override Vector3 CurDeltaPos
    {
        get { return CurPos-m_lastFrameCurPoint.LastGoodPointLastFrame; }
    }

    /// return the delta of the current position (preferably from the current frame...) from 
    /// the last frame's position.
    /// @note Do not use CurDeltaPosRaw during LateUpdate, this is because during LateUpdate 
    /// the previous point might have already been updated making CurDeltaPosRaw return 0! 
    public override Vector3 CurDeltaPosRaw
    {
        get { return CurDeltaPos; }
    }

    /// returns the current position with confidence
    /// 
    /// @param confidence the confidence of the point
    /// @return the position
    public Vector3 GetPosWithConfidence(out float confidence)
    {
        Point3D where = GetSkelPoint(out confidence);
        Vector3 res = NIConvertCoordinates.ConvertPos(where);
        return res;
    }


    public override Vector3 CurPos
    {
        get
        {
            return m_lastFrameCurPoint.LastGoodPoint;
        }
    }

    public override Vector3 CurPosRaw
    {
        get { return CurPos; }
    }

    public override Vector3 CurPosFromStart
    {
        get { return CurPos-StartingPos; }
    }
    public override Vector3 CurPosFromStartRaw
    {
        get { return CurPosFromStart; }
    }

    /// mono-behavior fixedUpdate
    /// This is used to update the position for the previous frame. 
    /// @note we use FixedUpdate because this can be updated multiple times during a frame.
    public void FixedUpdate()
    {
        float confidence;
        Point3D where = GetSkelPoint(out confidence);
        m_lastFrameCurPoint.UpdatePoint(where, confidence);
    }

    public override void StopTracking()
    {
        base.StopTracking();
        m_lastFrameCurPoint = null;
    }


    /// this method is responsible to get us a VALID skeleton capability of the relevant user.
    /// if the skeleton capability is not valid (or there is no calibrated user or any other
    /// such problem) then it returns null
    /// @param userID if the skeleton capability is good, this is the user ID of the relevant user
    /// @return the skeleton capability (null if there is a problem).
    public SkeletonCapability GetSkeletonCapability(out int userID)
    {
        userID = -1;
        if (m_mapper == null)
            return null; // no mapper
        int uniqueID = m_mapper.GetPlayerUniqueID(m_playerNum);
        if (uniqueID < 0)
            return null; // no such player
        userID = m_mapper.GetPlayerOpenNIID(uniqueID);
        if (userID < 0)
            return null; // no such player.
        if (m_mapper.m_contextManager == null ||
            m_mapper.m_contextManager.Valid == false ||
            m_mapper.m_contextManager.UserSkeletonValid == false)
            return null; // no relevant skeleton or session manager
        if (m_mapper.m_contextManager.UserGenrator.GetUserCalibrationState(userID) != NIUserAndSkeleton.CalibrationState.calibrated)
            return null; // no calibrated skeleton
        return m_mapper.m_contextManager.UserGenrator.Skeleton;
    }

    // protected methods


    /// an internal method to initialize the internal structures
    /// @note in most cases, inheriting methods should NOT override the base @ref InitTracking but
    /// rather override this method.
    /// @return true on success and false otherwise.
    protected override void InternalAwake()
    {
        base.InternalAwake();
        if (m_mapper == null)
        {
            m_mapper = FindObjectOfType(typeof(NIUsersToPlayerMapper)) as NIUsersToPlayerMapper;
            if (m_mapper == null)
                throw new System.Exception("Need to add a users to player mapper to the scene (NIUsersToPlayerMapper or use the NISkeletonPrefab)");
        }
    }

    /// an internal method to initialize the internal structures
    /// @note in most cases, inheriting methods should NOT override the base @ref InitTracking but
    /// rather override this method.
    /// @return true on success and false otherwise.
    protected override bool InitInternalStructures()
    {
        m_lastFrameCurPoint = new NIPositionTrackerFrameManager();
        return true;
    }


    /// This method tries to find the joint position of the relevant hand
    /// 
    /// @param confidence the confidence of the point found (if no point was found then a negative confidence is provided)
    /// @return true if we found a position, false otherwise (in which case point is undefined).
    protected Point3D GetSkelPoint(out float confidence)
    {
        confidence = -1.0f;
        int userID;
        SkeletonCapability skel = GetSkeletonCapability(out userID);
        if (skel == null)
            return Point3D.ZeroPoint;
        if (skel.IsJointAvailable(m_jointToUse) == false)
            return Point3D.ZeroPoint;
        SkeletonJointPosition skelPos = skel.GetSkeletonJointPosition(userID, m_jointToUse);
        confidence = skelPos.Confidence;
        //Debug.Log("pos=(" + skelPos.Position.X + "," + skelPos.Position.Y + "," + skelPos.Position.Z + ") confidence=" + skelPos.Confidence+" time="+Time.frameCount);
        return skelPos.Position;
    }

    /// used to get the last good value of the smooth point
    protected NIPositionTrackerFrameManager m_lastFrameCurPoint;
}
