/****************************************************************************
*                                                                           *
*  OpenNI Unity Toolkit                                                     *
*  Copyright (C) 2011 PrimeSense Ltd.                                       *
*                                                                           *
*                                                                           *
*  OpenNI is free software: you can redistribute it and/or modify           *
*  it under the terms of the GNU Lesser General Public License as published *
*  by the Free Software Foundation, either version 3 of the License, or     *
*  (at your option) any later version.                                      *
*                                                                           *
*  OpenNI is distributed in the hope that it will be useful,                *
*  but WITHOUT ANY WARRANTY; without even the implied warranty of           *
*  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the             *
*  GNU Lesser General Public License for more details.                      *
*                                                                           *
*  You should have received a copy of the GNU Lesser General Public License *
*  along with OpenNI. If not, see <http://www.gnu.org/licenses/>.           *
*                                                                           *
****************************************************************************/
// this file is used for documentation rather than code.
// it is aimed at the OpenNI samples tutorials

/**
@page OpenNISingleSkeletonTutorial Single Skeleton Sample

Welcome to the first tutorial. In this tutorial, we will place a model on the scene and allow a user 
to control it. We will also look at some debugging tools that help us to understand the process.

@note The assumption of each sample tutorial is that all relevant packages have been imported 
(including the engine, OpenNI, utilities and the sample package itself) and a new scene has been created.

<H1>Step 1: Creating the basic objects</H1>
The first thing we need to do is to start adding the basic objects, the first of which is the 
@ref OpenNISettingsPrefab. While this prefab can be built manually, and in some cases even broken into its
individual objects, in almost all cases the developer can drag and drop this prefab to the scene and 
configure it using the inspector.<br>
 
Let's take a look at this object. We will begin by opening it up and looking at its child objects.<br>
- We use a regular query as there is no requirement for anything special. @note This package was tested 
  using the supplied Primesense's implementation. Therefore if problem occur with different 
  implementations, it is possible to use NIPrimeSenseQuery which limits the implementation used.
- Nothing needs to be changed in the logger's filter, we want to see all the logging messages for now. 
  After debugging, we can turn off some or all of the options to filter out irrelevant messages.
- The OpenNI configuration object. The inspector for this object provides us with configuration options to
  make OpenNI behave as needed for the game. For now configure the following:
    - Mirroring is selected, as this provides the basic behavior we are looking for (we are looking from
      behind).<br>
    - We don't need any special XML file. @note It is very uncommon to need an XML file in this package as
      the package is built to simplify the use. The only standard use for the XML file is for playing back
      a recording. (see @ref OpenNIRecordAndPlayback "Recording and playing back sensor data").
    - Uncheck "Use Image Generation" checkbox, as we do not have an RGB image in this sample. @note The 
      current samples do not use the RGB image at all. Currently it is used only in the 
      NIImagemapViewerUtility utility.
    - Since the skeleton is the basis of this tutorial, we want to use it. Make sure the box is checked and
      that the smoothing value is 0.5, which is a good choice when controlling a skeleton.
    - The prefab has already taken care of linking the logger and query objects to the OpenNI 
      configuration object.
    - Since we don't do any playback, the "Use Playback mode" checkbox should be clear.

We will also add the NIDepthViewer and NIRadarViewer prefabs. These will enable us to see the depth 
data (i.e. knowing what the sensor sees) and which users are identified.<br><br><br>

Press play. <br>Nothing is seen in the game window but various messages appear in the console. Looking at
the console, you can see the initialization process.<br>
As the scene loads, various initializations are performed. These are sent to the console 
(based on the filters of the logger in @ref OpenNISettingsPrefab. <br>
You should see something like:<br><br>

<code>
In NIContext:Init(Logger (NIEventLogger))
In OpenNIContext.InitContext with logger=Logger (NIEventLogger) query=Query (NIQuery)
Creating type=Depth
In NIUserAndSkeleton:Init(Logger (NIEventLogger))
initializing with context
Creating type=User
etc.</code><br><br>

If any error messages appear, specifically exceptions or "context invalid" messages, this usually 
means that OpenNI framework, an OpenNI compliant middleware (e.g. NITE) or the sensor drivers are either not 
installed or are in an old version. In this situation it is best to reinstall them (see the readme.rtf file
and @ref OpenNIBeforeYouStart for more information). Also make sure the XML string is empty. 
If this does not solve the problem, please contact support@primesense.com<br>

When ending the game by pressing play again to return to edit mode, messages will appear telling us 
everything is being released.

<H3>Look at what the sensor sees in the Depth Viewer</H3>
The sensor can show depth information. The NIDepthViewer shows the depth image in the upper right
corner of the screen. We can configure the NIDepthViewer in a number of ways
(see @ref NIDepthmapViewerUtility for more information) but the main elements are:
- @b Context: This is basically the OpenNISettingsManager object. Generally speaking, you should drag the
relevant object from the OpenNISettingsPrefab. However, in practice we can leave it as "None", as the 
viewer will find the object by itself (assuming just one exists). @note This is true for many objects!
- <b>Place to draw</b>: Defines the rectangle the depth will be drawn on. The x,y values represent the 
distance from the relevant corner (defined in the Snap variable) and the width and height set the 
size of the window.
- @b Factor (which is default to 4): Sets the ratio between the original resolution provided by the sensor
and the one drawn. Specifically, if the sensor provides a resolution of QVGA (320*240) and we use a 
factor of 4, then we will only use a resolution of 80*60 (taking one pixel in 4 in each direction or 
a total of 1/16 of the pixels). This is used for performance as the NIDepthViewer is a debug tool and
not very efficient. @note Do not try to use a factor of 1 unless you have an extremely powerful computer. 
On most computers, a value of 4 is the best to use (2-3 for stronger ones).
- <b>Depth Map color</b>: Sets which base color is used.

<H3>Find the users in the Radar Viewer</H3>
The NIRadarViewer allows us to see users. A user is a moving object in the scene. <br>
Users are considered "players", i.e. someone who controls the input, only after they perform a calibration.
A user should stand in a calibration pose (standing straight with the arms to the sides and the hands up) 
for a couple of seconds. After that, the user is considered calibrated.<br>
An uncalibrated user is drawn in red, a user who is in the pose but has not been calibrated is colored in 
yellow and a calibrated user is green.

<H1>Step 2: Add the skeleton</H1>
Now we want to add the skeleton to control. To do this, we drag in the ConstructionWorkerPrefab from
the samples prefabs.<br>
For a short tutorial on how to create such a prefab, see @subpage OpenNICreatingASkeletonModelPrefabTutorial.
@note A second model is supplied under the MovableSoldier prefab. This is provided for convenience to 
have another example. The model here was imported from an older asset in a previous version of Unity.

<H1>Step 3: Control the skeleton</H1>
So far, we have added a model and attached a skeleton controller, or rather used a prefab in which this
was already the case, but we still don't see anything happening other than the skeleton assuming the 
calibration position. This is because the skeleton has not yet been attached to a player (i.e. we do 
not know which player controls it). <br>
Two elements are needed to solve this: A Users to Players mapper (see @ref NIUsersToPlayerMapper) and a 
Skeleton Manager (see @ref NISkeletonUsersManager). Or, we can simply use the @ref NISkeletonPrefab prefab.
<br><br>
Add a @ref NISkeletonPrefab to the scene and press play.<br>
Stand in front of the sensor, assume the calibration pose (the skeleton will stand in this pose)
and see how you control the soldier!

<H1>Step 4: Making it look better</H1>
While the sample is working fine now, it does not look pretty, so now we will improve its appearance:

<H2>Add a light</H2>
The scene right now has no lights, so let's add one. Add a new directional light game object and set its
rotation to 45, 315,45.

<H2>Add messages when we need to calibrate</H2>
While the skeleton controller positions the model to the calibration pose, it does not tell us we need to do it. 
To solve this, we use the CalibrationMessage prefab. Simply drag it to the scene.

<H2>Center things</H2>
At the moment the soldier is small, change the camera's Z position to -3 to zoom in.
<br><br>
Press play.<br>
We now have a skeleton controlled model that is waiting for us to calibrate and allows us to control its movements. 
We can also see which users are identified and their calibration state in the Radar Viewer and see 
the depth result from the sensor in the Depth Viewer.<br><br>

<H1>Enjoy your victory, you have completed your first sample!</H1>
Continue to the next sample @ref OpenNIMultipleSkeletonsTutorial or return to @ref OpenNISamplesOverview.
*/

