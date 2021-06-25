using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{
    public static event System.Action OnGuardSpotted;
    public Transform PathFinder;
    public float speed = 1;
    public float waittime = .2f;
    public float timeToSpotThePlayer = 0.5f;
    public Light SpotLight;
    public float viewDistance;
    float viewAngle;
    public LayerMask viewmask;
    Transform player;
    Color originalSpotLightColor;
    float playerVisibleTimer;
    
    public float turnspeed = 90;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        viewAngle = SpotLight.spotAngle;
        originalSpotLightColor = SpotLight.color;
        Vector3[] waypoints = new Vector3[PathFinder.childCount];
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = PathFinder.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }

        StartCoroutine(FollowPath(waypoints));
    }
    void Update()
    {
        if(CanSeePlayer())
        {
            playerVisibleTimer += Time.deltaTime;
        }
        else
        {
            playerVisibleTimer -= Time.deltaTime;
        }
        playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, timeToSpotThePlayer);
        SpotLight.color = Color.Lerp(originalSpotLightColor, Color.red, playerVisibleTimer/timeToSpotThePlayer);
        if(playerVisibleTimer>=timeToSpotThePlayer)
        {
            if(OnGuardSpotted!=null)
            {
                OnGuardSpotted();
            }
        }
    }
    bool CanSeePlayer()
    {
        if(Vector3.Distance(transform.position,player.position)<viewDistance)
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if(angleBetweenGuardAndPlayer<viewAngle/2f)
            {
                if(!Physics.Linecast(transform.position,player.position,viewmask))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private IEnumerator FollowPath(Vector3[] waypoints)
    {
        transform.position = waypoints[0];
        int i = 1;
        Vector3 targetWaypoint = waypoints[i];
        transform.LookAt(targetWaypoint);
        while (true)
        {

            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);
            if (transform.position == targetWaypoint)
            {
                i = (i + 1) % waypoints.Length;
                targetWaypoint = waypoints[i];
                yield return new WaitForSeconds(waittime);
                yield return StartCoroutine(Look(targetWaypoint));
            }
            yield return null;
        }
        


    }

    private IEnumerator Look(Vector3 looktarget)
    {
        Vector3 direction = (looktarget - transform.position).normalized;
        float targetangle = 90 - (Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg);
        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetangle)) > 0.05f)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetangle, turnspeed * Time.deltaTime);
            transform.eulerAngles = angle * Vector3.up;
            yield return null;
        }

    }

    

    public void OnDrawGizmos()
    {
        Transform previousposition = PathFinder.GetChild(0);
        
        foreach (Transform waypoint in PathFinder)
        {

            Gizmos.DrawSphere(waypoint.position, 0.3f);
            Gizmos.DrawLine(previousposition.position, waypoint.position);
            previousposition = waypoint;
        }
        Gizmos.DrawLine(PathFinder.GetChild(0).position, previousposition.position);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward*viewDistance);
    }


}