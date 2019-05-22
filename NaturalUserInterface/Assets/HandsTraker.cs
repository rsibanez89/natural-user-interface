using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OpenNI;


public class HandsTraker : MonoBehaviour {
	
	public NIPointTrackerManager manager;
	public NISkeletonTracker Skeleton_LeftShoulder;
	public NISkeletonTracker Skeleton_LeftElbow;
	public NISkeletonTracker Skeleton_LeftHand;
	public NISkeletonTracker Skeleton_RightShoulder;
	public NISkeletonTracker Skeleton_RightElbow;
	public NISkeletonTracker Skeleton_RightHand;
	public NISkeletonTracker Skeleton_Neck;
	
	private NIPointTracker LeftShoulder;
	private NIPointTracker LeftElbow;
	private NIPointTracker LeftHand;
	private NIPointTracker RightShoulder;
	private NIPointTracker RightElbow;
	private NIPointTracker RightHand;
	private NIPointTracker Neck;
	
	List<Vector3> positionListLeftShoulder = new List<Vector3>();
	List<Vector3> positionListLeftElbow = new List<Vector3>(); 
	List<Vector3> positionListLeftHand = new List<Vector3>(); 
	List<Vector3> positionListRightShoulder = new List<Vector3>(); 
	List<Vector3> positionListRightElbow = new List<Vector3>(); 
	List<Vector3> positionListRightHand = new List<Vector3>(); 
	List<Vector3> positionListNeck = new List<Vector3>(); 
	
	public TexturaEnPantalla gesto;
	public GameObject background;
	public Texture2D [] texturas;
	public GameObject [] elementos;
	private GameObject elemento;
	private Vector3 anteriorHandIz;
	private Vector3 anteriorHandDe;
	private Aplicacion aplicacion = Aplicacion.edicion;
	public Accion accion;
	private float newTime = 0f;

	private float escala;
	private int posObjetos = 0;
	private float rotaX;
	private float rotaY;
	private float rotaZ;	
	private Accion objetoSeleccionado;
	private int tiempoMenu = T;
	
	List<Accion> gestureAcceptedList = new List<Accion>();
	const float SwipeMinimalLength = 90.0f; //0.4f; 
	const float SwipeMaximalHeight = 30.0f;//0.2f
	const int SwipeMininalDuration = 250; 
	const int SwipeMaximalDuration = 1500; 
	//DateTime lastGestureDate = DateTime.Now; 
	
	const int bufferMuestreo=20;
	float lastGestureDate;
	int MinimalPeriodBetweenGestures = 0;
	
	const float guiaMaximalZ = 250.0f;
	const float guiaMaximalY = 200.0f;
	const float guiaMinimalY = 100.0f;
	const float guiaMaximalX = 200.0f;
	const float guiaMinimalX = 100.0f;
	const int T = 250;
	
	public enum Aplicacion {
		menu,
		edicion
	};
	
	public enum Accion {
		irAMenu,
		mover,
		moverIzq,
		moverDer,
		rotar,
		escalar,
		guia
	};
	
	// Use this for initialization
	void Start () {
		initTrakers();
		
		elemento = (GameObject) Instantiate((GameObject)elementos[posObjetos]);
		background.renderer.material.mainTexture = texturas[posObjetos];
		
		escala = elemento.transform.localScale.x;
		rotaX = elemento.transform.localRotation.x;
		rotaY = elemento.transform.localRotation.y;
		rotaZ = elemento.transform.localRotation.z;
		
	}
	
	void initTrakers(){	
		LeftShoulder = manager.GetTracker(Skeleton_LeftShoulder.GetTrackerType());
		LeftElbow = manager.GetTracker(Skeleton_LeftElbow.GetTrackerType());
		LeftHand = manager.GetTracker(Skeleton_LeftHand.GetTrackerType());
		RightShoulder = manager.GetTracker(Skeleton_RightShoulder.GetTrackerType());
		RightElbow = manager.GetTracker(Skeleton_RightElbow.GetTrackerType());
		RightHand = manager.GetTracker(Skeleton_RightHand.GetTrackerType());
		Neck = manager.GetTracker(Skeleton_Neck.GetTrackerType());
	}
		