/**
@page OpenNICreatingASkeletonModelPrefabTutorial Creating A model prefab to use with a skeleton controller tutorial
One of the most important things needed when using the skeleton controller is having a good model it can
control (see @ref AddingNewSkeletonModels "Adding New Skeleton Models" for more information). This 
tutorial aims at creating a prefab to be used (The ConstructionWorkerPrefab provided is the end result)
from the construction worker model provided in the Unity standard assets.
<H1>Step 1: Import the model</H1>
We want to import the character controller standard assets (go to Assets->Import package->character 
controller). While we can simply import everything, some of the assets there are not very useful for us.
We are only interested in the worker model. To import it, press the "none" button at the bottom left to
uncheck all assets and then check the checkbox to the left of the 
Standard Assets/Character Controllers/Sources/PrototypeCharacter asset (this will automatically check
all its children, which include the model itself: constructor.FBX and the required prerequisites such
as textures and materials).<br>
Drag the constructor model to the scene.<br>
<H1>Step 2: Fix the model up</H1>
First of all, we do not need the animations provided with it, so we can just remove this component.<br>
@note this severs the prefab connection.
The model must be standing in a "T" pose (i.e. standing erect with the arms extending 180 degrees to the
sides). The model we use is not 100% that. For this purpose we should fix up its orientation.<br>
As can be seen from the scene view, the hands of the construction workers are slanted down. We can 
fix that simply by changing the orientation of the relevant joints. Open up the child objects and do
the following changes:
- The orientation of Bip001 L Clavicle should be 0,270,180
- The orientation of Bip001 L UpperArm should be 0,0,0
- The orientation of Bip001 R Clavicle should be 0,90,180
- The orientation of Bip001 R UpperArm should be 0,0,0
@note some minor tweaks were made to other joints to round off the numbers...
<H1>Step 3: Connect the skeleton controller</H1>

Add a @ref NISkeletonController script to the model This script is responsible for 
controlling the skeleton.<br>
 
Look at the inspector for the @ref NISkeletonController component. 
It starts with a large number of public transforms (Head, Neck, Torso, Hands etc.) These are the 
transforms representing the joints we want to control. 
We should drag and drop the relevant game objects from the hierarchy to their position.<br>
@note In different implementations of OpenNI, different joints are supported. The joints attached here
are the ones supported in the supplied implementation!

<br>In our case we need to connect the joints to the child objects of the construction worker's game 
object according as follows:
- Head: Bip001 Head
- Torso: Bip001 Pelvis
- Left Shoulder: Bip001 L UpperArm
- Left Elbow: Bip001 L Forearm
- Left Hand: Bip001 L Hand
- Right Shoulder: Bip001 R UpperArm
- Right Elbow: Bip001 R Forearm
- Right Hand: Bip001 R Hand
- Left Hip: Bip001 L Thigh
- Left Knee: Bip001 L Calf
- Left Foot: Bip001 L Foot
- Right Hip: Bip001 R Thigh
- Right Knee: Bip001 R Calf
- Right Foot: Bip001 R Foot

Next the following three check boxes are used to see what is controlled:
- <b>"Update Root Positions?"</b> is aimed at schematic skeletons but are not good for models. Leave it 
  unchecked.
- <b> "Update Joint Positions?"</b> states whether the skeleton moves as a whole based on the position 
  of the torso. Check this box as we want the skeleton to move around.
- <b> "Update Joint Orientation?"</b> states whether the joints should change their orientation to match
  the skeleton. For rigged models, this is the important box to check so check it now.
<br>
<br>
<b>"Rotation Dampening"</b> is used to smooth the movement to avoid jerky changes. It should not be
changed from the default unless the movement is too jerky when rotating (and even then, use 
@ref OpenNISettingsManager's smoothing factor first).<br><br>

@b Scale is the scale of movement. The sensor receives the position data in mm. For models scaled in 
meters (as is the case here) this should be 0.001.

<H1>Step 4: Make it into a prefab</H1>
Congratulations you now have a model, all you need to do is make into a prefab (which is already available) 
Under ConstructionWorkerPrefab.
@note In order not to interfere with the process, the imported elements were moved in the package to 
Assets/OpenNI/Graphical assets/Construction Artwork/. This means one can simply import the standard 
assets package with no conflicts.
*/

