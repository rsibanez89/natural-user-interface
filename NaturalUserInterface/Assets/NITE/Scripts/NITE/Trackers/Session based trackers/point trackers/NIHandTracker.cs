/*******************************************************************************
*                                                                              *
*   OpenNI Unity Toolkit - NITE extension                                      *
*   Copyright (C) 2010 PrimeSense Ltd.                                         *
*                                                                              *
*******************************************************************************/
using UnityEngine;
using OpenNI;
using NITE;

/// @brief This class is responsible for tracking a single hand using NITE controls.
/// 
/// This class provides utilities for tracking the hand position (both smoothed out and raw) as well as 
/// for tracking specific gestures. It uses NITE control to provide superior results to skeleton trackers and
/// is not dependant on the skeleton (i.e. can work without calibration) but rather on NITE session 
/// (see @ref NITEHandsManager)
/// @ingroup NITETrackers
public class NIHandTracker : NIPointTracker 
{

    /// returns a unique name for the tracker type.
    /// @note this is what is used to identify the tracker
    /// @return the unique name.
    public override string GetTrackerType()
    {
        return "Primary point tracker (session based)";
    }


    /// the current position of the tracked point (smoothed)
    public HandPointContext CurPosContext
    {
        get { return m_curPoint; }
    }

    /// the current position of the tracked point (smoothed)
    public HandPointContext CurPosRawContext
    {
        get { return m_curPointRaw; }
    }

    /// return the last smooth good position (preferably from the current frame...)
    public override Vector3 CurPos
    {
        get { return m_lastFrameCurPoint.LastGoodPoint; }
    }

    /// return the last raw good position (preferably from the current frame...)
    public override Vector3 CurPosRaw
    {
        get { return m_lastFrameCurPointRaw.LastGoodPoint; }
    }

    /// return the delta of the last smooth good position (preferably from the current frame...) from the last
    /// frame's good position.
    public override Vector3 CurDeltaPos
    {
        get { return m_lastFrameCurPoint.LastGoodPoint - m_lastFrameCurPoint.LastGoodPointLastFrame; }
    }

    /// return the delta of the last Raw good position (preferably from the current frame...) from the last
    /// frame's good position.
    public override Vector3 CurDeltaPosRaw
    {
        get { return m_lastFrameCurPointRaw.LastGoodPoint - m_lastFrameCurPointRaw.LastGoodPointLastFrame; }
    }


    /// return the delta of the last smooth good position (preferably from the current frame...) from the 
    /// starting position
    public override Vector3 CurPosFromStart
    {
        get { return m_lastFrameCurPoint.LastGoodPoint - StartingPos; }
    }

    /// return the delta of the last Raw good position (preferably from the current frame...) from the 
    /// starting position
    public override Vector3 CurPosFromStartRaw
    {
        get { return m_lastFrameCurPointRaw.LastGoodPoint - StartingPos; }
    }


 
    /// returns the initial position of the point.
    /// @note in the basic implementation this is the focus point (as that is the initial position
    /// of the primary point when using focus). 
    public override Vector3 StartingPos
    {
        get 
        {
            Vector3 pos = Vector3.zero;
            if (m_valid == false)
                return pos;
            if (m_handsManager.Hands.Tracking == false)
                return pos;
            pos = NIConvertCoordinates.ConvertPos(m_handsManager.Hands.SessionManager.FocusPoint);

            return pos; 
        }
    }


    /// mono-behavior initialization
    protected override void InternalAwake()
    {
        base.InternalAwake();
        m_handPrimaryOverrideFilter = null;
        m_pointControlRaw = null;
        m_pointControlSmooth = null;
        m_pointControlSmoothFilter = null;
        NINITECheckVersion.Instance.ValidatePrerequisite();
        m_curPointRaw = new HandPointContext();
        m_curPoint = new HandPointContext();
        m_curPointRaw.ID = -1; // uninitialized
        m_curPoint.ID = -1; // uninitialized
        m_lastFrameCurPoint = null;
        m_lastFrameCurPointRaw = null;
    }

    /// an internal method to initialize the internal structures
    /// @note in most cases, inheriting methods should NOT override the base InitTracking but
    /// rather override this method.
    /// @return true on success and false otherwise.
    protected override bool InitInternalStructures()
    {
        if(m_context==null || m_context.Valid==false)
            return false; // we need hands for this...
        m_handsManager = FindObjectOfType(typeof(NITEHandsManager)) as NITEHandsManager;
        if (m_handsManager == null || m_handsManager.ValidWithInit == false)
        {
            throw new System.Exception("There is no NITEHandsManager in the scene!");
        }
        // need to make sure we don't get errors in initialization
        try
        {
            NINITECheckVersion.Instance.ValidatePrerequisite();
            // set up the raw control point (to get raw data)
            m_pointControlRaw = new PointControl();
            m_handsManager.Hands.SessionManager.AddListener(m_pointControlRaw);

            // set up the denoiser filter
            m_pointControlSmoothFilter = new PointDenoiser();
            m_handsManager.Hands.SessionManager.AddListener(m_pointControlSmoothFilter);
            // set up the smoothed control point (to get smoothed data).
            m_pointControlSmooth = new PointControl();
            m_pointControlSmoothFilter.AddListener(m_pointControlSmooth);

            // register the relevant callbacks
            m_pointControlRaw.PrimaryPointUpdate += new System.EventHandler<HandEventArgs>(myControl_PrimaryPointUpdate);
            m_pointControlRaw.PrimaryPointCreate += new System.EventHandler<HandFocusEventArgs>(myControl_PrimaryPointCreate);
            m_pointControlRaw.PrimaryPointDestroy += new System.EventHandler<IdEventArgs>(myControl_PrimaryPointDestroy);
            m_pointControlSmooth.PrimaryPointUpdate += new System.EventHandler<HandEventArgs>(myControl_PrimaryPointUpdateSmooth);

        }
        catch (System.Exception ex)
        {            
            Debug.Log("failed to initialize. Error=" + ex.Message);
            StopTracking();
            return false;
        }
        m_lastFrameCurPoint = new NIPositionTrackerFrameManager();
        m_lastFrameCurPointRaw = new NIPositionTrackerFrameManager();
        return true;
    }

