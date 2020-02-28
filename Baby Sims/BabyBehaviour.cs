using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using KobGamesSDKSlim;
using System.Linq;
using DG.Tweening;

public class BabyBehaviour : MonoBehaviour
{

	[Range(0f,1f)]
	public float Behaved = 1;

	public enum eBabyState
	{
		Default,
		Sleep,
		Turn,
		Play,
		Cry
	}

	public bool IsInteractable = true;
	public bool IsCommanded = false;
	public bool CanMove = true;

	public eBabyState State;

	[SerializeField] [Header("Desires")]
	public List<DesiredObject.eDesiredObjectType> DesireList = new List<DesiredObject.eDesiredObjectType>();
	public Bed.eBedType DesiredBedType;
	public int DesireAmount;

	[Header("References")]
	//public Material MaterialX;
	public Rigidbody RigidbodyY;
	public Transform TranformZ;
	public Transform DirectionalArrow;


	public Transform DesireBubble;
	private int m_DesireIndex;

	//public float DetectionDistance = 0.5f;
	//public float DetectRadius = 0.5f;

	public BabyAnimationHandler BabyAnimation;

	private Vector3 m_Direction;
	private Vector3 m_BedRotation = Vector3.zero; //new Vector3(270, 180, 0);
	private Vector3 m_StartPosition;
	private Vector3 m_StartRotation;

	private Quaternion m_TargetRotation;
	private Collider m_DummyBedCollider;

	[SerializeField]
	private bool m_DetectedWall;
	private bool m_LevelStarted = false;

	private DesiredObject m_DesiredObject;
	private Bed m_Bed;

	private RaycastHit m_DummyRaycastHit, m_DummyRaycastHitTwo;

	// Using this to avoid double entry ? Should this be necessary?
	private List<DesiredObject.eDesiredObjectType> m_DummyDesireObjects = new List<DesiredObject.eDesiredObjectType>();

	private BabyVariablesEditor m_BabyVars { get { return GameConfig.Instance.Baby; } }


	#region Unity Initialization
	//private void Awake() { }
	private void Start() {
		m_StartPosition = transform.position;
		m_StartRotation = transform.eulerAngles;
	}
	private void OnEnable() {
		GameManager.OnGameReset += OnReset;
		//GameManager.OnLevelLoaded += OnLevelLoaded;
		GameManager.OnLevelStarted += OnLevelStarted;
	}
	private void OnDisable() {
		GameManager.OnGameReset -= OnReset;
		//GameManager.OnLevelLoaded -= OnLevelLoaded;
		GameManager.OnLevelStarted -= OnLevelStarted;
	}
	#endregion

	#region Events
	public void OnReset() {
		StopAllCoroutines();
		KillAllMovement();
		disableDesireBubble();
		HideDirectionIndicator();

		m_DetectedWall = false;
		CanMove = false;
		IsInteractable = true;
		m_LevelStarted = false;
		IsCommanded = false;

		DOTween.Kill(this);

		//DOTween.Kill(DOTweenIDs.Rotate);
		//DOTween.Kill(DOTweenIDs.Opposite);

		State = eBabyState.Default;

		DesireBubble.gameObject.SetActive(false);
		DesireList.Clear();

		transform.eulerAngles = Vector3.zero;
		transform.position = m_StartPosition;
		transform.eulerAngles = m_StartRotation;

		if (m_DummyBedCollider != null) {
			m_DummyBedCollider.enabled = true;
			m_DummyBedCollider = null;
		}
	}

	//public void OnLevelLoaded() {
	//	//SetDesireList();
	//}

	public void OnLevelStarted() {
		m_LevelStarted = true;
		SetTimeToMove();
		StartCoroutine("ShowDesireBubble");
	}
	#endregion

	#region Unity Loop
	private void FixedUpdate() {
		if (!m_LevelStarted) return;

		if (CanMove) {
			RigidbodyY.velocity = transform.forward * m_BabyVars.MovementSpeed;
		} else {
			RigidbodyY.velocity = Vector3.zero; 
		}
	}
	#endregion