/**
@page OpenNIMultipleSkeletonsTutorial Multiple Skeletons Sample
This sample expands the previous one by having two “models” controlled by two players. In addition,
it will introduce skeleton built based on game objects rather than fully rigged models (very useful 
for prototyping and debugging).

@note The assumption of each sample tutorial is that all relevant packages have been imported 
(including the engine, OpenNI, utilities and the sample package itself) and a new scene has been created.

@note It is assumed that @ref OpenNISingleSkeletonTutorial has already been completed. 
While it is not necessary for the completion of the sample, this tutorial will not repeat elements
explained before.

<H1>Step 1: Creating the basic objects</H1>
We will begin as we did with the @ref OpenNISingleSkeletonTutorial, by adding the @ref OpenNISettingsPrefab.
As before, the only change we make to the default is to turn off the image generator.<br>
We would also need to add the RadarViewer and DepthViewer as similar to before. In addition, since we 
might want to see the users, add an NIUserViewer prefab. This will show the user in the view as pixels of 
a specific color.

<H1>Step 2: Add the skeletons</H1>
Now we will add two skeleton prefabs: The FullBodySkeletonCubes prefab and the UpperBodySkeletonCubes
prefab. These two prefabs are very similar with one exception; the UpperBodySkeletonCubes prefab has
no legs.<br>
These two skeleton models are very similar to the one used in the previous sample but they are also
very different. Their main goal is to provide a quick prototyping and debugging tool. You can make
the game using one (or both) of these prefabs and later on replace them with a different skeleton based
on a fully textured, fully rigged model.<br>
The skeletons are built by manually creating game objects. These game objects are all placed together,
which is why, in the scene viewer, you will just see a heap of objects and nothing which looks like a 
skeleton. <br>
Since these skeletons are not rigged (they are just a hierarchy of game objects), we need to manually
place their joints. The skeleton controller can do this for us by getting information from the 
skeleton. Therefore, unlike the rigged skeleton from the previous sample, the skeleton controller
of these prefabs has the Update Joint Positions check box marked. <br>
Also note that the scale here is different because we are using schematic objects.<br>
Position the two skeleton in y=2.5 and x=+/-2 respectively<br><br>

If we wanted to create these prefabs from scratch, all we  had to do was create a hierarchy of game
objects, add the skeleton controller script to the base object, drag and
drop the various joints to their proper place and configure it the same as the prefab.<br>

<H1>Step 3: Control the skeleton</H1>
As before, we will add the @ref NISkeletonPrefab prefab. Unlike before, we will manually
match skeletons to players, as we want player 0 to control the full body skeleton and player 1 to 
control the upper body skeleton.<br>
For this purpose, after dragging in the @ref NISkeletonPrefab, we will change the number of elements 
under skeleton controllers in the @ref NISkeletonUsersManager script to 2. We will then drag
the full body skeleton to element 0 and the upper body skeleton to element 1.
If we press play now we can already control the two skeletons (two players are needed to control both).

<H1>Step 4: Adding debugging tools</H1>
In order to better understand how the skeleton behaves we would like to see the skeleton's limbs.<br>
Add the SkeletonLineDebugger prefab to the game. Attach it to the skeleton controller of UpperBodySkeleton. This prefab which implements @ref NISkeletonControllerLineDebugger can be used to see the 
lines connecting the joints. The lines colors will change based on the confidence of the points (which
helps us discover problems with poses where certain joints are hidden from the sensor). These lines are
seen by default in the scene view only. To see them in the game view, enable the gizmos option in the 
game window. @note The lines will not be seen in standalone builds.<br>
Add the SkeletonLineRenderer prefab to the game. Attach it to the skeleton controller of FullBodySkeleton.
This prefab which implements @ref NISkeletonControllerLineRenderer can be used to see the
lines connecting the joints very similar to @ref NISkeletonControllerLineDebugger. The main difference
is that this script uses the LineRenderer component of Unity. This means the following:
- It is possible to change way the lines look (by changing the script).
- The lines will be seen in standalone builds.
- There is a larger overhead for this (drawing the lines,adding game objects to the hierarchy in real time
  for the line renderers).

<H1>Step 5: Making it look better</H1>
In this last step, we make the same modifications as in the previous tutorial:
Add a light and add messages when we need to calibrate (we already positioned the skeletons in a
previous step).<br>
Press play.<br>
Now have two skeletons that are waiting for us to calibrate. We can have two different players
take control of the two skeletons and move them. The skeletons are schematic. One of them shows the
full body and the other the upper body only.<br>
We can also see which users are identified in the Users Viewer, their calibration state in the Radar 
Viewer and the Depth in the Depth Viewer.<br><br>

<H1>Enjoy your victory, you have completed the sample!</H1>
Continue to the next sample @ref OpenNIInputControlTutorial or return to @ref OpenNISamplesOverview.
*/



