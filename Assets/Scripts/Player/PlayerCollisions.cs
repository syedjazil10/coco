using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private GameObject bloodParticles;
    [SerializeField] private AudioClip damage;
    [SerializeField] private AudioClip gate;
    [SerializeField] private AudioClip wall;
    [SerializeField] private AudioClip win;
    [SerializeField] private AudioClip collectible;
    [SerializeField] private AudioClip powerup;



    private Animator playerAnim;
    private bool isInvincible;
    private bool isMagnetActive;
    private bool isScoreDoublerActive;
    private bool hasStrengthPowerUp;
    private float strengthPowerUpTimer;

    private Collider playerCollider;
    [SerializeField] private float shieldDuration;
    [SerializeField] private float magnetRadius;
    [SerializeField] private float magnetDuration;
    [SerializeField] private float scoreDoublerDuration;
    [SerializeField] private float strengthPowerUpDuration;

    private Dictionary<string, bool> powerUpStatuses;

    [SerializeField] private TextMeshProUGUI collectibleCountText;
    private AudioSource audioSource;

    private void Awake()
    {
        playerAnim = GetComponent<Animator>();
        bloodParticles.SetActive(false);
        playerCollider = GetComponent<Collider>();
        InitializePowerUpStatuses();
        audioSource = GetComponent<AudioSource>();
    }

    private void InitializePowerUpStatuses()
    {
        powerUpStatuses = new Dictionary<string, bool>();
        powerUpStatuses.Add("Shield", false);
        powerUpStatuses.Add("Magnet", false);
        powerUpStatuses.Add("Doubler", false);
        powerUpStatuses.Add("Strength", false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "rite")
        {
            ActivatePowerUp("Shield");
            audioSource.PlayOneShot(powerup);
            Destroy(other.gameObject);
            return;
        }

        if (other.tag == "Magnet")
        {
            ActivatePowerUp("Magnet");
            audioSource.PlayOneShot(powerup);
            Destroy(other.gameObject);
            return;
        }

        if (other.tag == "Doubler")
        {
            ActivatePowerUp("Doubler");
            audioSource.PlayOneShot(powerup);
            Destroy(other.gameObject);
            return;
        }

        if (other.tag == "Strength")
        {
            ActivatePowerUp("Strength");
            audioSource.PlayOneShot(powerup);
            Destroy(other.gameObject);
            return;
        }

        if (other.tag == "Size")
        {
            int scoreIncrease = isScoreDoublerActive ? 2 : 1;
            GameEvents.instance.playerSize.Value += scoreIncrease;

            int collectibleCount = GameEvents.instance.playerSize.Value;
            collectibleCountText.text = collectibleCount.ToString();

            other.GetComponent<Collider>().enabled = false;
            other.transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
            {
                Destroy(other.gameObject);
            });
            audioSource.PlayOneShot(collectible);

        }

        if (!isInvincible)
        {
            if (other.tag == "Obstacle")
            {
                playerAnim.SetTrigger("kick");
                other.GetComponent<Block>().CheckHit();

                int collectibleCount = GameEvents.instance.playerSize.Value;
                collectibleCountText.text = collectibleCount.ToString();

                // Play collision sound 1
                audioSource.PlayOneShot(damage);
            }
            else if (other.tag == "Wall")
            {
                if (hasStrengthPowerUp)
                {
                    playerAnim.SetTrigger("kick");
                    other.GetComponent<Block>().CheckHitWithoutSize();
                    audioSource.PlayOneShot(wall);

                }
                else
                {
                    GameEvents.instance.gameLost.SetValueAndForceNotify(true);
                    bloodParticles.SetActive(true);
                    playerCollider.enabled = false;
                    audioSource.PlayOneShot(damage);
                }
            }
            else if (other.tag == "Gate")
            {
                other.GetComponent<Gate>().ExecuteOperation();
                int collectibleCount = GameEvents.instance.playerSize.Value;
                collectibleCountText.text = collectibleCount.ToString();

                // Play collision sound 2
                audioSource.PlayOneShot(gate);
            }
            else if (other.tag == "Saw" || other.tag == "Pendulum")
            {
                GameEvents.instance.gameLost.SetValueAndForceNotify(true);
                bloodParticles.SetActive(true);
                playerCollider.enabled = false;

                // Play collision sound 3
                audioSource.PlayOneShot(damage);
            }
            else if (other.tag == "Finish")
            {
                GameEvents.instance.gameWon.SetValueAndForceNotify(true);
                audioSource.PlayOneShot(win);

            }
        }
    }

    private void ActivatePowerUp(string powerUpName)
    {
        if (powerUpStatuses[powerUpName])
            return;

        powerUpStatuses[powerUpName] = true;

        switch (powerUpName)
        {
            case "Shield":
                ActivateShield();
                break;
            case "Magnet":
                ActivateMagnet();
                break;
            case "Doubler":
                ActivateScoreDoubler();
                break;
            case "Strength":
                ActivateStrengthPowerUp();
                break;
        }
    }

    private void DeactivatePowerUp(string powerUpName)
    {
        powerUpStatuses[powerUpName] = false;
    }

    private void ActivateShield()
    {
        if (isInvincible)
            return;

        isInvincible = true;
        StartCoroutine(ShieldDurationCountdown());
    }

    private IEnumerator ShieldDurationCountdown()
    {
        yield return new WaitForSeconds(shieldDuration);
        DisableShield();
    }

    private void DisableShield()
    {
        isInvincible = false;
        DeactivatePowerUp("Shield");
    }

    private void ActivateMagnet()
    {
        if (isMagnetActive)
            return;

        isMagnetActive = true;
        StartCoroutine(MagnetDurationCountdown());
    }

    private IEnumerator MagnetDurationCountdown()
    {
        yield return new WaitForSeconds(magnetDuration);
        isMagnetActive = false;
        DeactivatePowerUp("Magnet");
    }

    private void ActivateScoreDoubler()
    {
        if (isScoreDoublerActive)
            return;

        isScoreDoublerActive = true;
        StartCoroutine(ScoreDoublerDurationCountdown());
    }

    private IEnumerator ScoreDoublerDurationCountdown()
    {
        yield return new WaitForSeconds(scoreDoublerDuration);
        isScoreDoublerActive = false;
        DeactivatePowerUp("Doubler");
    }

    private void ActivateStrengthPowerUp()
    {
        hasStrengthPowerUp = true;
        StartCoroutine(StrengthPowerUpDurationCountdown());
    }

    private IEnumerator StrengthPowerUpDurationCountdown()
    {
        yield return new WaitForSeconds(strengthPowerUpDuration);
        hasStrengthPowerUp = false;
        DeactivatePowerUp("Strength");
    }

    private void Update()
    {
        if (isMagnetActive)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, magnetRadius);
            foreach (Collider collider in colliders)
            {
                if (collider.tag == "Size")
                {
                    Vector3 direction = transform.position - collider.transform.position;
                    collider.transform.position = Vector3.MoveTowards(collider.transform.position, transform.position, 5f * Time.deltaTime);
                }
            }
        }
    }
}