	// Update is called once per frame
	void Update () {
	//	if(validTrakers()){
				//Normalizo los puntos.
				positionListRightHand.Add(new Vector3(RightHand.CurPos.x - Neck.CurPos.x, RightHand.CurPos.y - Neck.CurPos.y, RightHand.CurPos.z - Neck.CurPos.z)); 
				positionListLeftHand.Add(new Vector3(LeftHand.CurPos.x - Neck.CurPos.x, LeftHand.CurPos.y - Neck.CurPos.y, LeftHand.CurPos.z - Neck.CurPos.z)); 
			
				if(aplicacion == Aplicacion.edicion){
					if(gestoGuia()){
						accion = Accion.guia;					
						gesto.pos = 0;
						clearLists();
					}
					
					if(accion == Accion.guia){
						accion = detectarGesto();							
					}				
	                			
					
					if (accion == Accion.irAMenu){						
						clearLists();
						tiempoMenu -=1;
						int valor = ((int)(tiempoMenu/50));
						gesto.pos = 8 - valor;		
											
						if (tiempoMenu == 0){
							tiempoMenu = T;
							aplicacion = Aplicacion.menu;
							gesto.posTexto = 1;
						}
					}
					else
						tiempoMenu = T;
					
					if (accion == Accion.escalar)	{
						gesto.pos = 1;
						clearLists();
						escalar();
					}
					
					if (accion == Accion.mover)	{
						gesto.pos = 3;
						clearLists();
						mover();
					}
					
					if (accion == Accion.rotar)	{
						gesto.pos = 2;
						clearLists();
						rotar();
					}
				}
				else
				{
					if(gestoGuia()){
						accion = Accion.guia;
						clearLists();
					}
					
					if(accion == Accion.guia){//estoy en menu					
						gesto.pos = 0;
						accion = detectarGestoEnMenu();							
					}
					
									
					if (accion == Accion.irAMenu){
						clearLists();
						tiempoMenu -=1;
						int valor = ((int)(tiempoMenu/50));
						gesto.pos = 13 - valor;
										
						if (tiempoMenu == 0){
							tiempoMenu = T;
							aplicacion = Aplicacion.edicion;	
							gesto.posTexto = 0;
						}
					}
					else
						tiempoMenu = T;
					
					if (accion == Accion.moverIzq)	{
						//Destroy(elemento.gameObject,2);//2 seg
						Destroy(elemento.gameObject);
						posObjetos += 1; 
						if (posObjetos	== elementos.Length)
								posObjetos = 0;
						gesto.pos = 3;
						elemento = (GameObject) Instantiate((GameObject)elementos[posObjetos]);	
						background.renderer.material.mainTexture = texturas[posObjetos];
						clearLists();
						accion = Accion.guia;
					}
					if (accion == Accion.moverDer)	{
						//Destroy(elemento.gameObject,2);//2 seg
						Destroy(elemento.gameObject);
						posObjetos = (posObjetos - 1);
						if (posObjetos	== -1)
								posObjetos = elementos.Length-1;
						gesto.pos = 3;
						elemento = (GameObject) Instantiate((GameObject)elementos[posObjetos]);
						background.renderer.material.mainTexture = texturas[posObjetos];
						clearLists();
						accion = Accion.guia;
					}
					
				}
				if (positionListRightHand.Count > bufferMuestreo) 
					{ 
					    positionListRightHand.RemoveAt(0);
						positionListLeftHand.RemoveAt(0);					
					}
			
		//}
		//else
		//	initTrakers();
	
		
	}
	
	private void clearLists(){
		positionListRightHand.Clear();
		positionListLeftHand.Clear();
	}
		
	
	private bool gestoMover(){
		int start = 0; 
		if (-(Neck.CurPos.z - LeftHand.CurPos.z) < guiaMaximalZ){
            for (int index = 0; index < positionListRightHand.Count-1; index++) 
            { 
				
				if ( (-(Neck.CurPos.z - positionListLeftHand[index].z) < guiaMaximalZ) || (UnityEngine.Mathf.Abs(positionListRightHand[start].y - positionListRightHand[index].y) > SwipeMaximalHeight))
                { 
                    start = index; 
                }
				else 
				{
	                if ((UnityEngine.Mathf.Abs(positionListRightHand[index].x - positionListRightHand[start].x) > SwipeMinimalLength)) 
						
	                {  	
						return true;
	                    
					}
				}
            }
		}
		return false;        
	}		
	