/**
@page OpenNIInputControlTutorial Control a cursor using the input (skeleton-based)

This sample shows how to define Axes using the NIInput class and control a simple cursor with it.

@note The assumption of each sample tutorial is that all relevant packages have been imported 
(including the engine, OpenNI, utilities and the sample package itself) and a new scene has been created.

<H1>Step 1: Setting the scene</H1>
As this is the third sample already, we will quickly move through the basic setting of the
scene, which is very similar to previous samples:
- Drag in an @ref OpenNISettingsPrefab and uncheck the "Use Image" check box. We will also 
  change the smoothing factor of the skeleton to 0.95. This will make the movement much more stable
  and much smoother, which is good when we want to track a specific point.
- Add a Radar Viewer prefab.
- Create an empty game object (name it PlayerMapper) and attach a @ref NIUsersToPlayerMapper 
  (we do not use the @ref NISkeletonPrefab because we don't really have skeleton controllers,
  we are only interested in the player ID).


<H1>Step 2: Add an input</H1>
We need to add an input controller. For this we will use the @ref InputControllerPrefab. This 
will provide us with a capability to define axes similar to the regular Unity's InputManager and 
access them in a way very similar to the use of the Input class (for more information, see 
@ref NIInputConcept). To that class, we need to add sources of input, including point trackers and
gestures (see @ref OpenNIPointTrackerConcept and @ref OpenNIGestureConcept for more 
information). 
 
<H1>Step 3: Add the skeleton hand tracker and configure the input</H1>
The @ref InputControllerPrefab contains a gesture manager and a hand tracker manager but they do not
contain any trackers in them. For our example, we will only move a cursor around so we don't really 
need any gestures, but we do need to decide what to track. Therefore, we will create a new game object
and attach the @ref NISkeletonTracker script to it. This script enables us to track a skeleton joint.
We will set the player num to 0 (we want to track the first player) and we will change the Joint to
use from inValid to rightHand so that we can track the right hand.<br>
While we have the tracker, we have not yet attached it to the manager, so let's do that now. Go to the
HandsManager child of the InputController game object and change the tracker's array size to 1. Drag 
the skeleton tracker game object there.<br>
For simplicity, we are going to use the Horizontal and Vertical axes (which are defined by default) to
control the 'x' and 'y' movement on the screen.
Let's configure these axes:
- We want to move the cursor along the screen. For this purpose we need the axis to be configured to
  normalize the values. We will change the sensitivity to 0.5 in both axes and will set the Horizontal 
  normalization to 320 and the Vertical one to 240. This will provide a value between -0.5 to 0.5 in
  each axis. It will also provide us a movement range of 32cm in each direction of the x axis and 
  24 cm in each direction of the y axis (this is aimed so we can cover the whole screen without
  having to stretch out too much but provide sufficient resolution so that not every small change
  would cause us to jump).
- Change the hand movement type to HandMovementFromStartingPoint. This is used to
  provide a continuous experience, i.e. when we start controlling, in the calibration pose, that
  position of the hand is set as the middle of the move area. This means that the movement is defined
  in an area 64cm wide, 48 cm tall centered on the starting position (where the calibration occurred)
  of the right hand.
- We will make sure the horizontal Axis choice is xAxis and the Vertical Axis choice is yAxis.
- Lastly we will change the tracker to use to "skeleton tracker for player 0 tracking joint 
  RightHand" this is the text defined for the skeleton tracker we added to the manager.

<H1>Step 4: Add the cursor</H1>
Create an empty game object (Name it cursor) and attach the @ref NIHandPositionInputPositionOnly script to it.
This script positions a box (using OnGUI) on a position based on the GetAxis result. Since we
get a value between -0.5 and 0.5, all we need to do is add 0.5 to the result and multiply it by
Screen.Width/Screen.Height to position the cursor correctly on the screen.

<H1>Step 5: Add the calibration message</H1>
Since we only start when a user is calibrated, we should give the user instruction to do so. Add the
CalibrationMessage prefab to the scene.

<br><br> 
Press play.<br>
We can now do the calibration pose, and then make the cursor move (in the 2D world) by moving
our hand.<br><br>

<H1>Enjoy your victory, you have completed the sample!</H1>
Continue to the next sample @ref OpenNINIGUITutorial or return to @ref OpenNISamplesOverview.
*/

