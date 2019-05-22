/*******************************************************************************
*                                                                              *
*   OpenNI Unity Toolkit - NITE extension                                      *
*   Copyright (C) 2010 PrimeSense Ltd.                                         *
*                                                                              *
*******************************************************************************/
using UnityEngine;
using System.Collections;

/// @brief class to check OpenNI Toolkit NITE extension versions.
/// 
/// This class implements the NICheckVersion class for the NITE extension.
/// @note If test version fails then an exception is thrown.
/// 
/// @note This is implemented as a singleton!
/// @ingroup NITEBasicObjects
public class NINITECheckVersion : NICheckVersion
{
    /// @brief Accessor to the singleton
    public static NINITECheckVersion Instance
    {
        get { return m_singleton; }
    }

    /// @brief Method to make sure all prerequisites are met before running the toolkit.
    /// 
    /// This method should be called before any class derived from dll a different package is
    /// created. Its goal is to throw a human readable exception if the prerequisites are not met
    /// to enable the developer to fix the situation (i.e. install the relevant prerequisite).
    /// The implementation here will check the the OpenNI toolkit. 
    public override void ValidatePrerequisite()
    {
        // first we want to make sure the OpenNI package is ok
        NIOpenNICheckVersion.Instance.ValidatePrerequisite(); 
        try
        {
            NIVersion ver = NIOpenNICheckVersion.Instance.GetVersion();
            if (m_minOpenNIToolkitVersion.CompareVersion(ref ver) < 0)
            {
                // we have an illegal version!
                throw new System.Exception("NITE extensions require the OpenNI Unity toolkit to be installed. The toolkit is too old (" + ver.ToString() + ", need minimum " + m_minOpenNIToolkitVersion + ". Please install the new package.");
            }
        }
        catch (System.Exception ex)
        {
            throw new System.Exception("NITE extensions require the OpenNI Unity toolkit to be installed. Failed to get the version of the OpenNI Unity toolkit with message " + ex.Message + ". This probably means that toolkit is not installed or an old version is installed, please install a new version.");
        }
    }

    /// @brief Gets the current version of the NITE dll
    /// @return the version structure
    public NIVersion GetNITEDllVersion()
    {
        throw new System.NotImplementedException("Currently we can't yet provide the NITE version! Please upgrade the NITE extension package!");
        //NIVersion ver = NIVersion.ZeroVersion;
        //ver.InitFromOpenNIVersion(OpenNI.Version.Current);
        //return ver;
    }
    /// @brief The minimal OpenNI Unity Toolkit version required for this package.
    protected NIVersion m_minOpenNIToolkitVersion;

    /// @brief Private constructor for singleton pattern.
    private NINITECheckVersion()
    {
        m_minOpenNIToolkitVersion = new NIVersion(0, 9, 0, 0);
        m_currentVersion = new NIVersion(0, 9, 0, 0);
    }

    /// @brief Private singleton
    private static NINITECheckVersion m_singleton=new NINITECheckVersion();
}

