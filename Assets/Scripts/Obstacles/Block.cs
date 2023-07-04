using TMPro;
using UnityEngine;
using DG.Tweening;

public class Block : MonoBehaviour
{
    [Header("Size & Color")]
    [SerializeField] private int startingSize;
    [SerializeField] private Material[] blockColor;
    [SerializeField] private MeshRenderer blockMesh;

    [Header("References")]
    [SerializeField] private GameObject completeBlock;
    [SerializeField] private GameObject brokenBlock;
    [SerializeField] private TextMeshPro blockSizeText;
    [SerializeField] private GameObject pickupPrefab; // The pickup prefab to be spawned
    [SerializeField] private float spawnOffset = 1f; // Offset distance from the broken block

    private void Awake()
    {
        completeBlock.SetActive(true);
        brokenBlock.SetActive(false);
        blockSizeText.text = startingSize.ToString();
        AssignColor();
    }

    private void AssignColor()
    {
        int colorIndex = (startingSize - 1) / 3;
        colorIndex = Mathf.Clamp(colorIndex, 0, blockColor.Length - 1);
        blockMesh.material = blockColor[colorIndex];
    }

    public void CheckHit()
    {
        Handheld.Vibrate();
        Camera.main.transform.DOShakePosition(0.1f, 0.5f, 5);

        if (GameEvents.instance.playerSize.Value > startingSize)
        {
            ParticleManager.instance.PlayParticle(0, transform.position);
            GameEvents.instance.playerSize.Value -= startingSize;
            completeBlock.SetActive(false);
            brokenBlock.SetActive(true);
            blockSizeText.gameObject.SetActive(false);
            SpawnPickup();
        }
        else
        {
            GameEvents.instance.gameLost.SetValueAndForceNotify(true);
        }
    }

    public void CheckHitWithoutSize()
    {
        Handheld.Vibrate();
        Camera.main.transform.DOShakePosition(0.1f, 0.5f, 5);

        ParticleManager.instance.PlayParticle(0, transform.position);
        completeBlock.SetActive(false);
        brokenBlock.SetActive(true);
        blockSizeText.gameObject.SetActive(false);
        SpawnPickup();
    }

    private void SpawnPickup()
    {
        Vector3 spawnPosition = transform.position + transform.forward * spawnOffset;
        Instantiate(pickupPrefab, spawnPosition, Quaternion.identity);
    }
}