	private bool gestoGuia(){
		//print("RightHand.CurPos.z" + RightHand.CurPos.z);
		//print("Neck.CurPos.z" + Neck.CurPos.z);
		if( (-(Neck.CurPos.z - RightHand.CurPos.z) < guiaMaximalZ) && (-(Neck.CurPos.z - LeftHand.CurPos.z) < guiaMaximalZ) )
			return true;
		return false;
	}
		
	private bool gestoMenu(){ 
		
	   // print("RightHand" + (RightHand.CurPos.x - RightElbow.CurPos.x));
       if( (-(Neck.CurPos.z - RightHand.CurPos.z) < guiaMaximalZ) && (-(Neck.CurPos.z - LeftHand.CurPos.z) < guiaMaximalZ) &&  
		   (UnityEngine.Mathf.Abs(Neck.CurPos.y - RightHand.CurPos.y) < 50.0f) && ((UnityEngine.Mathf.Abs(Neck.CurPos.y - RightElbow.CurPos.y) < 50.0f))
		   && (RightHand.CurPos.x - RightElbow.CurPos.x > guiaMaximalX) )
			return true;
		return false;
     }
		
	private bool gestoEscalar(){ 
        int start = 0; 
        for (int index = 0; index < positionListRightHand.Count-1; index++) 
        { 
		    // print("lefft dif X [" + index + "," + index + "]" + (positionListLeftHand[index].x - positionListLeftHand[index + 1].x ));
			//print("STRART - INDEX: " + (index - start));
		//print("Dif Y [" + start + "," + index + "]" + (UnityEngine.Mathf.Abs(positionList[start].y - positionList[index].y)));
			if ((UnityEngine.Mathf.Abs(positionListRightHand[start].y - positionListRightHand[index].y) > SwipeMaximalHeight*2.0f) 
		    	||(UnityEngine.Mathf.Abs(positionListLeftHand[start].y - positionListLeftHand[index].y) > SwipeMaximalHeight*2.0f) 
		    	||((positionListRightHand[index].x - positionListRightHand[index + 1].x) > -0.01f) 
		    	||((positionListLeftHand[index].x - positionListLeftHand[index + 1].x) < 0.01f))
            { 
                start = index; 
            }
			else 
			{
				if ((UnityEngine.Mathf.Abs(positionListRightHand[index].x - positionListRightHand[start].x) > SwipeMinimalLength*0.3f)&&
			        (UnityEngine.Mathf.Abs(positionListLeftHand[index].x - positionListLeftHand[start].x) > SwipeMinimalLength*0.3f)) 
                {   
							return true;
				}
			}

        }
	return false;
    }
	
	private bool gestoRotar(){ 
        int start = 0; 
        for (int index = 0; index < positionListRightHand.Count-1; index++) 
        { 
		    // print("lefft dif X [" + index + "," + index + "]" + (positionListLeftHand[index].x - positionListLeftHand[index + 1].x ));
			//print("STRART - INDEX: " + (index - start));
		//print("Dif Y [" + start + "," + index + "]" + (UnityEngine.Mathf.Abs(positionList[start].y - positionList[index].y)));
			if ((UnityEngine.Mathf.Abs(positionListRightHand[start].x - positionListRightHand[index].x) > SwipeMaximalHeight*2.0f) 
		    	||(UnityEngine.Mathf.Abs(positionListLeftHand[start].x - positionListLeftHand[index].x) > SwipeMaximalHeight*2.0f) 
		    	||((positionListRightHand[index].y - positionListRightHand[index + 1].y) > -0.01f) 
		    	||((positionListLeftHand[index].y - positionListLeftHand[index + 1].y) < 0.01f))
            { 
                start = index; 
            }
			else 
			{
                if ((UnityEngine.Mathf.Abs(positionListRightHand[index].y - positionListRightHand[start].y) > SwipeMinimalLength*0.3f)&&
			        (UnityEngine.Mathf.Abs(positionListLeftHand[index].y - positionListLeftHand[start].y) > SwipeMinimalLength*0.3f)) 
                {   
							return true;
				}
			}

        }
	return false;
    }
	
