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
// it is aimed at openNI


/** 
@page OpenNILicense OpenNI Unity Toolkit License
<H1>License</H1>
OpenNI Unity Toolkit - Copyright &copy; 2011 PrimeSense Ltd.<br><br>

The OpenNI Unity toolkit is free software: you can redistribute it and/or modify  
it under the terms of the GNU Lesser General Public License as published 
by the Free Software Foundation, either version 3 of the License, or  
(at your option) any later version.<br>
                                                                       
The OpenNI Unity toolkit is distributed in the hope that it will be useful,             
but WITHOUT ANY WARRANTY; without even the implied warranty of        
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the          
GNU Lesser General Public License for more details.<br>
                                                                        
You should have received a copy of the GNU Lesser General Public License 
along with OpenNI. If not, see <http://www.gnu.org/licenses/>.          
*/

/**
@page OpenNIFramework OpenNI Framework

The OpenNI organization is an industry-led, not-for-profit organization formed to certify and promote the 
compatibility and interoperability of Natural Interaction&reg; (NI) devices, applications and middleware.<br><br>

As a first step towards this goal, the organization has made available an open source framework – the OpenNI
framework – that provides an application programming interface (API) for writing applications utilizing natural
interaction. This API covers communication with both low-level devices (e.g. vision and audio sensors), as well 
as high-level middleware solutions (e.g. for visual tracking using computer vision).<br>

The API framework itself is also sometimes referred to as OpenNI. <br><br>
For more detailed information and much more, see <a href="http://www.openni.org/">http://www.openni.org/</a> 
*/


/** 
@page OpenNISettingsPrefab OpenNISettings Prefab

The OpenNISettings prefab is aimed at providing basic configuration and settings for OpenNI 
capabilities and one place for all initialization. It contains three game objects:

<H1>Logger</H1>
The logger is used to filter out messages created by the various OpenNI elements. It filters based
on which categories and sources to show. Nominally, all options are marked, but if one or more category or 
source is not interesting, you can simply uncheck them. The messages will show up in the console (using 
Unity's Debug.Log method).<br><br>
The Logger is based on the @ref NIEventLogger script

<H1>Query</H1>
OpenNI supports many implementations and a user might have various ones installed. In order to ensure
that everything required by the game is supported, a query can be used. To build a new query,
the developer must inherit the NIQuery class, and in the construction, build a new openNI query that 
limits the implementations that can be used. For most purposes however, this is not needed.<br><br>
For simplicity, two queries are supplied in the implementation: 
- @ref NIQuery, which does not filter anything (it is used by default and is probably good 
enough for most games).
- @ref NIPrimeSenseQuery, which limits the nodes to Primesense&trade; implementation, which is the only one 
that has been tested.

<H1>OpenNI Configuration</H1>

<H2> Overview</H2>
The OpenNI configuration child object is the basic object used to initialize and configure OpenNI. 
While it is not mandatory, it is designed to be used as a singleton and the internal structures it uses
are actual singletons (see @subpage SingletonImplementationOverview for more information). <br>
The object uses the @ref OpenNISettingsManager script to provide the functionality and uses a custom 
inspector (@ref NIOpenNISettingsManagerInspector) to provide easy access to all relevant data.
The  @ref OpenNISettingsManager class also provides easy programmatic tools to query and configure
the internal OpenNI objects. 

<H2>Custom Inspector</H2>

The custom inspector has two modes:
- Edit mode: This mode is active during editing. It enables changing the overall behavior of the base
  objects
- Run mode: This mode is active while the program is running. It limits the changes that can be made but 
  shows the state of various elements.

While in running mode, it provides the following customization options:
- Mirror behavior – sets if the image and depth data is mirrored or not (default is true).
- XML file – if specific nodes should be loaded
  - This is relevant mainly for OpenNI experts who want to load specifically created nodes. For most purposes, leave this empty.
- Use image generator? – If the game requires access to the RGB camera data, this should be checked.
  Otherwise, this should be unchecked to save CPU.
- Use user generator? – If the game uses the skeleton or multiple users (which is the case for most games), 
  this should be checked. Otherwise, this should be unchecked to save CPU.
- Smoothing factor – a number between 0 and 1 that sets the smoothness of skeleton movement. 
  - A value of 0 means no smoothing is performed
  - A value of 1 means smoothing is so strong that almost no movement occurs
  - A good value is 0.5 for most games.
  - For tracking the hands, for example, for GUI, a value of 0.9-0.95 is useful. Note however,
    that under these factors, the skeleton will be a little slow to respond.
- Logger and query (the objects defined above, which are by default connected in the prefab).
<br><br>
 
When running, the inspector shows only values that can be changed while running. In addition, it will 
show all current users for the users and skeleton view, including extra information about them, such
as userID, uniqueID, calibration state and center of mass.

*/

