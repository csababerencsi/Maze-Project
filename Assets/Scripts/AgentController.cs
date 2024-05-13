using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AgentController : Agent
{
    [SerializeField] Transform _keyTransform;
    [SerializeField] Transform _doorTransform;
    [SerializeField] Transform _agentTransform;

    [SerializeField] GameObject _keyObject;
    [SerializeField] float _moveSpeed = 10f;
    Vector3 _spawnPoint = new(-20f, 1f, 23f);
    [SerializeField] Animator anim;

    float _maxKeyReward = 2f;
    float _maxDoorReward = 4f;
    float _minReward = -1f;
    float totalReward;

    private void FixedUpdate()
    {
        float _agentToKeyDistance = Vector3.Distance(_agentTransform.position, _keyTransform.position);
        float _agentToDoorDistance = Vector3.Distance(_agentTransform.position, _doorTransform.position);

        float keyReward = Mathf.Clamp(_maxKeyReward - _agentToKeyDistance, _minReward, _maxKeyReward);
        float doorReward = Mathf.Clamp(_maxDoorReward - _agentToDoorDistance, _minReward, _maxDoorReward);

        totalReward = keyReward + doorReward;
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = _spawnPoint;
        transform.rotation = Quaternion.Euler(0, 180f, 0f);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.z);
        if (_keyTransform != null)
        {
            sensor.AddObservation(_keyTransform.localPosition.x);
            sensor.AddObservation(_keyTransform.localPosition.z);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var actionTaken = actions.ContinuousActions;

        float _actionSpeed = (actionTaken[0] + 1) / 2;
        float _actionSteering = actionTaken[1];
        transform.Translate(_actionSpeed * Vector3.forward * _moveSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up, _actionSteering * 180f * Time.deltaTime);
        AddReward(totalReward);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> actions = actionsOut.ContinuousActions;
        actions[0] = -1; // Vertical
        actions[1] = 0; // Horizontal


        if (Input.GetKey(KeyCode.W))
        {
            actions[0] = +1;
        }

        if (Input.GetKey(KeyCode.A))
        {
            actions[1] = -1;
        }

        if (Input.GetKey(KeyCode.S))
        {
            actions[0] = -1;
        }

        if (Input.GetKey(KeyCode.D))
        {
            actions[1] = +1;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Key"))
        {
            AddReward(2);
            anim.SetTrigger("KeyTrigger");
            Destroy(_keyObject.gameObject);

        }
        if (collider.CompareTag("Door"))
        {
            AddReward(4);
            EndEpisode();
            anim.ResetTrigger("KeyTrigger");
        }

        if (collider.CompareTag("Wall"))
        {
            AddReward(-1);
            EndEpisode();
        }
    }
}