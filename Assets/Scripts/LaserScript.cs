using UnityEngine;

public class LaserScript : MonoBehaviour
{
    private SpriteRenderer laserHitRenderer;
    private SpriteRenderer laserHitBgRenderer;
    private SpriteRenderer laserNoseRenderer;

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        laserHitRenderer = GetComponentsInChildren<SpriteRenderer>()[0];
        laserHitRenderer.enabled = false;

        laserHitBgRenderer = GetComponentsInChildren<SpriteRenderer>()[1];
        laserHitBgRenderer.enabled = false;

        laserNoseRenderer = GetComponentsInChildren<SpriteRenderer>()[2];
    }

    void Update()
    {
        lineRenderer.SetPosition(0, transform.position);
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x + 1, transform.position.y), transform.right);

        laserNoseRenderer.transform.position = transform.position;

        if (hit.collider && (hit.collider.CompareTag("Veggie") || hit.collider.CompareTag("VeggieDone")))
        {
            lineRenderer.SetPosition(1, new Vector3(hit.point.x, hit.point.y, transform.position.z));
            laserHitRenderer.transform.position = new Vector2(hit.point.x, hit.point.y);
            laserHitRenderer.enabled = true;

            laserHitBgRenderer.transform.position = new Vector2(hit.point.x, hit.point.y);
            laserHitBgRenderer.enabled = true;
            if (hit.collider.CompareTag("Veggie"))
            {
                hit.collider.gameObject.GetComponent<VegScript>().veggieDown();
            }
        }
        else
        {
            lineRenderer.SetPosition(1, transform.right * 2000);
            laserHitRenderer.enabled = false;
            laserHitBgRenderer.enabled = false;
        }
    }
}
