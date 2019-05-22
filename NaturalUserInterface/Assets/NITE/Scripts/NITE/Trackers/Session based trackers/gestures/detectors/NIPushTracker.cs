/*******************************************************************************
*                                                                              *
*   OpenNI Unity Toolkit - NITE extension                                      *
*   Copyright (C) 2010 PrimeSense Ltd.                                         *
*                                                                              *
*******************************************************************************/
using UnityEngine;
using OpenNI;
using NITE;

/// @brief This class implements the gesture tracker for the push gesture
/// @ingroup NITETrackers
public class  NIPushTracker : NIGestureTracker
{


    /// Release the gesture
    public override void ReleaseGesture()
    {
        NIHandTracker tracker = m_pointTracker as NIHandTracker;
        if (tracker == null)
            return;
        tracker.RemoveListener(m_pushDetector);
        m_pointTracker = null;
    }

    /// base constructor
    public NIPushTracker()
    {
        NIOpenNICheckVersion.Instance.ValidatePrerequisite();
        m_pushDetector = new PushDetector();
        m_pushDetector.Push += new System.EventHandler<VelocityAngleEventArgs>(PushDetected);
    }


    /// This is true if the gesture is in the middle of doing (i.e. it has detected but not gone out of the gesture).
    /// for our purposes this means the time from the last push detection is less than the immediate duration of the node.
    /// @return a value between 0 and 1. 0 means no gesture, 1 means the gesture has been detected. 
    /// A value in the middle is not an option here.
    public override float GestureInProgress()
    {
        if (m_timeDetected > Time.time - m_immediateDuration)
            return 1.0f;
        return 0.0f;
    }


    // protected methods

    /// Gesture initialization
    /// 
    /// This method is responsible for initializing the gesture to work with a specific hand tracker
    /// @param hand the hand tracker to work with
    /// @return true on success, false on failure (e.g. if the hand tracker does not work with the gesture).
    protected override bool InternalInit(NIPointTracker hand)
    {
        NIHandTracker curHand = hand as NIHandTracker;
        if (curHand == null)
            return false;
        if (curHand.AddListener(m_pushDetector) == false)
            return false;
        m_immediateDuration = ((float)m_pushDetector.ImmediateDuration) / 1000.0f;
        return true;
    }

    // protected members
    protected PushDetector m_pushDetector; ///< the push detector to use.
    protected float m_immediateDuration; ///< the duration in seconds to continue after pushing where push is still "in progress"

    // private methods (callbacks)

    /// callback when we detect a push
    /// @param sender the object that sent the event
    /// @param e the event arguments
    void PushDetected(object sender, VelocityAngleEventArgs e)
    {
        m_timeDetected = Time.time;
        m_frameDetected = Time.frameCount;
        DetectGesture();
    }
}