	#region Physics
	[SerializeField]
	float m_TriggerTime;
	float m_TriggerThreshold = 2f;
	private void OnTriggerStay(Collider i_Collider) {
		if (!m_LevelStarted) return;
		if (State == eBabyState.Default) {

			switch (i_Collider.tag) {
				case "Bed":
				case "DesireObject":
				case "Wall":

					//if (Physics.Raycast(transform.position, transform.forward * m_BabyVars.RayDetectionDistance, out m_DummyRaycastHit)) {
					//switch (m_DummyRaycastHit.collider.transform.tag) {
					//case "Bed":
					//case "DesireObject":
					//case "Wall":
					m_TriggerTime += Time.deltaTime;
					if (!IsCommanded) {
						CanMove = false;
						BabyAnimation.Idle();
					} else if (IsCommanded) {
						CanMove = true;
						BabyAnimation.Crawl();
					} else if (CanMove) {
						BabyAnimation.Crawl();
					}

					if (m_TriggerTime >= m_BabyVars.TriggerStayThreshold) {
						m_TriggerTime = 0;
						m_DetectedWall = true;

						DOTween.Kill(this);
						MoveOpposite();
						//}
						//break;
						//}

					}
					break;
			}
		}
	}

	private void OnTriggerExit(Collider i_Collider) {
		if (!m_LevelStarted) return;

		switch (i_Collider.tag) {
			case "Bed":
			case "DesireObject":
			case "Wall":
				m_TriggerTime = 0;
				break;
		}
	}
	private void OnTriggerEnter(Collider i_Collider) {
		if (!m_LevelStarted) return;

		DOTween.Kill(this);
		//DOTween.Kill(DOTweenIDs.Opposite);

		switch (i_Collider.tag) {
			case "DesireObject":
				m_DesiredObject = i_Collider.GetComponent<DesiredObject>();
				if (m_DesiredObject != null) {
					if (DesireList[0] == m_DesiredObject.Type && IsCommanded) {
						setNextDesire();
						babyPlay(BabyAnimationHandler.eSitType.Happy);
						BabyManager.Instance.AddProgression();
					} else {
						if (!m_DetectedWall) {
							m_DetectedWall = true;
							if (State != eBabyState.Sleep || State != eBabyState.Play || State != eBabyState.Cry) {
								waitToMove(m_BabyVars.BabyWallCryDuration, false);
							}
						}
					}
				}
				break;

			case "Bed":
				if (State != eBabyState.Sleep) {
					if (DesireList[0] == DesiredObject.eDesiredObjectType.Sleep) {
						m_Bed = i_Collider.gameObject.GetComponent<Bed>();

						if (m_Bed.Type == DesiredBedType && !m_Bed.IsOccupied && IsCommanded) { // Enter if Bed matches Desired Bed Type
							m_DummyBedCollider = i_Collider;
							snapToBed(m_Bed.transform.GetChild(0).position);
							BabyManager.Instance.AddProgression();
							
						}
						else {
							if (!m_DetectedWall) {
								m_DetectedWall = true;
								if (State != eBabyState.Sleep || State != eBabyState.Play || State != eBabyState.Cry) {
									waitToMove(m_BabyVars.BabyWallCryDuration, false);
								}
							}
						}
					}
				}
				break;

			case "Wall":
				if (!m_DetectedWall) {
					m_DetectedWall = true;
					if (State != eBabyState.Sleep || State != eBabyState.Play || State != eBabyState.Cry) {
						waitToMove(m_BabyVars.BabyWallCryDuration, true);
					}
				}
				break;
		}
	}

	private void OnCollisionEnter(Collision i_Collision) {
		if (!m_LevelStarted) return;

		switch (i_Collision.transform.tag) {
			case "Baby":
				if (!IsCommanded && !m_DetectedWall) {
					if (Vector3.Dot(transform.position, i_Collision.transform.position) > 0) {
						DOTween.Kill(this);
						m_DetectedWall = true;
						//babyCry();
						waitToMove(m_BabyVars.BabyWallCryDuration, true);
					}
				}
				break;
		}
	}
	#endregion

	private void waitToMove(float time,bool cry) {
		if (State == eBabyState.Sleep) return;

		if (cry) {
			babyCry();
		} else {
			BabyAnimation.Idle();
			//StopMovement();
		}

		DOVirtual.DelayedCall(time, MoveOpposite).SetId(DOTweenIDs.Opposite);

	}

	public void MoveOpposite() {
		if (IsCommanded || State == eBabyState.Sleep) { DOTween.Kill(this); return; }

		int i_Rand = Random.Range(0, 3);
		switch (i_Rand) {
			case 0:
				SetChangeDirection(transform.position - transform.forward);
				break;
			case 1:
				SetChangeDirection(transform.position - transform.right);
				break;
			case 2:
				SetChangeDirection(transform.position + transform.right);
				break;
		}
		//if(i_Rand >= 2) {
		//	SetChangeDirection(transform.position - transform.right);
		//} else
		//	SetChangeDirection(transform.position - transform.forward);
	}

