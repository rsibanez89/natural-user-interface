using UnityEngine;
using System.Collections;
using OpenNI;

public class TexturePosition : MonoBehaviour {
	
	public NIMapViewerBaseUtility.ScreenSnap m_snap;
	
	public Rect m_placeToDraw;
	
	public GUIText label1;
	
	// Use this for initialization
	void Start () {
		transform.position = new Vector3(5.0f, Screen.height - 12 ,0.0f); 
	}
	
	void OnGUI()
    {
		Rect posToPut = m_placeToDraw;
		switch(m_snap)
		{
            case NIMapViewerBaseUtility.ScreenSnap.UpperRightCorner:
			{
				posToPut.x=Screen.width-m_placeToDraw.x-m_placeToDraw.width;
				break;
			}
            case NIMapViewerBaseUtility.ScreenSnap.LowerLeftCorner:
			{
				posToPut.y=Screen.height-m_placeToDraw.y-m_placeToDraw.height;
				break;
			}
            case NIMapViewerBaseUtility.ScreenSnap.LowerRightCorner:
			{
				posToPut.x=Screen.width-m_placeToDraw.x-m_placeToDraw.width;
				posToPut.y=Screen.height-m_placeToDraw.y-m_placeToDraw.height;
				break;
			}
			
		}
		GUI.BeginGroup(posToPut);
        GUI.Box(new Rect(0, 0, m_placeToDraw.width, m_placeToDraw.height), label1.text);
		GUI.EndGroup();
	}
}
