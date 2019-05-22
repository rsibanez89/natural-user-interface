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


/// @brief abstraction of the context
///
/// This class is responsible for abstracting the initialization of an OpenNI context. 
/// It is a singleton which is responsible for holding the basic (internal) OpenNI context and 
/// depth node. It also provides the tools to add other nodes based on requests from other objects.
/// @ingroup OpenNIBasicObjects
public class NIContext : NIWrapperObject
{
    // public accessors
    /// Tells us if we are valid.
    /// @note the test checks only the context. It is assumed that internally if anything else goes wrong
    /// the context will become null as well.
    public override bool Valid 
    {
        get { return m_context!=null; }
    }

    /// Accessor to @ref m_context
    public Context BasicContext  
    {
        get { return m_context; }
    }

    /// Accessor to @ref m_depth
    public DepthGenerator Depth
    {
        get { return m_depth; }
    }



    /// Accessor to the singleton (@ref m_singleton)
    public static NIContext Instance
    {
        get { return m_singleton; }
    }

    /// holds the mirroring capability of the context  @note mirror should be true by default!
    public bool Mirror
    {
        get { return Valid && BasicContext.GlobalMirror; }
        set { if (Valid && m_playerbackMode==false) BasicContext.GlobalMirror = value; }
    }

    // public methods

    /// @brief Initialize the context object
    /// 
    /// This method initializes the context. It is assumed that this would be done once on the
    /// beginning of the game. Note that this is called on the singleton so multiple initializations
    /// will simply do nothing (which might cause problems with other objects). If for some reason a new
    /// initialization is needed, the @ref Dispose method should be called first (however ALL
    /// other initializations will be required first such as reinitializing the nodes). Also, it is assumed that
    /// every node created externally using the @ref CreateNode method will be released (using the @ref ReleaseNode
    /// method) BEFORE releasing the context.
    /// @param logger the logger object we will enter logs into
    /// @param query A query limiting the nodes found.
    /// @param xmlFileName this will hold an xml file to initialize from. A null or empty filename
    /// will simply be ignored and the object will be built without it. An illegal filename will cause
    /// an exception!
    /// @param playerbackMode If this is true then the context is in playback mode (see 
    /// @ref OpenNIRecordAndPlayback "Recording and playing back sensor data").
    /// @return true on success, false on failure. 
    /// @note if the context was already initialized this will fail but the context would be valid!
    /// @note This implementation assumes that a depth generator is required for ALL uses including
    /// the creation of the skeleton. In theory, it is possible that some implementations will not 
    /// require a depth generator. This is currently not supported.
    public virtual bool Init(NIEventLogger logger, NIQuery query, string xmlFileName, bool playerbackMode)
    {
        if (Valid)
        {
            if(logger!=null)
                logger.Log("trying to initialized a valid context!", NIEventLogger.Categories.Initialization, NIEventLogger.Sources.BaseObjects);
            return false; // failed to initialize
        }
        if (InitLogger(logger) == false)
        {
            Dispose();
            return false; // we failed an initialization step.
        }
        Log("In OpenNIContext.InitContext with logger=" + logger + " query=" + query, NIEventLogger.Categories.Initialization, NIEventLogger.Sources.BaseObjects);
        if(m_context!=null || m_scriptNode!=null || m_depth!=null)
            throw new System.Exception("valid is false but internal structures are not null! m_context=" + m_context + " m_scriptNode=" + m_scriptNode + " m_depth=" + m_depth);
        m_Logger = logger;
        m_query = query;
        if (xmlFileName != null && xmlFileName.CompareTo("")!=0)
        {
            try
            {
                NIOpenNICheckVersion.Instance.ValidatePrerequisite();
                m_context = Context.CreateFromXmlFile(xmlFileName, out m_scriptNode);
            }
            catch
            {
                Log("failed to create from xmlFile!", NIEventLogger.Categories.Errors, NIEventLogger.Sources.BaseObjects);
                Dispose();
                return false;
            }                      
        }
        else
        {
            try
            {
                NIOpenNICheckVersion.Instance.ValidatePrerequisite();
                m_context = new Context();
            }
            catch (System.Exception ex)
            {
                if (ex as System.DllNotFoundException != null)
                {
                    throw new System.DllNotFoundException("The dll for OpenNI is not there. Please install OpenNI (using the mega installer");
                }
                else Debug.Log(ex.Message);
            }
            
        }
		if (m_context==null)
		{
            Log("failed to create a context!", NIEventLogger.Categories.Errors, NIEventLogger.Sources.BaseObjects);
            Dispose();
			return false;
		}
        m_playerbackMode = playerbackMode;
        m_depth = CreateNode(NodeType.Depth) as DepthGenerator;
        return true;
	}

