using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonSpawner : MonoBehaviour
{
    public GameObject personPrefab;
    public int numberOfPeople;
    public float impulseTime;
    public bool isGaussian;
    public float spawnProbability;
    float ftimer = 0;
    float ScreenHalfWidth,ScreenHalfHeight;
    float personWidth,personHeight;
    public static float spawnRange = 30;
    public float initialSpeedAverage = 1;
    void Start()
    {
        ScreenHalfHeight = Camera.main.orthographicSize;
        ScreenHalfWidth = ScreenHalfHeight*Camera.main.aspect;
        
        
        personHeight = personPrefab.GetComponent<Renderer>().bounds.size.y;
        print(personPrefab.GetComponent<Renderer>().bounds.size.y);
        // personWidth = personPrefab.GetComponent<Collider>().bounds.size.x;

        // for (int i = 0; i < numberOfPeople; i++)
        // {
        //     Vector3 spawnPoint = new Vector3(-10,Random.Range(-ScreenHalfHeight*WallScript.Haba + personHeight/2, ScreenHalfHeight*WallScript.Haba - personHeight/2),0);
        //     Instantiate(personPrefab,spawnPoint,personPrefab.transform.rotation);
        // }
    }

    void FixedUpdate()
    {
        float rngGoingRight = Random.Range(0f,1f);
        float rngGoingLeft = Random.Range(0f,1f);
        if(isGaussian){
            if(rngGoingRight<=spawnProbability){
                Vector3 spawnPoint = new Vector3(-spawnRange,Random.Range(-ScreenHalfHeight*WallScript.Haba + personHeight, ScreenHalfHeight*WallScript.Haba - personHeight),0);
                GameObject newPerson = Instantiate(personPrefab,spawnPoint,personPrefab.transform.rotation);
                Person newPersonScript = newPerson.GetComponent<Person>();
                newPersonScript.isGoingRight = true;
                newPersonScript.initialSpeed = Random.Range(initialSpeedAverage*0.8f,initialSpeedAverage*1.2f);
                // int Rng = Random.Range(0,3);
                // if(Rng == 2){
                //     newPersonScript.alwaysZenshin = true;
                //     newPersonScript.alwaysYuzuru = false;
                // } 
                // else if(Rng == 1){
                //     newPersonScript.alwaysZenshin = false;
                //     newPersonScript.alwaysYuzuru = true;
                // }
                // else{
                //     newPersonScript.alwaysZenshin = false;
                //     newPersonScript.alwaysYuzuru = false;
                // }
            }
            if(rngGoingLeft<=spawnProbability){
                Vector3 spawnPoint = new Vector3(spawnRange,Random.Range(-ScreenHalfHeight*WallScript.Haba + personHeight, ScreenHalfHeight*WallScript.Haba - personHeight),0);
                GameObject newPerson = Instantiate(personPrefab,spawnPoint,Quaternion.Euler(new Vector3(0,0,90)));
                Person newPersonScript = newPerson.GetComponent<Person>();
                newPersonScript.isGoingRight = false;
                newPersonScript.initialSpeed = Random.Range(initialSpeedAverage*0.8f,initialSpeedAverage*1.2f);
                // int Rng = Random.Range(0,3);
                // if(Rng == 2){
                //     newPersonScript.alwaysZenshin = true;
                //     newPersonScript.alwaysYuzuru = false;
                // } 
                // else if(Rng == 1){
                //     newPersonScript.alwaysZenshin = false;
                //     newPersonScript.alwaysYuzuru = true;
                // }
                // else{
                //     newPersonScript.alwaysZenshin = false;
                //     newPersonScript.alwaysYuzuru = false;
                // }
            }
        }
        else{
            if(rngGoingRight <= Mathf.Exp(-impulseTime/ftimer)){
                Vector3 spawnPoint = new Vector3(-10,Random.Range(-ScreenHalfHeight*WallScript.Haba + personHeight, ScreenHalfHeight*WallScript.Haba - personHeight),0);
                GameObject newPerson = Instantiate(personPrefab,spawnPoint,personPrefab.transform.rotation);
                Person newPersonScript = newPerson.GetComponent<Person>();
                newPersonScript.isGoingRight = true;
                newPersonScript.initialSpeed = Random.Range(0.75f,1.5f);
                ftimer = 0;
            }
            else ftimer += Time.deltaTime;
        }
    }
}