/**
@page OpenNINIGUITutorial NIGUI (skeleton-based)

This sample shows us how to create a GUI using the NIGUI tools. We will be seeing various GUI
elements and we will be able to control them by using the right hand of the first player. In order
to simulate a left mouse click, e.g. to click on a button, we will use the timed steady gesture.
This means that when the right hand does not move, a counter will begin and a click will occur after
several seconds.

@note The assumption of each sample tutorial is that all relevant packages have been imported 
(including the engine, OpenNI, utilities and the sample package itself) and a new scene has been created.

@note We assume the previous sample was followed, as input etc. is not explained again.

<H1>Step 1: Add the basic objects:</H1> 
As we did previously, we will add the basic elements. As most of these were explained before, we will 
simply list them and their configuration
- Drag in an @ref OpenNISettingsPrefab and uncheck the "Use Image" check box. We will also 
  change the smoothing factor of the skeleton to 0.95. This will make the movement much more stable
  and much smoother, which is good when we want to track a specific point.
- Add a Radar Viewer prefab.
- Create an empty game object (name it PlayerMapper) and attach a @ref NIUsersToPlayerMapper 
  (we do not use the @ref NISkeletonPrefab because we don't really have skeleton controllers,
  we are only interested in the player ID).

<H1>step 2: Create and configure the input:</H1> 
We will be using the input to control the GUI. We will do so by tracking the position of the right
hand of the first player. We will click using the steady gesture (for which we will set a timed click).

<H2>Create the tracking and gesture objects and the input object</H2>
- Create an empty game object (name it PointTracker) and attach the @ref NISkeletonTracker script 
  to it.
    - Set the player num to 0 
    - Set the joint to be the right hand.
- Create an empty game object (name it GestureFactory) and drag the NISteadySkeletonGestureFactory to
  it. The gesture factory is used to create the gesture (see @ref OpenNIGestureConcept. For more
  details on the settings below, see @ref NISteadySkeletonGestureFactory).
    - Set the Time to click to 2 seconds (this is how long we need to wait)
    - Set the Time to reset to 10000 (we don't want to reset)
    - Set the Steady Test time to 0.5 (we check the steady over 0.5 seconds)
    - Set the SteadyStdSqrThreshold to 2 
    - Set the UnsteadyStdSqrThreshold to 8 
    - Set the MaxMoveFromFirstSteady to 40
- Drag a new Input controller prefab into the scene
    - Go to the HandsManager child of the InputController game object and change the tracker's 
      array size to 1. Drag the PointTracker game object there.<br>
    - Go to the GestureManager child of the InputController game object and change the gesture's 
      array size to 1. Drag the GestureFactory game object there.<br>

<H2>Configure the input </H2>
We will be using the default cursor (see below), which forces us to define the following axes:
- NIGUI_X to move along the x axis
- NIGUI_Y to move along the y axis
- NIGUI_CLICK to click

For this purpose, we need to perform the following configurations:
- NIGUI_X
    - Name: NIGUI_X
    - NIInput axis?: select the check box (this will NOT be defined in InputManager)
    - Gesture: None
    - Dead: 0.001
    - Sensitivity: 0.5
    - Invert: not checked
    - Type: HandMovementFromStartingPoint
    - Normalization: 320
    - Axis: xAxis
    - Tracker to use: "skeleton tracker for player 0 tracking joint RightHand"
- NIGUI_Y
    - Name: NIGUI_Y
    - NIInput axis?: select the check box (this will NOT be defined in InputManager)
    - Gesture: None
    - Dead: 0.001
    - Sensitivity: 0.5
    - Invert: not checked
    - Type: HandMovementFromStartingPoint
    - Normalization: 240
    - Axis: yAxis
    - Tracker to use: "skeleton tracker for player 0 tracking joint RightHand"
- NIGUI_CLICK
    - Name: NIGUI_CLICK
    - NIInput axis?: select the check box (this will NOT be defined in InputManager)
    - Gesture: SteadySkeletonGesture
    - Dead: 0.001
    - Sensitivity: 1
    - Invert: not checked
    - Type: Gesture
    - Normalization: -1
    - Axis: xAxis
    - Tracker to use: "skeleton tracker for player 0 tracking joint RightHand"

<H1>Step 3: Add a %NIGUI prefab</H1>
Since we wish to showcase the GUI we need a GUI system. To do this we will use %NIGUI 
(see @ref NIGUIConcepts). To quickly and easily add it to our scene without too
much work, we will use the default cursor that is available in @ref NIGUIPrefab.
We have already defined the relevant axes in NIInput for the cursor to work so we already
have a fully functioning %NIGUI.

<H1>Step 4: Add the GUI example script</H1>
While we now have a functioning GUI system, we still need to create the actual buttons, scrollbars
etc. To do this, we will create an empty game object (name it ExampleGUI). We will attach the
@ref GUIExample script to it. This script uses the various GUI elements to show buttons,
repeat buttons, scroll area (with scroll bars), sliders etc.

<H1>step 5: add the calibration message</H1>
Since we only start when a user is calibrated, we should give the user instruction to do so. Add the
CalibrationMessage prefab to the scene.<br><br>

Press play.<br>
We can now click on the various buttons, change the sliders and interact with the GUI.
@note 
- In order to click, place the cursor in the relevant position and leave your hand steady there. 
The cursor will slowly change color. When it is fully green, it will click. If the hand is not steady 
enough, the click will not occur.
- When using sliders (and scroll bars), if one clicks on the actual scroll thumb, the scrollbar/
slider will move with the movement of the hand until a new click is used.
- Some of the labels will react to clicking on buttons, sliders etc. to provide feedback.

<H1>Enjoy your victory, you have completed the sample!</H1>
Continue to the next sample @ref OpenNISimpleGameTutorial or return to @ref OpenNISamplesOverview.
*/


