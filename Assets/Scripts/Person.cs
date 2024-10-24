using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

public class Person : MonoBehaviour
{
    public float speed;  
    public bool isMoving = false; 
    public Vector3 targetPosition; 
    private GroupManager groupManager;
    private HashSet<float> usedSpeeds = new HashSet<float>();
    TextMeshPro SpeedText;
    private void Awake()
    {
        groupManager = FindAnyObjectByType<GroupManager>();
        SpeedText = GetComponentInChildren<TextMeshPro>();
        if (groupManager == null && SpeedText == null)
        {
            return;
        }
    }
    private void Start()
    {
        speed = GetUniqueRandomSpeed();
    }
    void Update()
    {
        if (isMoving)
        {
            MoveToTarget();
        }
        ApplySeparation();
    }
    float GetUniqueRandomSpeed()
    {
        float newSpeed;
        do
        {
            newSpeed = Random.Range(2f, 10f);
        }
        while (usedSpeeds.Contains(newSpeed)); 
        usedSpeeds.Add(newSpeed);
        SpeedText.text = newSpeed.ToString("F2");
        return newSpeed;
    }
    void ApplySeparation()
    {
        Person[] people = FindObjectsOfType<Person>();
        Vector3 separationForce = Vector3.zero;
        foreach (var person in people)
        {
            if (person != this)
            {
                float distance = Vector3.Distance(transform.position, person.transform.position);
                if (distance < 1f)
                {
                    Vector3 direction = transform.position - person.transform.position;
                    separationForce += direction.normalized / distance;
                }
            }
        }
        transform.position += separationForce * Time.deltaTime;
    }
    public void MoveToTarget(Vector3 target)
    {
        targetPosition = target;
        isMoving = true;
    }
    void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            isMoving = false;
            groupManager.OnPersonReachedTable(this);
        }
    }
    public IEnumerator InteractWithTable()
    {
        transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        GetComponent<MeshRenderer>().material.color = Color.red;
        yield return new WaitForSeconds(5f);
        Vector3 randomPosition = new Vector3(Random.Range(0,7), transform.position.y, Random.Range(-10f, -20f));
        MoveToTarget(randomPosition);
        transform.localScale = Vector3.one / 2;
        GetComponent<MeshRenderer>().material.color = Color.white;
        groupManager.OnTableInteractionComplete();
       
    }
    public void SetGroupManager(GroupManager manager)
    {
        groupManager = manager;
    }
}

