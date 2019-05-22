/*******************************************************************************
*                                                                              *
*   OpenNI Unity Toolkit - NITE extension                                      *
*   Copyright (C) 2010 PrimeSense Ltd.                                         *
*                                                                              *
*******************************************************************************/
using UnityEngine;
using OpenNI;
using NITE;

/// @brief This class implements a factory which allows us to receive a push gesture and assign it to a hand.
/// @ingroup NITETrackers
public class  NIPushGestureFactory : NIGestureFactory
{
    /// returns a unique name for the gesture type.
    /// @note this is what is used to identify the factory
    /// @return the unique name.
    public override string GetGestureType()
    {
        return "PushGesture";
    }

    /// this creates the correct object implementation of the tracker
    /// @return the tracker object. 
    protected override NIGestureTracker GetNewTrackerObject()
    {
        return new NIPushTracker();
    }
}
