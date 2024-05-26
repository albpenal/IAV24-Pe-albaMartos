using System.Collections;
using System.Collections.Generic;
using UCM.IAV.Movimiento;
using UnityEngine;

public class PlayerLife : MonoBehaviour
{
    [SerializeField]
    private int life = 100;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
}
