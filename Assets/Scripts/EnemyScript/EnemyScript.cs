using UnityEngine;
using UnityEngine.AI;

// Tandaan: Dinagdag natin ang ", IDamageable" dito
public class EnemyScript : MonoBehaviour, IDamageable
{
    public NavMeshAgent agent;
    public Transform player;

    [Header("Layer Masks")]
    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;

    // --- BAGONG SECTION: Para sa Health ng Kalaban ---
    [Header("Enemy Stats")]
    public float health = 100f;

    [Header("Patrolling")]
    public float walkPointRange = 10f;
    private Vector3 walkPoint;
    private bool walkPointSet;

    [Header("Attacking")]
    public float timeBetweenAttacks = 2f;
    public int attackDamage = 10;
    private bool alreadyAttacked;

    [Header("States")]
    public float sightRange = 15f;
    public float attackRange = 2f;
    private bool playerInSightRange;
    private bool playerInAttackRange;

    private PlayerHealthShield playerHealth;

    private void Awake()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            playerHealth = playerObject.GetComponent<PlayerHealthShield>();
        }
        else
        {
            Debug.LogError("EnemyScript cannot find the Player. Make sure your player is tagged 'Player'.");
        }
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (playerInAttackRange) { AttackPlayer(); }
        else if (playerInSightRange) { ChasePlayer(); }
        else { Patrolling(); }
    }

    // --- BAGONG METHOD: Ito ang function na galing sa IDamageable ---
    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log($"{transform.name} took {amount} damage. Health is now {health}.");

        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{transform.name} has been destroyed.");
        // Pwede kang magdagdag ng death animation o effects dito
        Destroy(gameObject);
    }

    // --- Ang mga natitirang methods ay pareho lang ---
    private void Patrolling()
    {
        if (!walkPointSet) { SearchWalkPoint(); }
        else { agent.SetDestination(walkPoint); }

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            walkPointSet = false;
        }
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(walkPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
            walkPoint = hit.position;
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}