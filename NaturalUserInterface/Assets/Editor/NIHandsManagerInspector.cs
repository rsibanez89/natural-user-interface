
/*******************************************************************************
*                                                                              *
*   PrimeSense NITE 1.4.x - Unity wrapper                                      *
*   Copyright (C) 2010 PrimeSense Ltd.                                         *
*                                                                              *
*******************************************************************************/

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using OpenNI;


/// @brief A class to define the inspector of NITEHandsManager
/// 
/// This class is responsible for adding various inspector capabilities to the NITEHandsManager tool
/// @ingroup NITEBasicObjects
[CustomEditor(typeof(NITEHandsManager))]
public class NIHandsManagerInspector : Editor
{
    /// the index of the focus choice in the string array
    protected int m_focusGestureChoice;
    /// the index of the refocus choice in the string array
    protected int m_refocusGestureChoice;
    /// used as a list of gestures from the current list.
    protected string[] m_currentVersionGestureList = null;
    /// A list of legal gestures from the default
    protected string[] m_defaultVersionGestureList = { "Wave", "Click", "RaiseHand", "MovingHand" };

    /// the index of the current list we use.
    protected bool m_useBasicGestureList;
    /// should Focus be used
    protected bool m_useFocus;
    /// should refocus be used
    protected bool m_useRefocus;

    /// this tells us if we are continuing from before or moved to a new window
    /// based on this we will know if we need to reinitialize all the values
    protected bool m_initializedFromBefore=false;

    /// internal GUIContent, this is used to easily add tool tips.
    protected GUIContent m_myContent = new GUIContent("string label", "tooltip");

    /// This method finds the index of a gesture in a list.
    /// 
    /// @param gestureList the list of gestures we are searching in
    /// @param gesture the gesture we are trying to match
    /// @return the index of the gesture in the list (-1 if not found).
    protected int FindIndexInList(string[] gestureList, string gesture)
    {
        if (gesture == null)
            return -1; // not found.
        for (int i = 0; i < gestureList.Length; i++)
        {
            if (gesture.CompareTo(gestureList[i]) == 0)
                return i;
        }
        return -1; // not found
    }

    /// Initializes the list of gestures (from the current version) if it is not yet initialized.
    /// @return true on success and false if it couldn't initialize.
    protected bool InitGestures()
    {
        if (m_currentVersionGestureList != null)
            return true; // it is already filled.
        // we either succeed of fail so we do all the calculation in one try-catch block.
        try
        {
            // we need the gesture node. So for that we will create a context and a gesture node on it.
            Context tmpContext = new Context(); 
            GestureGenerator gesture = tmpContext.CreateAnyProductionTree(NodeType.Gesture, null) as GestureGenerator;
            m_currentVersionGestureList = gesture.EnumerateAllGestures(); // hold all legal gestures
            // cleanup
            gesture.Dispose();
            tmpContext.Dispose();
        }
        catch (System.Exception ex)
        {
            Debug.Log("FAILED TO INITIALIZE!. Message=" + ex.Message);
            m_currentVersionGestureList = null;
            return false;
        }
        return true;
    }

    /// the goal of this method is to initialize the indices based on the target object.
    protected void InitFromTarget()
    {
        // for easy access to the object
        NITEHandsManager handsManagerObject = target as NITEHandsManager;
        if (handsManagerObject == null)
        {
            if (target != null)
                Debug.Log(target.GetType());
            else
                Debug.Log("target is null!");
        }
        bool shouldFocus = handsManagerObject.m_focusGesture == null || handsManagerObject.m_focusGesture.CompareTo("") != 0;
        int focusGesture =-1;
        if (shouldFocus)
            focusGesture = FindIndexInList(m_defaultVersionGestureList, handsManagerObject.m_focusGesture);

        bool shouldRefocus = handsManagerObject.m_refocusGesture == null || handsManagerObject.m_refocusGesture.CompareTo("") != 0;
        int refocusGesture=-1;
        if (shouldRefocus)
            refocusGesture = FindIndexInList(m_defaultVersionGestureList, handsManagerObject.m_refocusGesture);
        if ((shouldFocus == false || focusGesture >= 0) && (shouldRefocus == false || refocusGesture >= 0))
        {
            // this is a good find
            m_refocusGestureChoice = refocusGesture;
            m_focusGestureChoice = focusGesture;
            m_useFocus = shouldFocus;
            m_useRefocus = shouldRefocus;
            m_useBasicGestureList = true;
            return;
        }
        // if we are here then we couldn't find a good match, lets try the same with the current version
        if (InitGestures() == false)
        {
            // can't find a good choice at all. Lets reinitialize to nothing
            m_refocusGestureChoice = -1;
            m_focusGestureChoice = -1;
            m_useFocus = false;
            m_useRefocus = false;
            m_useBasicGestureList = true;
            return;
        }
        focusGesture = -1;
        if (shouldFocus)
            focusGesture = FindIndexInList(m_currentVersionGestureList, handsManagerObject.m_focusGesture);
        refocusGesture = -1;
        if (shouldRefocus)
            refocusGesture = FindIndexInList(m_currentVersionGestureList, handsManagerObject.m_refocusGesture);
        if ((shouldFocus == false || focusGesture >= 0) && (shouldRefocus == false || refocusGesture >= 0))
        {
            // this is a good find
            m_refocusGestureChoice = refocusGesture;
            m_focusGestureChoice = focusGesture;
            m_useFocus = shouldFocus;
            m_useRefocus = shouldRefocus;
            m_useBasicGestureList = false;
            return;
        }
        // if we are here neither was a good choice so lets reinitialize
        m_refocusGestureChoice = -1;
        m_focusGestureChoice = -1;
        m_useRefocus = false;
        m_useFocus = false;
        m_useBasicGestureList = true;
    }

