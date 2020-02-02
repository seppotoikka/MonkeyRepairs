using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Range(1,2)]
    public int playerNumber;
    Rigidbody rb;
    float turnSpeedEulersPerTick;
    GameObject pickup;
    public Transform pickupPosition;
    Collider pickupOnRadius;
    public GameObject pickupEffectPrefab;

    float moveSpeed = 4;
    bool buttonPressed;
    bool buttonHeld;
    bool buttonUp;
    float throwWindUp = -1;

    readonly string P1H1 = "HorizontalP1";
    readonly string P1V1 = "VerticalP1";
    readonly string P1H2 = "Horizontal2P1";
    readonly string P1V2 = "Vertical2P1";
    readonly string P1B = "Fire1P1";

    readonly string P2H1 = "HorizontalP2";
    readonly string P2V1 = "VerticalP2";
    readonly string P2H2 = "Horizontal2P2";
    readonly string P2V2 = "Vertical2P2";
    readonly string P2B = "Fire1P2";

    string H1
    {
        get
        {
            if (playerNumber == 1)
                return P1H1;
            return P2H1;
        }
    }
    string V1
    {
        get
        {
            if (playerNumber == 1)
                return P1V1;
            return P2V1;
        }
    }
    string H2
    {
        get
        {
            if (playerNumber == 1)
                return P1H2;
            return P2H2;
        }
    }
    string V2
    {
        get
        {
            if (playerNumber == 1)
                return P1V2;
            return P2V2;
        }
    }
    string B
    {
        get
        {
            if (playerNumber == 1)
                return P1B;
            return P2B;
        }
    }

    Vector3 randomWalkInput;

    Vector3 pickupVelocity;

    public MonkeyIK monkeyIK;

    bool stunned;

    public Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        turnSpeedEulersPerTick = 360 * Time.fixedDeltaTime;
        StartCoroutine(RandomWalkInput());
    }

    private void Update()
    {
        if (Input.GetButtonDown(B))
            buttonPressed = true;
        else if (Input.GetButton(B))
            buttonHeld = true;
        if (Input.GetButtonUp(B))
        {
            buttonHeld = false;
            buttonUp = true;
        }
            
    }

    void FixedUpdate()
    {
        //Reset forces
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        Vector3 input = Vector3.zero;
        if (!stunned)
            input = (Vector3.forward * Input.GetAxis(V1) + Vector3.right * Input.GetAxis(H1));
        animator.SetFloat("MoveSpeed", Mathf.Clamp01(input.magnitude));
        float deltaDot = Vector3.Dot(input, randomWalkInput);
        float randomModifier = 1;
        if (deltaDot < 0)
        {
            randomModifier = Mathf.Pow(1 - deltaDot, 2);
        }
        randomModifier = pickup == null ? randomModifier : randomModifier * 2;
        transform.rotation = Quaternion.Euler(randomWalkInput.z * 15 * randomModifier + input.z * 5, 
            rb.rotation.eulerAngles.y, randomWalkInput.x * 15 + input.x * 5);
        if (input.magnitude > 0.05f)
            input +=  randomWalkInput * randomModifier;
        Vector3 balanceInput = (Vector3.forward * Input.GetAxis(V2) + Vector3.right * Input.GetAxis(H2));        
        if (input.magnitude > 1)
            input.Normalize();
        float turnAngle = Vector3.SignedAngle(transform.forward, input, Vector3.up);
        if (Mathf.Abs(turnAngle) > turnSpeedEulersPerTick)
        {
            turnAngle = Mathf.Sign(turnAngle) * turnSpeedEulersPerTick;
        }
        if (Mathf.Abs(turnAngle) > 3f)
            transform.Rotate(Vector3.up * turnAngle);
        float dot = Vector3.Dot(input, transform.forward);
        if (dot > 0)
            rb.MovePosition(rb.position + input * Time.fixedDeltaTime * dot * moveSpeed);

        if (buttonPressed)
        {
            buttonPressed = false;
            if (pickup != null)
            {
                throwWindUp = 0;                
            }           
        }

        if (buttonHeld && throwWindUp > -0.5f)
        {
            throwWindUp += Time.fixedDeltaTime;
        }

        if (buttonUp)
        {
            buttonHeld = false;
            buttonUp = false;
            if (pickup != null)
            {                
                float force = Mathf.Clamp(throwWindUp * 500, 200, 1000);
                ThrowPickup(transform.forward, force + 100 * dot);
                throwWindUp = -1;
                AudioManager.instance.ThrowSound();
            }
            else
            {
                if (pickupOnRadius != null)
                {
                    GameObject pu = Instantiate(pickupEffectPrefab);
                    pu.transform.position = pickupOnRadius.transform.position;
                    GameManager.instance.BoxPickedUp(pickupOnRadius.transform);
                    GameObject pickupPrefab = GameManager.instance.GetPartPrefab(pickupOnRadius.GetComponent<PartsBox>().type);
                    Destroy(pickupOnRadius.gameObject);
                    pickupOnRadius = null;
                    pickupPosition.transform.localRotation = Quaternion.identity;
                    pickup = Instantiate(pickupPrefab);
                    pickup.transform.SetParent(pickupPosition);
                    pickup.transform.localPosition = Vector3.zero;
                    pickupVelocity = Vector3.zero;
                    Pickup puScript = pickup.GetComponent<Pickup>();
                    monkeyIK.leftHandTarget = puScript.leftHandIKTarget;
                    monkeyIK.rightHandTarget = puScript.rightHandIKTarget;
                    monkeyIK.ikActive = true;
                    AudioManager.instance.PickupSound();
                }
            }
        }

        if (pickup != null)
        {
            Vector3 rot = ConvertRot(pickup.transform.rotation.eulerAngles);
            Vector3 angleForce = new Vector3(rot.z, 0, rot.x).normalized * Mathf.Max(rot.x, rot.z) / 30;
            Vector3 carryInput = (Vector3.forward * Input.GetAxis(V2) + Vector3.right * Input.GetAxis(H2)).normalized;
            float inputDot = Vector3.Dot(carryInput, pickup.transform.localPosition);
            inputDot = Mathf.Clamp01(inputDot);
            carryInput *= 2 + inputDot;
            pickupVelocity += (angleForce + carryInput) * Time.fixedDeltaTime;
            //clamp velo
            if (pickupVelocity.magnitude > 0.4f)
                pickupVelocity = pickupVelocity.normalized * 0.4f;
            Vector3 moveDelta = pickupVelocity * Time.fixedDeltaTime;
            pickup.transform.position += moveDelta;
            Vector3 locPos = pickup.transform.localPosition;
            locPos.y = 0;
            pickup.transform.localPosition = locPos;
            pickup.transform.localRotation = Quaternion.identity;
            if (Mathf.Abs(locPos.x) > 0.9f || Mathf.Abs(locPos.z) > 0.9f)
            {
                ThrowPickup(pickup.transform.localPosition * -1, 100);
                StartCoroutine(Stun());
                animator.SetTrigger("Fall");
                StartCoroutine(FallSound());
            }
        }
    }

    IEnumerator Stun()
    {
        stunned = true;
        yield return new WaitForSeconds(1.5f);
        stunned = false;
    }

    Vector3 ConvertRot(Vector3 rot)
    {
        rot.x = rot.x % 360;
        rot.z = rot.z % 360;
        if (Mathf.Abs(rot.x) > 180)
            rot.x -= 360 * Mathf.Sign(rot.x);
        if (Mathf.Abs(rot.z) > 180)
            rot.z -= 360 * Mathf.Sign(rot.z);
        return rot;
    }

    IEnumerator FallSound()
    {
        yield return new WaitForSeconds(0.5f);
        AudioManager.instance.FallSound();
    }

    void ThrowPickup(Vector3 direction, float force)
    {
        pickup.transform.SetParent(null);
        Rigidbody pickupRb = pickup.GetComponent<Rigidbody>();
        pickupRb.isKinematic = false;
        pickupRb.useGravity = true;
        pickupRb.AddForce(direction * force + transform.up * (100 + force / 5));
        pickup = null;
        monkeyIK.ikActive = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        pickupOnRadius = other;
        other.GetComponent<PartsBox>().Show();
    }

    private void OnTriggerExit(Collider other)
    {
        pickupOnRadius = null;
        other.GetComponent<PartsBox>().Hide();
    }

    IEnumerator RandomWalkInput()
    {
        float maxValue = 0.15f;
        float maxTime = 1.5f;
        float minTime = 0.75f;
        Vector3 oldRandom = Vector3.zero;
        while (true)
        {
            float effectDuration = Random.Range(minTime, maxTime);
            Vector3 newRandom = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            newRandom = newRandom.normalized * maxValue;
            float timer = 0;
            while (timer < effectDuration)
            {
                timer += Time.deltaTime;
                randomWalkInput = Vector3.Lerp(oldRandom, newRandom, timer / effectDuration);
                yield return null;
            }
            oldRandom = newRandom;
        }
    }
}
