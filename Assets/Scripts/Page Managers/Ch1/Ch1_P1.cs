using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class Ch1_P1 : PageManager 
{
	public static int numbOfFails;
    public GameObject allSprites;
    public Image propCandle;
    public Image propSalt;
    public Image propCup;	
	public Image emptyRestaurant;
    public Image fayeErnestRestaurant;
    public Image caption1;
    public Image caption2;
    public Image speechBubble1;
    public Image speechBubble2;
    public Image speechBubble3;
    public Image speechBubble4;
    public Image speechBubble5;
    public Image speechBubble6;
    public Image speechBubble7;
    public Image speechBubble8;
    CutscenePlayer cutscenePlayer;
	Fader faderUtility;
	Image[] spriteRenderers;

    bool isAcceptingInput = false;
    bool nextStepRequested = false;

	void Awake()
	{
		cutscenePlayer = FindFirstObjectByType<CutscenePlayer>();
        faderUtility = FindFirstObjectByType<Fader>();
		spriteRenderers = allSprites.GetComponentsInChildren<Image>();        
    }

	protected override void Start()
	{
		base.Start();        
        Debug.Log ("numbOfFails: " + numbOfFails);
        StartCoroutine(PlayStorySequence());
        StartCoroutine(BeginInputCooldown(3.0f));
    }

	void Update()
	{
		//numbOfFails = (State)cutscenePlayer.GetIntegerVariable("State");
	}

    public void	fail()
	{
		numbOfFails+=1;		
		SetSolved (advance: false);
	}

	public void win()
    {
		SetSolved (advance: true);
	}

	IEnumerator PlayStorySequence()
	{
		if (numbOfFails == 0)
		{            
            SetSpriteRenderersVisibility(false);            

            yield return new WaitForSeconds(2);
            
			faderUtility.FadeInUiImage(emptyRestaurant);
            Camera.main.GetComponent<CameraSaturation>().Desaturate();
            
            yield return new WaitUntil(() => nextStepRequested == true);
            nextStepRequested = false;

            faderUtility.FadeInUiImage(fayeErnestRestaurant);
            faderUtility.FadeInUiImage(propCandle);
            faderUtility.FadeInUiImage(propCup);
            faderUtility.FadeInUiImage(propSalt);

            yield return new WaitUntil(() => nextStepRequested == true);
            nextStepRequested = false;

			faderUtility.FadeInUiImage(caption1);

            yield return new WaitUntil(() => nextStepRequested == true);
            nextStepRequested = false;

            faderUtility.FadeInUiImage(speechBubble1);

            yield return new WaitUntil(() => nextStepRequested == true);
            nextStepRequested = false;

            faderUtility.FadeInUiImage(speechBubble2);

            yield return new WaitUntil(() => nextStepRequested == true);
            nextStepRequested = false;

            faderUtility.FadeOutUiImage(speechBubble1, 0.5f);
            faderUtility.FadeInUiImage(speechBubble3, 0.5f);

            yield return new WaitUntil(() => nextStepRequested == true);
            nextStepRequested = false;

            faderUtility.FadeOutUiImage(speechBubble2, 0.5f);
            faderUtility.FadeInUiImage(speechBubble4, 2);

            yield return new WaitUntil(() => nextStepRequested == true);
            nextStepRequested = false;

            faderUtility.FadeOutUiImage(caption1, 1);
            faderUtility.FadeOutUiImage(speechBubble3, 1);
            faderUtility.FadeOutUiImage(speechBubble4, 1);

            yield return new WaitForSeconds(2.0f);            

            Camera.main.GetComponent<CameraSaturation>().Saturate();

        }
		else if (numbOfFails >= 1)
		{            
            SetSpriteRenderersVisibility(false);            
			faderUtility.FadeInUiImage(fayeErnestRestaurant);
            faderUtility.FadeInUiImage(propCandle);
            faderUtility.FadeInUiImage(propCup);
            faderUtility.FadeInUiImage(propSalt);			
        }
        

		yield return null;
	}
	void SetSpriteRenderersVisibility(bool isVisible)
	{
		foreach (Image sprite in spriteRenderers)
		{
			if (isVisible)
				sprite.color = new Color(1, 1, 1, 1);
			else
                sprite.color = new Color(1, 1, 1, 0);            
		}
	}

    public void OnScreenClicked()
    {        
        if (!isAcceptingInput)
            return;
        
        nextStepRequested = true;
        StartCoroutine(BeginInputCooldown());
    }

    IEnumerator BeginInputCooldown(float clickCooldownTime = 2.0f)
    {
        isAcceptingInput = false;
        yield return new WaitForSeconds(clickCooldownTime);
        isAcceptingInput = true;
    }
}
