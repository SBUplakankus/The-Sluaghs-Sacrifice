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
    }

    // Update is called once per frame
    void Update()
    {
        navAgent.SetDestination(player.transform.position);
    }

    private NavMeshAgent navAgent;
    private Player player;
    private NavMeshPath path;
}
