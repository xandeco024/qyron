using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SimpleItem : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] ItemData itemData;
    [SerializeField] float speed;

    void Awake()
    {
        GetComponent<SpriteRenderer>().sprite = itemData.sprite;
    }

    void Start()
    {
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        GetComponent<Rigidbody>().AddForce(randomDirection * speed, ForceMode.Impulse);
    }

    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<PlayableCharacter>())
        {
            PlayableCharacter player = collision.gameObject.GetComponent<PlayableCharacter>();
            player.Heal(itemData.health);
            Vector3 textPosition = new Vector3(player.transform.position.x, player.transform.position.y + 1, player.transform.position.z - 1);
            Instantiate(itemData.textPrefab, textPosition, Quaternion.identity).GetComponentInChildren<Text>().SetHeal(itemData.health.ToString());
            Destroy(gameObject);
        }
    }
}
