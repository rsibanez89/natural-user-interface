/*******************************************************************************
*                                                                              *
*   OpenNI Unity Toolkit - NITE extension                                      *
*   Copyright (C) 2010 PrimeSense Ltd.                                         *
*                                                                              *
*******************************************************************************/
// this file is used for documentation rather than code.
// this file is aimed at NITE

/**
@page NITEFramework NITE&tm; Middleware

NITE Middleware provides a set of computer-vision algorithms that analyze the observed scene, in order to identify 
objects and produce meaningful information from the depth image.<br>
For more information, see <a href="http://www.primesense.com">http://www.primesense.com</a> under 
products->NITE middleware
*/

/**
@page NITEOverview NITE Extensions Overview

The NITE extension package is an extension to the OpenNI Unity Toolkit package. 
While the OpenNI package provides everything required to create a game, the NITE&tm; extension provides 
additional capabilities that mostly relates to controlling GUI using hand gestures. <br><br>

The NITE extension package provides high quality point tracking (e.g. to track hands) and a mechanism for managing
a session of user control. A session of a user that controls the system starts when this user performs a focus 
gesture - a gesture that signals the system that this user requests to gain control. Once the focus gesture is 
detected, the session tracks the hand that performed the focus gesture. <br>

If the hand tracking is lost, a grace period is used in which a refocus gesture could also be used to continue 
(a refocus gesture is an easier but less distinctive gesture). For example, a user can wave to the sensor to get
focus and simply raise a hand to do refocus.<br><br>

The best place to start with the NITE extension is to read the documentation under 
Assets/NITE/Documentation/NITEPackageDocumentation.chm.<br>
This documentation provides a high-level overview of the package and its capabilities, tutorials on how to use the 
provided tools and documentation of every class, method, member and parameter.<br><br>

@note This is an extension of the basic OpenNI Unity Toolkit package and therefore will not work without the
OpenNI Unity Toolkit package. It is assumed that the OpenNI Unity Toolkit package has already been installed and you
have familiarized yourself with it.
 
The NITE&tm; assets are based on the @subpage NITEFramework.<br><br>

Two main elements are included in the NITE module:
- Basic objects (mainly the @ref NITEHandsManager), which hold (internally) the session manager and
  allow us to configure the way NITE controls behave. A prefab (@subpage HandsManagerPrefab) is provided
  for ease of use.
  @note This works in a similar way to OpenNI Settings Manager in the base OpenNI Package, including 
  the implementation of @ref NIHandControl as a singleton and its abstraction in @ref NITEHandsManager.
  See the Singleton Implementation Overview in the base OpenNI package for more information.
- Trackers (hand trackers and gesture trackers), which use the NITE capabilities. This is the
  first extension package that builds on Point Tracker Concept and Gesture Concept to add new trackers and gestures.


*/

/** 
@page NITEMainCapabilities NITE Module Main Capabilities

The NITE package is aimed at expanding the base OpenNI package and providing additional capabilities
derived from its use of the @subpage NITEFramework. These additional capabilities can be summarized
in two main elements:

<H1>NITE Controls-based Hand Tracking</H1>
NITE controls provides a much higher quality hand tracking than provided by the skeleton it can used 
without the skeleton and there is no need for calibration. The main new capability is a hand and gesture
tracking package. This package provides the new @ref NIHandTracker class and several simple gestures.

<H1>Seamless Integration with OpenNI Capabilities</H1>

The NITE controls tracking can be seamlessly integrated with OpenNI capabilities, such as Input
management and GUI. All you need to do is drag and drop the NITE controls-based hand tracking and
gestures to the game exactly as you would OpenNI based hand tracking and gestures.
*/


