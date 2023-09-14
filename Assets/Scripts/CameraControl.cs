using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    Vector3 _defaultCameraPos = new Vector3(0,0,-10);
    float _defaultCameraSize = 15;
    Camera _camera;
    bool _gameRunning = false;
    Bounds _levelBounds = new Bounds();
    Bounds _cameraBounds = new Bounds();
    [SerializeField] float _minCameraZoom;

    private void Start()
    {
        _camera = GetComponent<Camera>();
    }
    // Update is called once per frame
    void Update()
    {
        if (_gameRunning)
        {
            if (GameController.GameState != GameController.State.Game)
            {
                transform.position = _defaultCameraPos;
                _camera.orthographicSize = _defaultCameraSize;
                _gameRunning = false;
            }
            // set out min/max coorodinates needed for camera to first player, that way we have base values at least
            float maxX = GameController.Instance.Players[0].transform.position.x;
            float minX = GameController.Instance.Players[0].transform.position.x;
            float maxY = GameController.Instance.Players[0].transform.position.y;
            float minY = GameController.Instance.Players[0].transform.position.y;
            foreach (Player p in GameController.Instance.Players)
            {
                Vector3 pos = p.transform.position;
                if (pos.x < minX)
                    minX = pos.x;
                else if (pos.x > maxX) 
                    maxX = pos.x;
                if (pos.y < minY)
                    minY = pos.y;
                else if (pos.y > maxY) 
                    maxY = pos.y;
            }
            minX -= 2;
            minY -= 2;
            maxX += 2;
            maxY += 2;
            if (maxX - minX > (maxY - minY)*1.75f)
            {

            }
        }

        if (!_gameRunning && GameController.GameState == GameController.State.Game)
        {
            _gameRunning = true;
        }
    }
}
