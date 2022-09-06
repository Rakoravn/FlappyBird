using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {
    private const float CAMERA_ORTHO_SIZE = 50f;
    private const float PIPE_WIDTH = 65f;
    private const float PIPE_HEAD_HEIGHT = 3.75f;
    private const float PIPE_MOVE_SPEED = 30f;
    private const float PIPE_DESTROY_X_POSITION = -120f;
    private const float PIPE_SPAWN_X_POSITION = +120f;
    private const float BIRD_X_POSITION = 0f;

    private static Level instance;

    public static Level GetInstance() {
        return instance;
    }

    private List<Pipe> pipeList;
    private int pipesPassedCount;
    private int pipesSpawned;
    private float pipeSpawnTimer;
    private float pipeSpawnTimerMax;
    private float gapSize;
    private State state;

    public enum Difficulty {
        Easy,
        Medium,
        Hard,
        Impossible,
    }

    private enum State {
        WaitingToStart,
        Playing,
        Dead,
    }

    private void Awake() {
        instance = this;
        pipeList = new List<Pipe>();
        pipeSpawnTimerMax = 2f;
        SetDifficulty(Difficulty.Easy);
        state = State.WaitingToStart;
    }

    private void Start() {
        Bird.GetInstance().OnDeath += Bird_OnDeath;
        Bird.GetInstance().OnStartPlaying += Bird_OnStartedPlaying;  
    }

    private void Bird_OnStartedPlaying(object sender, System.EventArgs e) {
        state = State.Playing;
    }

    private void Bird_OnDeath(object sender, System.EventArgs e) {
        state = State.Dead;
    }

    private void Update() {
        if (state == State.Playing) {
            HandlePipeMovement();
            HandlePipeSpawning();
        }
    }

    private void HandlePipeSpawning() {
        pipeSpawnTimer -= Time.deltaTime;
        if (pipeSpawnTimer < 0) {
            pipeSpawnTimer += pipeSpawnTimerMax;
            float heightEdgeLimit = 10f;
            float minHeight = 60f;//gapSize * .5f + heightEdgeLimit;
            float totalHeight = CAMERA_ORTHO_SIZE * 2f;
            float maxHeight = 60f;//totalHeight - gapSize * .5f - heightEdgeLimit;
            float height = Random.Range(minHeight, maxHeight);
            CreateGapPipes(height, gapSize, PIPE_SPAWN_X_POSITION);
        }
    }

    private void HandlePipeMovement() {
        for (int i = 0; i < pipeList.Count; i++) {
            Pipe pipe = pipeList[i];
            bool isToTheRightOfTheBird = pipe.GetXPosition() > BIRD_X_POSITION;
            pipe.Move();
            if (isToTheRightOfTheBird && pipe.GetXPosition() <= BIRD_X_POSITION && pipe.IsBottom()) {
                //Pipe passed bird
                pipesPassedCount++;
                SoundManager.PlaySound(SoundManager.Sound.Score);
            }
            if (pipe.GetXPosition() < PIPE_DESTROY_X_POSITION) {
                //Destroy pipe
                pipe.DestroySelf();
                pipeList.Remove(pipe);
                i--;
            }
        }
    }

    private void SetDifficulty(Difficulty difficulty) {
        switch (difficulty) {
            case Difficulty.Easy:
                gapSize = 50f;
                pipeSpawnTimerMax = 1.5f;
                break;
            case Difficulty.Medium:
                gapSize = 40f;
                pipeSpawnTimerMax = 1.3f;
                break;
            case Difficulty.Hard:
                gapSize = 33f;
                pipeSpawnTimerMax = 1.2f;
                break;
            case Difficulty.Impossible:
                gapSize = 24f;
                pipeSpawnTimerMax = 1.0f;
                break;
        }
    }

    private Difficulty GetDifficulty() {
        if (pipesSpawned >= 50) { return Difficulty.Impossible; }
        if (pipesSpawned >= 35) { return Difficulty.Hard; }
        if (pipesSpawned >= 20) { return Difficulty.Medium; }
        return Difficulty.Easy;
    }

    private void CreateGapPipes(float gapY, float gapSize, float xPosition) {
        float x = Random.Range(0, gapY/2);
        float y = gapY / 2 - x;

        CreatePipe(gapY, xPosition, true, x);
        CreatePipe(120 - gapY, xPosition, false, y);
        pipesSpawned++;
        SetDifficulty(GetDifficulty());
    }

    private void CreatePipe(float height, float xPosition, bool createOnBottom, float yPosMod) {
        //CREATE PIPEHEAD
        Transform pipeHead = Instantiate(GameAssets.GetInstance().pfPipeHead);
        float pipeHeadYPosition;
        if (createOnBottom) {
        pipeHeadYPosition = -CAMERA_ORTHO_SIZE + height - PIPE_HEAD_HEIGHT * .5f;
        } else {
        pipeHeadYPosition = +CAMERA_ORTHO_SIZE - height + PIPE_HEAD_HEIGHT * .5f;
        }
        pipeHead.position = new Vector3(xPosition, pipeHeadYPosition);

        //CREATE PIPEBODY
        Transform pipeBody = Instantiate(GameAssets.GetInstance().pfPipeBody);

        float pipeBodyYPosition;
        if (createOnBottom) {
            pipeBodyYPosition = -CAMERA_ORTHO_SIZE-10 + yPosMod;
        } else {
            pipeBodyYPosition = +CAMERA_ORTHO_SIZE+10 - yPosMod;
            pipeBody.localScale = new Vector3(1, -1, 1);
        }
        pipeBody.position = new Vector3(xPosition, pipeBodyYPosition);

        SpriteRenderer pipeBodySpriteRenderer = pipeBody.GetComponent<SpriteRenderer>();
        pipeBodySpriteRenderer.size = new Vector2(height, height);

        BoxCollider2D pipeBodyBoxCollider = pipeBody.GetComponent<BoxCollider2D>();
        pipeBodyBoxCollider.size = new Vector2(height / 10, height);
        pipeBodyBoxCollider.offset = new Vector2(0.8f, -2f);

        Pipe pipe = new Pipe(pipeHead, pipeBody, createOnBottom);
        pipeList.Add(pipe);
    }

    public int GetPipesSpawned() {
        return pipesSpawned;
    }

    public int GetPipesPassedCount() {
        return pipesPassedCount;
    }

    private class Pipe {
        private Transform pipeHeadTransform;
        private Transform pipeBodyTransform;
        private bool isBottom;

        public Pipe(Transform pipeHeadTransform, Transform pipeBodyTransform, bool isBottom) {
            this.pipeHeadTransform = pipeHeadTransform;
            this.pipeBodyTransform = pipeBodyTransform;
            this.isBottom = isBottom;
        }

        public void Move() {
            pipeHeadTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
            pipeBodyTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
        }

        public float GetXPosition() {
            return pipeHeadTransform.position.x;
        }

        public bool IsBottom() {
            return isBottom;
        }

        public void DestroySelf() {
            Destroy(pipeHeadTransform.gameObject);
            Destroy(pipeBodyTransform.gameObject);
        }
    }
}
