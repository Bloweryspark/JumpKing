using UnityEngine;

public class GatoDialogo : MonoBehaviour
{
    public GameObject dialogo;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            dialogo.SetActive(true);
        }

          Debug.Log("Algo entro al trigger");

    if(other.CompareTag("Player"))
    {
        dialogo.SetActive(true);
    }

    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            dialogo.SetActive(false);
        }
    }

}