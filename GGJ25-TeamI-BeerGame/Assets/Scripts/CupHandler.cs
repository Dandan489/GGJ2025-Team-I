using Unity.Mathematics;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CupHandler : MonoBehaviour
{
    public Transform canTransform;
    public Transform cupEdgeL;
    public Transform cupEdgeR;
    public Transform pourPoint;

    private float maxVolume = 200000f;
    private float curBubble = 0f;
    private float curFluid = 0f;

    public Transform fluid;
    public Transform bubble;
    private float maxFluidHeight = 5f;
    public Sprite drip;
    public Sprite stream;
    public SpriteRenderer streamRenderer;
    private float streamThreshold = 30f;

    public Bottles chosenBottle;

    // private float bubbleDecayRate = 0.01f;

    private void Start() {
        Restart();
    }

    private void Update() {
        List<float> getSpeed = GetPouringSpeed(canTransform.eulerAngles.z);
        float fluidSpeed = getSpeed[0];
        float bubbleSpeed = getSpeed[1];
        Debug.Log((fluidSpeed+bubbleSpeed));
        if((fluidSpeed+bubbleSpeed) > streamThreshold){
            streamRenderer.sprite = stream;
        }
        else if((fluidSpeed+bubbleSpeed) > 0){
            streamRenderer.sprite = drip;
        }
        else{
            streamRenderer.sprite = null;
        }
        if(pourPoint.position.x >= cupEdgeL.position.x && pourPoint.position.x <= cupEdgeR.position.x){
            curFluid += fluidSpeed;
            curBubble += bubbleSpeed;
            AdjustScale();
        }
        // DecayBubbles();
    }

    private void AdjustScale(){
        float fluidScale = curFluid/maxVolume*maxFluidHeight;
        fluid.localScale = new Vector3(1, fluidScale, 1);
        float bubbleScale = curBubble/maxVolume*maxFluidHeight;
        if(fluidScale!=0) bubbleScale/=fluidScale;
        bubble.localScale = new Vector3(1, bubbleScale, 1);
    }

    private List<float> GetPouringSpeed(float tiltAngle)
    {
        List<float> returnVal = new List<float> { 0f, 0f };
        tiltAngle-=90f;
        tiltAngle = math.max(tiltAngle, 0f);
        returnVal[0] = tiltAngle * chosenBottle.speedFactor * chosenBottle.beerPercentage[0];
        returnVal[1] = tiltAngle * chosenBottle.speedFactor * chosenBottle.foamPercentage[0];
        for (int i = chosenBottle.thresholdAngle.Count-1; i >= 0 ; i--) {
            if (tiltAngle > chosenBottle.thresholdAngle[i]) {
                returnVal[0] = (tiltAngle) * chosenBottle.speedFactor * chosenBottle.beerPercentage[i+1];
                returnVal[1] = (tiltAngle) * chosenBottle.speedFactor * chosenBottle.foamPercentage[i+1];
                break;
            }
        }
        return returnVal;
    }

    // private void DecayBubbles()
    // {
    //     if (curBubble > 0)
    //     {
    //         curBubble -= curBubble * bubbleDecayRate * Time.deltaTime;
    //         curBubble = math.max(curBubble, 0);
    //         AdjustScale();
    //     }
    // }

    public bool IsFull(){
        return curBubble+curFluid > maxVolume;
    }

    public float GetPercentage(){
        if((curBubble+curFluid)/maxVolume < 0.8f) return -1f;
        return curBubble/(curBubble+curFluid);
    }

    public void Restart(){
        curBubble = 0f;
        curFluid = 0f;
        AdjustScale();
    }
}