    /// This method creates the inspector when the program is running. In this mode we
    /// only show information on what is initialized, valid etc. but allow no changes.
    protected void RunInspector()
    {
        // for easy access to the object
        NITEHandsManager handsManagerObject = target as NITEHandsManager;

        // basic test. If the object is invalid, nothing else matters.
        if (handsManagerObject.ValidWithInit == false)
        {
            EditorGUILayout.LabelField("NI was not initialized and therefore everything is invalid!", "");
            return;
        }

        EditorGUILayout.LabelField("Hands control is ", handsManagerObject.ValidWithInit ? "Valid" : "not valid");
        // the foldout allows to see just relevant info (and order it a bit).
        // note that even if the hands control is invalid, data exists here
        EditorGUILayout.LabelField("Focus Gesture used:", handsManagerObject.m_focusGesture);
        EditorGUILayout.LabelField("Refocus Gesture used:", handsManagerObject.m_refocusGesture);
        EditorGUILayout.LabelField("Current state: ", "" + handsManagerObject.Hands.CurrentState);
        EditorGUILayout.Space();
    }

    /// This method creates the inspector when the program is NOT running. In this mode we
    /// allow initialization but do not show any running information
    protected void InitInspector()
    {
        // for easy access to the object
        NITEHandsManager handsManagerObject = target as NITEHandsManager;

        // using hands control
        if(m_initializedFromBefore==false)
        {
            InitFromTarget();
            m_initializedFromBefore = true;
        }
        // choose gestures.
        m_myContent.text = "Use basic gesture list";
        m_myContent.tooltip = "The basic gesture list is supported by all implementation, If false this uses the values available on the local machine";
        bool useBasicGestureList = EditorGUILayout.Toggle(m_myContent, m_useBasicGestureList);
        string[] gestures = m_defaultVersionGestureList;
        if (useBasicGestureList == false) 
        {
            // if we are here we need to make sure m_currentVersionGestureList holds the current list
            // of gestures.
            if (InitGestures())
            {
                gestures = m_currentVersionGestureList;
            }
            else
            {
                Debug.Log("Can't initialize current version!!!!!, defaulting to regular version!");
                m_useBasicGestureList=false;
            }
        }
        // we changed the gesture list so we need to find the new index which corresponds to
        // the previous choices
        if (m_useBasicGestureList !=  useBasicGestureList)
        {
            m_focusGestureChoice = FindIndexInList(gestures, handsManagerObject.m_focusGesture);
            m_useFocus = m_focusGestureChoice >= 0;
            m_refocusGestureChoice = FindIndexInList(gestures, handsManagerObject.m_refocusGesture);
            m_useRefocus = m_refocusGestureChoice>=0;
            m_useBasicGestureList = useBasicGestureList;
        }

        bool useFocus = EditorGUILayout.Toggle("Use Focus?", m_useFocus);
        if (useFocus != m_useFocus)
        {
            if (useFocus == false)
            {
                handsManagerObject.m_focusGesture = "";
                m_focusGestureChoice = -1;
            }
            else
            {
                m_focusGestureChoice = 0;
            }
        }
        m_useFocus = useFocus;
        if (m_useFocus)
        {
            EditorGUI.indentLevel += 2;
            m_focusGestureChoice = EditorGUILayout.Popup("Gesture to use", m_focusGestureChoice, gestures);
            EditorGUI.indentLevel -= 2;
            handsManagerObject.m_focusGesture = gestures[m_focusGestureChoice];
        }

        // we might not want to use refocus (which will make it into "").
                
        bool useRefocus = EditorGUILayout.Toggle("Use refocus?", m_useRefocus);
        if (useRefocus != m_useRefocus)
        {
            if (useRefocus == false)
            {
                handsManagerObject.m_refocusGesture = "";
                m_refocusGestureChoice = -1;
            }
            else
                m_refocusGestureChoice = 0;
        }
        m_useRefocus = useRefocus;
        if (m_useRefocus)
        {
            EditorGUI.indentLevel += 2;
            m_refocusGestureChoice = EditorGUILayout.Popup("Gesture to use", m_refocusGestureChoice, gestures);
            EditorGUI.indentLevel -= 2;
            handsManagerObject.m_refocusGesture = gestures[m_refocusGestureChoice];
        }
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }

    /// implementation of the Editor inspector GUI (this is what is called to see the inspector).
    override public void OnInspectorGUI()
    {
        EditorGUI.indentLevel = 0;
        EditorGUIUtility.LookLikeInspector();
        if(EditorApplication.isPlaying)
        {
            // we are running so we can only get some feedback
            RunInspector();
        }
        else 
        {
            // we are not running so we can initialize stuff
            InitInspector();
        }
        EditorGUI.indentLevel = 0;

        // this is part of making sure we update during running. This adds the update function
        // to the editor application update which is called 100 times per second.
        if (m_initialized == false)
        {
            EditorApplication.update += Update;
            m_initialized = true;
        }
    }

    /// if true we initialized and need to do nothing
    private bool m_initialized = false;

    void Update()
    {
        // to make sure this updates when we are not in focus too..
        if (EditorApplication.isPlaying)
            Repaint();
    }
}
