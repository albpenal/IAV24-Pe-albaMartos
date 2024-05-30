using System.Collections;
using System.Collections.Generic;
using UCM.IAV.Movimiento;
using UnityEngine;

public class PlayerLife : MonoBehaviour
{
    [SerializeField]
    private int life = 100;

    [SerializeField]
    private AudioSource hitSound;

    [SerializeField]
    private AudioSource gameMusic;

    private void Awake()
    {
        gameMusic.Play();
    }
    public void Damage(int dam)
    {
        hitSound.Play();
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
