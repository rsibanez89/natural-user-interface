/*******************************************************************************
*                                                                              *
*   OpenNI Unity Toolkit - NITE extension                                      *
*   Copyright (C) 2010 PrimeSense Ltd.                                         *
*                                                                              *
*******************************************************************************/
using UnityEngine;
using OpenNI;

/// @brief  This class is a utility class which should be attached to a visible game object. It will move
/// the game object to a position based on the position of the hand it is attached to.
/// @ingroup NITESamples
public class NIHandPosition : MonoBehaviour 
{
    /// the hand manager in order to get the hand tracker
    public NIPointTrackerManager m_manager;

    /// the scale (sensitivity) between moving the hand and moving in the game world
    public float m_scale;

    /// holds the initial position of the game object we are attached to (this is so that
    /// we move from that initial position
    protected Vector3 m_initialPosition;

    /// True if we wish to apply denoising 
    public bool m_applyDenoise;

    /// the hand tracker providing us with position information
    protected NIPointTracker m_tracker;

    /// the initialization (mono-behavior)
    public void Start()
    {
        if (m_manager == null)
            m_manager = FindObjectOfType(typeof(NIPointTrackerManager)) as NIPointTrackerManager;

        m_tracker = m_manager.GetTracker("Primary point tracker (session based)");
        if (m_tracker == null)
            throw new System.Exception("The requested tracker is not attached!");
        m_initialPosition = transform.position;
    }

    /// the mono-behavior update
    public void Update()
    {
        if (m_tracker.Valid == false)
            return; // we wait till it is valid.
        Vector3 newPos;
        if (m_applyDenoise)
            newPos = m_tracker.CurPos;
        else
            newPos = m_tracker.CurPosRaw;
        newPos -= m_tracker.StartingPos;
        newPos *= m_scale;
        transform.position = newPos + m_initialPosition;

    }


    /// @brief makes sure we release everything on quit
    /// 
    /// This will make sure everything the context is released when quitting (important for playing and
    /// stopping in the editor).
    /// @note - This is based on the MonoBehavior.OnAppliactionQuit.
    public void OnApplicationQuit()
    {
        if (m_tracker != null)
        {
            m_manager.ReleaseTracker("Primary point tracker (session based)");
            m_tracker = null;
        }
    }

    /// @brief makes sure we release everything on destroy
    /// 
    /// This will make sure everything released when the object is destroyed
    /// (important for playing and stopping in the editor).
    /// @note - This is based on the MonoBehavior.OnDestroy.
    public void OnDestroy()
    {
        if (m_tracker != null)
        {
            m_manager.ReleaseTracker("Primary point tracker (session based)");
            m_tracker = null;
        }
    }
}