    /// @brief Used to add a new node to the context
    /// 
    /// This will find a new node. If it is already there it will be found and if not
    /// it will be created. 
    /// @note - It is the responsibility of the caller to call this only after @ref Init has been
    /// called successfully (if valid is false this will return null).
    /// @note - It is the responsibility of the caller to call @ref ReleaseNode on every node created BEFORE
    /// calling @ref Dispose.
    /// 
    /// @param nt The type of node
    /// @return the node created as a ProductionNode. It is the responsibility of the caller to cast it 
    /// into the appropriate type.
    public ProductionNode CreateNode(NodeType nt)
	{
        Log("Creating type=" + nt, NIEventLogger.Categories.Initialization, NIEventLogger.Sources.BaseObjects);
        if (!Valid)
        {
            Log("Failed to create a node of type " + nt + " because the context is not initialized", NIEventLogger.Categories.Errors, NIEventLogger.Sources.BaseObjects);
            return null;
        }
		ProductionNode ret=null; // this will hold the node
		try
		{
            // first we will try to find an existing one if it was already created by some means
            // (such as loading it from xml when inheriting this class).
			ret = m_context.FindExistingNode(nt);
		}
		catch
		{
            // if we are here then there is no node of the relevant type, we should
            // instead create a new one.
            Query queryToUse = null;
            if (m_query != null)
                queryToUse = m_query.UsedQuery;
            try
            {
                ret = m_context.CreateAnyProductionTree(nt, queryToUse);
            }
            catch (System.Exception ex)
            {
                m_Logger.Log(ex.Message, NIEventLogger.Categories.Errors, NIEventLogger.Sources.BaseObjects);
                return null;
            }
            

            // Since generators need to start generating, we will check if the
            // new node is a Generator and if so start generating...
			Generator g = ret as Generator;
			if (g != null)
			{
				g.StartGenerating();
			}
		}
		return ret;
	}


    /// @brief Used to release an added node
    /// 
    /// This will release an added node from the context.
    /// @note - It is the responsibility of the caller to call this only on nodes created using the @ref CreateNode
    /// method and only when the context is valid! This method should be called on every node created BEFORE
    /// calling @ref Dispose.
    ///
    /// @param node The node to release
    public void ReleaseNode(ProductionNode node)
    {
        Log("disposing type=" + node.GetType(), NIEventLogger.Categories.Initialization, NIEventLogger.Sources.BaseObjects);
        // note no need to stop generating a generator because disposing will do it for us.
        node.Dispose();
    }
	

    /// @brief updates the context
    /// 
    /// This will make sure the context is updated if necessary.
    /// @note - This does NOT wait for the update so it is up to the user method to check if new
    /// data is available.
    public virtual void UpdateContext()
    {
        if (m_playerbackMode)
        {
            m_context.WaitAnyUpdateAll(); // this is a hack until the problem is fixed as currently playback does not work with WaitNoneUpdateAll...
        }
        else
        {
            m_context.WaitNoneUpdateAll();
        }
        
    }


    /// @brief Releases all elements from the context
    /// 
    /// This will make sure the context and relevant objects are released and changed to null.
    /// @note Before this method is called, any node added using the @ref CreateNode method should
    /// have the @ref ReleaseNode method called on it.
    public override void Dispose()
    {
        Log("Disposing of context!", NIEventLogger.Categories.Initialization, NIEventLogger.Sources.BaseObjects);
        if(m_depth!=null)
        {
            ReleaseNode(m_depth);
            m_depth = null;
        }

        if(m_scriptNode!=null)
        {
            m_scriptNode.Dispose();
            m_scriptNode = null;
        }
        if(m_context!=null)
        {
            m_context.StopGeneratingAll();
            m_context.Dispose();
            m_context = null;
        }
    }


    // protected members

    /// @brief The singleton itself
    /// 
    /// NIContext uses a singleton pattern. There is just ONE NIContext object which is used by all.
    protected static NIContext m_singleton = new NIContext();

    /// An internal script node used to release nodes created when initializing from an xml
    protected ScriptNode m_scriptNode;

    /// An internal query used to limit which nodes can be created when creating new nodes
    protected NIQuery m_query;

    /// An internal object using the the OpenNI basic context (not to be confused with NIContext, m_context represent
    /// the context received from the OpenNI dll.
    protected Context m_context;
    /// An internal link to the depth node (to follow depth information used throughout the system).
    protected DepthGenerator m_depth;

    /// If this is true then the context is in playback mode. 
    /// @see @ref OpenNIRecordAndPlayback "Recording and playing back sensor data"
    protected bool m_playerbackMode;
    // private methods


    /// @brief private constructor
    /// 
    /// This is part of the singleton pattern, a protected constructor cannot be called externally (although
    /// it can be inherited for extensions. In which case the extender should also replace the singleton!
    private NIContext()
    {
        m_context = null;
        m_depth = null;
        m_scriptNode = null;
    }

}
