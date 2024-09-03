using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using Triggers;
using UI;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Creature : MonoBehaviour
{
    private bool _chasing;
    public Transform[] positions;
    public Transform currentTarget;
    private DemonAudio _audio;
    // Start is called before the first frame update
    void Start()
    {
        _audio = GetComponent<DemonAudio>();
        initPos = transform.position;
        chaseRadius = innerChaseRadius;
        navAgent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<Player>();
        lights = new GameObject[2];
        lightOffsets = new Vector3[2];
        Transform[] ChildTransforms = GetComponentsInChildren<Transform>();
        foreach (Transform ChildTransform in ChildTransforms)
        {
            if (ChildTransform.gameObject.name == "Bone.005")
            {
                headTransform = ChildTransform;
            }
            if (ChildTransform.gameObject.name == "EyeLight1")
            {
                lights[0] = ChildTransform.gameObject;
            }
            if (ChildTransform.gameObject.name == "EyeLight2")
            {
                lights[1] = ChildTransform.gameObject;
            }
        }
        lightOffsets[0] = lights[0].transform.position - headTransform.position;
        lightOffsets[1] = lights[1].transform.position - headTransform.position;
        currentTarget = positions[Random.Range(0, positions.Length)];
    }

    void LateUpdate()
    {
        if (bForceReturningHome)
        {
            navAgent.SetDestination(currentTarget.position);
            if (Vector3.Distance(transform.position, currentTarget.position) < innerChaseRadius * 0.5f)
            {
                bForceReturningHome = false;
            }
        }
        else
        {
            bool bHit = Physics.Raycast(
                transform.position,
                (player.transform.position - transform.position).normalized,
                out RaycastHit hit,
                12.0f,
                1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Player"));

            bool bSeePlayer = bHit && hit.transform.CompareTag("Player");
            if (bSeePlayer)
            {
                playerInvisibleTime = 0.0f;
                if (!_chasing)
                {
                    _audio.PlayChaseMusic();
                    UIController.Instance.ShowHint("Hold Shift to Sprint");
                    _chasing = true;
                }
            }
            else
            {
                playerInvisibleTime += Time.deltaTime;
                if (_chasing)
                {
                    _audio.EaseOutMusic();
                    UIController.Instance.HideHint();
                    _chasing = false;
                }
            }

            float playerDist = Vector3.Distance(player.transform.position, transform.position);
            float tetherDist = Vector3.Distance(transform.position, initPos);
            if (playerDist < chaseRadius && tetherDist < tetherDistance && playerInvisibleTime < 3.0f)
            {
                navAgent.SetDestination(player.transform.position);
                chaseRadius = outerChaseRadius;
            }
            else
            {
                if (tetherDist > tetherDistance)
                {
                    bForceReturningHome = true;
                }

                navAgent.SetDestination(currentTarget.position);
                chaseRadius = innerChaseRadius;
            }
            
            if (navAgent.remainingDistance < 0.5f && !_chasing)
            {
                currentTarget = positions[Random.Range(0, positions.Length)];
            }
        }

        animator.SetFloat("Speed", navAgent.velocity.magnitude);

        for (int i = 0; i < 2; ++i)
        {
            Vector3 offset = Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up) * lightOffsets[i];
            lights[i].transform.position = headTransform.position + offset;
        }

        Collider[] hitColliders = new Collider[128];
        int overlapCount = Physics.OverlapSphereNonAlloc(
            transform.position, 
            3.0f, 
            hitColliders,
                1 << LayerMask.NameToLayer("Hanging Bodies")
                | 1 << LayerMask.NameToLayer("Player")
                | 1 << LayerMask.NameToLayer("Lights")
        );
        for (int i = 0; i < overlapCount; ++i)
        {
            if (hitColliders[i].CompareTag("HangingBody"))
            {
                hitColliders[i].GetComponentInParent<HangingBody>().BeginSwing();
            }
            else if (hitColliders[i].CompareTag("Player"))
            {
                player.Respawn();
                bForceReturningHome = true;
                UIController.Instance.HideHint();
            }
            else // lights
            {
                hitColliders[i].GetComponent<LightTrigger>().TurnOffLight();
            }
        }
    }

    public float innerChaseRadius = 12.0f;
    public float outerChaseRadius = 15.0f;
    public float tetherDistance = 20.0f;

    private float playerInvisibleTime=10.0f;
    private float chaseRadius;
    private Vector3 initPos;
    private Transform headTransform;
    public Animator animator;
    private NavMeshAgent navAgent;
    private Player player;
    private NavMeshPath path;
    private GameObject[] lights;
    private Vector3[] lightOffsets;
    private Quaternion headInitOrientation;
    private bool bForceReturningHome;
}
