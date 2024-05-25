/*    
   Copyright (C) 2020-2023 Federico Peinado
   http://www.federicopeinado.com
   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).
   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DropDown : MonoBehaviour
{
    public bool gameMode = false;
    void Start()
    {
       // Establece changeSize al OnValueChanged del Dropdown
        if(!gameMode)
            gameObject.GetComponent<Dropdown>().onValueChanged.AddListener(delegate { UCM.IAV.Movimiento.GameManager.instance.ChangeName(); });
       else
            gameObject.GetComponent<Dropdown>().onValueChanged.AddListener(delegate { UCM.IAV.Movimiento.GameManager.instance.setNumMinos(); });

    }
}
