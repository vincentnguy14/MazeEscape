using System.Collections.Generic;
using UnityEngine;

public class MonsterMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Transform player;
    public MazeGenerator mazeGenerator;
    public float detectionRange = 8f;

    // Monster audio
    public AudioSource footstepAudio;
    public AudioSource growlAudio;
    public AudioSource ambientAudio;

    enum MonsterState
    {
        Patrol,
        Chase,
        Search
    }

    MonsterState currentState = MonsterState.Patrol;

    Vector2Int lastKnownPosition;
    float searchTimer = 0f;

    void Awake()
    {
        // Automatically find the Audio Source on the Monster
        if (footstepAudio == null)
        {
            footstepAudio = GetComponent<AudioSource>();
        }

        // Find the growl Audio Source
        if (growlAudio == null)
        {
            AudioSource[] audioSources = GetComponents<AudioSource>();

            if (audioSources.Length > 1)
            {
                growlAudio = audioSources[1];
            }
        }

        // Find the ambient breathing Audio Source
        if (ambientAudio == null)
        {
            AudioSource[] audioSources = GetComponents<AudioSource>();

            if (audioSources.Length > 2)
            {
                ambientAudio = audioSources[2];
            }
        }
    }

    void Update()
    {
        if (player == null || mazeGenerator == null)
            return;

        // Remember where the monster was before moving
        Vector3 previousPosition = transform.position;

        switch (currentState)
        {
            case MonsterState.Patrol:
                Patrol();
                break;

            case MonsterState.Chase:
                Chase();
                break;

            case MonsterState.Search:
                Search();
                break;
        }

        // Check if the monster actually moved
        bool isMoving =
            Vector3.Distance(previousPosition, transform.position) > 0.001f;

        // Play footsteps while moving
        if (isMoving)
        {
            PlayFootsteps();
        }
        else
        {
            StopFootsteps();
        }

        // Update monster breathing based on distance
        UpdateAmbientSound();
    }

    void Patrol()
    {
        if (CanSeePlayer())
        {
            currentState = MonsterState.Chase;

            // Play detected growl
            if (growlAudio != null && !growlAudio.isPlaying)
            {
                growlAudio.Play();
            }

            return;
        }
    }

    void Chase()
    {
        if (!CanSeePlayer())
        {
            lastKnownPosition =
                Vector2Int.RoundToInt(player.position);

            currentState = MonsterState.Search;
            return;
        }

        Vector2Int currentPosition =
            Vector2Int.RoundToInt(transform.position);

        Vector2Int playerPosition =
            Vector2Int.RoundToInt(player.position);

        Vector2Int nextPosition =
            FindNextPosition(currentPosition, playerPosition);

        transform.position = Vector3.MoveTowards(
            transform.position,
            new Vector3(
                nextPosition.x,
                nextPosition.y,
                transform.position.z
            ),
            moveSpeed * Time.deltaTime
        );
    }

    void Search()
    {
        // If the monster sees the player again, go back to chasing
        if (CanSeePlayer())
        {
            currentState = MonsterState.Chase;
            searchTimer = 0f;

            // Play detected growl
            if (growlAudio != null)
            {
                growlAudio.Play();
            }

            return;
        }

        // Move toward the last place where the player was seen
        Vector2Int currentPosition =
            Vector2Int.RoundToInt(transform.position);

        Vector2Int nextPosition =
            FindNextPosition(currentPosition, lastKnownPosition);

        transform.position = Vector3.MoveTowards(
            transform.position,
            new Vector3(
                nextPosition.x,
                nextPosition.y,
                transform.position.z
            ),
            moveSpeed * Time.deltaTime
        );

        // Once the monster reaches the last known position,
        // start counting how long it searches
        if (currentPosition == lastKnownPosition)
        {
            searchTimer += Time.deltaTime;

            if (searchTimer >= 3f)
            {
                searchTimer = 0f;
                currentState = MonsterState.Patrol;
            }
        }
    }

    void PlayFootsteps()
    {
        if (footstepAudio != null && !footstepAudio.isPlaying)
        {
            footstepAudio.Play();
        }
    }

    void StopFootsteps()
    {
        if (footstepAudio != null && footstepAudio.isPlaying)
        {
            footstepAudio.Stop();
        }
    }

    void UpdateAmbientSound()
    {
        if (ambientAudio == null || player == null)
            return;

        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        float maxDistance = detectionRange;

        // 0 = far away
        // 1 = very close
        float volume =
            1f - Mathf.Clamp01(distance / maxDistance);

        // Maximum breathing volume = 0.5
        ambientAudio.volume = volume * 0.5f;

        // Start the breathing sound
        if (!ambientAudio.isPlaying)
        {
            ambientAudio.Play();
        }
    }

    bool CanSeePlayer()
    {
        Vector2 direction = player.position - transform.position;
        float distance = direction.magnitude;

        // Player is too far away
        if (distance > detectionRange)
            return false;

        RaycastHit2D[] hits = Physics2D.RaycastAll(
            transform.position,
            direction.normalized,
            distance
        );

        foreach (RaycastHit2D hit in hits)
        {
            // Ignore the monster's own collider
            if (hit.collider.gameObject == gameObject)
                continue;

            // We can see the player
            if (hit.collider.transform == player ||
                hit.collider.transform.root == player.root)
            {
                return true;
            }

            // Something else is blocking the view
            return false;
        }

        return false;
    }

    Vector2Int FindNextPosition(
        Vector2Int start,
        Vector2Int target)
    {
        Queue<Vector2Int> queue =
            new Queue<Vector2Int>();

        Dictionary<Vector2Int, Vector2Int> cameFrom =
            new Dictionary<Vector2Int, Vector2Int>();

        queue.Enqueue(start);
        cameFrom[start] = start;

        Vector2Int[] directions =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            if (current == target)
                break;

            foreach (Vector2Int direction in directions)
            {
                Vector2Int next = current + direction;

                if (!mazeGenerator.IsWalkable(next))
                    continue;

                if (cameFrom.ContainsKey(next))
                    continue;

                queue.Enqueue(next);
                cameFrom[next] = current;
            }
        }

        // No path found
        if (!cameFrom.ContainsKey(target))
            return start;

        // Work backwards from the player to the monster
        Vector2Int step = target;

        while (cameFrom[step] != start)
        {
            step = cameFrom[step];
        }

        return step;
    }
}
