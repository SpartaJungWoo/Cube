using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadMovement : MonoBehaviour
{
    private Transform startPos;
    private Transform endPos;
    private float moveSpeed = 3f;
    private float startTime;
    private float journeyLength;
    private float spawnInterval = 5f;

    private bool isMoving = false;

    private GameManager.FaceGroup referencedFaceGroup;

    public void Initialize(Transform start, Transform end, float interval, GameManager.FaceGroup faceGroup)
    {
        startPos = start;
        endPos = end;
        spawnInterval = interval;
        referencedFaceGroup = faceGroup;

        transform.position = startPos.position;
        transform.rotation = startPos.rotation;
        transform.localScale = startPos.localScale;

        startTime = Time.time;
        journeyLength = Vector3.Distance(startPos.position, endPos.position);
    }

    public void StartMoving()
    {
        isMoving = true;
    }

    void Update()
    {
        if (isMoving)
            MoveQuad();
    }

    void MoveQuad()
    {
        float distCovered = (Time.time - startTime) * moveSpeed;
        float fractionOfJourney = distCovered / journeyLength;

        transform.position = Vector3.Lerp(startPos.position, endPos.position, fractionOfJourney);
        transform.rotation = Quaternion.Lerp(startPos.rotation, endPos.rotation, fractionOfJourney);
        transform.localScale = Vector3.Lerp(startPos.localScale, endPos.localScale, fractionOfJourney);

        if (fractionOfJourney >= 1f)
        {
            isMoving = false;

            int currentTopFace = GameManager.instance.DetermineTopFace();
            if (referencedFaceGroup == (GameManager.FaceGroup)(currentTopFace - 1))
            {
                Debug.Log("성공: Quad가 참조한 face와 현재 큐브의 윗면이 일치합니다!");
            }
            else
            {
                Debug.Log("실패: Quad가 참조한 face와 현재 큐브의 윗면이 일치하지 않습니다!");
                GameManager.instance.LifeLost();
            }

            Destroy(gameObject, 0f); // 즉시 삭제

            // Quad가 삭제된 후 이벤트 등의 처리를 추가할 수 있습니다.
        }
    }
}
