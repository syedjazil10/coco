using UnityEngine;

namespace PathCreation.Examples
{
    public class PathFollower : MonoBehaviour
    {
        [SerializeField] private float startSpeed = 3f;
        [SerializeField] private float maxSpeed = 10f;
        [SerializeField] private float speedIncreaseInterval = 5f;

        public float pathProgress { get; private set; }
        public PathCreator pathCreator;
        public EndOfPathInstruction endOfPathInstruction;

        private float currentSpeed;
        private float distanceTravelled;
        private float timeSinceLastSpeedIncrease;

        private void Start()
        {
            if (pathCreator != null)
                pathCreator.pathUpdated += OnPathChanged;

            currentSpeed = startSpeed;
        }

        private void Update()
        {
            if (pathCreator != null && GameEvents.instance.gameStarted.Value
                && !GameEvents.instance.gameWon.Value && !GameEvents.instance.gameLost.Value)
            {
                distanceTravelled += currentSpeed * Time.deltaTime;
                transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
                transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
                pathProgress = pathCreator.path.GetPercentage(distanceTravelled);

                timeSinceLastSpeedIncrease += Time.deltaTime;
                if (timeSinceLastSpeedIncrease >= speedIncreaseInterval)
                {
                    timeSinceLastSpeedIncrease = 0f;
                    IncreaseSpeed();
                }
            }
        }

        private void OnPathChanged()
        {
            distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
        }

        private void IncreaseSpeed()
        {
            currentSpeed = Mathf.Min(currentSpeed + 0.5f, maxSpeed);
        }
    }
}
