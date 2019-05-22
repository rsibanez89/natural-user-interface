/*******************************************************************************
*                                                                              *
*   OpenNI Unity Toolkit - NITE extension                                      *
*   Copyright (C) 2010 PrimeSense Ltd.                                         *
*                                                                              *
*******************************************************************************/
using UnityEngine;
using OpenNI;
using NITE;

/// @brief This class implements a factory which allows us to receive a steady gesture and assign it to a hand.
/// @ingroup NITETrackers
public class  NISteadyGestureFactory : NIGestureFactory
{
    /// this is the time between the first detection of a steady gesture and the
    /// time it is considered to have "clicked". This is used to make timed steady
    /// gestures. A value of 0 (or smaller) ignores the timing element.
    public float m_timeToClick;
    /// this is the time after the "click" event when we reset the steady (i.e. as if we got
    /// a not steady event). This is to simulate a repeat fire button.
    /// @note this means that if one leaves his hand steady continuously then the event will fire
    /// every timeToClick+timeToReset seconds starting from timeToClick seconds after the initial
    /// steady.
    public float m_timeToReset; 

    /// returns a unique name for the gesture type.
    /// @note this is what is used to identify the factory
    /// @return the unique name.
    public override string GetGestureType()
    {
        return "SteadyGesture";
    }

    /// this creates the correct object implementation of the tracker
    /// @return the tracker object. 
    protected override NIGestureTracker GetNewTrackerObject()
    {
        return new NISteadyTracker(m_timeToClick, m_timeToReset);
    }
}
