using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pois : MonoBehaviour
{
    #region Variables
    private GameManager gm;
    private Camera mainCamera;

    private bool dragged = false;
    private bool clicked = false;
    private bool placed = false;
    private bool graphèmeIsShown = true;
    private Vector2 offset;
    private Rigidbody2D rigidBody;
    private GameObject textObject;

    [HideInInspector] public GameObject objectivePoint;
    [HideInInspector] public Graphème graphème;
    [HideInInspector] public PoisSlot slotScript;

    [Header("Physics")]
    [SerializeField] private float speed;
    [SerializeField] private float breakDistance;

    [Space]

    [Header("Slots")]
    [SerializeField] private float validationDistance = 4;
    #endregion

    private void Start()
    {
        gm = GameManager.GetInstance();
        mainCamera = gm.mainCamera;
        rigidBody = GetComponent<Rigidbody2D>();
        textObject = transform.parent.GetChild(1).gameObject;
    }

    private void Update()
    {
        if (placed) return;

        //Drag check
        if (clicked && Vector2.Distance(GetMousePos()-offset, transform.position) > 1)
        {
            clicked = false;
            dragged = true;
        }

        if (dragged)
        {
            Vector2 mousePosition = GetMousePos();

            //Break follow if distance is too great
            if (Vector2.Distance(mousePosition - offset, transform.position) > breakDistance)
            {
                dragged = false;
                rigidBody.velocity = Vector2.zero;
                return;
            }

            //Slow velocity when close to mouse
            rigidBody.velocity = (((mousePosition - offset) - (Vector2)transform.position)) * (Vector2.Distance(transform.position, mousePosition - offset)) * speed;
        }

        //Text follow
        textObject.transform.position = transform.position;
    }

    private void OnMouseDown()
    {
        if (placed) return;
        clicked = true;

        offset = GetMousePos() - (Vector2)transform.position;
    }

    private void OnMouseUp()
    {
        if (placed) return;

        if (clicked)
        {
            clicked = false;
            SetText(!graphèmeIsShown);
        }

        if (dragged)
        {
            dragged = false;
            rigidBody.velocity = rigidBody.velocity*0.1f;

            if (objectivePoint != null && slotScript.graphème.graphème == graphème.graphème && Vector2.Distance(transform.position, objectivePoint.transform.position) < validationDistance)
            {
                placed = true;
                transform.position = objectivePoint.transform.position;
                transform.parent.parent = objectivePoint.transform;
                rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;

                //Text follow
                textObject.transform.position = transform.position;
                if (!graphèmeIsShown)
                {
                    SetText(true);
                }

                gm.PoisPlaced();
            }
        }
    }

    public void SetText(bool showGraphème)
    {
        Text text = transform.parent.GetComponentInChildren<Text>();
        if (showGraphème)
        {
            text.text = graphème.graphème;
            text.fontStyle = FontStyle.Normal;
            graphèmeIsShown = true;
        }
        else
        {
            text.text = graphème.phonème;
            text.fontStyle = FontStyle.Italic;
            graphèmeIsShown = false;
        }
    }

    private Vector2 GetMousePos()
    {
        return mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }
}
