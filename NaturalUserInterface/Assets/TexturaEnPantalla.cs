using UnityEngine;
using System.Collections;

public class TexturaEnPantalla: MonoBehaviour {
	
	//Variables publicas para que puedan ser editadas desde el editor de Unity
	public Rect areaDeDibujo = new Rect(0,0,275,206.25f);
	public Rect areaDeDibujoTexto = new Rect(0,0,275,50);
	
	public bool valido = false;
	public Texture2D [] texturaDelRectangulo;
	public Texture2D [] texturaTexto;
	public int pos = 14;
	public int posTexto = 0;
	public enum Ubicacion {
        UpperLeftCorner,
        UpperRightCorner,
        LowerLeftCorner,
        LowerRightCorner
    };
	
	public Ubicacion ubicacion;
	
	// Sobreescribo el método, para poder usar la GUI.
	void OnGUI() {
		if(valido){
			Rect posicionDondeDibujar = areaDeDibujo; //por defecto
			Rect posicionDondeDibujarTexto = areaDeDibujoTexto; //por defecto
			switch (ubicacion) {
	           	case Ubicacion.UpperRightCorner:
	                {
						posicionDondeDibujar.x = Screen.width - areaDeDibujo.x - areaDeDibujo.width;
	                	break;	
	                }
	            case Ubicacion.LowerLeftCorner:
	                {
	                    posicionDondeDibujar.y = Screen.height - areaDeDibujo.y - areaDeDibujo.height;						
				 		//posicionDondeDibujar.y = Screen.height - areaDeDibujo.width - 20;						
						posicionDondeDibujarTexto.y = Screen.height - areaDeDibujoTexto.height - areaDeDibujoTexto.y - areaDeDibujo.height;						
	                    break;
	                }
	            case Ubicacion.LowerRightCorner:
	                {
	                    posicionDondeDibujar.x = Screen.width - areaDeDibujo.x - areaDeDibujo.width;
	                    posicionDondeDibujar.y = Screen.height - areaDeDibujo.y - areaDeDibujo.height;
	                    break;
	                }
	        }
			GUI.DrawTexture(posicionDondeDibujar, texturaDelRectangulo[pos], ScaleMode.StretchToFill);			
			GUI.DrawTexture(posicionDondeDibujarTexto, texturaTexto[posTexto], ScaleMode.StretchToFill);		
		}
	}
}