/** 
@page NISkeletonPrefab NISkeletonPrefab

This prefab is a quick way to add a @ref NIUsersToPlayerMapper and a @ref NISkeletonUsersManager, which
are used to control skeletons in the scene.

<H1>Mapping Users to Players</H1>
@ref NIUsersToPlayerMapper is an object that attempts to figure out which user moving in the scene is
which player. <br>
It does this in a relatively simple way: the first player to calibrate is player 0, the second is 
player 1 and so on. If a player is lost, the next user to calibrate will get the lowest free number.<br>
@note When a user moves out of the sensor's field of view, it is "exited" but not lost. When
the player returns, the sensor should automatically figure out it is the same user as before and 
not lose its calibration data. 

<H1>Mapping Skeletons to Players</H1>
While @ref NIUsersToPlayerMapper is responsible for mapping users to players, 
@ref NISkeletonUsersManager is responsible for mapping players to skeleton controllers. The
@ref NIUsersToPlayerMapper script links to the mapper. You should drag the relevant object, however,
in practice we can leave it as "None" since the manager will find the object itself (assuming just one
exists and only one is used, which should generally be the case). For the prefab, this is already 
linked.<br>
The more important part is a list of skeleton controllers. This array contains the order of the 
controllers, so player 0 will control the controller at index 0, player 1 will control the controller
at index 1 and so on.<br>
To fill the list, we can use one of two methods:
- Set the number of elements and drag and drop each of the skeletons controllers to the relevant
index (i.e. the specific order of which skeleton is controlled by which player).
- Leave it with 0 elements. In this case, in the game the list will be automatically populated by
all skeleton controllers in the scene.  Their order though cannot be
guaranteed. This method is good when the order is not important (e.g. when there is just one controller).
*/

/**
@page SingletonImplementationOverview Internal Singleton Implementation
The internal implementation of initialization and configuration uses the singleton pattern. This is 
because we aim to consolidate the use of OpenNI to a single entry point whenever possible.<br>
For this purpose the following classes are used:
- @ref NIContext, which handles the basic context everything relates to
- @ref NIImage, which handles the RGB image (if needed)
- @ref NIUserAndSkeleton, which handles the identification of users and basic skeleton capabilities

These are abstracted by the use of @ref OpenNISettingsManager.
@note Since these are singletons, all initialization and release done by everyone affects the same 
class. Additionally, when loading a level, a specific release/reinitialize is required if you want to
start from scratch. This is done automatically by @ref OpenNISettingsManager. If you would rather
keep the information, such as the calibrated skeletons, user to player mapping etc., then all 
relevant objects, such as @ref OpenNISettingsManager, must be created on the first level and 
DontDestroyOnLoad or similar solutions used.
*/



/**
@page InputControllerPrefab InputController Prefab
The InputController prefab is simply an Input with a GestureManager and PointTrackerManager already
attached. 
@see @ref NIInputConcept
@see @ref OpenNIPointTrackerConcept
@see @ref OpenNIGestureConcept
@note The prefab does not include any point trackers or gestures attached to it. That still has to
be added to the scene by the developer.
 */

