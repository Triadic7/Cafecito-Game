using System.Collections;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    /// <summary>
    /// The parts of the car.
    /// </summary>
    [SerializeField]
    private SpriteRenderer[] bodySprites;

    /// <summary>
    /// How fast the car moves.
    /// </summary>
    [SerializeField]
    private float moveSpeed = 5f;

    private bool isPaused = false;

    public IEnumerator DriveForSeconds(float duration, bool moveRight)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            MoveVehicle(moveRight);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Moves the vehicle left or right.
    /// </summary>
    /// <param name="moveRight">If the vehicle moves right.</param>
    private void MoveVehicle(bool moveRight)
    {
        // Move the vehicle.
        float direction = moveRight ? 1f : -1f;
        transform.position += new Vector3(direction * moveSpeed * Time.deltaTime, 0f, 0f);

        // Flip the vehicle to face the correct direction.
        Vector3 localScale = transform.localScale;
        localScale.x = Mathf.Abs(localScale.x) * (moveRight ? -1f : 1f);
        transform.localScale = localScale;
    }

    private void Start()
    {
        RandomizeColor();
        var pauseMenu = FindFirstObjectByType<PauseMenu>();
        pauseMenu.OnPause += () => { isPaused = true; };
        pauseMenu.OnUnpause += () => { isPaused = false; };
    }

    /// <summary>
    /// Randomize color of the car.
    /// </summary>
    private void RandomizeColor()
    {
        // Generate a random color with RGB between 0 and 1.
        Color color = new Color(Random.value, Random.value, Random.value);

        foreach (var sprite in bodySprites)
        {
            sprite.color = color;
        }
    }
}
