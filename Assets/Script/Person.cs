using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    public bool isGoingRight;
    public bool alwaysZenshin;
    public bool alwaysYuzuru;
    Rigidbody personBody;
    Vector3 forward = new Vector3(1,0,0);
    Quaternion initialRotation;
    Quaternion plus45Rotation;
    Quaternion minus45Rotation;
    Quaternion desiredRotation;
    float height,width;
    float ScreenHalfWidth;
    public float personalSpace;
    public float chooseSideRadius;
    public float initialSpeed;
    float speed;
    float rngTimer = 0f;
    int lastRng = -1;
    int lastRng1 = -1;
    MovementState movementState;
    public float turnSpeedDegrees;
    public float kasokudo;
    public float maxSpeed;
    public bool isShoumenShoutotsu;
    Direction4 isShoumenShoutotsuGoDirection;
    public enum Direction4
    {
        N,
        E,
        S,
        W,
    }
    public enum MovementState
    {
        AT_EASE,
        KASOKU,
        GENSOKU,
        TURN_E,
        TURN_W,
    }
    void Start()
    {
        personBody = GetComponent<Rigidbody>();
        height = GetComponent<Collider>().bounds.size.y;
        width = GetComponent<Collider>().bounds.size.x;
        // print(GetComponent<Collider>().bounds.size);
        // print(transform.rotation.eulerAngles);
        ScreenHalfWidth = Camera.main.orthographicSize*Camera.main.aspect;
        initialRotation = transform.rotation;
        desiredRotation = initialRotation;
        plus45Rotation = Quaternion.Euler(new Vector3(initialRotation.eulerAngles.x,initialRotation.eulerAngles.y,initialRotation.eulerAngles.z + 45f));
        minus45Rotation = Quaternion.Euler(new Vector3(initialRotation.eulerAngles.x,initialRotation.eulerAngles.y,initialRotation.eulerAngles.z - 45f));
        speed = initialSpeed;
        movementState = MovementState.AT_EASE;
        if(Settings.isBuild){
            personalSpace = Settings.PersonalSpace;
            chooseSideRadius = Settings.ChooseSideRange;
        }
        StartCoroutine("MovementCouroutine");
    }

    void Update()
    {
        // if(isGoingRight)forward = RotateZ(new Vector3(speed,0,0), desiredRotation.eulerAngles.z - 270);
        // else forward = RotateZ(new Vector3(-speed,0,0), desiredRotation.eulerAngles.z - 90);
        if(isGoingRight)forward = RotateZ(new Vector3(speed,0,0), transform.rotation.eulerAngles.z - 270);
        else forward = RotateZ(new Vector3(-speed,0,0), transform.rotation.eulerAngles.z - 90);
        personBody.velocity = forward;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, personalSpace);
        Dictionary<Direction4,bool> isInDirection4 = new Dictionary<Direction4,bool>(){
            {Direction4.N, false},
            {Direction4.E, false},
            {Direction4.S, false},
            {Direction4.W, false},
        };
        if(isShoumenShoutotsu) MainBehaviour4(isInDirection4);
        else if(hitColliders.Length>1){
            foreach (var hitCollider in hitColliders)
            {
                if(transform.position!=hitCollider.transform.position){
                    if(hitCollider.transform.gameObject.layer == LayerMask.NameToLayer("UpperWall")){
                        if(isGoingRight)isInDirection4[Direction4.W] = true;
                        else isInDirection4[Direction4.E] = true;
                    }
                    else if(hitCollider.transform.gameObject.layer == LayerMask.NameToLayer("LowerWall")){
                        if(isGoingRight)isInDirection4[Direction4.E] = true;
                        else isInDirection4[Direction4.W] = true;
                    }
                    else{
                        Direction4 directionOfCollider = GetDirection4(transform.position,hitCollider.transform.position);
                        if(directionOfCollider == Direction4.N && hitCollider.transform.gameObject.layer == LayerMask.NameToLayer("Person")){
                            Person colliderScript = hitCollider.transform.gameObject.GetComponent<Person>();
                            if(directionOfCollider == Direction4.N&&isGoingRight!=colliderScript.isGoingRight){
                                isShoumenShoutotsu = true;
                                isShoumenShoutotsuGoDirection = ChooseSide();
                                colliderScript.ImGoingThisWay(isShoumenShoutotsuGoDirection);
                            }
                        }
                        isInDirection4[directionOfCollider] = true;
                    }
                }
            }
            MainBehaviour4(isInDirection4);
        }
        else{
            AtEase();
        }
        if(isGoingRight){
            // if(transform.position.x >= ScreenHalfWidth + width){
            if(transform.position.x >= PersonSpawner.spawnRange){
                Destroy(gameObject);
            }
        }
        else{
            // if(transform.position.x <= -ScreenHalfWidth - width){
            if(transform.position.x <= -PersonSpawner.spawnRange){
                Destroy(gameObject);
            }
        }
        if(lastRng!=-1||lastRng1!=-1){
            rngTimer += Time.deltaTime;
            if(rngTimer>1f){
                rngTimer = 0f;
                lastRng = -1;
                lastRng1 = -1;
            }
        }
    }
    Direction4 ChooseSide(){
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, chooseSideRadius, 1 << LayerMask.NameToLayer("Person"));
        float ourSideCountOnEast = 0;
        float ourSideCountOnWest = 0;
        float theirSideCountOnEast = 0;
        float theirSideCountOnWest = 0;
        foreach (var hitCollider in hitColliders)
        {
            if(transform.position!=hitCollider.transform.position){
                Direction4 Side = GetSide(transform.position,hitCollider.transform.position);
                Person colliderScript = hitCollider.transform.gameObject.GetComponent<Person>();
                if(isGoingRight!=colliderScript.isGoingRight){
                    if(Side == Direction4.E) theirSideCountOnEast += 1;
                    if(Side == Direction4.W) theirSideCountOnWest += 1;
                } 
                else{
                    if(Side == Direction4.E) ourSideCountOnEast += 1;
                    if(Side == Direction4.W) ourSideCountOnWest += 1;
                }
            }
        }
        float ratioOnEast = ourSideCountOnEast/(ourSideCountOnEast + theirSideCountOnEast);
        float ratioOnWest = ourSideCountOnWest/(ourSideCountOnWest + theirSideCountOnWest);
        if(ratioOnEast>=ratioOnWest){
            return Direction4.E;
        }
        else return Direction4.W;
    }
    void ImGoingThisWay(Direction4 gothisway){
        isShoumenShoutotsu = true;
        isShoumenShoutotsuGoDirection = gothisway;
    }
    void MainBehaviour4(Dictionary<Direction4,bool> isindirection){
        if(isShoumenShoutotsu){
            if(isShoumenShoutotsuGoDirection == Direction4.E) TurnE();
            else if(isShoumenShoutotsuGoDirection == Direction4.W) TurnW();
            isShoumenShoutotsu = false;
            return;
        }

        int rng;
        int directionsFilled = 0;
        for (int i = 0; i < 4; i++)
        {
            if(isindirection[Direction4.N + i]){
                directionsFilled++;
            }
        }
        if(directionsFilled==4){
            AtEase();
            return;
        }
        else if(directionsFilled==3){
            if(isindirection[Direction4.E]&&isindirection[Direction4.W]){
                if(isindirection[Direction4.S]) Kasoku();
                else Gensoku();
            }
            else{
                if(isindirection[Direction4.E]) TurnW();
                else TurnE();
            }
            return;
        }
        else if(directionsFilled==2){
            if(isindirection[Direction4.E]&&isindirection[Direction4.W]){
                if(alwaysZenshin)Kasoku();
                else if(alwaysYuzuru)Gensoku();
                else{
                    if(lastRng == -1){
                        lastRng = Random.Range(0,2);
                    }
                    rng = lastRng;
                    if(rng==1) Kasoku();
                    else Gensoku();
                }
            }
            else if(isindirection[Direction4.N]&&isindirection[Direction4.S]){
                if(lastRng == -1){
                    lastRng = Random.Range(0,2);
                }
                rng = lastRng;
                if(rng==1) TurnE();
                else TurnW();
            }
            else if(isindirection[Direction4.N]){
                if(isindirection[Direction4.E]){
                    if(alwaysZenshin)TurnW();
                    else if(alwaysYuzuru)Gensoku();
                    else{
                        if(lastRng == -1){
                            lastRng = Random.Range(0,2);
                        }
                        rng = lastRng;
                        if(rng==1) TurnW();
                        else Gensoku();
                    }
                }
                if(isindirection[Direction4.W]){
                    if(alwaysZenshin)TurnE();
                    else if(alwaysYuzuru)Gensoku();
                    else{
                        if(lastRng == -1){
                            lastRng = Random.Range(0,2);
                        }
                        rng = lastRng;
                        if(rng==1) TurnE();
                        else Gensoku();
                    }
                }
            }
            else{
                if(isindirection[Direction4.E]){
                    TurnW();
                }
                if(isindirection[Direction4.W]){
                    TurnE();
                }
            }
            return;
        }
        else{
            if(isindirection[Direction4.N]){
                if(alwaysZenshin){
                    if(lastRng == -1){
                        lastRng = Random.Range(0,2);
                    }
                    rng = lastRng;
                    if(rng==1) TurnE();
                    else TurnW();
                }
                else if(alwaysYuzuru){
                    Gensoku();
                }
                else {
                    if(lastRng == -1){
                        lastRng = Random.Range(0,2);
                    }
                    rng = lastRng;
                    if(rng==1){
                        if(lastRng1 == -1){
                            lastRng1 = Random.Range(0,2);
                        }
                        rng = lastRng1;
                        if(rng==1) TurnE();
                        else TurnW();
                    }
                    else{
                        Gensoku();
                    }
                }
            }
            else if(isindirection[Direction4.S]) Kasoku();
            else if(isindirection[Direction4.E]) TurnW();
            else TurnE();
            return;
        }
    }
    void Kasoku(){
        desiredRotation = initialRotation;
        // speed = initialSpeed*1.5f;
        // movementState = MovementState.KASOKU;
    }
    void Gensoku(){
        desiredRotation = initialRotation;
        // speed = initialSpeed*0.5f;
        movementState = MovementState.GENSOKU;
    }
    void TurnW(){
        desiredRotation = plus45Rotation;
        // speed = initialSpeed;
        movementState = MovementState.TURN_W;
    }
    void TurnE(){
        desiredRotation = minus45Rotation;
        // speed = initialSpeed;
        movementState = MovementState.TURN_E;
    }
    void AtEase(){
        desiredRotation = initialRotation;
        // speed = initialSpeed;
        movementState = MovementState.AT_EASE;
    }
    IEnumerator MovementCouroutine(){
        while(true){
            if(movementState == MovementState.AT_EASE){
                while(speed!=initialSpeed||transform.rotation.eulerAngles.z!=desiredRotation.eulerAngles.z){
                    AdjustRotation(Time.deltaTime);
                    if(speed>initialSpeed + 0.1f) speed -= kasokudo*Time.deltaTime;
                    else if(speed<initialSpeed - 0.1f) speed += kasokudo*Time.deltaTime;
                    else speed = initialSpeed;
                    yield return null;
                }
            }
            else if(movementState == MovementState.KASOKU){
                while(speed<maxSpeed||transform.rotation.eulerAngles.z!=desiredRotation.eulerAngles.z){
                    AdjustRotation(Time.deltaTime);
                    speed += kasokudo*Time.deltaTime;
                    yield return null;
                }
            }
            else if(movementState == MovementState.GENSOKU){
                while(speed>0||transform.rotation.eulerAngles.z!=desiredRotation.eulerAngles.z){
                    AdjustRotation(Time.deltaTime);
                    speed -= kasokudo*Time.deltaTime;
                    if(speed<0) speed = 0;
                    yield return null;
                }
            }
            else if(movementState == MovementState.TURN_E||movementState == MovementState.TURN_W){
                while(speed!=initialSpeed||transform.rotation.eulerAngles.z!=desiredRotation.eulerAngles.z){
                    AdjustRotation(Time.deltaTime);
                    if(speed>initialSpeed + 0.1f) speed -= kasokudo*Time.deltaTime;
                    else if(speed<initialSpeed - 0.1f) speed += kasokudo*Time.deltaTime;
                    else speed = initialSpeed;
                    yield return null;
                }
            }
            yield return null;
        }
    }
    void AdjustRotation(float deltatime){
        if(transform.rotation.eulerAngles.z>desiredRotation.eulerAngles.z + 1f)  transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x,transform.rotation.eulerAngles.y,transform.rotation.eulerAngles.z - turnSpeedDegrees*deltatime));
        else if(transform.rotation.eulerAngles.z<desiredRotation.eulerAngles.z - 1f)  transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x,transform.rotation.eulerAngles.y,transform.rotation.eulerAngles.z + turnSpeedDegrees*deltatime));
        else transform.rotation = desiredRotation;
    }
    Direction4 GetDirection4(Vector3 thisPosition, Vector3 colliderPosition){
        Vector3 rawDirection = colliderPosition - thisPosition;
        rawDirection = rawDirection.normalized;
        Direction4 returnValue;
        if(Mathf.Abs(rawDirection.y)>Mathf.Abs(rawDirection.x)){
            if(rawDirection.y>0){
                returnValue =  Direction4.W;
            }
            else returnValue =  Direction4.E;
        }
        else{
            if(rawDirection.x>0){
                returnValue = Direction4.N;
            }
            else returnValue = Direction4.S;
        }

        if(!isGoingRight)returnValue = (Direction4)(((int)returnValue + 2)%4);
        return returnValue;
    }
    Direction4 GetSide(Vector3 thisPosition, Vector3 colliderPosition){
        Vector3 rawDirection = colliderPosition - thisPosition;
        rawDirection = rawDirection.normalized;
        Direction4 returnValue;
        if(rawDirection.y>0){
            returnValue =  Direction4.W;
        }
        else{
            returnValue =  Direction4.E;
        }
        if(!isGoingRight)returnValue = (Direction4)(((int)returnValue + 2)%4);
        return returnValue;
    }
    Vector3 RotateZ(Vector3 direc,float Theta){
        return new Vector3(direc.x*Mathf.Cos(Theta/180*Mathf.PI)-direc.y*Mathf.Sin(Theta/180*Mathf.PI),direc.y*Mathf.Cos(Theta/180*Mathf.PI)+direc.x*Mathf.Sin(Theta/180*Mathf.PI),direc.z);
    }
}
