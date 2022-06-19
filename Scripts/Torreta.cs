using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torreta : MonoBehaviour
{
    public GoToGoal enemigo;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Se reduce la salud del enemigo según la función gaussiana
        if (enemigo.salud > 0)  
        {
            float x = Mathf.Pow((enemigo.transform.localPosition.x - transform.localPosition.x),2)/2;
            float y = Mathf.Pow((enemigo.transform.localPosition.y - transform.localPosition.y),2)/2;
            enemigo.salud-= Mathf.Exp(-(x+y));

            transform.LookAt(enemigo.transform);
        }
        
    }
}
