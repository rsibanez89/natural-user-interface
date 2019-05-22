/*******************************************************************************
*                                                                              *
*   OpenNI Unity Toolkit - NITE extension                                      *
*   Copyright (C) 2010 PrimeSense Ltd.                                         *
*                                                                              *
*******************************************************************************/
// this file is used for documentation rather than code.
// it is aimed at the NITE samples tutorials

/**
@page NITESphereControlsTutorial Control a Sphere Using the Hands Manager

In this sample, we will move a sphere with our hand. The basic idea here is the use of the hands manager
to follow the position.


@note The assumption of each sample tutorial is that all relevant packages have been imported 
(including the engine, OpenNI, utilities and the sample package itself) and a new scene has been created.

<H1>Step 1: Setting the Scene</H1>
As with previous samples, we will begin by adding the OpenNISettings Prefab to the scene and we will
uncheck the "Use Image?" checkbox. In addition, unlike previous samples, we will also uncheck the "Use 
Users and Skeleton?" checkbox, as we are not interested in the skeleton.<br><br>

We will also add the HandsManager prefab. This is the heart of the @ref NITEExtension. It is 
responsible for managing the session. For simplicity, we will use the basic definitions used in the
prefab (use the basic gesture list, use click for focusing and not using refocus at all).


<H1>Step 2: Add the hand trackers</H1>
Create an empty game object (name it HandTracker) and add the @ref NIHandTracker script to it. This is
a point tracker that uses the primary hand of the session.<br><br>
Create another empty game object (name it HandTrackersManager) and attach the NIPointTrackerManager
script to it. Now add the HandTracker to its list of trackers (as was done in previous samples).

<H1>Step 3: Add the sphere</H1>
Create a new sphere game object. Attach the @ref NIHandPosition script to it. This script will 
simply move the sphere to a position which is the same as its initial position, plus the hand position
(scaled). Set the scale to 0.01. Also set the "Apply Denoise" option (so the sphere will move smoothly).

In addition, to improve how everything looks, add a light point at 0,0,-10 with 0 rotation and a 
range of 60.

<br>
Press play.<br>
We can now see the sphere on the screen. If we do the focus gesture (moving the arm straight forward
and then backwards, also known as the "click" gesture) we will take control of the sphere,
which will move with us in the 3D space.<br><br>

<H1>Enjoy your victory, you have completed your first NITE sample!</H1>
Continue to the next sample @ref NICursorInputTutorial or return to @ref NITESamplesList
*/

/**
@page NICursorInputTutorial Control a cursor using the input (hands manager based)
This sample repeats the OpenNI Input Control Tutorial with two differences:
- It uses the NITE controls instead of a skeleton hand
- It adds a feedback for clicking (by using the push gesture)

In this sample, we will take control of a cursor in the 2D world and provide feedback on clicking using
the push gesture.

@note The assumption of each sample tutorial is that all relevant packages have been imported 
(including the engine, OpenNI, utilities and the sample package itself) and a new scene has been created.

<H1>Step 1: Setting the scene</H1>
As with the previous NITE sample, we will begin by adding the OpenNISettings Prefab to 
the scene. We will uncheck the "Use Image" checkbox and the "Use Users and Skeleton?" checkbox.
<br><br>
Add the HandsManager prefab (leave the basic definitions as they are in the prefab).

<H1>Step 2: Add the input and trackers</H1>
Create an empty game object (name it HandTracker) and add the @ref NIHandTracker script to it.<br><br>
Create an empty game object (name it GestureTracker) and add the @ref NIPushGestureFactory script to it.<br><br>
Add the InputController prefab.<br>
Add the HandTracker tracker to the HandsManager child of the Input (not to be confused with the 
HandsManager prefab) and add the GestureTracker to the GestureManager child of the Input.
 
As with the OpenNI sample, for simplicity, we are going to use the Horizontal and Vertical axes 
(which are defined by default) to control the x and y movement on the screen. However, now we will
not normalize the result. <br>
This means that we need to make the following changes to the default:
- Sensitivity should be 0.5
- Normalization should be 320 for Horizontal, 240 for Vertical.
- Type should be HandMovementFromStartingPoint
- Tracker to use should be "Primary point tracker (session based)".

In addition, we will add a FireExample axis. Configure it as follows (everything should be 
familiar by now):
- Name should be "FireExample"
- NIInput axis?: Select the check box (this will NOT be defined in InputManager)
- Gesture: PushGesture
- Dead: 0.001
- Sensitivity: 1
- Invert: not checked
- Type: Gesture
- Normalization: -1
- Axis: xAxis
- Tracker to use should be "Primary point tracker (session based)".

<H1>Step 4: Add the cursor</H1>
Create an empty game object (name it cursor) and attach the @ref NIHandPositionInput script to it.
This script is very similar to NIHandPositionInputPositionOnly script used in the original, OpenNI sample
but adds a "click" option.
<br><br> 
Press play.<br>
If we make a focus gesture (moving the arm straight forward and then backwards, also known as the
"click" gesture) we will take control of the cursor and allow it to move in the 2D world. Also,
if we make a "push" gesture (similar to "click" but does not have to return) we can see the 
box on the left the time we last clicked.<br><br>

<H1>Enjoy your victory, you have completed your second NITE sample!</H1>
Continue to the next sample @ref NITENIGUITutorial or return to @ref NITESamplesList
*/



