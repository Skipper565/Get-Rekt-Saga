using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InputFieldManager : MonoBehaviour {

    public bool isDone = false;

    void Start()
    {
        if (PlayerPrefs.GetString("playerName") != "")
        {
            GameObject.Find("NicknameEdit").SetActive(false);
            isDone = true;
            return;
        }

        GameObject.Find("MainMenu").SetActive(false);
        PlayerController.inMenu = true;
        var input = gameObject.GetComponent<InputField>();
        var se = new InputField.SubmitEvent();
        se.AddListener(SubmitName);
        input.onEndEdit = se;

        //or simply use the line below, 
        //input.onEndEdit.AddListener(SubmitName);  // This also works
    }

    private void SubmitName(string arg0)
    {
        //Debug.Log(arg0);
        if (arg0 == "")
        {
            return;
        }

        PlayerPrefs.SetString("playerName", arg0);

        //Destroy(this.gameObject);
    }

	// Update is called once per frame
	void Update () {
        if (!isDone)
        {
            GameObject.Find("MainMenu").GetComponent<Canvas>().sortingOrder = -1;
        }
        
	}

    public void DoneAndClose()
    {
        GameObject.Find("MainMenu").SetActive(true);
        //GameObject.Find("NicknameEdit").SetActive(false);
        GameObject.Find("NicknameEdit").transform.FindChild("Panel").GetComponent<Animator>().SetTrigger("hideMenu");
        GameObject.Find("MainMenu").GetComponent<Canvas>().sortingOrder = 100;
        isDone = true;
        PlayerController.inMenu = false;
    }
}
