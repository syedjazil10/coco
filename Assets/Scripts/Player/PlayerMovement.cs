using UniRx;
using UnityEngine;
using UniRx.Triggers;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    private CompositeDisposable subscriptions = new CompositeDisposable();

    [SerializeField] private float limitX;
    [SerializeField] private float sidewaySpeed;
    [SerializeField] private Transform playerModel;

    private bool lockControls;
    private Vector3 targetPosition;

    private void OnEnable()
    {
        StartCoroutine(Subscribe());
    }

    private IEnumerator Subscribe()
    {
        yield return new WaitUntil(() => GameEvents.instance != null);
        this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButton(0))
            .Subscribe(x =>
            {
                if (GameEvents.instance.gameStarted.Value && !GameEvents.instance.gameLost.Value
                && !GameEvents.instance.gameWon.Value)
                {
                    MovePlayer();
                }
            })
            .AddTo(subscriptions);

        GameEvents.instance.gameWon.ObserveEveryValueChanged(x => x.Value)
            .Subscribe(value =>
            {
                if (value)
                    lockControls = true;
            })
            .AddTo(subscriptions);

        GameEvents.instance.gameLost.ObserveEveryValueChanged(x => x.Value)
            .Subscribe(value =>
            {
                if (value)
                    lockControls = true;
            })
            .AddTo(subscriptions);
    }

    private void OnDisable()
    {
        subscriptions.Clear();
    }

    private void MovePlayer()
    {
        if (Input.GetMouseButton(0))
        {
            float percentageX = (Input.mousePosition.x - Screen.width / 2) / (Screen.width * 0.5f) * 2;
            percentageX = Mathf.Clamp(percentageX, -1.0f, 1.0f);
            float targetPosX = percentageX * limitX;
            targetPosition = new Vector3(0, targetPosX, 0);
        }

        playerModel.localPosition = Vector3.Lerp(playerModel.localPosition, targetPosition, Time.deltaTime * sidewaySpeed);
        playerModel.localPosition = new Vector3(0, playerModel.localPosition.y, 0);
    }
}
