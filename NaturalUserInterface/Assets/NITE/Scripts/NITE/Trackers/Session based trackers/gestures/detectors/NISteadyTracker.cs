/*******************************************************************************
*                                                                              *
*   OpenNI Unity Toolkit - NITE extension                                      *
*   Copyright (C) 2010 PrimeSense Ltd.                                         *
*                                                                              *
*******************************************************************************/
using UnityEngine;
using OpenNI;
using NITE;

/// @brief This class implements the gesture tracker for the steady gesture
/// @ingroup NITETrackers
public class  NISteadyTracker : NIGestureTracker
{

    /// Release the gesture
    public override void ReleaseGesture()
    {
        NIHandTracker tracker = m_pointTracker as NIHandTracker;
        if (tracker == null)
            return;
        tracker.RemoveListener(m_steadyDetector);
        m_pointTracker = null;
    }

    /// base constructor
    /// @param timeToClick this is the time one needs to remain steady to for the event to fire 
    /// (and the gesture recognized).
    /// @param timeToReset the time AFTER firing the event where the steady resets (i.e. as if
    /// the hand moved). 
    /// @note this means that if one leaves his hand steady continuously then the event will fire
    /// every timeToClick+timeToReset seconds starting from timeToClick seconds after the initial
    /// steady.
    public NISteadyTracker(float timeToClick, float timeToReset)
    {
        NIOpenNICheckVersion.Instance.ValidatePrerequisite();
        m_steadyDetector = new SteadyDetector();
        m_steadyDetector.Steady += new System.EventHandler<SteadyEventArgs>(DetectSteadyEvent);
        m_steadyDetector.NotSteady += new System.EventHandler<SteadyEventArgs>(DetectNotSteadyEvent);
        m_currentSteady = false;
        m_firstSteady = float.MaxValue;
        m_firedSteadyEvent = false;
        m_timeToClick = timeToClick;
        m_timeToReset = timeToClick + timeToReset;
    }

    /// This is true if the gesture is in the middle of doing (i.e. it has detected but not gone out of the gesture).
    /// for our purposes this means the steady event has occurred and the unsteady has not occurred yet
    /// @return a value between 0 and 1. 0 means no gesture, 1 means the gesture has been detected and 
    /// held for a while. A value in the middle means the gesture has been detected and has been held
    /// this portion of the time required to fire the trigger (@ref m_timeToClick).
    public override float GestureInProgress()
    {
        if (m_currentSteady==false)
            return 0.0f;
        float diffTime = Time.time - m_firstSteady;
        if (diffTime >= m_timeToClick || m_timeToClick<=0)
            return 1.0f;
        return diffTime / m_timeToClick;
    }

    /// used for updating every frame
    /// @note While we are still steady (i.e. we haven't gotten a "not steady" event) we update
    /// the time and frame every frame!
    public override void UpdateFrame()
    {
        if (m_currentSteady)
        {
            float diffTime = Time.time - m_firstSteady;
            if (diffTime >= m_timeToClick || m_timeToClick <= 0)
            {
                InternalFireDetectEvent();
            }
            if (diffTime > m_timeToReset)
            {
                m_currentSteady = false;
                m_steadyDetector.Reset();
            }
        }
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
        return curHand.AddListener(m_steadyDetector);
    }

    // protected members
    protected SteadyDetector m_steadyDetector; ///< the steady detector we use to detect the gesture
    protected bool m_currentSteady;            ///< holds true if we are currently steady and false otherwise (we detected not steady)
    protected float m_firstSteady;             ///< holds the time we discovered the steady for the first time (becomes irrelevant every time a not steady is detected.
    protected bool m_firedSteadyEvent;          ///< holds true if we fired the steady event for the latest steady and false otherwise.
    /// this is the time between the first detection of a steady gesture and the
    /// time it is considered to have "clicked". This is used to make timed steady
    /// gestures. A value of 0 (or smaller) ignores the timing element.
    protected float m_timeToClick;
    /// this is the time after the initial steady when we reset the steady (i.e. as if we got
    /// a not steady event). This is to simulate a repeat fire button.
    protected float m_timeToReset; 
   
    
    /// this marks the result as clicked by updating the time and frame and the first
    /// time after the last change it also fires the gesture event.
    protected virtual void InternalFireDetectEvent()
    {

        m_timeDetected = Time.time;
        m_frameDetected = Time.frameCount;
        if (m_firedSteadyEvent == false)
        {
            DetectGesture();
            m_firedSteadyEvent = true;
        }
    }

    // private methods (callbacks)
    
    /// callback when we become steady
    /// @param sender the object that sent the event
    /// @param e the event arguments
    void DetectSteadyEvent(object sender, SteadyEventArgs e)
    {
        m_currentSteady = true;
        m_firstSteady = Time.time;
        m_firedSteadyEvent = false;
    }

    /// callback when we become unsteady
    /// @param sender the object that sent the event
    /// @param e the event arguments
    void DetectNotSteadyEvent(object sender, SteadyEventArgs e)
    {
        m_currentSteady = false;
    }


}