/**
@page NITEFilesOrganization File Organization for the NITE Module

The resources provided are organized into a folder tree under the Assets/NITE directory. In addition,
custom inspectors are provided in the Editor directory.<br>
@note As stated previously, NITE extension package is an extension to the OpenNI Unity Toolkit package and as such 
cannot be used without the base OpenNI package. The file organization here is very similar to that of OpenNI.

The NITE directory contains the following:
- <b>Documentation</b> - This structure is very similar to the structure of OpenNI documentation.
   it holds the main documentation for the NITE extensions and includes:
    - <b>The NITEPackageDocumentation.chm file</b>: This is the main documentation file. It holds 
       everything needed in order to use this package.
    - <b>NITEDoxyfile</b> - This is a doxygen configuration file aimed at creating the documentation. 
       @note It will create html files on Documentation/NITE directory on the same level as the Assets
       directory for those who prefer html to chm.
    - The <b>"Additional documentation sources"</b> directory, which holds files to create the
      documentation. This includes basic pages for the different modules and a main page contained in
      NITEMainPage.cs
      @note The documentation for the actual code is inside the code.
    - The <b>"Images"</b> directory, which holds images used in the documentation (such as the NITE
       logo)
- <b>Graphical assets</b>: these are various assets that are used in the samples and include
  3D models, textures, materials etc. 
- <b>Interface library</b>: This directory is divided into two sub directories.
    - <b>NITE engine</b> - this includes a C# .Net dll containing the NITE interface.
- <b>Prefabs</b>: This directory holds prefabs used by the NITE extension
- <b>Sample scenes</b>: This directory holds the end product of all samples, i.e. what they should look
  like after the tutorials are finished.
- <b>Scripts</b>: This is where all the code is located. The code is divided into sub directories based on
  the sub module they belong to.
    - <b>NITE</b>: Contains the main scripts. This is the basis for the NITE extension.
    - <b>Samples</b> - These are scripts specific for the samples. They are needed to make the samples
      work.
*/

/**
@page NITEBeforeYouStart Before you Start
The NITE extension is just that, an extension that provides additional functionality to the main elements 
provided in the base OpenNI package.<br>

Because NITE extensions is an extension of the basic OpenNI Unity Toolkit package, it will not work without the OpenNI
Unity Toolkit package. It is assumed that the OpenNI Unity Toolkit package has already been installed and you have 
familiarized yourself with it.
<br><br>

@note To use NITE, the NITE&tm; middleware @b MUST be installed.
*/

/**
@page NITEAdditionalTopics Additional Topics

Most additional topics are discussed under the OpenNI Additional topics in the OpenNI base package. NITE 
only imposes some fine tuning as described below:
- <b>Supported environments</b>: The supported environment is basically the same as OpenNI. Of course, 
  NITE must be installed in order for it to work.
- <b>Loading levels</b>: If a level is loaded without change, in addition to losing calibration, the
  NITE session will also be lost. 
*/


/** 
@page NITELicense OpenNI Unity Toolkit - NITE extension License
OpenNI Unity Toolkit - NITE extension - Copyright &copy; 2011 PrimeSense Ltd.<br><br>
Using the NITE extension requires accepting the EULA which appears under 
Assets/NITE/Documentation/NITE EULA.rtf<br> If you do not accept the EULA, please delete the NITE
extensions portion of the package.<br>
*/

/**

@page HandsManagerPrefab Hands Manager prefab
The Hands Manager prefab is aimed at providing basic configuration and settings for the NITE extensions 
capabilities and one place for all initialization. It is very similar to the OpenNI Settings Prefab.<br>
This is implemented by using the @ref NITEHandsManager mono-behavior class for initialization, using
the @ref NIHandsManagerInspector class as a custom inspector and hides the low level NITE engine in the 
@ref NIHandControl singleton.
*/

/**
@defgroup NITEExtension NITE extension package
@copydoc NITEOverview
<br><br>
Beyond the basic scripts included in NITE, a prefab is used called HandsManager, which is 
the equivalent of the OpenNI configuration element in the OpenNI Settings Prefab available in
the base OpenNI package.
*/

/**
@defgroup NITEBasicObjects NITE Base Objects
@brief The NITE base objects module is responsible for the base capabilities of NITE, which are all
bundled in the @ref HandsManagerPrefab. These are extensions to the OpenNI Skeleton Base Objects module.
@ingroup NITEExtension
*/

/**
@defgroup NITETrackers NITE Trackers
@brief The NITE trackers module is responsible for the addition of the various NITE controls-based 
trackers, including the hand based @ref NIHandTracker and the various gesture trackers. These trackers
use the session manager built in NITE to provide improved hand tracking, which can be done without a 
skeleton and with superior quality, and gestures built on it. <br>
This is an extension to the OpenNI Trackers  module and its sub-modules.
@ingroup NITEExtension
*/
