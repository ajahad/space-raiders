using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCController : MonoBehaviour
{
    [SerializeField] private ShipMotor motor;

    [Header("Target")]
    [SerializeField] private Transform playerShip;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float arrivalRadius = 5f;

    [Header("Tuning")]
    [SerializeField] private float pitchSensitivity = 2f;
    [SerializeField] private float yawSensitivity   = 2f;
    [SerializeField] private float rollSensitivity  = 1f;
    [SerializeField] private float throttleDeadzone = 30f;

    [Header("Radiuses/Radii???")]
    [SerializeField] private float chaseRange = 80f;
    [SerializeField] private float attackRange = 2f;

    private float pitch, yaw, roll, thrust, strafe;

    private enum NPCState {Idle, Patrol, Chase, Attack}
    private NPCState state = NPCState.Patrol;

    private int waypointIndex = 0;

    private void Awake()
    {
        if (motor == null)
            motor = GetComponent<ShipMotor>();
    }

    private void Update()
    {
        UpdateState();
        UpdateSteering();
        //CheckDeath();
    }

    private void UpdateState()
    {
        float distance = Vector3.Distance(transform.position, playerShip.position);

        state = distance < attackRange ? NPCState.Attack :
        distance < chaseRange ? NPCState.Chase : 
        waypoints.Length > 0 ? NPCState.Patrol :
        NPCState.Idle;
    }

    private void FixedUpdate()
    {
        motor.SetInput(thrust, strafe, 0f, pitch, yaw, roll);
    }

    private void UpdateSteering()
    {
        Transform target = GetCurrentTarget();

        if (target == null || state == NPCState.Idle)
        {
            thrust = pitch = yaw = roll = strafe = 0f;
            return;
        }

        Vector3 toTarget = target.position - transform.position;
        float distance = toTarget.magnitude;
        Vector3 localToTarget = transform.InverseTransformDirection(toTarget.normalized); //Claude
        float angleOff = Vector3.Angle(transform.forward, toTarget);

        pitch = Mathf.Clamp(-localToTarget.y * pitchSensitivity, -1f, 1f);
        yaw = Mathf.Clamp( localToTarget.x * yawSensitivity,   -1f, 1f);
        roll = Mathf.Clamp(-localToTarget.x * rollSensitivity,  -1f, 1f);
        thrust = (distance > arrivalRadius && angleOff < throttleDeadzone) ? 1f : 0f;
        strafe = 0f;

        if (state == NPCState.Patrol && distance < arrivalRadius)
            waypointIndex = (waypointIndex + 1) % waypoints.Length;
    }

    private Transform GetCurrentTarget(){
        switch (state) {
            case NPCState.Chase:
                return playerShip;
            case NPCState.Patrol:
                return waypoints.Length > 0 ? waypoints[waypointIndex] : null;
            default:
                return null;
        }
    }

    private void CheckDeath(){
        if(state == NPCState.Attack)
            SceneManager.LoadScene("GameOver");
    }

    private void OnTriggerEnter(Collider other){
        if(other.CompareTag("Asteroid"))
            Destroy(gameObject);
    }
}