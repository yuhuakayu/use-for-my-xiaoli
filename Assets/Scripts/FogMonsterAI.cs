using UnityEngine;

public class FogMonsterAI : MonoBehaviour
{
    [Header("Patrol")]
    public Transform[] waypoints;
    public float patrolSpeed = 1.4f;
    public float waypointReachDistance = 0.35f;

    [Header("Chase")]
    public Transform player;
    public float chaseSpeed = 3.3f;
    public float detectionRange = 8f;
    public float loseRange = 12f;

    private FogMonster fogMonster;
    private int waypointIndex;
    private bool isChasing;

    private void Start()
    {
        fogMonster = GetComponent<FogMonster>();

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
        {
            return;
        }

        UpdateState();

        if (isChasing && player != null)
        {
            MoveTo(player.position, chaseSpeed);
        }
        else
        {
            Patrol();
        }
    }

    private void UpdateState()
    {
        if (player == null)
        {
            isChasing = false;
            return;
        }

        bool revealed = fogMonster == null || fogMonster.IsRevealed;
        float distance = Vector3.Distance(transform.position, player.position);

        if (!isChasing && revealed && distance <= detectionRange)
        {
            isChasing = true;
        }
        else if (isChasing && (!revealed || distance >= loseRange))
        {
            isChasing = false;
        }
    }

    private void Patrol()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            return;
        }

        Transform waypoint = waypoints[waypointIndex];
        MoveTo(waypoint.position, patrolSpeed);

        if (Vector3.Distance(transform.position, waypoint.position) <= waypointReachDistance)
        {
            waypointIndex = (waypointIndex + 1) % waypoints.Length;
        }
    }

    private void MoveTo(Vector3 targetPosition, float speed)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.01f)
        {
            return;
        }

        transform.position += direction.normalized * speed * Time.deltaTime;
        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
    }
}
