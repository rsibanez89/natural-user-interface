/*******************************************************************************
*                                                                              *
*   OpenNI Unity Toolkit - NITE extension                                      *
*   Copyright (C) 2010 PrimeSense Ltd.                                         *
*                                                                              *
*******************************************************************************/


using UnityEngine;
using OpenNI;

/// @brief A base class to configure and use OpenNI and NITE capabilities
/// 
/// This class is responsible for abstracting the initialization of an OpenNI context as well as its nodes.
/// It also provides various configuration options for how these will be created and shows their behavior in the
/// inspector
/// @ingroup NITEBasicObjects
public class NITEHandsManager : MonoBehaviour
{
    // public objects to be defined by the user

    /// this holds the gesture string for focusing. 
    /// @note we hide this from the inspector because we override it in a custom inspector.
    public string m_focusGesture;
    /// this holds the gesture string for refocusing
    /// @note we hide this from the inspector because we override it in a custom inspector.
    public string m_refocusGesture;

    /// the context to work with
    OpenNISettingsManager m_context;
    // public accessors

    /// Accessor to @ref m_handsControl
    public NIHandControl Hands
    {
        get { return m_handsControl; }
    }

    /// Will hold true if this object is valid. If false, no NI capability can be used!
    public bool Valid
    {
        get 
        {
            return m_context!=null && m_context.Valid && m_handsControl.Valid; 
        }
    }

    /// this is exactly like @ref Valid except it also makes sure the hands manager is initialized
    /// before the test.
    public bool ValidWithInit
    {
        get
        {
            if (m_initializedHands == false)
                InternalInit();
            return Valid;
        }
    }
    /// @brief updates the context
    /// 
    /// This is a mono-behavior FixedUpdate. We use FixedUpdate to make sure this is called before other updates
    /// even if the frame rate is low (to give the context and other elements time to update).
    /// This method is responsible for updating the internal objects (such as the context).
    public void FixedUpdate()
    {
        if (ValidWithInit == false)
            return;
        m_handsControl.UpdateHands();
        
    }

    /// @brief Initializes everything
    /// 
    /// This is a mono-behavior Awake which is used to initialize everything. Note that since internal objects
    /// such as the context are singleton, they are constructed before BUT they are initialized here.
    /// This means that before the Awake function is called, all public members (such as m_Logger, m_query 
    /// and m_XMLFileName) must have been initialized and that no one can use NIConfigurableContext before this
    /// is finished.     
    public void Awake()
    {
        m_initializedHands = false;
    }


    /// this method makes sure we are initialized when we reach
    protected virtual void InternalInit()
    {
        if (m_context == null)
        {
            m_context = FindObjectOfType(typeof(OpenNISettingsManager)) as OpenNISettingsManager;
            if (m_context == null)
            {
                throw new System.Exception("Hands manager needs a valid NIConfigurableContextManager to work");
            }
        }
        if (m_context.Valid == false)
        {
            throw new System.Exception("Hands manager needs a valid NIConfigurableContextManager to work");
        }
        m_handsControl = NIHandControl.Instance;
        m_handsControl.Init(m_context.m_Logger, m_context.CurrentContext, m_focusGesture, m_refocusGesture);
        m_initializedHands = true;
    }

    /// @brief makes sure we release everything on quit
    /// 
    /// This will make sure the context is released when quitting (important for playing and
    /// stopping in the editor).
    /// @note - This is based on the MonoBehavior.OnAppliactionQuit.
    public void OnApplicationQuit()
    {
        Log("In NIHandsManager.OnApplicationQuit",  NIEventLogger.Categories.Initialization, NIEventLogger.Sources.BaseObjects);
        ReleaseNI();
    }

    /// @brief makes sure we release everything on destroy
    /// 
    /// This will make sure the context is released when the object is destroyed
    /// (important for playing and stopping in the editor).
    /// @note - This is based on the MonoBehavior.OnDestroy.
    public void OnDestroy()
    {
        Log("In NIHandsManager.OnDestroy", NIEventLogger.Categories.Initialization, NIEventLogger.Sources.BaseObjects);
        ReleaseNI();
    }

    /// This does a safe log (i.e. checks if the logger is null before logging
    /// @param str the string to log
    /// @param category the category of the log
    /// @param source the source of the log
    public void Log(string str, NIEventLogger.Categories category, NIEventLogger.Sources source)
    {
        if (m_context != null)
            m_context.Log(str, category, source);
    }
    // protected methods

    /// @brief Releases all internal objects.
    /// 
    /// Release all internal objects in a reverse order to initialization.
    /// @note after releasing this object becomes uninitialized!!!
    protected void ReleaseNI()
    {
        if (m_handsControl!=null && m_handsControl.Valid)
        {
            m_handsControl.Dispose();
            m_handsControl = null;
        }
        m_initializedHands = false;
    }

    // protected members

    /// A link to the a hands control object (currently generated from the singleton) 
    protected NIHandControl m_handsControl;

    /// this is true when the hands object has been initialized. False, otherwise.
    protected bool m_initializedHands;
}
