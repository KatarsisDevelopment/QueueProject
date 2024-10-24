using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GroupManager : MonoBehaviour
{
    public List<Person> people;  
    public Transform tablePosition;
    private Queue<Person> queue = new Queue<Person>();
    private bool isTableOccupied = false;
    public GameObject PersonPrefab;
    private void Awake()
    {
        for (int i = 0; i < 30; i++)
        {
            var newObject = Instantiate(PersonPrefab, GetRandomPosition(), Quaternion.identity);
            people.Add(newObject.GetComponent<Person>());
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SelectRandomPerson();
        }
        if (!isTableOccupied && queue.Count > 0)
        {
            Person nextPerson = queue.Peek(); 
            nextPerson.MoveToTarget(tablePosition.position);  
        }
    }
    Vector3 GetRandomPosition()
    {
        float x = Random.Range(-10f, 0f);
        float y = 1;
        float z = Random.Range(5f, 10f);
        return new Vector3(x, y, z);
    }
    void SelectRandomPerson()
    {
        List<Person> availablePeople = new List<Person>();
        foreach (Person p in people)
        {
            if (!p.isMoving && !queue.Contains(p) && p != null)
            {
                availablePeople.Add(p);
            }
        }
        if (availablePeople.Count > 0)
        {
            Person randomPerson = availablePeople[Random.Range(0, availablePeople.Count)];
            queue.Enqueue(randomPerson);
            Vector3 queuePosition = GetQueuePosition();
            randomPerson.MoveToTarget(queuePosition);
        }
    }
    Vector3 GetQueuePosition()
    {
        return tablePosition.position + new Vector3(0, 0, queue.Count * 2f);
    }
    public void OnPersonReachedTable(Person person)
    {
        float TableDistance = Vector3.Distance(person.transform.position, tablePosition.position);
        if (TableDistance < 1f)
        {
            isTableOccupied = true;  
            queue.Dequeue();
            StartCoroutine(person.InteractWithTable());
        }
    }
    public void OnTableInteractionComplete()
    {
        isTableOccupied = false;
    }
}

