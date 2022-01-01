using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Variables
    //INSPECTOR

    [Header("Liste de mots")]
    [SerializeField] private List<Mot> mots;

    [Space]

    [Header("Paramètres")]
    [SerializeField] private int motsNb;
    [SerializeField] private bool randomize;
    [SerializeField] private int additionalGraphèmes = 2;

    [Space]

    [Header("Holders")]
    [SerializeField] private GameObject cosseHolder;
    [SerializeField] private SpriteRenderer motImageHolder;

    [Space]

    [Header("Prefabs")]
    [SerializeField] private GameObject poisPrefab;
    [SerializeField] private GameObject cosseStart;
    [SerializeField] private GameObject cosseSegment;
    [SerializeField] private GameObject cosseEnd;

    [Space]

    [Header("FX")]
    [SerializeField] private Sprite poisHighlight;

    [Space]

    [Header("UI")]
    [SerializeField] private GameObject canvas;


    //GAME

    static GameManager instance;

    [HideInInspector] public Camera mainCamera;
    [HideInInspector] private AudioManager aM;

    private int motIndex = 0;
    private int slotIndex = 0;
    private List<Mot> motsList;
    private List<PoisSlot> slotsList;
    private List<GameObject> additionalPois;
    private Graphème[] graphèmes;
    #endregion
    public static GameManager GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);

        instance = this;

        mainCamera = Camera.main;

        graphèmes = Resources.LoadAll<Graphème>("ScriptableObjects/Graphèmes");
    }

    private void Start()
    {
        aM = AudioManager.GetInstance();
    }

    private List<Mot> SetNewList(List<Mot> list)
    {
        if (randomize)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Mot temp = list[i];
                int randomIndex = Random.Range(i, mots.Count);
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }
        Debug.Log(list);

        if(motsNb > 0)
        {
            List<Mot> tempList = new List<Mot>();

            for (int i = 0; i < motsNb; i++)
            {
                tempList.Add(list[i]);
            }
            list = tempList;
        }

        return list;
    }

    private void SetMot()
    {
        Mot currentMot = motsList[motIndex];
        slotsList = new List<PoisSlot>();
        additionalPois = new List<GameObject>();
        motImageHolder.sprite = currentMot.image;
        aM.ChangeClip("MotAnnounce", currentMot.audio);
        aM.Play("MotAnnounce");

        float cosseStartSize = cosseStart.transform.GetChild(1).GetComponent<SpriteRenderer>().bounds.size.x;
        float cosseEndSize = cosseEnd.transform.GetChild(1).GetComponent<SpriteRenderer>().bounds.size.x;
        float cosseSegmentSize = cosseSegment.transform.GetChild(1).GetComponent<SpriteRenderer>().bounds.size.x;
        float cosseXSize = cosseStartSize + cosseEndSize + cosseSegmentSize*(currentMot.graphèmes.Count-2);
        float nextX = 0;
        for (int i = 0; i < currentMot.graphèmes.Count; i++)
        {
            if (i == 0)
            {
                Vector3 pos = cosseHolder.transform.position + new Vector3(-(cosseXSize / 2) + cosseStartSize / 2, 0f, 0f);
                nextX = pos.x + cosseStartSize / 2 + cosseSegmentSize / 2;
                GameObject newCosse = GameObject.Instantiate(cosseStart, pos, cosseStart.transform.rotation, cosseHolder.transform);
                GameObject poisSlot = newCosse.transform.GetChild(0).gameObject;
                PoisSlot slotScript = poisSlot.GetComponent<PoisSlot>();
                slotsList.Add(slotScript);
                Vector2 position = new Vector2(Random.Range(cosseHolder.transform.position.x - (cosseXSize / 2), cosseHolder.transform.position.x + (cosseXSize / 2)), cosseHolder.transform.position.y);
                CreatePois(position, poisSlot, currentMot.graphèmes[i], slotScript);
            }
            else if (i == currentMot.graphèmes.Count - 1)
            {
                Vector3 pos = cosseHolder.transform.position + new Vector3((cosseXSize / 2) - cosseEndSize / 2, 0, 0);
                GameObject newCosse = GameObject.Instantiate(cosseEnd, pos, cosseStart.transform.rotation, cosseHolder.transform);
                GameObject poisSlot = newCosse.transform.GetChild(0).gameObject;
                PoisSlot slotScript = poisSlot.GetComponent<PoisSlot>();
                slotsList.Add(slotScript);
                Vector2 position = new Vector2(Random.Range(cosseHolder.transform.position.x - (cosseXSize / 2), cosseHolder.transform.position.x + (cosseXSize / 2)), cosseHolder.transform.position.y);
                CreatePois(position, poisSlot, currentMot.graphèmes[i], slotScript);
            }
            else
            {
                Vector3 pos = cosseHolder.transform.position + new Vector3(nextX, 0, 0);
                nextX = pos.x + cosseSegmentSize;
                GameObject newCosse = GameObject.Instantiate(cosseSegment, pos, cosseStart.transform.rotation, cosseHolder.transform);
                GameObject poisSlot = newCosse.transform.GetChild(0).gameObject;
                PoisSlot slotScript = poisSlot.GetComponent<PoisSlot>();
                slotsList.Add(slotScript);
                Vector2 position = new Vector2(Random.Range(cosseHolder.transform.position.x - (cosseXSize / 2), cosseHolder.transform.position.x + (cosseXSize / 2)), cosseHolder.transform.position.y);
                CreatePois(position, poisSlot, currentMot.graphèmes[i], slotScript);
            }
        }

        for(int i = 0; i < additionalGraphèmes; i++)
        {
            Vector2 position = new Vector2(Random.Range(cosseHolder.transform.position.x - (cosseXSize / 2), cosseHolder.transform.position.x + (cosseXSize / 2)), cosseHolder.transform.position.y);
            CreatePois(position, graphèmes[Mathf.RoundToInt(Random.Range(0, graphèmes.Length))]);
        }

        StartCoroutine(IStartHighlightTimer());
    }

    private void CreatePois(Vector2 pos, GameObject parentCosse, Graphème graphème, PoisSlot slotScript)
    {
        GameObject newPois = Instantiate(poisPrefab, parentCosse.transform.position, transform.rotation);
        Pois poisScript = newPois.GetComponentInChildren<Pois>();
        poisScript.objectivePoint = parentCosse;
        poisScript.slotScript = slotScript;
        poisScript.graphème = graphème;
        poisScript.SetText(true);
    }

    private void CreatePois(Vector2 position, Graphème graphème)
    {
        GameObject newPois = Instantiate(poisPrefab, position, transform.rotation);
        Pois poisScript = newPois.GetComponentInChildren<Pois>();
        poisScript.graphème = graphème;
        poisScript.SetText(true);
        additionalPois.Add(newPois);
    }

    public void PoisPlaced()
    {
        slotsList[slotIndex].graphème = null;

        //Word complete check
        if(slotIndex >= motsList[motIndex].graphèmes.Count - 1)
        {
            motIndex += 1;

            //Win check 
            if(motIndex >= motsNb)
            {
                Debug.Log("Win !");

                slotIndex = 0;
                motIndex = 0;

                foreach (PoisSlot slot in slotsList)
                {
                    Destroy(slot.transform.parent.gameObject);
                }
                foreach (GameObject pois in additionalPois)
                {
                    Destroy(pois);
                }

                motImageHolder.sprite = null;

                canvas.SetActive(true);

                return;
            }

            slotIndex = 0;
            foreach(PoisSlot slot in slotsList)
            {
                Destroy(slot.transform.parent.gameObject);
            }
            foreach(GameObject pois in additionalPois)
            {
                Destroy(pois);
            }

            SetMot();
        }
        else
        {
            slotIndex += 1;
            slotsList[slotIndex].graphème = motsList[motIndex].graphèmes[slotIndex];
            slotsList[slotIndex].GetComponent<SpriteRenderer>().sprite = poisHighlight;
        }
    }

    IEnumerator IStartHighlightTimer()
    {
        yield return new WaitForSeconds(1f);
        slotsList[slotIndex].graphème = motsList[motIndex].graphèmes[slotIndex];
        slotsList[slotIndex].GetComponent<SpriteRenderer>().sprite = poisHighlight;
        StopHighlightTimer();
    }

    private void StopHighlightTimer()
    {
        StopCoroutine(IStartHighlightTimer());
    }

    public void StartGame()
    {
        motsList = SetNewList(mots);
        SetMot();
        canvas.SetActive(false);
    }
}
