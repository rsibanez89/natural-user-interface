
/*******************************************************************************
*                                                                              *
*   OpenNI Unity Toolkit - NITE extension                                      *
*   Copyright (C) 2010 PrimeSense Ltd.                                         *
*                                                                              *
*******************************************************************************/

using System.Collections.Generic;
using UnityEngine;
using OpenNI;
using NITE;
 
/// @brief abstraction of hand position and gesture handling
/// 
/// This class is responsible for abstracting gestures handling and NITE session.
/// <br>In order for the hand to be tracked this class must be in session. A session is
/// started through a focus gesture (a gesture done with one hand which tells us to follow that
/// hand). If the hand is lost, there is a timeout where a simpler gesture can be used to refocus.
/// @ingroup NITEBasicObjects
public class NIHandControl : NIWrapperContextDependant
{
    // enum declaration

    /// represent the state of the session 
    public enum SessionStates 
    { 
        NotInSession, ///< we are not in a session, do the focus gesture to start
        InSession,    ///< we are in a session and tracking the hand
        Refocusing    ///< we lost the hand and are not tracking, use either the refocus or focus gestures to resume
    };

    // public accessors

    /// Accessor for m_sessionState
    public SessionStates CurrentState
    {
        get { return Valid ? m_sessionState : SessionStates.NotInSession; }
    }

    /// Accessor which returns true when we are tracking the hand (false when we need any 
    /// user action such as focus or refocus)
    public bool Tracking
    {
        get { return Valid && m_sessionState == SessionStates.InSession; }
    }


    /// Accessor which returns true if the NIHandControl is valid and initialized and false if anything is wrong
    public override bool Valid
    {
        get { return base.Valid && m_sessionManager!=null; }
    }


    /// Accessor to @ref m_sessionManager
    public SessionManager SessionManager
    {
        get { return m_sessionManager; }
    }


    /// Accessor to the singleton (@ref m_singleton)
    public static NIHandControl Instance
    {
        get { return m_singleton; }
    }

    /// @brief Initialize the hand control
    /// 
    /// This method provides the initialization for the hands control initializing all internal objects. 
    /// It assumes the context is already valid (otherwise we fail).
    /// @note - Since we use a singleton pattern, multiple initializations will simply delete the old
    /// objects and create new ones!
    /// @note - It is the responsibility of the initializer to call @ref Dispose.
    /// @param context the context to use
    /// @param focusGesture the gesture used for focusing (i.e. this is aimed to set the hand which we will follow)
    /// @param refocusGesture the gesture used for refocus (when we lose focus temporarily)
    /// @param logger the logger object we will enter logs into
    /// @return true on success, false on failure. 
    public bool Init(NIEventLogger logger, NIContext context, string focusGesture, string refocusGesture)
    {
        Dispose(); // to make sure we are not initialized
        if (InitWithContext(logger, context) == false)
            return false;
        // we will create the user gestures and hands which are needed for the session manager
        // but are not needed externally (as we never access them directly).
        ProductionNode node = context.CreateNode(NodeType.Gesture);
        if (node == null)
        {
            Dispose();
            return false;
        }
        m_internalNodes.Add(node);
        node = context.CreateNode(NodeType.Hands) as HandsGenerator;
        if (node == null)
        {
            Dispose();
            return false;
        }
        m_internalNodes.Add(node);
        try
        {
            NINITECheckVersion.Instance.ValidatePrerequisite();
            m_sessionManager = new SessionManager(context.BasicContext, focusGesture, refocusGesture);
            if (refocusGesture.CompareTo("") == 0)
            {
                m_sessionManager.QuickRefocusTimeout = 0;
            }
            else
                m_sessionManager.QuickRefocusTimeout = 15000;
            m_sessionManager.SessionStart += new System.EventHandler<PositionEventArgs>(HandsControlSessionStart);
            m_sessionManager.SessionEnd += new System.EventHandler(HandsControlSessionEnd);
            m_pointControl = new PointControl();
            m_sessionManager.AddListener(m_pointControl);
            m_pointControl.PrimaryPointDestroy += new System.EventHandler<IdEventArgs>(SessionLostFocus);
        }
        catch (System.Exception ex)
        {
            Log("failed to create session manager with error " + ex.Message, NIEventLogger.Categories.Errors, NIEventLogger.Sources.Hands);
            Dispose();
            return false;
        }

        return true;
    }

    /// @brief updates the hands control
    /// 
    /// This will make sure the hand control is updated if necessary.
    /// @note - This does NOT wait for the update so it is up to the user method to check if new
    /// data is available.
    public virtual void UpdateHands()
    {
        m_sessionManager.Update(m_context.BasicContext);
    }

    /// @brief Release a previously initialize controller.
    /// 
    /// This method releases a previously initialized hand controller (including all relevant data).
    /// @note - Since we use a singleton pattern, only one place should do a release, otherwise the
    /// result could become problematic for other objects
    /// @note - It is the responsibility of the whoever called @ref Init to do the release. 
    /// @note - The release should be called BEFORE releasing the context. If the context is invalid, the result is
    /// undefined.
    public override void Dispose()
    {
        if (m_context == null)
            return;
        foreach(ProductionNode node in m_internalNodes)
        {
            if (node != null)
                m_context.ReleaseNode(node);
        }
        m_internalNodes.Clear();
        if(m_sessionManager!=null)
        {
            m_sessionManager.Dispose();
            m_sessionManager = null;
        }
        if(m_pointControl!=null)
        {
            m_pointControl.Dispose();
            m_pointControl = null;
        }
        m_sessionState = SessionStates.NotInSession;
        base.Dispose();
    }

    // protected members

    /// The current session state 
    protected SessionStates m_sessionState;
    /// this is the name for the gesture used to focus to the session 
    protected string m_focusGestureName;
    /// this is the name for the gesture used to refocus to the session 
    protected string m_refocusGestureName;

    /// Internal only: This list contains nodes necessary for the session manager to work. It is internal only.
    protected List<ProductionNode> m_internalNodes;
    /// the session manager
    protected SessionManager m_sessionManager;
    /// @brief The singleton itself
    /// 
    /// NIHandControl uses a singleton pattern. There is just ONE NIImage object which is used by all.
    protected static NIHandControl m_singleton = new NIHandControl();

    /// Internal: used in order to register the lost of focus
    protected PointControl m_pointControl;


    // protected methods (event handlers)

    void HandsControlSessionEnd(object sender, System.EventArgs e)
    {
        Log("Session ended", NIEventLogger.Categories.Callbacks, NIEventLogger.Sources.Hands);
        m_sessionState = SessionStates.NotInSession;
    }

    void HandsControlSessionStart(object sender, PositionEventArgs e)
    {
        Log("Session started", NIEventLogger.Categories.Callbacks, NIEventLogger.Sources.Hands);
        m_sessionState = SessionStates.InSession;
    }

    void SessionLostFocus(object sender, IdEventArgs e)
    {
        Log("refocus stated", NIEventLogger.Categories.Callbacks, NIEventLogger.Sources.Hands);
        m_sessionState = SessionStates.Refocusing;
    }

    // private methods

    /// @brief private constructor
    /// 
    /// This is part of the singleton pattern, a protected constructor cannot be called externally (although
    /// it can be inherited for extensions. In which case the extender should also replace the singleton!
    private NIHandControl()
    {
        m_sessionState = SessionStates.NotInSession;
        NINITECheckVersion.Instance.ValidatePrerequisite();
        m_internalNodes = new List<ProductionNode>();
        m_sessionManager = null;
    }
}
