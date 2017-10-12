using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

    public LevelManagement lvlMan;

    public void StartGame() {

        SceneManager.LoadScene("Level_One");
        PlayerController.atomicNumber = 1;

    }

    public void ContinueGame() {

        SceneManager.LoadScene("Level" + lvlMan.lvlNum++);

    }

}
