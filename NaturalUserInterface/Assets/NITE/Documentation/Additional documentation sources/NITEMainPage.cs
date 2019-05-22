/*******************************************************************************
*                                                                              *
*   OpenNI Unity Toolkit - NITE extension                                      *
*   Copyright (C) 2010 PrimeSense Ltd.                                         *
*                                                                              *
*******************************************************************************/
// This file is used for documentation rather than code.
// This holds pages which are unique to the NITE documentation (including the main page)

/**

@mainpage OpenNI Unity Toolkit - NITE extension

<H1>Introduction</H1>

The NITE extension package is an extension to the OpenNI Unity Toolkit package. While the OpenNI package provides
everything required to create a game, the NITE&tm; extension provides additional capabilities that mostly relates to 
controlling GUI using hand gestures.<br><br>

The NITE&tm; extension package provides high quality point tracking (e.g. to track hands) and a mechanism for managing 
a session of user control. A session of a user that controls the system starts when this user performs a focus gesture -
a gesture that signals the system that this user requests to gain control. Once the focus gesture is detected, the 
session tracks the hand that performed the focus gesture. <br>
If the hand tracking is lost, a grace period is used in which a refocus gesture could also be used to continue 
(a refocus gesture is an easier but less distinctive gesture). For example, a user can wave to the sensor to get focus
and simply raise a hand to do refocus.<br><br>

The best place to start with the NITE extension is to read the documentation under 
Assets/NITE/Documentation/NITEPackageDocumentation.chm.<br>
This documentation provides a high-level overview of the package and its capabilities, tutorials on how to use the
provided tools and documentation of every class, method, member and parameter.<br><br>


@see For more information the OpenNI Unity toolkit documentation.


In order to better understand the package, you should read the following chapters first:

- @subpage NITEOverview
- @subpage NITEMainCapabilities
- @subpage NITEFilesOrganization
- @subpage NITEBeforeYouStart
- @subpage NITESamplesOverview
- @subpage NITEAdditionalTopics

@see @ref NITEExtension

In addition, full documentation is provided for all files.
<H1>License</H1>
@copydoc NITELicense
*/
