using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
{
    public static float Range, PersonalSpace,ChooseSideRange,AverageSpeed,RateOfPeople;
    public Slider RangeSlider, PersonalSpaceSlider,ChooseSideRangeSlider,AverageSpeedSlider,RateOfPeopleSlider;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Range = RangeSlider.value;
        PersonalSpace = PersonalSpaceSlider.value;
        ChooseSideRange = ChooseSideRangeSlider.value;
        AverageSpeed = AverageSpeedSlider.value;
        RateOfPeople = RateOfPeopleSlider.value;
    }

    public void Hajimeru(){
        SceneManager.LoadScene("Shoutengai");
    }
    public void Default(){
        SceneManager.LoadScene("Menu");
    }
}
