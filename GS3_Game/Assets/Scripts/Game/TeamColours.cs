using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem;

public class TeamColours : MonoBehaviour
{
    public static TeamColours Instance;
    private MaterialPropertyBlock mpb;
    [SerializeField] private Renderer targetRenderer;
    private void Awake()
    {
        /*
        mpb = new MaterialPropertyBlock();
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }*/
    }

    public void SetTexture(Texture2D texture)
    {
        targetRenderer.GetPropertyBlock(mpb);
        mpb.SetTexture("_Team_Texture", texture);
        targetRenderer.SetPropertyBlock(mpb);
    }

    public int team1Index = 0;
    public int team2Index = 1;
    private List<Material> colours = new List<Material>();
    public List<Texture2D> textures = new List<Texture2D>();
    [Header("Team1 materials")]
    public Material Team1Mat;
    public Material Team1Mat2;
    public Material Team1Tank;
    [Header("Team2 materials")]
    public Material Team2Mat;
    public Material Team2Mat2;
    public Material Team2Tank;
    private void Start()
    {
        colours = GetComponent<Colours>().colours;
        //Team 1 textures
        SetTeam1();
        //Team 2 textures
        SetTeam2();
    }
    public void NextColour(int playerNo)
    {
        //players 1 & 3
        if (playerNo % 2 == 0)
        {
            team1Index++;
            if (team1Index >= textures.Count)
            {
                team1Index = 0;
            }
            SetTeam1();
        }
        else
        {
            team2Index++;
            if (team2Index >= textures.Count)
            {
                team2Index = 0;
            }
            SetTeam2();
        }
    }
    public void PreviousColour(int playerNo)
    {
        //players 1 & 3
        if (playerNo % 2 == 0)
        {
            team1Index--;
            if (team1Index < 0)
            {
                team1Index = textures.Count-1;
            }
            SetTeam1();
        }
        else
        {
            team2Index--;
            if (team2Index < 0)
            {
                team2Index = textures.Count-1;
            }
            SetTeam2();
        }
    }
    private void SetTeam1()
    {
        Team1Mat.SetTexture("_Team_Texture", textures[team1Index]);
        Team1Mat2.SetTexture("_Team_Texture", textures[team1Index]);
        Team1Tank.SetTexture("_BaseMap", textures[team1Index]);
        Team1Tank.SetTexture("_EmissionMap", textures[team1Index]);
    }
    private void SetTeam2()
    {
        Team2Mat.SetTexture("_Team_Texture", textures[team2Index]);
        Team2Mat2.SetTexture("_Team_Texture", textures[team2Index]);
        Team2Tank.SetTexture("_BaseMap", textures[team2Index]);
        Team2Tank.SetTexture("_EmissionMap", textures[team2Index]);
    }
}