/**
@page NIInputConcept NIInput Concepts

<H1>Overview</H1>
Unity supports an Input Class and an InputManager to configure it. The Input Class and InputManager 
do not support Natural Interactions, their input is limited to more traditional input facilities, such as 
keyboard, mouse and joysticks. The purpose of the NIInput class is to extend these capabilities
to Natural Interactions input. <br>
The implementation of this is the creation of a new NIInput class, which enables us to use
an object of that class instead of the regular input. 
<br><br>
The basic use of %NIInput is simple: 
- Add an NIInput game object to the scene
- Configure it in the custom inspector
- Drag and drop it to the various objects that plan to use it.
- Inside the scripts that use it, use it as if it was the regular Input class.<br>

@note Unlike the Input class, %NIInput is NOT a static variable and its configuration is done using
a custom inspector (rather than a global option as InputManager). This both simplifies the implementation and 
provides a little more flexibility. Nominally, exactly one %NIInput game object should be added to the 
scene but if you want to control several input schemes, you can add several %NIInput objects to the scene
and change the reference to %NIInput inherent in objects that use it.


<H2>Configuring an Input</H2>
The custom inspector of the %NIInput class is designed to be very similar to the  
<A HREF="http://unity3d.com/support/documentation/Components/class-InputManager.html"> 
InputManager </A> provided by Unity. Anyone who wants to define an axis can do one of the
following:
- Go to the regular InputManager and define an axis; this will affect regular input only
- Go to the custom inspector and define an axis; this will affect Natural Interactions input only
- Redefine the same axis in both to allow both regular and Natural Interactions input.<br>

The configuration of %NIInput's custom inspector looks very similar to the InputManager 
(see @ref NIInputInspector for more details). The important thing to remember is that when using
GetAxis and GetAxisRaw, the value received will be the one with the highest absolute value between
all implementations of the same axis name (similar to the definition in InputManager but extended to
include %NIInput axes as well).


<H2>Using the Input</H2>

<H3>Extending Unity's Input Class</H3>
Using %NIInput is very similar to using the regular <A HREF="http://unity3d.com/support/documentation/Manual/Input.html"> 
Input</A> provided by Unity and actually extends its <A HREF="http://unity3d.com/support/documentation/ScriptReference/Input.html"> 
functionality</A>. To use it simply call the GetAxis(axisName) method and use the results exactly as
is used on traditional input sources. Furthermore, %NIInput encompasses all the functionality of the 
Input class. What this means is that calling Input.GetAxis(axisName) is already done inside the 
GetAxis implementation. In fact, NIInput contains just about any Input methods and properties you 
need, including ones that have no relation to Natural Interactions (such as GetKeyUp method). 
ensures that, you can replace Input with the NIInput object wherever Input is regularly
used.
<br>

<H3>Special %NIInput functionality</H3>
The regular Input class has some utility methods to get direct information on specific input types,
such as GetKeyUp to know when a keyboard key was released. Similar utilities are provided for natural
interface (and specifically for gestures).<br>
When using GetAxis, the value for the gesture will be non-zero, from the time the gesture was detected
and up to a frame after it was finished. This can be divided over several frames. In fact, gestures 
are considered detected for 2 frames; the current and the last one (to avoid missing a frame!). This
is because the gesture can occur at any time during the frame. It is the responsibility of the 
caller to decide what happens if GetAxis is used.<br>
For developers who want to make sure a gesture is detected only once, two options exist:
- Use the @ref NIInput::HasFiredSinceTime method. This method receives a time and tells us if the gesture has
  occurred since that time. The regular use of this is to save the time (using Time.time) when we last
  checked for the gesture and provide that as an input. This is useful mainly for situations where it
  is important for the developer that the detection occurs at a specific time, such as inside an 
  update call.
- Use events (by using the @ref NIInput::RegisterCallbackForGesture and
  @ref NIInput.UnRegisterCallbackForGesture methods). These methods allow the user to register for 
  an event when the gesture is detected. This is mainly relevant when we want to react immediately to
  the event.

@see @ref NIInput for a complete list of its methods.

<H2>Point Trackers and Gestures</H2>
It is important to note that you can get the data on Natural Interactions for a myriad of sources. This
is very similar to being able to use multiple joysticks and multiple buttons per joystick. To simulate
this, we use point trackers (to simulate joysticks) and gestures (to simulate button clicks). 
While the point trackers themselves (@subpage OpenNIPointTrackerConcept) and the gestures themselves 
(@subpage OpenNIGestureConcept) can be used beyond input, for the purpose of the input we can 
consider them as simply that; a list of "joystick" sources, which can be different hands of different
players or even other body joints, and a list of various buttons, defined by the various gestures.

<H2>NIInput Prefab</H2>
For easy usage of the NIInput class, you can use the @subpage InputControllerPrefab
*/

