/*******************************************************************************
*                                                                              *
*   OpenNI Unity Toolkit - NITE extension                                      *
*   Copyright (C) 2010 PrimeSense Ltd.                                         *
*                                                                              *
*******************************************************************************/
using UnityEngine;

/// /// @brief utility class to follow hand position in the x/y axes using NIInput.
/// 
/// This class is a utility which should be attached to an invisible game object. It 
/// will draw (using OnGUI box) the position of the hand it is attached to on the
/// screen. In addition it count the pressing of a fire axis
/// It assumes there are a horizontal and vertical axes and a FireAxis for pushing in the NIInput inspector.
/// @ingroup NITESamples
public class NIHandPositionInput : MonoBehaviour 
{
    /// the input we follow
    protected NIInput m_input;

    /// holds the initial position of the game object we are attached to (this is so that
    /// we move from that initial position
    protected Vector3 m_initialPosition;

    /// the initialization (mono-behavior)
    public void Start()
    {
        m_initialPosition = transform.position;
        m_initialized = false;
        m_lastCallbackTime = -1.0f;
        m_input = FindObjectOfType(typeof(NIInput)) as NIInput;
        if (m_input == null)
            throw new System.Exception("No NIInput in scene!!!");
    }

    /// The mono-behavior update
    public void Update()
    {       
        if (m_initialized == false)
        {
            // we do it here to make sure the axis is initialized!
            m_input.RegisterCallbackForGesture(InternalCallback, "FireExample");
            m_initialized = true;
        }
    }

    bool m_initialized;
    float m_lastCallbackTime;
    int m_lastCallbackFrame;

    /// internal callback to check the callback
    /// @param hand the hand tracker which created this callback
    protected void InternalCallback(NIPointTracker hand)
    {
        m_lastCallbackTime = Time.time;
        m_lastCallbackFrame = Time.frameCount;
    }


    void OnGUI()
    {
        // move the cursor
        float x = m_input.GetAxis("Horizontal");
        float y = -m_input.GetAxis("Vertical");
        x += 0.5f;
        x *= (Screen.width - 20);
        y += 0.5f;
        y *= (Screen.height - 20);
        GUI.Box(new Rect(x, y, 20, 20), "");

        // show the clicks
        GUILayout.Box(string.Format("Current Time={0:000.00}, current frame={1:0000}",Time.time,Time.frameCount));
        if (m_lastCallbackTime<0)
            GUILayout.Box("Click by moving your hand straight forward and back");
        else
            GUILayout.Box(string.Format("Last time click callback was called on time={0:00.00}, frame={1:0000}", m_lastCallbackTime, m_lastCallbackFrame));
    }
}