    /// Method to add a listener
    /// 
    /// This method is used to add a new listener to the hand tracker. 
    /// @note it is the responsibility of the caller to later call @ref RemoveListener
    /// @param newListener the listener to add
    /// @return true on success, false on failure (for example because we are not valid)
    public bool AddListener(PointControl newListener)
    {
        if (Valid == false)
            return false;
        m_pointControlSmoothFilter.AddListener(newListener);
        return true;
    }


    /// Method to remove a previously added listener a listener
    /// 
    /// This method is used to remove a previously added listener. 
    /// @note it is the responsibility of the caller to @ref AddListener to call this.
    /// @param oldListener the listener to remove
    /// @return true on success, false on failure (for example because we are not valid)
    public bool RemoveListener(PointControl oldListener)
    {
        if (Valid == false)
            return false;
        m_pointControlSmoothFilter.RemoveListener(oldListener);
        return true;
    }

 
    /// @brief Releases all internal objects.
    /// 
    /// Release all internal objects in a reverse order to initialization.
    /// @note after releasing this object becomes uninitialized!!!
    public override void StopTracking()
    {
        m_curPointRaw.ID = -1; // uninitialized
        m_curPoint.ID = -1; // uninitialized
        if (m_pointControlRaw != null)
        {
            if (m_handsManager != null && m_handsManager.Valid)
            {
                m_handsManager.Hands.SessionManager.RemoveListener(m_pointControlRaw);
            }
            m_pointControlRaw.Dispose();
            m_pointControlRaw = null;
        }
        if (m_pointControlSmooth != null)
        {
            if (m_pointControlSmoothFilter != null)
                m_pointControlSmoothFilter.RemoveListener(m_pointControlSmooth);
            m_pointControlSmooth.Dispose();
            m_pointControlSmooth = null;
        }

        if (m_pointControlSmoothFilter != null)
        {
            if (m_handsManager != null && m_handsManager.Valid)
            {
                m_handsManager.Hands.SessionManager.RemoveListener(m_pointControlSmoothFilter);
            }
            m_pointControlSmoothFilter.Dispose();
            m_pointControlSmoothFilter = null;
        }
        m_lastFrameCurPoint = null;
        m_lastFrameCurPointRaw = null;
        base.StopTracking();
    }



    // protected members

    /// Internal the point controller to get the raw input.
    protected PointControl m_pointControlRaw = null;
    /// Internal the point controller to get the smoothed input.
    protected PointControl m_pointControlSmooth = null;

    /// Internal filter to make the point control be denoised.
    protected PointDenoiser m_pointControlSmoothFilter = null;

    /// the point filter to force the primary hand to be the chosen one
    protected PointFilter m_handPrimaryOverrideFilter = null;

    /// a point control which tries to find the correct hand.
    protected PointControl m_handChooser = null;

    /// a router to choose between @ref m_handPrimaryOverrideFilter and @ref m_handChooser
    protected FlowRouter m_flowRouter = null;

    /// this holds the current point value (after smoothing)
    protected HandPointContext m_curPoint;
    /// this holds the current raw point value (without smoothing)
    protected HandPointContext m_curPointRaw;

    /// used to get the last good value of the smooth point
    protected NIPositionTrackerFrameManager m_lastFrameCurPoint;

    /// used to get the last good value of the raw point
    protected NIPositionTrackerFrameManager m_lastFrameCurPointRaw;

    /// the hands manager which holds the session we are part of.
    protected NITEHandsManager m_handsManager;

    // protected methods

      // private methods (callbacks)
    /// callback when we lose the session (logging only)
    /// @param sender the object that sent the event
    /// @param e the event arguments
    void myControl_PrimaryPointDestroy(object sender, IdEventArgs e)
    {
        m_handsManager.Log("lost point for " + this + " id=" + e.ID, NIEventLogger.Categories.Callbacks, NIEventLogger.Sources.Hands);
    }

    /// callback when we start the session (logging only)
    /// @param sender the object that sent the event
    /// @param e the event arguments
    void myControl_PrimaryPointCreate(object sender, HandFocusEventArgs e)
    {
        m_handsManager.Log("found primary point for " + this + " id=" + e.Hand.ID, NIEventLogger.Categories.Callbacks, NIEventLogger.Sources.Hands);
    }

    /// callback when the point is updated
    /// @param sender the object that sent the event
    /// @param e the event arguments

    void myControl_PrimaryPointUpdate(object sender, HandEventArgs e)
    {
        m_lastFrameCurPointRaw.UpdatePoint(e.Hand.Position, e.Hand.Confidence);
        m_curPointRaw = e.Hand;
    }

    /// callback when the point is updated
    /// @param sender the object that sent the event
    /// @param e the event arguments

    void myControl_PrimaryPointUpdateSmooth(object sender, HandEventArgs e)
    {
        m_lastFrameCurPoint.UpdatePoint(e.Hand.Position, e.Hand.Confidence);
        m_curPoint = e.Hand;
    }
}