/**
@page OpenNISimpleGameTutorial A simple game example

In this sample, we will create a very simple game using everything we have learned so far. The game will
include a skeleton (build from the cubes). In addition, balls will fall from the sky in random 
intervals and random places. The user will be able to hit the balls, sending them flying. A score
board will count the number of balls that have fallen and how many the user hits. The user will be able to
strike a pose that will stop the game and show two buttons: to continue or to quit.

@note The assumption of each sample tutorial is that all relevant packages have been imported 
(including the engine, OpenNI, utilities and the sample package itself) and a new scene has been created.

<br>
@note We assume the previous samples were completed as no explanation will be given.

<H1>Step 1: Creating the basic objects - skeleton</H1>

We will begin as we did previously, by adding a skeleton to the scene. 
- Add a @ref OpenNISettingsPrefab to the scene and turn off the image generator.
- Add a Radar viewer
- Add the FullBodySkeletonCubes prefab
- Add the @ref NISkeletonPrefab prefab. 

Pressing play now would allow us to see the users in the Radar viewer, do a calibration and control
the skeleton.<br>

<H1>Step 2: Add the balls creation</H1>
We want to start dropping balls from the sky. Create an empty game object (name in BallCreator) and
attach the @ref BallCreator script to it. This script will create the balls. <br>
Drag and drop the BallPrefab prefab to the "prefab" variable. This prefab holds the balls we will be
creating. Also drag and drop the skeleton to the "where" variable (this tells us around what to create
the balls).<br>
The ball creator is a simple script that creates the balls, counts them and shows (through the OnGUI
method) the current score. 

The BallPrefab is a simple sphere rigid body aimed at colliding with the joints. The @ref NICollider 
script was added to it which basically applies force based on the collision.<br><br>

Press play<br>
We can see balls falling from the sky but if we hit them, nothing happens. The reason for this is that 
the joints are moved by the skeleton controller, which positions them in the correct place. Their speed 
is 0 at this point (we set position and not speed). This means that the collision will do nothing.
On the other hand, we would expect the @ref NICollider script to handle this because it has an
OnTriggerEnter method to handle the collision. The problem is that NICollider changes the balls
position by applying a force in the direction of the collider's (the joint) movement. But our joints
do not have speed in them. For this purpose, we will use the @ref JointSpeeder script.<br>
The JointSpeed script (when attached to a joint) is responsible for calculating its speed based on its
position. Add this script to every joint in the skeleton (allowing all joints
to hit the balls). @note This will break the prefab connection of the skeleton.<br>

Pressing play now will allow us to hit the balls and they will fly away.

<H1>Step 3: Add an exit gesture</H1> 
We want the player to be able to end the game. We need to identify an exit gesture
and when that occurs, to show the user a GUI allowing them to continue or exit.<br>
Unfortunately, we do not have a proper gesture defined. This provides an excellent opportunity to create a 
new gesture in the @subpage OpenNIGestureTutorial.<br>
Once the new exit gesture is ready, we can use it in the following steps.

<H1>step 4: Add the GUI</H1> 
Although we have now created an exit gesture, we have not yet started to use it. 

<H2>Create the GUI hierarchy</H2>
We will first create an empty game object and name it GUI. All game objects below
should be children of that game object (this is used to clean up the hierarchy and not for any code
reason).

- Create an empty game object (name it Gestures) and create two empty child game objects for it. 
Name them SteadyGesture and ExitGesture.
- Attach the NISteadySkeletonGesture script to the SteadyGesture game object. As with the 
@ref OpenNINIGUITutorial sample, we will set the parameters as follows:
    - Set the Time to click to 2 seconds (this is how long we need to wait)
    - Set the Time to reset to 10000 (we don't want to reset)
    - Set the Steady Test time to 0.5 (we check the steady over 0.5 seconds)
    - Set the SteadyStdSqrThreshold to 2 
    - Set the UnsteadyStdSqrThreshold to 8 
    - Set the MaxMoveFromFirstSteady to 40
- Attach the @ref NIExitPoseGestureFactory to the ExitGesture game object. Set its parameters as 
  follows:
    - Set the Time to hold pos to 1 seconds (this is how long we need to wait)
    - Set the max move speed to 100 (we allow movement of up to 10 cm per second overall)
    - Set the Time to save points to 0.5 (we will consider points along 0.5 second)
    - Set the Angle tolerance to 20 (this means that we don't really need 45 degrees for each 
      arm, but rather that 20-65 degrees is suitable).
    - @note The exit gesture here is a very simple example of how to create a gesture. It is @b NOT meant 
      as a robust solution!
    - @note The exit gesture is built upon the right hand tracker, but it is in fact irrelevant which
      joint is used (only the player matters because the whole skeleton is used to detect it).
- Create an empty game object, name it PointTracker and attach the NISkeletonTracker script to it. Set
the player number to 0 and the joint to the right hand.
- Add an NIInput prefab to the scene. Attach the point tracker and two gestures to its managers.
- Add a NIGUI prefab to the scene.

<H2>Configure the input </H2>
We will be using the default cursor (see below), which forces us to define the following axes:
- NIGUI_X to move along the x axis
- NIGUI_Y to move along the y axis
- NIGUI_CLICK to click

Now we need to perform the following configurations:
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
    - Tracker to use: "skeleton tracker for player 0 tracking joint RightHand"
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
    - Tracker to use: "skeleton tracker for player 0 tracking joint RightHand"
- NIGUI_CLICK
    - Name: NIGUI_CLICK
    - NIInput axis?:Select the check box (this will NOT be defined in InputManager)
    - Gesture: SteadySkeletonGesture
    - Dead: 0.001
    - Sensitivity: 1
    - Invert: not checked
    - Type: Gesture
    - Normalization: -1
    - Axis: xAxis
    - Tracker to use: "skeleton tracker for player 0 tracking joint RightHand"

In addition, we want to detect the exit gesture based on the input as well, so we will create a fourth
axis for that:

- SkeletonSwitch
    - Name: SkeletonSwitch
    - NIInput axis?: Select the check box (this will NOT be defined in InputManager)
    - Gesture: ExitPoseGesture
    - Dead: 0.001
    - Sensitivity: 1
    - Invert: not checked
    - Type: Gesture
    - Normalization: -1
    - Axis: xAxis
    - Tracker to use: "skeleton tracker for player 0 tracking joint RightHand"

<H1>Step 4: Add the GUI control script</H1>
While we have configured the GUI, we have not yet added the GUI itself. Attach the @ref SkeletonGuiControl
script to the GUI game object and set its image variable to be the exit_sample image (in the Graphical 
assets/Images directory). This script keeps tabs on the mode we are in 
(@ref SkeletonGuiControl.SkeletonGUIModes). We begin in SkeletonMode, where the skeleton is seen and the
GUI is deactivated. In this mode, @ref SkeletonGuiControl simply tries to locate the exit pose. 
Once the pose is detected, a GUI telling us to hold the pose will appear and after a little time we will
move to GUIMode in which the skeleton is deactivated (can't be seen) and the GUI appears.<br><br>
Pressing play now will provide us with all the functionality of the game. We can see the balls, hit 
them, keep score and when we stand in the exit pose (hands crossed in front of us) for a couple
of seconds, a GUI will appear allowing us to return to the game or exit.
@note Because of the use of Application.Quit, exiting will only work in the standalone version. Inside 
the editor it will do nothing.

<H1>Step 5: Improve the way it looks</H1>
We now have a functional game but we should make it look better:
- Set the camera options:
    - Position the camera at 0,0,-10
    - Set the camera's background to brown (R=160, G=145,B=125,A=255)
    - Add the smoothFollow script. Set its parameters as:
        - Target: drag & drop the skeleton controller main object
        - Distance: 7
        - Height: 0
        - Height: 2
        - Height: 3
- Add a floor
    - Create a new plane game object. Rename it to Floor.
    - Position it at 0,0,0 and scale it to 10, 10, 10
    - On its mesh renderer, drag in the Floor material (Available from General Assets->Samples Assets->Floor assets->Materials
    - @note We do not correct the skeleton's position relative to the floor. 
      @see @subpage PinningToTheFloor for more information
- Add a directional light
    - Set the direction to: 45,300,45
- Drag and add the CalibrationMessage prefab to the scene.

Press play.<br>
You now have a fully functioning (albeit simple) working game. After calibration, the user controls
skeleton movement. Balls drop from the sky and the user needs to hit them. A score board is used to see
the current score (how many balls were hit out of how many). The user can strike a pose (the exit pose),
which will stop the game and show two buttons; to continue or to quit.

<H1>Enjoy your victory, you have completed the sample! Now you know how to use the OpenNI module</H1>
return to @ref OpenNISamplesOverview.
*/


