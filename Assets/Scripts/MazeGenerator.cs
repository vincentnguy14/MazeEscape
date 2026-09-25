using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public GameObject wallPrefab;
    public GameObject player;
    public GameObject monster;
    public GameObject exit;
    public int monsterMinDistance = 8;

    private bool[,] maze;

    void Start()
    {
        GenerateMaze();

        // Put player at the starting position
        player.transform.position = new Vector3(1, 1, 0);

        SpawnExit();

        // Spawn monster far enough away from player
        SpawnMonster();
    }

    void GenerateMaze()
    {
        int mazeWidth = width * 2 + 1;
        int mazeHeight = height * 2 + 1;

        maze = new bool[mazeWidth, mazeHeight];

        // Start with everything as a wall
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                maze[x, y] = true;
            }
        }

        // Generate the main maze
        CarvePath(1, 1);

        // Add a few extra openings to create alternate routes
        AddExtraPaths();

        // Spawn the wall objects
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                if (maze[x, y])
                {
                    Instantiate(
                        wallPrefab,
                        new Vector3(
                            x - (mazeWidth - 1) / 2f,
                            y - (mazeHeight - 1) / 2f,
                            0
                        ),
                        Quaternion.identity
                    );
                }
            }
        }
    }

    void AddExtraPaths()
    {
        int mazeWidth = width * 2 + 1;
        int mazeHeight = height * 2 + 1;

        // Try several times to create extra openings
        for (int i = 0; i < 25; i++)
        {
            int x = Random.Range(1, mazeWidth - 1);
            int y = Random.Range(1, mazeHeight - 1);

            // Only remove a wall that has paths on opposite sides
            if (maze[x, y])
            {
                bool horizontalPaths =
                    x > 0 && x < mazeWidth - 1 &&
                    !maze[x - 1, y] && !maze[x + 1, y];

                bool verticalPaths =
                    y > 0 && y < mazeHeight - 1 &&
                    !maze[x, y - 1] && !maze[x, y + 1];

                if (horizontalPaths || verticalPaths)
                {
                    maze[x, y] = false;
                }
            }
        }
    }
    void SpawnExit()
    {
        List<Vector2Int> possiblePositions = new List<Vector2Int>();

        int mazeWidth = width * 2 + 1;
        int mazeHeight = height * 2 + 1;

        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                if (!maze[x, y])
                {
                    Vector3 worldPosition = new Vector3(
                        x - (mazeWidth - 1) / 2f,
                        y - (mazeHeight - 1) / 2f,
                        0
                    );

                    float distance = Vector3.Distance(
                        worldPosition,
                        player.transform.position
                    );

                    // Don't put the exit right next to the player
                    if (distance >= 10f)
                    {
                        possiblePositions.Add(new Vector2Int(x, y));
                    }
                }
            }
        }

        if (possiblePositions.Count > 0)
        {
            Vector2Int exitPosition =
                possiblePositions[Random.Range(0, possiblePositions.Count)];

            exit.transform.position = new Vector3(
                exitPosition.x - (mazeWidth - 1) / 2f,
                exitPosition.y - (mazeHeight - 1) / 2f,
                0
            );
        }
    }

    void SpawnMonster()
    {
        List<Vector2Int> possiblePositions = new List<Vector2Int>();

        int mazeWidth = width * 2 + 1;
        int mazeHeight = height * 2 + 1;

        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                // Only choose open/walkable spaces
                if (!maze[x, y])
                {
                    Vector3 worldPosition = new Vector3(
                        x - (mazeWidth - 1) / 2f,
                        y - (mazeHeight - 1) / 2f,
                        0
                    );

                    float distance = Vector3.Distance(
                        worldPosition,
                        player.transform.position
                    );

                    // Make sure monster isn't too close to player
                    if (distance >= monsterMinDistance)
                    {
                        possiblePositions.Add(new Vector2Int(x, y));
                    }
                }
            }
        }

        if (possiblePositions.Count > 0)
        {
            Vector2Int spawnPosition =
                possiblePositions[Random.Range(0, possiblePositions.Count)];

            monster.transform.position = new Vector3(
                spawnPosition.x - (mazeWidth - 1) / 2f,
                spawnPosition.y - (mazeHeight - 1) / 2f,
                0
            );
        }
    }

    public bool IsWalkable(Vector2Int worldPosition)
    {
        int mazeWidth = width * 2 + 1;
        int mazeHeight = height * 2 + 1;

        int x = worldPosition.x + (mazeWidth - 1) / 2;
        int y = worldPosition.y + (mazeHeight - 1) / 2;

        if (x < 0 || x >= mazeWidth || y < 0 || y >= mazeHeight)
        {
            return false;
        }

        return !maze[x, y];
    }

    void CarvePath(int x, int y)
    {
        maze[x, y] = false;

        List<Vector2Int> directions = new List<Vector2Int>
        {
            new Vector2Int(2, 0),
            new Vector2Int(-2, 0),
            new Vector2Int(0, 2),
            new Vector2Int(0, -2)
        };

        // Randomize directions
        for (int i = 0; i < directions.Count; i++)
        {
            int randomIndex = Random.Range(i, directions.Count);

            Vector2Int temp = directions[i];
            directions[i] = directions[randomIndex];
            directions[randomIndex] = temp;
        }

        foreach (Vector2Int direction in directions)
        {
            int nextX = x + direction.x;
            int nextY = y + direction.y;

            if (nextX > 0 && nextX < maze.GetLength(0) - 1 &&
                nextY > 0 && nextY < maze.GetLength(1) - 1 &&
                maze[nextX, nextY])
            {
                // Remove the wall between the cells
                maze[x + direction.x / 2, y + direction.y / 2] = false;

                // Continue recursively
                CarvePath(nextX, nextY);
            }
        }
    }
}