	public void SetTimeToMove() {
		DOVirtual.DelayedCall(Random.Range(1, m_BabyVars.TimeUntilMoveAfterStart), () => { BabyAnimation.Crawl(); BeginMovement(); });
	}

	public void SetChangeDirection(Vector3 i_Dir) {
		if (State == eBabyState.Sleep) return;


		Vector3 i_BabyPos = transform.position;
		i_BabyPos.y = 0;
		i_Dir.y = 0;
		m_Direction = i_Dir - i_BabyPos;

		m_TargetRotation = Quaternion.LookRotation(m_Direction);

		DOTween.Kill(this);

		if (State == eBabyState.Sleep || State == eBabyState.Play) {
			return;
		} else {
			changeRotationToWalk();
		}

	}

	private void changeRotationToWalk() {
		if (State == eBabyState.Sleep) return;
		float i_RotationTimer = 0;
		float i_Progress = 0;
		bool i_ThresholdMet = false;

		if (IsCommanded) {
			ShowDirectionIndicator(); // Visual Indicator
		}

		StopMovement();

		BabyAnimation.Crawl();
		//BabyAnimation.Idle();
		State = eBabyState.Turn;

		transform.DORotate(m_TargetRotation.eulerAngles, m_BabyVars.RotationDuration).
			SetId(DOTweenIDs.Rotate).
			OnUpdate(() => {
				if(State == eBabyState.Sleep) { snapToBed(m_Bed.transform.GetChild(0).position);  DOTween.Kill(this); }
				if (!i_ThresholdMet) {
					
					DirectionalArrow.rotation = m_TargetRotation;
					i_RotationTimer += Time.deltaTime;
					i_Progress = i_RotationTimer / m_BabyVars.RotationDuration;
					if (i_Progress >= m_BabyVars.RotationThreshold) {
						if (State == eBabyState.Play) {
							HideDirectionIndicator();
							i_ThresholdMet = true;
						}
						else {
							HideDirectionIndicator();
							BeginMovement();
							BabyAnimation.Crawl();
							i_ThresholdMet = true;
						}
					}
				}
			}).
			OnComplete(() => {
				//IsCommanded = false;

				m_DetectedWall = false;
				if(State != eBabyState.Sleep) {
					BabyAnimation.Crawl();
					BeginMovement();
					m_DetectedWall = false;
					State = eBabyState.Default;
				}
				
			});

	}

	public void HideDirectionIndicator() {
		DirectionalArrow.gameObject.SetActive(false);
	}

	public void ShowDirectionIndicator() {
		DirectionalArrow.gameObject.SetActive(true);
	}

	public void SetDesiredBedType(int index) {
		//DesiredBedType = (Bed.eBedType)Random.Range(0, System.Enum.GetValues(typeof(Bed.eBedType)).Length);
		DesiredBedType = (Bed.eBedType)index;
	}

	public void SetDesireAmount(int i_Amount) {
		DesireAmount = i_Amount;
	}

	public void SetDesireList() {
		int i_Rand = Random.Range(0, m_BabyVars.MaxDesireListSize + 1); // We include +1 because Random does not include passed in number.

		m_DummyDesireObjects = System.Enum.GetValues(typeof(DesiredObject.eDesiredObjectType)).Cast<DesiredObject.eDesiredObjectType>().ToList();
		DesiredObject.eDesiredObjectType m_DesiredObject;

		//SetDesireAmount(i_Rand);

		for (int i = 0; i < DesireAmount; i++) {
			i_Rand = Random.Range(0, m_DummyDesireObjects.Count - 1);

			m_DesiredObject = m_DummyDesireObjects[i_Rand];

			m_DummyDesireObjects.Remove(m_DesiredObject);

			DesireList.Add(m_DesiredObject);
		}

		DesireList.Add(DesiredObject.eDesiredObjectType.Sleep); // We always want Sleep to be the last on the list.

		BabyManager.Instance.DesireAmount += DesireList.Count;
	}

	public void StopMovement() {
		RigidbodyY.isKinematic = true;
		CanMove = false;
	}

	public void BeginMovement() {
		CanMove = true;
		RigidbodyY.isKinematic = false;
	}

	public void Commanded() {
		IsCommanded = true;
		m_TriggerTime = 0;
		StopCoroutine("CommandTimer");
		StartCoroutine("CommandTimer");
		//DOVirtual.DelayedCall(m_BabyVars.CommandDuration, () => { IsCommanded = false; }).SetId(DOTweenIDs.Command);
	}

