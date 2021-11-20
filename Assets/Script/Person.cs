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
    public float initialSpeed;
    float speed;
    float rngTimer = 0f;
    int lastRng = -1;
    int lastRng1 = -1;
    public enum Direction4
    {
        N,
        E,
        S,
        W,
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
    }

    void Update()
    {
        if(isGoingRight)forward = RotateZ(new Vector3(speed,0,0), desiredRotation.eulerAngles.z - 270);
        else forward = RotateZ(new Vector3(-speed,0,0), desiredRotation.eulerAngles.z - 90);
        personBody.velocity = forward;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, personalSpace);
        if(hitColliders.Length>1){
            Dictionary<Direction4,bool> isInDirection4 = new Dictionary<Direction4,bool>(){
                {Direction4.N, false},
                {Direction4.E, false},
                {Direction4.S, false},
                {Direction4.W, false},
            };
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
                    else isInDirection4[GetDirection4(transform.position,hitCollider.transform.position)] = true;
                }
            }
            MainBehaviour4(isInDirection4);
        }
        else{
            AtEase();
        }
        if(isGoingRight){
            if(transform.position.x >= ScreenHalfWidth + width){
                Destroy(gameObject);
            }
        }
        else{
            if(transform.position.x <= -ScreenHalfWidth - width){
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
    void MainBehaviour4(Dictionary<Direction4,bool> isindirection){
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
                    print(3);
                    if(lastRng == -1){
                        lastRng = Random.Range(0,2);
                    }
                    rng = lastRng;
                    if(rng==1) TurnE();
                    else TurnW();
                }
                else if(alwaysYuzuru){
                    print(1);
                    Gensoku();
                }
                else {
                    print(2);
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
        speed = initialSpeed*1.5f;
    }
    void Gensoku(){
        desiredRotation = initialRotation;
        speed = initialSpeed*0.5f;
    }
    void TurnW(){
        desiredRotation = plus45Rotation;
        speed = initialSpeed;
    }
    void TurnE(){
        desiredRotation = minus45Rotation;
        speed = initialSpeed;
    }
    void AtEase(){
        desiredRotation = initialRotation;
        speed = initialSpeed;
    }
    // IEnumerator MovementCouroutine(){

    //     while(speed<maxSpeed){

    //         yield return null;
    //     }
    // }
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
    Vector3 RotateZ(Vector3 direc,float Theta){
        return new Vector3(direc.x*Mathf.Cos(Theta/180*Mathf.PI)-direc.y*Mathf.Sin(Theta/180*Mathf.PI),direc.y*Mathf.Cos(Theta/180*Mathf.PI)+direc.x*Mathf.Sin(Theta/180*Mathf.PI),direc.z);
    }
}