/**
@page OpenNIGestureTutorial Adding a New Gesture Example
<H1>Overview</H1>
One of the most common extensions to this package is the addition of new trackers (and mostly gesture trackers).
In the future, gesture packs will probably be added.<br>
For this purpose, it is important to understand how to add gestures and new trackers. For simplicity, 
we will focus on creating a new gesture.<br>
It is not uncommon for a game to have two modes: The first is the actual playing of the game (in which we 
control the skeleton, move around and actually play). The second is a UI mode in which we are presented 
with a menu (resume, exit, save, load etc.).<br>
In order to access the menu in the middle of a game session, a special movement must be made. This is commonly known as the exit pose,
as it is also used to exit the program. The exit pose recommended by OpenNI Arena is to stand erect with
the hands crossed in front of the body. For our purposes, we will use the following, simplistic 
implementation.<br>
We will track the right and left hands and elbows and we will consider the user to be in the relevant pose once the 
following conditions are met:
- There is very little movement (very low average speed).
- The right hand is angled around 45 degrees toward the left
- The left hand is angled around 45 degrees toward the right
- The right hand is to the right from the left
- Both hands are above their opposite elbows.

@note This is a very simplistic implementation, useful mainly for demonstration and explanation and not ready for 
production. It might catch other gestures that are not the exit pose and may miss the pose on certain angles
(where the hands and elbows are too close to the body for example). <br>
For our purposes, however, this is enough since we do not have any conflicting poses in our sample.<br>

To implement it, we will need to implement two classes:
- @ref NIExitPoseDetector - This will perform the actual detection of the exit pose
- @ref NIExitPoseGestureFactory - This is used to create and initialize the detector.

<H1>Creating the Detector</H1>

Implementing a gesture detector is relatively easy, all we need to do is extend the @ref NIGestureTracker
abstract class.<br>
In order to track the positions, we will use the @ref NITimedPointSpeedListUtility class. We will add the
position of the hands and elbows to objects of this class so that we have an average position and speed
for each of them.<br>

@note The @ref NIExitPoseDetector is well documented (both at the API level in @ref NIExitPoseDetector and inside the 
methods themselves to understand how they work). Because of this, we will not explain every detail of the 
implementation but instead will stay at a high level and leave the reader to delve in.

The implementation of the @ref NIGestureTracker abstract class can be divided into the following main parts:

<H2>Initializing, updating and releasing</H2>
<H3>Initialization</H3>
@ref NIExitPoseDetector (as a child of @ref NIGestureTracker) is @b NOT a mono-behavior object. Instead it is created
using "new" in the @ref NIExitPoseGestureFactory. For this purpose some of its initialization is done using a 
constructor (see @ref NIExitPoseDetector.NIExitPoseDetector "NIExitPoseDetector").<br>

@ref NIGestureTracker provides various initialization methods. While @ref NIGestureTracker.Init "Init" is the one
often visible to other methods, @ref NIGestureTracker.InternalInit "InternalInit" is the one to be overridden by the implementer.
In our case, we need the hand tracker to be a skeleton and therefore we check that this is true as part of the
initialization.<br><br>



<H3>Updating</H3>
@ref NIGestureTracker.UpdateFrame "UpdateFrame" is called every frame (by the @ref NIGestureFactory object). This is used to update
the tracking information. In our case, this is used to fill up the points of where the relevant joints (hands, elbows)
are located and to analyze them to see if we are in the pose. Furthermore, it is assumed that the pose is considered
"triggered" only when we have held it for a while. This is also checked in the updating (and when relevant an event
is triggered).
<H3>Releasing</H3>
The @ref NIGestureTracker.ReleaseGesture "ReleaseGesture" method is used to release everything we need. 
In this case, this is trivial.

<H2>Gesture detection feedback</H2>

@ref NIGestureTracker provides several options to query for the state of the gesture:
- @ref NIGestureTracker.GetLastGestureTime "GetLastGestureTime" - tells us the time (in Time.time) the gesture 
  was last detected.
- @ref NIGestureTracker.GetLastGestureFrame "GetLastGestureFrame" - tells us the frame (in Time.frameCount) the
  gesture was last detected.
- @ref NIGestureTracker.GestureInProgress "GestureInProgress" - tells us if the gesture is in progress and how it is 
  progressing. 
  @note While we have an internal detection of the pose (i.e. telling us that the user is holding the exit pose) this
  is @b NOT a detection of the gesture. For many gestures, the behavior is not a "detected"/"not detected" result but
  rather has a progress. For our exit pose, once we detect the pose for the first time, the user has to hold that pose
  for a certain time. For this reason, the result of the detection is a value between 0 (not detected) and 1 (detected
  and held for the time we need). A value in the middle represents the portion of the holding time (so if we need to
  hold the pose for 2 seconds and 1 second has past then the value would be 0.5).<br>
  Other gestures might have various meaning for this flow. For example, if we have a gesture of sending the arms in a 
  "T" pose, we might start detecting when the arms are at 80 degrees from the body and slowly rise in value to the 
  full effect at 90 degrees.
- Using events - If one wants to know immediately when a gesture is detected (in our case held for the relevant time)
  events should be used. For this purpose, @ref NIGestureTracker provides us with an event handler 
  (@ref NIGestureTracker.m_gestureEventHandler "m_gestureEventHandler" using the delegate 
  @ref NIGestureTracker.GestureEventHandler "GestureEventHandler") we can use to register or unregister the event to.

<H2>Implementing the detection</H2>
The logic of detecting the actual pose is implemented by getting the positions of the joints (hands, elbows) in every
frame (see @ref NIExitPoseDetector.FillPoints "FillPoints"). A test is then performed to see if we are in
pose (inside @ref NIExitPoseDetector.TestExitPose "TestExitPose", which also checks the speed in
@ref NIExitPoseDetector.IsSteady "IsSteady"). The @ref NIExitPoseDetector.UpdateFrame "UpdateFrame"
method calls all these methods and does the book keeping to check when the pose was first detected. 
When enough time has passed it calls NIExitPoseDetector.InternalFireDetectEvent "InternalFireDetectEvent" (which calls
@ref NIGestureTracker.DetectGesture "DetectGesture") to fire the event.

@note In our example, we use only the player number from the hand tracker. On other gesture trackers (such as 
the @ref NISteadySkeletonHandDetector) we track the actual point.

<H1>Creating the factory</H1>
The factory creation is even simpler than the detector creation. The factory extends @ref NIGestureFactory, which 
is responsible for the update method (to call update frame) and to provide an inspector to configure everything. 
For this reason all @ref NIExitPoseGestureFactory needs to do is add the public variables
(to be configured in the inspector) and create a new @ref NIExitPoseDetector inside 
@ref NIExitPoseGestureFactory.GetNewTrackerObject "GetNewTrackerObject" with the relevant constructor parameters, 
and then return it.<br>
We also implement the @ref NIExitPoseGestureFactory.GetGestureType "GetGestureType" method that provides us with a 
string. This string is used by @ref NIGestureManager to provide inspectors, such as the @ref NIInput Inspector 
(@ref NIInputInspector), a name to enable the user to distinguish between gestures.
*/
