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
    [SerializeField] float _minCameraZoom = 5;

    private void Start()
    {
        _camera = GetComponent<Camera>();
    }
    // Update is called once per frame
    void Update()
    {
        float zoomLevel;
        Vector2 camPos;
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
            minX -= 5;
            minY -= 5;
            maxX += 5;
            maxY += 5;
            if (maxX - minX > (maxY - minY)*1.75f)
            {
                zoomLevel = Mathf.Clamp((maxX - minX)/3.5f, _minCameraZoom, _levelBounds.extents.x/1.75f);
            }
            else
            {
                // do Y zoom
                zoomLevel = Mathf.Clamp((maxY - minY) / 2, _minCameraZoom, _levelBounds.extents.y);
            }
            camPos.x = Mathf.Clamp (minX + (maxX-minX)/2,_levelBounds.center.x - _levelBounds.extents.x + zoomLevel*1.75f, _levelBounds.center.x + _levelBounds.extents.x - zoomLevel * 1.75f);
            camPos.y = Mathf.Clamp(minY + (maxY - minY) / 2, _levelBounds.center.y - _levelBounds.extents.y + zoomLevel, _levelBounds.center.y + _levelBounds.extents.y - zoomLevel); //minY + (maxY - minY) / 2;

            //Debug.DrawLine (camPos + new Vector2(zoomLevel*1.75f,zoomLevel), camPos + new Vector2(zoomLevel * 1.75f, - zoomLevel));
            //Debug.DrawLine(camPos + new Vector2(zoomLevel * 1.75f, - zoomLevel), camPos + new Vector2(- zoomLevel * 1.75f, -zoomLevel));
            //Debug.DrawLine(camPos + new Vector2(-zoomLevel * 1.75f, -zoomLevel), camPos + new Vector2(-zoomLevel * 1.75f, zoomLevel));
            //Debug.DrawLine(camPos + new Vector2(-zoomLevel * 1.75f, zoomLevel), camPos + new Vector2(zoomLevel * 1.75f, zoomLevel));

            _camera.transform.position = new Vector3(camPos.x,camPos.y,-10);
            _camera.orthographicSize = zoomLevel;

        }

        if (!_gameRunning && GameController.GameState == GameController.State.Game)
        {
            _gameRunning = true;
            _levelBounds = GameObject.FindGameObjectWithTag("Level").transform.Find("Bounds").GetComponent<BoxCollider2D>().bounds;
        }
    }
}