/**
@page NITENIGUITutorial NIGUI (hands manager based)

This sample is the same as the OpenNI GUI Tutorial sample with the only difference being the use of NITE 
controls rather than skeleton hand. Because of this, this tutorial will be VERY similar to the previous
one.<br>
We will be seeing various GUI elements and we will be able to control them by using the primary session
hand. In order to simulate a left mouse click, e.g. to click on a button, we will use the timed 
steady gesture. This means that when the hand does not move, a counter will begin and a click will 
occur after several seconds.

@note The assumption of each sample tutorial is that all relevant packages have been imported 
(including the engine, OpenNI, utilities and the sample package itself) and a new scene has been created.

<H1>Step 1: Add the basic objects:</H1> 
As with the previous NITE sample, we will begin by adding the OpenNISettings Prefab to 
the scene. We will uncheck the "Use Image" checkbox and the "Use Users and Skeleton?" checkbox.
<br><br>
Add the HandsManager Prefab (leave the basic definitions as they are in the prefab).

<H1>Step 2: Create and configure the input:</H1> 
Create an empty game object (name it HandTracker) and add the @ref NIHandTracker script to it.<br><br>
Create an empty game object (name it GestureTracker) and add the @ref NISteadyGestureFactory script to it.
- Set the Time to click to 2 seconds (this is how long we need to wait)
- Set the Time to reset to 10000 (we don't want to reset)

<br><br>

Add the InputController prefab.<br>
Add the HandTracker tracker to the HandsManager child of the Input (not to be confused with the 
HandsManager prefab) and add the GestureTracker to the GestureManager child of the Input.<br><br>
As with the OpenNI sample, we will be using the default cursor (see below), which forces us to 
define the following axes:
- NIGUI_X to move along the x axis
- NIGUI_Y to move along the y axis
- NIGUI_CLICK to click

For this purpose, we need to perform the following configurations:
- NIGUI_X
    - Name: NIGUI_X
    - NIInput axis?: Select the check box (this will NOT be defined in InputManager)
    - Gesture: None
    - Dead: 0.001
    - Sensitivity: 0.5
    - Invert: not checked
    - Type: HandMovementFromStartingPoint
    - Normalization: 320
    - Axis: xAxis
    - Tracker to use: "Primary point tracker (session based)"
- NIGUI_Y
    - Name: NIGUI_Y
    - NIInput axis?: Select the check box (this will NOT be defined in InputManager)
    - Gesture: None
    - Dead: 0.001
    - Sensitivity: 0.5
    - Invert: not checked
    - Type: HandMovementFromStartingPoint
    - Normalization: 240
    - Axis: yAxis
    - Tracker to use: "Primary point tracker (session based)"
- NIGUI_CLICK
    - Name: NIGUI_CLICK
    - NIInput axis?: Select the check box (this will NOT be defined in InputManager)
    - Gesture: SteadyGesture
    - Dead: 0.001
    - Sensitivity: 1
    - Invert: not checked
    - Type: Gesture
    - Normalization: -1
    - Axis: xAxis
    - Tracker to use: "Primary point tracker (session based)" 

<H1>Step 3: Add a %NIGUI prefab</H1>
Add the NIGUI prefab to the scene.

<H1>Step 4: Add the GUI example script</H1>
Create an empty game object (name it ExampleGUI). Attach the GUIExample script to it. 

Press play.<br>
We can now click on the various buttons, change the sliders and interact with the GUI.
@note 
- In order to click, place the cursor in the relevant position and leave your hand steady there. 
The cursor will slowly change color. When it is fully green, it will click. If the hand is not steady 
enough, the click will not occur.
- When using sliders (and scroll bars), if you click on the actual scroll thumb, the scrollbar/
slider will move with the movement of the hand until a new click is used.
- Some of the labels will react to clicking on buttons, sliders etc. to provide feedback.

<H1>Enjoy your victory, you have completed your third NITE sample!</H1>
Return to @ref NITESamplesList
*/

