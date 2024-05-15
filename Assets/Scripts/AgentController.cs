using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AgentController : Agent
{
    // Key
    [SerializeField] Transform _keyTransform;
    [SerializeField] GameObject _keyObject;

    // Door
    [SerializeField] Transform _doorTransform;
    [SerializeField] GameObject _doorObject;
    [SerializeField] Animator anim;

    // Agent
    [SerializeField] Transform _agentTransform;
    Vector3 _spawnPoint = new(-20f, 1f, 23f);
    [SerializeField] float _moveSpeed = 5f;
    
    const float _maxDistance = 70.71067f;
    int _episodeCount = 0;

    public override void OnEpisodeBegin()
    {
        anim.ResetTrigger("KeyTrigger");
        _keyObject.SetActive(true);
        transform.localPosition = _spawnPoint;
        transform.rotation = Quaternion.Euler(0, 180f, 0f);

        // All possible key locations
        Vector3 _1stSpawnPoint = new(-8.89f, 0.5f, -9.21f);
        Vector3 _2ndSpawnPoint = new(-23.14f, 0.5f, -22f);
        Vector3 _3rdSpawnPoint = new(-3.08f, 0.5f, -0.1f);
        Vector3 _4thSpawnPoint = new(7.26f, 0.5f, 14f);
        Vector3 _5thSpawnPoint = new(22.57f, 0.5f, -17.39f);
        Vector3 _6thSpawnPoint = new(17.32f, 0.5f, 16.84f);
        Vector3 _7thSpawnPoint = new(5.95f, 0.5f, 14.14f);
        Vector3 _8thSpawnPoint = new(-7.47f, 0.5f, 14.79f);
        Vector3 _9thSpawnPoint = new(-7.47f, 0.5f, 10.01f);
        Vector3 _10thSpawnPoint = new(-7.47f, 0.5f, 22.01f);
        Vector3 _11thSpawnPoint = new(-22.1f, 0.5f, 8.46f);

        // Select random key location for every 100th episode
        if (_episodeCount % 100 == 0)
        {
            int _randomNumber = Random.Range(0, 11);
            switch (_randomNumber)
            {
                case 0:
                    _keyTransform.localPosition = _1stSpawnPoint;
                    break;
                case 1:
                    _keyTransform.localPosition = _2ndSpawnPoint;
                    break;
                case 2:
                    _keyTransform.localPosition = _3rdSpawnPoint;
                    break;
                case 3:
                    _keyTransform.localPosition = _4thSpawnPoint;
                    break;
                case 4:
                    _keyTransform.localPosition = _5thSpawnPoint;
                    break;
                case 5:
                    _keyTransform.localPosition = _6thSpawnPoint;
                    break;
                case 6:
                    _keyTransform.localPosition = _7thSpawnPoint;
                    break;
                case 7:
                    _keyTransform.localPosition = _8thSpawnPoint;
                    break;
                case 8:
                    _keyTransform.localPosition = _9thSpawnPoint;
                    break;
                case 9:
                    _keyTransform.localPosition = _10thSpawnPoint;
                    break;
                case 10:
                    _keyTransform.localPosition = _11thSpawnPoint;
                    break;
                default:
                    break;
            }
        }
        _episodeCount++;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.z);

        sensor.AddObservation(_keyTransform.localPosition.x);
        sensor.AddObservation(_keyTransform.localPosition.z);

        sensor.AddObservation(_doorTransform.localPosition.x);
        sensor.AddObservation(_doorTransform.localPosition.z);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        var actionTaken = actions.ContinuousActions;

        float _actionSpeed = (actionTaken[0] + 1) / 2;
        float _actionSteering = actionTaken[1];
        transform.Translate(_actionSpeed * Vector3.forward * _moveSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up, _actionSteering * 180f * Time.deltaTime);

        float _keyToAgentDist = Vector3.Distance(_keyTransform.localPosition, transform.localPosition) / _maxDistance;
        float _doorToAgentDist = Vector3.Distance(_doorTransform.localPosition, transform.localPosition) / _maxDistance;

        if (_keyObject.activeSelf)
        {
            AddReward(-_keyToAgentDist / 10);
        }
        else
        {
            AddReward(-_doorToAgentDist / 10);
        }

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
            AddReward(2f);
            anim.SetTrigger("KeyTrigger");
            _keyObject.SetActive(false);

        }
        if (collider.CompareTag("Door"))
        {
            AddReward(4f);
            EndEpisode(); 
        }

        if (collider.CompareTag("Wall"))
        {
            AddReward(-0.01818f);
            EndEpisode();
        }
    }
}