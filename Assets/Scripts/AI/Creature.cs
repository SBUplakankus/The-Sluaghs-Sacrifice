using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

public class Creature : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
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
    }

    void LateUpdate()
    {
         navAgent.SetDestination(player.transform.position);
         animator.SetFloat("Speed", navAgent.speed);
         if (navAgent.speed > 0.01f)
         {
             animator.speed = 0.75f;
         }
         else
         {
             animator.speed = 1.0f;
         }
         for (int i = 0; i < 2; ++i)
         {
             Vector3 offset = Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up) * lightOffsets[i];
             lights[i].transform.position = headTransform.position + offset;
         }
    }

    private Transform headTransform;
    public Animator animator;
    private NavMeshAgent navAgent;
    private Player player;
    private NavMeshPath path;
    private GameObject[] lights;
    private Vector3[] lightOffsets;
    private Quaternion headInitOrientation;
}
