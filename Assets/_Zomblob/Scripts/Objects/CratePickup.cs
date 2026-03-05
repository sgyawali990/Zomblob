using UnityEngine;

public class CratePickup : MonoBehaviour
{
    public GameObject weaponPrefab;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Transform socket = other.transform.Find("PlayerModel/WeaponSocket");

            if (socket != null)
            {
                GameObject weapon = Instantiate(weaponPrefab, socket);

                weapon.transform.localPosition = Vector3.zero;

                weapon.transform.localRotation = weaponPrefab.transform.localRotation;
            }

            Destroy(gameObject);
        }
    }
}