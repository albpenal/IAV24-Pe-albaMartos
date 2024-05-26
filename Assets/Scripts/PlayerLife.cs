using System.Collections;
using System.Collections.Generic;
using UCM.IAV.Movimiento;
using UnityEngine;

public class PlayerLife : MonoBehaviour
{
    [SerializeField]
    private int life = 100;

    public void Damage(int dam)
    {
        life -= dam;
        if(life <= 0)
        {
            life = 0;
            // setear texto de derrota y volver al menú 
            GameManager.instance.GameOver();
        }
    }
    
    public int getLife()
    {
        return life;
    }
}
