using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell : MonoBehaviour
{
    [SerializeField] private GameObject _leftWall, _rightWall, _frontWall, _backWall, _unvisitedBlock;
    public bool IsVisited { get; private set; }

    public void Visit()
    {
        IsVisited = true;
        _unvisitedBlock.gameObject.SetActive(false);
    }

    public void ClearLeftWall() => _leftWall.SetActive(false);
    public void ClearRightWall() => _rightWall.SetActive(false);
    public void ClearFrontWall() => _frontWall.SetActive(false);
    public void ClearBackWall() => _backWall.SetActive(false);
}