	private IEnumerator CommandTimer() {
		float i_Time = 0;
		while(i_Time < m_BabyVars.CommandDuration) {
			i_Time += Time.deltaTime;
			yield return null;
		}

		if(i_Time>= m_BabyVars.CommandDuration) {
			IsCommanded = false;
		}
		yield return null;
	}


	public void KillAllMovement() {

		CanMove = false;
		m_DetectedWall = false;
		RigidbodyY.isKinematic = true;
		DOTween.Kill(this);
		//State = eBabyState.Default;
	}

	private void snapToBed(Vector3 pos) {

		StopCoroutine("ShowDesireBubble");
		State = eBabyState.Sleep;
		KillAllMovement();

		DOTween.Kill(this);

		transform.position = pos;
		transform.eulerAngles = m_BedRotation;
		

		//m_DummyBedCollider.enabled = false;
		HideDirectionIndicator(); // In case the baby has already reached the bed while trying to change it's direction.

		BabyAnimation.Sleep();
		m_Bed.ChangeComplete();	
		DesireBubble.gameObject.SetActive(false);
	}

	private void interactWith(DesiredObject.eDesiredObjectType i_Type) {
		BabyManager.Instance.AddProgression();
		switch (i_Type) {
			case DesiredObject.eDesiredObjectType.Sleep:
				snapToBed(m_Bed.transform.GetChild(0).position);
				break;
			default:

				break;
		}

	}

	private void disableDesireBubble() {
		DesireBubble.gameObject.SetActive(false);

		for (int i = 0; i < DesireBubble.transform.childCount; i++) {
			DesireBubble.GetChild(i).gameObject.SetActive(false);
		}
	}

	private void enableDesireBubble(bool i_Happy = false) {
		m_DesireIndex = (int)System.Enum.Parse(typeof(DesiredObject.eDesiredObjectType), DesireList[0].ToString());
		DesireBubble.gameObject.SetActive(true);
		if (!i_Happy) {
			DesireBubble.GetChild(DesireBubble.transform.childCount - 1).gameObject.SetActive(false);
			DesireBubble.GetChild(m_DesireIndex).gameObject.SetActive(true);
		}
		else {
			DesireBubble.GetChild(m_DesireIndex).gameObject.SetActive(false);
			DesireBubble.GetChild(DesireBubble.transform.childCount - 1).gameObject.SetActive(true);
		}

	}

	private IEnumerator ShowDesireBubble() {
		int i_Variation;
		float i_RandTime;

		while(DesireList.Count > 0) {
			i_Variation = Random.Range(0, 100);
			i_RandTime = Random.Range(0, m_BabyVars.MinDesireBubbleInterval);

			// this is so bubble does not show up when baby is already playing.
			if (State == eBabyState.Play)
				yield return new WaitUntil(() => State != eBabyState.Play);

			yield return new WaitForSeconds(i_RandTime);

			if (i_Variation < m_BabyVars.DesireBubbleVariation) {

				if (IsInteractable) {
					enableDesireBubble();
				}
				i_RandTime = Random.Range(m_BabyVars.MinDesireBubbleInterval, m_BabyVars.MaxDesireBubbleInterval);

				yield return new WaitForSeconds(i_RandTime);
				disableDesireBubble();
			}
		}

		yield return null;
	}

	private void babyPlay(BabyAnimationHandler.eSitType type = 0) {
		StopMovement();
		State = eBabyState.Play;
		IsInteractable = false;
		CanMove = false;
		switch (type) {
			case BabyAnimationHandler.eSitType.Happy:
				BabyAnimation.CrawlToSit(BabyAnimationHandler.eSitType.Happy);
				break;
			default:
				BabyAnimation.CrawlToSit(BabyAnimationHandler.eSitType.Default);
				break;
		}

		SetChangeDirection(m_DesiredObject.transform.position);

		enableDesireBubble(true);
		transform.DORotate(m_TargetRotation.eulerAngles, m_BabyVars.RotationDuration).OnUpdate( () => { CanMove = false; }).
			OnComplete(() => {
				DOVirtual.DelayedCall(m_BabyVars.PlayTimeDuration, () => {
					State = eBabyState.Default;
					enableDesireBubble();
					IsInteractable = true;
					CanMove = true;
				});
			});
	}

	private void babyCry() {
		StopMovement();
		HideDirectionIndicator();
		State = eBabyState.Cry;
		BabyAnimation.Cry();
	}

	private void setNextDesire() {
		disableDesireBubble();

		if (DesireList.Count > 0) {
			DesireList.RemoveAt(0);
		}

	}

}