	private bool gestoDeslizarAIzq(){
		int start = 0; 
		if (-(Neck.CurPos.z - LeftHand.CurPos.z) < guiaMaximalZ){
            for (int index = 0; index < positionListRightHand.Count-1; index++) 
            { 
				
				if ( (-(Neck.CurPos.z - positionListLeftHand[index].z) < guiaMaximalZ) || (UnityEngine.Mathf.Abs(positionListRightHand[start].y - positionListRightHand[index].y) > SwipeMaximalHeight))
                { 
                    start = index; 
                }
				else 
				{
	                if ((- positionListRightHand[index].x + positionListRightHand[start].x) > SwipeMinimalLength)
	                {  	return true;
	                    
					}
				}
            }
		}
		return false;        
	}
	
	private bool gestoDeslizarADer(){
		int start = 0; 
		if (-(Neck.CurPos.z - LeftHand.CurPos.z) < guiaMaximalZ){
            for (int index = 0; index < positionListRightHand.Count-1; index++) 
            { 
				
				if ( (-(Neck.CurPos.z - positionListLeftHand[index].z) < guiaMaximalZ) || (UnityEngine.Mathf.Abs(positionListRightHand[start].y - positionListRightHand[index].y) > SwipeMaximalHeight))
                { 
                    start = index; 
                }
				else 
				{
	                if ((positionListRightHand[index].x - positionListRightHand[start].x) > SwipeMinimalLength)
	                {  	return true;
	                    
					}
				}
            }
		}
		return false;        
	}
	
	private Accion detectarGesto(){
		if (gestoMenu())
			return Accion.irAMenu;
		if (gestoRotar())
			return Accion.rotar;
		if(gestoEscalar())
			return Accion.escalar;
		if(gestoMover())
			return Accion.mover;
		return accion;
	}
	
	private Accion detectarGestoEnMenu(){		
		if (gestoMenu())
			return Accion.irAMenu;
		if (gestoDeslizarAIzq()){	
			return Accion.moverIzq;
		}
		if (gestoDeslizarADer()){	
			return Accion.moverDer;
		}
		return Accion.guia;
	}
	
	private void mover (){
		gestureAcceptedList.Add(Accion.mover);
		float x = elemento.transform.position.x;
		float y = elemento.transform.position.y;
		elemento.transform.position = new Vector3(x + RightHand.CurDeltaPos.x/50.0f, y + RightHand.CurDeltaPos.y/50.0f, elemento.transform.position.z);
	}	
	
	private void rotar(){
	//Quaternion q = new Quaternion();
		
		if(UnityEngine.Mathf.Abs(RightHand.CurPos.x-LeftHand.CurPos.x) > guiaMaximalX){
			if(RightHand.CurPos.x>LeftHand.CurPos.x)
				rotaX += 2.0f;
			else
				rotaX -= 2.0f;
		}
		
		
		if(UnityEngine.Mathf.Abs(RightHand.CurPos.y-LeftHand.CurPos.y) > guiaMaximalX){
			if(RightHand.CurPos.y>LeftHand.CurPos.y)
				rotaY += 2.0f;
			else
				rotaY -= 2.0f;
		}
		/*rotaX = elemento.transform.rotation.x+(RightHand.CurPos.x-LeftHand.CurPos.x)/10;
		float y = elemento.transform.rotation.y+(RightHand.CurPos.y-LeftHand.CurPos.y)/10;
		float z = elemento.transform.rotation.z+(RightHand.CurPos.z-LeftHand.CurPos.z)/10;*/
		//print(rotaX  + "," + rotaY + ", " + rotaZ);
		elemento.transform.rotation = Quaternion.Euler(rotaY,rotaX,rotaZ);	
	}	
	
	private void escalar (){
		gestureAcceptedList.Add(Accion.escalar); 
		float distance = (RightHand.CurPos.x-LeftHand.CurPos.x);
		
		if (distance > 500.0f)
			escala += escala/50.0f;
		if(distance < 300.0f)
			escala -= escala/50.0f;
		
		if (escala < 0.0f)
				escala = 0.0f;
		
		elemento.transform.localScale = new Vector3(escala, escala, escala);
	}	
		
}
