﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonEvents : MonoBehaviour
{
    //Variables||
        
        //UI windows
        public GameObject confirmationWindow;
        public GameObject loginForm;
        public GameObject signupForm;
        public GameObject loadingScreen;

        public GameObject background;
        public GameObject loadingBar;

        public GameObject createAccountLabel;

        //The current index for the game build
    public int MainGame = 0;
    //_________||


    public void OpenNotification()
    {
        //This is gonna show up the confirmation box
        confirmationWindow.SetActive(!confirmationWindow.active);
    }

    public void StartOffline()
    {
        //Start the scene loading
        StartCoroutine("LoadAsync");
    }

    public void ChangeCurrentWindow()
    {
        //Get the current window state
        bool onLogin = loginForm.active;

        if (onLogin)
        {
            //In case the user is on the login window
            loginForm.SetActive(false);
            signupForm.SetActive(true);

            createAccountLabel.GetComponent<Text>().text = "Already have an account?";
        }
        else
        {
            //In case the user is on the signup window
            loginForm.SetActive(true);
            signupForm.SetActive(false);

            createAccountLabel.GetComponent<Text>().text = "Create Account";
        }

    }

    public void CancelOffline()
    {
        //This closes the window
        confirmationWindow.SetActive(false);
    }

    IEnumerator LoadAsync()
    {
        //Add some sort of transition

        //Turn the loading screen on
        loadingScreen.SetActive(true);
        loadingBar.SetActive(true);

        //Just wait
        yield return new WaitForSeconds(1);



        //Start actually loading the new scene
        AsyncOperation opereration = SceneManager.LoadSceneAsync(MainGame);

        //Track the new scene progress
        while (!opereration.isDone)
        {
            //Calculate current progress
            float CurrentProgress = Mathf.Clamp(opereration.progress / 0.9f, 1, 2);

            //Get the bar fill object
            GameObject barFill = loadingScreen.GetComponent<Transform>().GetChild(0).GetChild(0).GetChild(0).gameObject;

            //Change the bar size
            barFill.GetComponent<RectTransform>().localScale = new Vector3(CurrentProgress, 1, 1);

            //Restart the loop
            yield return null;
        }

    }

}

