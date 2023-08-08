using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    private float speed = 5f; // 회전 속도를 조절하는 변수
    private Vector2 swipeStartPos;
    private float minSwipeDistance = 0.05f; // 스와이프 거리를 조절하는 변수
    private bool isSwiping = false;
    private Quaternion targetRotation;
    private Quaternion defaultRotation;
    private int currentTopFace = 1; // 현재 맨 위에 올라온 면의 번호를 저장하는 변수

    private void Start()
    {
        // 기본 회전값을 설정합니다.
        defaultRotation = Quaternion.Euler(20f, 0f, 0f);
        transform.rotation = defaultRotation;
    }

    void Update()
    {
        // 터치 시작
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began))
        {
            isSwiping = true;
            // 터치 시작 위치를 저장합니다.
            if (Input.GetMouseButtonDown(0))
                swipeStartPos = Input.mousePosition;
            else
                swipeStartPos = Input.touches[0].position;
        }

        // 터치 중간
        if (isSwiping && (Input.GetMouseButton(0) || (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Moved)))
        {
            Vector2 swipeEndPos;
            // 터치 끝난 위치를 저장합니다.
            if (Input.GetMouseButton(0))
                swipeEndPos = Input.mousePosition;
            else
                swipeEndPos = Input.touches[0].position;

            // 스와이프 거리를 계산합니다.
            Vector2 swipeDirection = swipeEndPos - swipeStartPos;

            // 스와이프 거리가 설정한 값보다 큰 경우에만 회전합니다.
            if (swipeDirection.magnitude >= minSwipeDistance)
            {
                // 스와이프 방향의 X와 Y 이동 값을 구합니다.
                float swipeX = swipeDirection.x;
                float swipeY = swipeDirection.y;

                // 큐브를 회전시킵니다. (Y축 회전은 항상 0이 되도록 하고, X와 Z만 활용)
                transform.Rotate(swipeY * Time.deltaTime * speed, 0.0f, -swipeX * Time.deltaTime * speed, Space.World);
            }
        }

        // 터치 끝
        if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended))
        {
            isSwiping = false;
            // 누적된 회전값을 반올림하여 최종 회전값을 계산합니다.
            Vector3 finalRotation = CalculateFinalRotation();
            targetRotation = Quaternion.Euler(finalRotation);

            // 맨 위에 올라온 면을 판단하여 저장합니다.
            currentTopFace = DetermineTopFace();

            // 맨 위에 올라온 면이 face1인 경우에 맞춰서 회전값을 보정합니다.
            int diff = 1 - currentTopFace;
            for (int i = 0; i < diff; i++)
            {
                transform.Rotate(90f, 0f, 0f, Space.World);
            }

            // Debug.Log()를 사용하여 맨 위에 올라온 면을 출력합니다.
            Debug.Log("현재 맨 위에 올라온 면: Face" + currentTopFace);
        }

        // 현재 회전값을 목표 회전값으로 보간하여 부드러운 회전을 적용합니다.
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
    }

    // 가장 가까운 90도의 배수 값을 구하는 함수
    private float GetNearest90Degree(float angle)
    {
        float nearestMultiple = Mathf.Round(angle / 90f) * 90f;
        return nearestMultiple;
    }

    // 누적된 회전값을 반올림하여 최종 회전값을 계산하는 함수
    private Vector3 CalculateFinalRotation()
    {
        Vector3 currentRotation = transform.eulerAngles;

        // 기본 회전값과의 차이를 계산하여 90도 배수로 가깝게 보정합니다.
        float newXRotationDiff = currentRotation.x - defaultRotation.eulerAngles.x;
        float newZRotationDiff = currentRotation.z - defaultRotation.eulerAngles.z;

        float newXRotation = GetNearest90Degree(newXRotationDiff) + defaultRotation.eulerAngles.x;
        float newZRotation = GetNearest90Degree(newZRotationDiff) + defaultRotation.eulerAngles.z;

        return new Vector3(newXRotation, 0f, newZRotation);
    }

    // 현재 맨 위에 올라온 면을 판단하는 함수
    private int DetermineTopFace()
    {
        float maxY = float.MinValue;
        int topFace = 1;

        // face1~6의 면의 position y 좌표값을 비교하여 가장 큰 값을 찾아 맨 위에 올라온 면으로 판단합니다.
        for (int i = 1; i <= 6; i++)
        {
            Transform face = transform.Find("face" + i);
            if (face != null)
            {
                float faceY = face.position.y;
                if (faceY > maxY)
                {
                    maxY = faceY;
                    topFace = i;
                }
            }
        }

        return topFace;
    }
}
