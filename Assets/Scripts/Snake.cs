using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    private Vector2 _direction = Vector2.right;
    private Vector2 _screenBounds;

    private List<Transform> _segments = new List<Transform>();
    [SerializeField] private int initialSize = 3;
    [SerializeField] private Transform segmentPrefab;
   


    private void Start()
    {
        ResetState();

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

        for(int i = _segments.Count -1; i > 0; i--)
        {
            _segments[i].position = _segments[i - 1].position;
        }

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

    private void Grow()
    {
        Transform segment = Instantiate(this.segmentPrefab);
        segment.position = _segments[_segments.Count - 1].position;

        _segments.Add(segment);
    }

    private void ResetState()
    {
        for (int i =1; i < _segments.Count; i++)
        {
            Destroy(_segments[i].gameObject);
        }
        _segments.Clear();
        _segments.Add(this.transform);

        for (int i =1; i < this.initialSize; i++)
        {
            _segments.Add(Instantiate(this.segmentPrefab));
        }

        this.transform.position = Vector3.zero;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Food")
        {
            Grow();
        }
        else if (other.tag == "Obstacle"){
            ResetState();
        }
    }
}