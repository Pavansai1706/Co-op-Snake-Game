using UnityEngine;

public class Snake : MonoBehaviour
{
    private Vector2 _direction = Vector2.right;
    private Vector2 _screenBounds;

    void Start()
    {
        _screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            _direction = Vector2.up;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            _direction = Vector2.down;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            _direction = Vector2.left;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            _direction = Vector2.right;
        }
    }

    private void FixedUpdate()
    {
        // Calculate the new position based on the current position and direction
        Vector3 newPosition = transform.position + (Vector3)_direction;

        // Wrap around the screen if the snake moves beyond the screen boundaries
        if (newPosition.x > _screenBounds.x)
            newPosition.x = -_screenBounds.x;
        else if (newPosition.x < -_screenBounds.x)
            newPosition.x = _screenBounds.x;

        if (newPosition.y > _screenBounds.y)
            newPosition.y = -_screenBounds.y;
        else if (newPosition.y < -_screenBounds.y)
            newPosition.y = _screenBounds.y;

        // Update the position
        transform.position = newPosition;
    }
}
