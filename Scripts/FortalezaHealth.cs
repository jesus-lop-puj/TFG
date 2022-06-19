using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FortalezaHealth : MonoBehaviour
{
    public float health;
    public float maxHealth;

    public GameObject healthBarUI;
    public Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        slider.value = calculateHealth();
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = calculateHealth();

        if (health <= 0) //Si la salud se reduce a cero destruimos el objeto asociado (Fortaleza)
        {
            Destroy(gameObject);
        }

        Debug.Log(health);

    }

    //Funcion para calcular la salud actual
    //Devuelve un valor entre 0 y 1 (que son los valores que toma el slider)
    float calculateHealth()
    {
        return health / maxHealth;
    }

}
