using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class GoToGoal : Agent
{

    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform almacenTransform;
    [SerializeField] private Transform torretaTransform;
    [SerializeField] private Transform torretaBaseTransform;
    public FortalezaHealth saludFortaleza;
    public AlmacenOro saludAlmacenOro;
    public float dañoAFortaleza = 0f;

    public float salud;
    public float saludAnterior;
    public float saludMax;
    public float dinero;
    public bool multiplicarAtaque;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-5.5f,4.5f),5.9f,Random.Range(-18f,-15f));
        targetTransform.localPosition = new Vector3(-0.35f, 5.91f, Random.Range(-4f, +3f));
        almacenTransform.localPosition = new Vector3(-10f, 5.91f, Random.Range(-9f, +3f));
        torretaTransform.localPosition = new Vector3(Random.Range(-10f, 10f), 5.91f, Random.Range(-12f, almacenTransform.localPosition.z-2f));
        torretaBaseTransform.localPosition = new Vector3(torretaTransform.localPosition.x, torretaTransform.localPosition.y, torretaTransform.localPosition.z);

        //Volvemos a inicializar la salud del enemigo, la fortaleza y el almacen
        salud = saludMax;
        saludAnterior = salud;
        saludFortaleza.health = saludFortaleza.maxHealth;
        saludAlmacenOro.health = saludAlmacenOro.maxHealth;

        //Pierde el dinero recaudado
        dinero = 0f;
        //Pierde las ventajas que tenia
        multiplicarAtaque = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
        sensor.AddObservation(almacenTransform.localPosition);
        sensor.AddObservation(torretaTransform.localPosition);
    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        dañoAFortaleza = actions.ContinuousActions[2];

        float moveSpeed = 10f;
        
        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
        
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
        continuousActions[2] = 1;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Fortaleza>(out Fortaleza fortaleza))
        {
            AddReward(+1f);

            //Si ha llegado hasta el objetivo le hará daño
            if (multiplicarAtaque) //Si tiene la bonificación de ataque
            {
                saludFortaleza.health -= 2f * Mathf.Abs(dañoAFortaleza);
                AddReward(+2f*Mathf.Abs(dañoAFortaleza));
            }
            else
            {
                saludFortaleza.health -= Mathf.Abs(dañoAFortaleza);
                AddReward(+Mathf.Abs(dañoAFortaleza));
            }

            if (saludFortaleza.health <= 0)
            {
                EndEpisode();
            }
        }
        if (other.TryGetComponent<Muro>(out Muro muro)) //Si se choca con un muro acaba el episodio
        {
            SetReward(-1f);
            EndEpisode();
        }
        if (other.TryGetComponent<AlmacenOro>(out AlmacenOro almacen)) //Si se choca con un almacen de oro roba dinero
        {
            //Si ha llegado hasta el almacen le hará daño
            if (saludAlmacenOro.health > 0)
            {
                saludAlmacenOro.health -= Mathf.Abs(dañoAFortaleza);
                AddReward(+Mathf.Abs(dañoAFortaleza));
                dinero += Mathf.Abs(dañoAFortaleza);
            }
            
        }
    }

    private void Update()
    {
        if(salud < saludAnterior){
            AddReward(salud-saludAnterior);
            saludAnterior = salud;
        }

        if (salud <= 0)
        {
            EndEpisode();
        }

        if (dinero >= saludAlmacenOro.maxHealth)
        {
            multiplicarAtaque = true;
        }
    }
}
