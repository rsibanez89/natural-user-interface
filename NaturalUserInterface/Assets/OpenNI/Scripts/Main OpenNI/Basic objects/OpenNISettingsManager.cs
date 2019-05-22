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

using UnityEngine;
using OpenNI;

/// @brief A base class to configure and use OpenNI capabilities
/// 
/// This class is responsible for abstracting the initialization of an OpenNI context as well as 
/// its nodes. It also provides various configuration options for how these will be created and 
/// shows their behavior in the inspector
/// @ingroup OpenNIBasicObjects
public class OpenNISettingsManager : MonoBehaviour
{
    // public objects to be defined by the user

    /// The logger, this will be set by the game object to see where to send the log information 
    public NIEventLogger m_Logger;
    /// The query, this will be set by the game object to see which query to use.
    public NIQuery m_query;
    /// This holds an XML file name. It is used if we want to predefine the context and nodes (e.g. to do 
    /// more complex queries when choosing the nodes. A choice of an empty string means no XML should be used.
    public string m_XMLFileName;

    /// true if we wish to use an image generator
    public bool m_useImageGenerator;
    /// true if we wish to use a skeleton
    public bool m_useSkeleton;

    /// this is used to initialize the mirroring
    public bool m_initMirroring;

    /// If this is true then the context is in playback mode. 
    /// @see @ref OpenNIRecordAndPlayback "Recording and playing back sensor data"
    public bool m_playerbackMode;

    // public accessors

    /// controls the smoothing factor of the skeleton. The smoothing factor is a value between 0 
    /// (no smoothing at all) and 1 (no movement). 
    /// A good starting value for regular skeleton control is 0.5
    /// A good starting value for tracking a hand for GUI control is 0.9-0.95
    public float SmoothFactor
    {
        get
        {
            return m_smoothingFactor;
        }
        set 
        {
            if (Valid && m_useSkeleton && m_userAndSkeletonControl.Valid)
                m_userAndSkeletonControl.SkeletonSmoothingFactor=value;
            m_smoothingFactor = value;
        }
    }



    /// Accessor to @ref m_context.  
    public NIContext CurrentContext
    {
        get { return m_context; }
    }

    /// Accessor to @ref m_image
    public NIImage Image
    {
        get { return m_image; }
    }

    /// Accessor to @ref m_userAndSkeletonControl
    public NIUserAndSkeleton UserGenrator
    {
        get { return m_userAndSkeletonControl; }
    }

    /// Will hold true if this object is valid. If false, no NI capability can be used!
    public bool Valid
    {
        get { return m_context.Valid; }
    }

    /// Will hold true if we can use the Image data
    public bool ImageValid
    {
        get { return Valid && m_image.Valid; }
    }

    /// Will hold true if we can use the user and skeleton data
    public bool UserSkeletonValid
    {
        get { return Valid && m_userAndSkeletonControl.Valid; }
    }


    /// holds the mirroring capability of the context  @note mirror should be true by default!
    public bool Mirror
    {
        get { return m_context != null && m_context.Mirror; }
        set { if (m_context != null) m_context.Mirror = value; }
    }



    /// @brief updates the context
    /// 
    /// This is a mono-behavior FixedUpdate. We use FixedUpdate to make sure this is called before other updates
    /// even if the frame rate is low (to give the context and other elements time to update).
    /// This method is responsible for updating the internal objects (such as the context).
    public void FixedUpdate()
    {
        if (m_context.Valid)
            m_context.UpdateContext();
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
        m_context = NIContext.Instance;
        if(m_context.Valid==false)
        {
            // we need to initialize...
            Debug.Log(m_XMLFileName);
            if (m_context.Init(m_Logger, m_query, m_XMLFileName, m_playerbackMode) == false)
            {
                m_Logger.Log("FAILED TO INITIALIZE CONTEXT!!!", NIEventLogger.Categories.Errors, NIEventLogger.Sources.BaseObjects);
                return;
            }
        }
        Mirror = m_initMirroring;
        m_image = NIImage.Instance;
        if (m_useImageGenerator)
            m_image.Init(m_Logger, m_context);
        else m_image.Dispose(); // to make sure it is invalid
        m_userAndSkeletonControl = NIUserAndSkeleton.Instance;
        if (m_useSkeleton)
        {
            m_userAndSkeletonControl.Init(m_Logger, m_context);
            m_userAndSkeletonControl.SkeletonSmoothingFactor = m_smoothingFactor;
        }
        else m_userAndSkeletonControl.Dispose(); // to make sure it is invalid
    }


    /// @brief makes sure we release everything on quit
    /// 
    /// This will make sure the context is released when quitting (important for playing and
    /// stopping in the editor).
    /// @note - This is based on the MonoBehavior.OnAppliactionQuit.
    public void OnApplicationQuit()
    {
        m_Logger.Log("In OpenNIContext.OnApplicationQuit",  NIEventLogger.Categories.Initialization, NIEventLogger.Sources.BaseObjects);
        ReleaseNI();
    }

    /// @brief makes sure we release everything on destroy
    /// 
    /// This will make sure the context is released when the object is destroyed
    /// (important for playing and stopping in the editor).
    /// @note - This is based on the MonoBehavior.OnDestroy.
    public void OnDestroy()
    {
        m_Logger.Log("In OpenNIContext.OnDestroy", NIEventLogger.Categories.Initialization, NIEventLogger.Sources.BaseObjects);
        ReleaseNI();
    }

    /// This does a safe log (i.e. checks if the logger is null before logging
    /// @param str the string to log
    /// @param category the category of the log
    /// @param source the source of the log
    public void Log(string str, NIEventLogger.Categories category, NIEventLogger.Sources source)
    {
        if (m_Logger != null)
            m_Logger.Log(str, category, source);
    }
    // protected methods

    /// @brief Releases all internal objects.
    /// 
    /// Release all internal objects in a reverse order to initialization.
    /// @note after releasing this object becomes uninitialized!!!
    protected void ReleaseNI()
    {
        if (m_useSkeleton && m_userAndSkeletonControl!=null && m_userAndSkeletonControl.Valid)
            m_userAndSkeletonControl.Dispose();
        if (m_useImageGenerator && m_image!=null && m_image.Valid)
            m_image.Dispose();
        if (m_context.Valid)
            m_context.Dispose();
    }

    // protected members

    /// a link to the context (currently generated from the singleton) 
    protected NIContext m_context;
    /// a link to the an image generator (currently generated from the singleton) 
    protected NIImage m_image;

    /// a link to the a user and skeleton control object (currently generated from the singleton) 
    protected NIUserAndSkeleton m_userAndSkeletonControl;

    /// holds the smoothing factor for the skeleton
    /// @note this is public for use in the inspector DO NOT USE IT BY ANYTHING ELSE!
    public float m_smoothingFactor;
}
