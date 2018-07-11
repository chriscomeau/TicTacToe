using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GridSpace : MonoBehaviour {

    public Button button;
    public Text buttonText;
    //public string playerSide;
    //private Image imageX;
    //private Image imageO;
    public Sprite spriteX;
    public Sprite spriteO;
    public Sprite spriteCoin;
    public Sprite spriteBomb;
    public Sprite spriteEmpty;

    private GameController gameController;


    public void SetGameControllerReference(GameController controller)
    {
        gameController = controller;
    }

    public void ResetSpace()
    {
        button.image.sprite = spriteEmpty;


        Button button2 = button.GetComponentInParent<Button>();
        Color newColor = Color.clear;

        button2.GetComponent<Image>().color = newColor;
    }


    //scale
    IEnumerator MyCoroutine()
    {
        scale();
        yield return new WaitForSeconds(0.3f);
        normalFlash();
    }

    /*void OnCollisionEnter(Collision _other)
    {
        _other.rigidbody.AddExplosionForce(explosionStrength, this.transform.position, 5);
        source.PlayOneShot(bounceNoise, 1f);
        StartCoroutine(MyCoroutine());
    }*/

    void scale()
    {
        button.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
    }

    void normalFlash()
    {
        button.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }


    public void SetWin()
    {
        Button button2 = button.GetComponentInParent<Button>();

        //Color newColor = Color.green;
        Color newColor = new Color32(0x00, 0xFF, 0x51, 0xFF);
        //00ff51
        button2.GetComponent<Image>().color = newColor;
    }

    public void SetLose()
    {
        Button button2 = button.GetComponentInParent<Button>();

        //Color newColor = Color.red;
        Color newColor = new Color32(0xfF, 0x00, 0x49, 0xFF);
        //ff0049

        button2.GetComponent<Image>().color = newColor;
    }

    public void SetSpace()
    {
        //smaller
        button.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);


        if(buttonText.text != "")
        {
            //invalid
            gameController.PlayWrongSound();

            return;
        }

        buttonText.text = gameController.GetPlayerSide();

        button.image.sprite = (buttonText.text == "X") ? spriteX : spriteO;
        Button button2 = button.GetComponentInParent<Button>();


        Color newColor = Color.white;

        if (buttonText.text == "X")
        {
            //x

            Color[] newColors = new Color[] 
            {
                Color.white,

                new Color32(0xfd, 0x04, 0x82, 0xFF), //pink
                new Color32(0x35, 0xda, 0xed, 0xFF), //turquoise
                //new Color32(0xf2, 0xdb, 0x0f, 0xFF), //yellow
                new Color32(0x8f, 0x0e, 0xfc, 0xFF), //purple
                new Color32(0xF8, 0x87, 0x58, 0xFF), //orange, f87858

                new Color32(0x68, 0x88, 0xFC, 0xFF),//blue, 6888fc
                //more colors Yellow, orange, purple


            };

            //loop until different
            do
            {
                //Utils.Shuffle(newColors);
                int index = Random.Range(0, newColors.Length);
                newColor = newColors[index];

            } while (gameController.lastColors.Any(item => item == newColor));


            //add it
            gameController.lastColors.Add(newColor);

            if(newColors.Length == gameController.lastColors.Count)
            {
                //full, reset
                gameController.lastColors = new List<Color32>();
            }
                                              
        }
        else 
        {
            //o
            newColor = new Color(0.7f, 0.7f, 0.7f, 1f);
        }


        button2.GetComponent<Image>().color = newColor;


        //button.GetComponent<Image>().image = imageX;



        button.interactable = false;

        gameController.EndTurn();

        gameController.PlayClickSound();


        //scale?
        //StartCoroutine(MyCoroutine());
    }

    public void SetCoin()
    {
        //smaller
        //button.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        button.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);


        //buttonText.text = "C"; //gameController.GetPlayerSide();

        button.image.sprite = spriteCoin;
        Button button2 = button.GetComponentInParent<Button>();


        Color newColor = Color.white;

        newColor = new Color32(0xf2, 0xdb, 0x0f, 0xFF); //yellow;

        button2.GetComponent<Image>().color = newColor;


        //button.GetComponent<Image>().image = imageX;



        //button.interactable = false;

        //gameController.EndTurn();

        //gameController.PlayClickSound();


        //scale?
        //StartCoroutine(MyCoroutine());
    }


	// Use this for initialization
	/*void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}*/
}