/**
@page OpenNIPointTrackerConcept The Point Tracker Concept
Natural Interaction enables using body motion to control input. In an analogy for a mouse, this means tracking some
point for position and using a gesture to "click". Unlike a mouse, complex gestures can be used to create a large number
of possible "clicks" and even provide them with a range of values.

Also unlike a mouse, there are many moving elements in the body and therefore the input source for the gesture (as
well the source of the position) can come vary. For example the source could be any of the skeleton's joints<br>

Whatever the source is, the basic behavior of that source is similar. For this purpose, the 
@ref NIPointTracker was defined. This class is an abstraction of how we use any and all points 
tracking and is a base class for various point trackers. The point trackers that inherit it
can (and do) implement various behaviors to track specific points, such as @ref NISkeletonTracker,
which we can use to track skeleton joints for specific players. <br>
Since the same point tracker is often used by many objects and we wish to control its initialization
properly, as well as finding the proper tracker for our purposes, the @ref NIPointTrackerManager is 
used. The developer simply drags and drops the various point trackers to the manager and when any 
game object needs to use it, the manager knows when to initialize and, through reference counting,
when to free the tracker.
@note It is possible, and very useful, to add the same type of tracker more than once to the manager.
A good example of this would be to track several players using the @ref NISkeletonTracker. You can
simply add several objects with the %NISkeletonTracker attached to them, each configured for a 
different joint of a different player.

@note Currently only limited trackers are implemented (see @ref NISkeletonTracker)
but it is very easy to add new ones simply by extending the @ref NIPointTracker class. The basic idea
is that in the future, one can simply add packages of new tracker implementations.
 */

/**
@page OpenNIGestureConcept The Concepts of Gesture and Gesture Factory
Gestures are the "key press" and "mouse button click" of Natural Interactions. They provide
us with events that can be used to take actions. A gesture can be something that occurs once, e.g. pushing with the
hand, which ends when the push is detected, continue over time (e.g. leaving the hand steady for
timed clicks) or even provide a new axis, e.g. making a walking gesture and getting a speed measurement 
from it.<br><br>

A gesture can work on various sources such as a skeleton joint. Because of this, a gesture is always
attached to some point tracker (see @ref OpenNIPointTrackerConcept). This could be
a general tracker tracking a point, or, more likely, a specific type of tracker, such as tracking
the skeleton.<br>
 
A gesture implements the @ref NIGestureTracker class, which supplies the interfaces to see
when a gesture was detected. This can be done by registering to events, by checking if the gesture
occurred since a given time, getting the frame when a gesture last occurred, or tracking a gesture 
currently in progress. <br>
For each specific gesture, an implementation of the %NIGestureTracker class is needed.<br>
Some gestures simply detect when something happened, while other gestures also have a timed element
in them. For example the steady gesture, implemented for a skeleton tracker by 
@ref NISteadySkeletonHandDetector, detects when the tracked hand is steady (doesn't move). 
It also counts the time where the hand doesn't move and therefore can provide the detected event only
after a specific time has passed, all the while showing us the progress.
<br><br>
Since the same gesture detection can be done on several hands, we use a gesture factory to create a 
new gesture and connect it to a relevant tracker whenever needed. The gesture factory is responsible
for the initialization. 
@note The initialization will fail if the wrong type of gesture is used with the wrong type of tracker.
For example, if you use the NITE gesture package, which includes a NITE based gesture (requires
NITE hand tracker) then the initialization will fail if you supply it with a skeleton based
OpenNI tracker.

As with point trackers, a Gesture manager (@ref NIGestureManager) is used to allow the developer to drag
and drop the gestures to the manager and gives us the legal gestures to use.
 */


