using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject quadPrefab;
    public GameObject[] posLineStarts;
    public GameObject[] posLineEnds;
    public GameObject playerN;

    public GameObject life1;
    public GameObject life2;
    public GameObject life3;

    public Text scoreText; // Score Text 오브젝트

    private float minSpawnInterval = 4f;
    private float maxSpawnInterval = 7f;
    private float timer = 0f;
    private float spawnInterval = 1f;

    private int currentLife = 3;
    private int currentScore = 0; // 현재 점수

    public enum FaceGroup
    {
        Face1,
        Face2,
        Face3,
        Face4,
        Face5,
        Face6
    }

    public delegate void QuadCreatedDelegate(FaceGroup quadFaceGroup);

    public event QuadCreatedDelegate OnQuadCreated;

    private void Start()
    {
        instance = this;
        InitializeFaceMaterials();
        UpdateScoreUI(); // 초기 스코어를 표시해줍니다.
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnAndMoveQuad();
            timer = 0f;
            spawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
        }
    }

    public void SpawnAndMoveQuad()
    {
        int startPosIndex = Random.Range(0, posLineStarts.Length);
        GameObject startPos = posLineStarts[startPosIndex];
        GameObject endPos = posLineEnds[startPosIndex];

        GameObject quadInstance = Instantiate(quadPrefab);

        FaceGroup randomFaceGroup = (FaceGroup)Random.Range(0, 6);
        Material quadMaterial = GetFaceMaterial(randomFaceGroup);
        Renderer quadRenderer = quadInstance.GetComponent<Renderer>();
        quadRenderer.material = quadMaterial;

        QuadMovement quadMovement = quadInstance.GetComponent<QuadMovement>();
        quadMovement.Initialize(startPos.transform, endPos.transform, spawnInterval, randomFaceGroup);
        quadMovement.StartMoving();

        OnQuadCreated?.Invoke(randomFaceGroup);
    }

    private Material[] faceMaterials;

    private void InitializeFaceMaterials()
    {
        faceMaterials = new Material[System.Enum.GetValues(typeof(FaceGroup)).Length];
        Renderer playerRenderer = playerN.GetComponent<Renderer>();

        for (int i = 0; i < faceMaterials.Length; i++)
        {
            Transform face = playerN.transform.Find("face" + (i + 1));
            if (face != null)
            {
                faceMaterials[i] = new Material(playerRenderer.material);
                faceMaterials[i].color = face.GetComponent<Renderer>().material.color;
            }
        }
    }

    private Material GetFaceMaterial(FaceGroup faceGroup)
    {
        return faceMaterials[(int)faceGroup];
    }

    // 현재 맨 위에 올라온 면을 판단하는 함수
    public int DetermineTopFace()
    {
        float maxY = float.MinValue;
        int topFace = 1;

        for (int i = 1; i <= 6; i++)
        {
            Transform face = playerN.transform.Find("face" + i);
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

    public void LifeLost()
    {
        currentLife--;

        switch (currentLife)
        {
            case 2:
                Destroy(life1);
                break;
            case 1:
                Destroy(life2);
                break;
            case 0:
                Destroy(life3);
                // 마지막 Life가 사라진 후 게임 종료 처리를 호출합니다.
                StartCoroutine(GameOverCoroutine());
                break;
        }
    }

    // 매칭 성공 시에 호출되는 함수
    public void MatchSuccess()
    {
        currentScore += 50; // 점수를 50씩 증가시킵니다.
        UpdateScoreUI(); // 스코어 UI를 업데이트합니다.
    }

    private IEnumerator GameOverCoroutine()
    {
        // Life3 이미지가 사라지기를 기다립니다.
        yield return new WaitForEndOfFrame();

        // 마지막 Life가 사라진 후 3초 후에 게임 종료 처리를 실행합니다.
        yield return new WaitForSeconds(1.5f);

        // 게임 종료 처리
        Debug.Log("게임 종료!");
        // 여기에 게임 종료에 대한 처리를 추가할 수 있습니다.
    }

    private void UpdateScoreUI()
    {
        scoreText.text = currentScore.ToString(); // 스코어 UI를 업데이트합니다.
    }
}
