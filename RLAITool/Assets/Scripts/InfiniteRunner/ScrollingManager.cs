using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CameraExtensions
{
    public static Vector2 GetSizeInViewport(this Camera camera, Vector2 size)
    {
        return camera.ViewportToWorldPoint (
                new Vector3(size.x, size.y, 0f)
            ) - 
            (camera.ViewportToWorldPoint (
                new Vector3(0f, 0f, 0f)
            ));
    }
}

public class ScrollingManager : MonoBehaviour {

    [Header("Reference")]
    public ScrollingObject scrollingObjectPrefab;

    [Header("Objects configuration")]
    public int maxObjects;
    public Vector2 viewportOffset;

    private Transform[] objects;
    private int firstObjectIndex;

    private Vector2 worldOffset;

    private Vector3 scrollDirection;

	// Use this for initialization
	protected void Start () {
        objects = new Transform[maxObjects];

        worldOffset = Camera.main.GetSizeInViewport(viewportOffset);

		GenerateObjectsInArray();

        firstObjectIndex = 0;
	}
	
	// Update is called once per frame
	void Update () {
        GameObject firstObject = objects[firstObjectIndex].gameObject;

        if (IsObjectOutsideScreen(firstObject)) {

            // Remove da posicao inicial
            firstObject = OnObjectReplace(firstObject);
            objects[firstObjectIndex] = firstObject.transform;
            // Atualiza posicao
            objects[firstObjectIndex].localPosition = GetNewObjectPosition(firstObjectIndex);
            // Atualiza index do primeiro elemento
            firstObjectIndex = (firstObjectIndex + 1) % objects.Length;
        }
	}

    public virtual Vector3 GetNewObjectPosition(int currentPosition)
    {
        // Index da posicao anterior
        int lastObjectIndex = 
            (objects.Length + currentPosition - 1) % objects.Length;

        // Posiciona atras do ultimo pipe
        return objects [lastObjectIndex].localPosition +
            Vector3.Scale((-1) * scrollDirection, 
                    scrollingObjectPrefab.size);
    }

    public virtual bool IsObjectOutsideScreen(GameObject goOnFirstPosition)
    {
        Vector3 screenPos = 
            Camera.main.WorldToViewportPoint (
                goOnFirstPosition.transform.position
            );

        return screenPos.x < -0.5f;
    }

    public virtual GameObject OnObjectReplace(GameObject goOnFirstPosition)
    {
        return goOnFirstPosition;
    }

    protected virtual Vector3 GetInitialGenerationPosition()
    {
        Vector3 objectPosition = Camera.main.ViewportToWorldPoint (
                new Vector3(1f + viewportOffset.x, 1f + viewportOffset.y)
            );

        // Initial position should be defined according viewportOffset dimensions
        // If viewportOffset defines only one dimension, So should the objectPosition
        if (viewportOffset.x == 0f)
            objectPosition.x = 0f;
        if (viewportOffset.y == 0f)
            objectPosition.y = 0f;

        return objectPosition;
    }

    protected virtual Vector3 GetScrollDirection()
    {
        return scrollingObjectPrefab.scrollVelocity.normalized;
    }

    public virtual void GenerateObjectsInArray()
    {
        Vector3 objectPosition = GetInitialGenerationPosition();

        scrollDirection = GetScrollDirection();

        for (int i = 0; i < maxObjects; i++) {
            // Encontra a posicao de cada pipe
            objectPosition += Vector3.Scale((-1) * scrollDirection, scrollingObjectPrefab.size) ;

            objects [i] = GenerateObject(objectPosition);
        }
    }

    protected virtual Transform GenerateObject(Vector3 localPosition)
    {
        // Criar cada pipe
        Transform objectTransform = Instantiate(scrollingObjectPrefab.transform) as Transform;
        objectTransform.parent = transform;
        objectTransform.localPosition = localPosition;

        return objectTransform;
    }
}