/**
@defgroup OpenNIPackage OpenNI package
@brief This bundles all the functionality of OpenNI in a single module as part of the OpenNI Unity Toolkit.

The OpenNI package includes everything a user needs to create amazing games using the 
@ref OpenNIFramework. It includes the assets, samples and utilities.
@see @ref OpenNIPackageOverview
*/


/**
@defgroup OpenNIAssets OpenNI Assets
@brief This is the main module for the @ref OpenNIPackage. It includes all the code needed to create 
amazing games based on the skeleton provided by OpenNI. <br>
All of the code in this module is built upon the interfaces defined in the @ref OpenNIFramework. 
This means that if you install OpenNI with an OpenNI compliant middleware and supporting sensor, everything
in this package will work. Furthermore, all code was written inside Unity and is available here.
<br><br>
The OpenNI module includes more than just scripts, it also includes several prefabs that
can be used for faster implementation. These include:
- @ref OpenNISettingsPrefab
- @ref NISkeletonPrefab
- @ref InputControllerPrefab
@ingroup OpenNIPackage
*/

/**
@defgroup OpenNIBasicObjects OpenNI Basic Objects
@brief The basic objects to wrap the @ref OpenNIFramework

This submodule is responsible for wrapping the basic objects of the @ref OpenNIFramework. It is
aimed at performing all relevant initialization behind the scenes and providing simple customization options.
It also provides easy to use simplified access to the internals using scripts.<br>
For simplicity, the @ref OpenNISettingsPrefab is used to package everything in an easy-to-use prefab.
@ingroup OpenNIAssets
*/

/**
@defgroup SkeletonBaseObjects OpenNI Skeleton Usage Basic Objects
@brief Provides the Capability to wrap the skeleton's usage

This sub-module is responsible for wrapping the use of the skeleton and player to user mapping. It is
based on the usage of the @ref NISkeletonController to control the skeleton directly. The mapping of
Players to skeleton is done by @ref NISkeletonUsersManager and the @ref NIUsersToPlayerMapper handles
the mapping of human users to players. For simplicity, the @ref NISkeletonPrefab bundles the two 
managers.
@ingroup OpenNIAssets
*/

/**
@defgroup OpenNITrackers OpenNI Trackers
@brief Provides the capability to track joints and hands as well as gestures performed by them.

One of the main goals of the @ref OpenNIPackage is to provide an easy way to add new capabilities. To
do this, a tracker system has been introduced. The tracker system is composed of two main sub-modules;
the @ref OpenNIPointTrackers module, which is responsible for tracking a point (e.g. a hand) and the
@ref OpenNIGestureTrackers, which is responsible for understanding gestures and poses.<br>
<br>
The idea is that you can simply define a new tracker and add it, and then everything based on the 
trackers (such as GUI and Input) will simply be able to start using it. By using this system, 
developers can use "tracker packs" that supply new and interesting behavior. These can be added easily by simply 
dragging the new tracker to the scene.<br>

For more information, see @ref OpenNIPointTrackerConcept.
@ingroup OpenNIAssets
*/

/**
@defgroup OpenNIPointTrackers OpenNI Point Trackers
@brief Provides the capability to track joints and hands.

As stated in the @ref OpenNITrackers module, this sub-module is responsible for providing a capability
to track joints, hands or any moving point. Its goal is to become the "movement" element of a new 
"joystick" or position source.<br>

For more information, see @ref OpenNIPointTrackerConcept.
@ingroup OpenNITrackers
*/

/**
@defgroup OpenNIGestureTrackers OpenNI gesture trackers
@brief Provides the capability to track gestures.

As stated in the @ref OpenNITrackers module, this sub-module is responsible for providing a gesture
and pose tracking capability. The goal is to detect poses and gestures created by a point tracker. <br>
For more information, see @ref OpenNIGestureConcept.

@ingroup OpenNITrackers
*/

/**
@defgroup OpenNIInput NIInput
@brief Extends Unity's Input class.

This sub-module is basically an implementation of the @ref NIInputConcept.
@ingroup OpenNIAssets
*/
