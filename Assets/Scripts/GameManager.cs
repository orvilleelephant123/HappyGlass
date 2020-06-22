﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public LineRenderer LineRenderer;
    private int pointCount = 0;
    private List<Vector2> pointList = new List<Vector2>();
    private bool allowDraw = true;
    private bool drawEnded = false;

    public Slider PenValue;
    public Text PercentText;
    public GameObject Star1;
    public GameObject Star2;
    public GameObject Star3;

    public LayerMask LayerMask;

    public Rigidbody2D Rig;

    public ParticleGenerator particleGenerator;
    public GameObject Pencil;

    private void Awake()
    {
        Pencil.SetActive(false);
        LineRenderer.positionCount = 0;
        PenValue.value = 1.0f;
        PercentText.text = "100%";
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0) && allowDraw)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (!pointList.Contains(mousePosition))
            {
                Pencil.SetActive(true);
                Pencil.transform.position = mousePosition;

                LineRenderer.positionCount = pointCount + 1;
                LineRenderer.SetPosition(pointCount, mousePosition);
                pointCount++;
                pointList.Add(mousePosition);

                if (pointList.Count > 1)
                {
                    var point1 = pointList[pointCount - 2];
                    var point2 = pointList[pointCount - 1];

                    var distance = Vector3.Distance(point1,point2);
                    PenValue.value = PenValue.value - distance / 10.0f;
                    PercentText.text = (int)(PenValue.value * 100) + "%";
                    Star3.SetActive(PenValue.value > 0.75f);
                    Star2.SetActive(PenValue.value > 0.5f);
                    Star1.SetActive(PenValue.value > 0.25f);

                    if (PenValue.value <= 0)
                    {
                        allowDraw = false;
                        Pencil.SetActive(false);
                    }

                    var currentColliderObject = new GameObject("Collider");
                    currentColliderObject.transform.position = (point1 + point2) / 2;
                    currentColliderObject.transform.parent = LineRenderer.gameObject.transform;
                    currentColliderObject.transform.right = (point2 - point1).normalized;

                    var currentBoxCollider2D = currentColliderObject.AddComponent<BoxCollider2D>();
                    currentBoxCollider2D.size = new Vector2((point2 - point1).magnitude, 0.05f);

                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    var result = Physics2D.Raycast(ray.origin, Vector3.forward,100, LayerMask);
                    if (result)
                    {
                        Debug.Log(result.transform.name);
                        allowDraw = false;
                    }
                }

            }
        }
        if (Input.GetMouseButtonUp(0) || !allowDraw)
        {
            allowDraw = false;
            DrawEnd();
        }
    }

    void DrawEnd()
    {
        if (!drawEnded)
        {
            drawEnded = true;
            Rig.bodyType = RigidbodyType2D.Dynamic;
            Rig.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            Rig.mass = 100.0f;
            particleGenerator.GenerateParticle();
            Pencil.SetActive(false);
        }
    }
}