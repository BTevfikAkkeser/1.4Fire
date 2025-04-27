using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class DriftController : MonoBehaviour
{
    internal enum CarDriveType
    {
        RearWheelDrive,
        FourWheelDrive
    }

    internal enum SpeedType
    {
        MPH,
        KPH
    }
    internal enum SelectPreset
    {
        Custom,
        Drift,
        Stock,
        Racing
    }
    public enum DriftType
    {
        Automatic,
        Assisted_Manual,
        Manual_NoAssits
    }
    [SerializeField] private SelectPreset m_Preset = SelectPreset.Custom;
    [SerializeField] private CarDriveType m_CarDriveType = CarDriveType.FourWheelDrive;
    public DriftType m_DriftType = DriftType.Assisted_Manual;
    [SerializeField] private SpeedType m_SpeedType;
    [SerializeField] private float m_MaxTorque;
    [SerializeField] private float m_ReverseTorque;
    public float m_MaximumSteerAngle;
    public float InputTurnSensitivity = 1;
    public float driftSensitivity;
    [Range(1f, 20f)]
    [Tooltip("Cutoff Speed for drifting. Smaller values represent lesser speed for the vehicle to start drifting")]
    public float SpeedThreshold;
    [Range(1f, 20f)]
    [Tooltip("Transition from normal steering to drift. Smaller values represent quicker transition time")]
    public float TractionThreshold;
    public AnimationCurve FrictionCurve;
    [Range(0, 1)] [SerializeField] private float m_SteerHelper; // 0 is raw physics , 1 the car will grip in the direction it is facing
    [Range(0, 1)] [SerializeField] private float m_TractionControl; // 0 is no traction control, 1 is full interference
    [SerializeField] private float m_MaxHandbrakeTorque;
    [SerializeField] private float m_Downforce = 100f;
    [SerializeField] private float m_Topspeed = 200;
    public int NoOfGears = 5;
    [SerializeField] private int m_GearNum;

    private float m_RevRangeBoundary = 1f;
    [SerializeField] private float m_SlipLimit;
    [SerializeField] private float m_BrakeTorque;

    private Quaternion[] m_WheelMeshLocalRotations;
    private float m_SteerAngle;
    public float m_GearFactor;
    private float m_OldRotation;
    private float m_CurrentTorque;
    private Rigidbody m_Rigidbody;

    public float horizontalInput;
    public float verticalInput;
    public bool isBreaking;
    [SerializeField]
    [Tooltip("Do not modify. Read Only. The turning value for the front wheels")]
    private float steerControl;
    private float turnAngleY;
    float n, prev, diff, curr, difftot;
    int last_gear;
    private float m_RearSteerAngle;
    [Tooltip("Vehicle's Drift angle. Higher values correspond to more aggressive turning angle")]
    public float DriftAttackAngle;
    [Range(0.0f, 1f)]
    [Tooltip("Experimental. Set to 0 for optimal performance. Weight of direction given to vehicle during initialtion of drift. Higher value corresponds to the duration of full grip when turning before losing grip and counter steering. Value - 0 is for directly counter steering and 1 is for full grip")]
    public float CounterSteerOffset;
    [SerializeField] private Vector3 ComOffset;
    public WheelCollider[] m_WheelColliders = new WheelCollider[4];
    public GameObject[] m_WheelMeshes = new GameObject[4];
    [HideInInspector]
    public bool backfire = false;
    [HideInInspector]
    public float speedoffset = 0;
    [HideInInspector]
    public float frictionVal = 1;
    [HideInInspector]
    public bool auto = false;

    public float BrakeInput { get; private set; }
    public float CurrentSpeed { get { return m_Rigidbody.velocity.magnitude * 2.23693629f; } }
    public float MaxSpeed { get { return m_Topspeed; } }
    public float Revs { get; private set; }
    public float AccelInput { get; private set; }

    // Use this for initialization
    private void Start()
    {
        switch (m_Preset)
        {
            case SelectPreset.Racing:
                SpeedThreshold = MaxSpeed;
                break;
            case SelectPreset.Stock:
                SpeedThreshold = 11;
                DriftAttackAngle = 35;
                break;
            case SelectPreset.Drift:
                SpeedThreshold = 5;
                DriftAttackAngle = 50;
                break;
        }

        m_WheelMeshLocalRotations = new Quaternion[4];
        for (int i = 0; i < 4; i++)
        {
            m_WheelMeshLocalRotations[i] = m_WheelMeshes[i].transform.localRotation;
        }
        m_WheelColliders[0].attachedRigidbody.centerOfMass = ComOffset;

        m_MaxHandbrakeTorque = float.MaxValue;

        m_Rigidbody = GetComponent<Rigidbody>();
        m_CurrentTorque = m_MaxTorque - (m_TractionControl * m_MaxTorque);
    }

    private void FixedUpdate()
    {
        // Klavye inputları
        horizontalInput = Input.GetAxis("Horizontal") * InputTurnSensitivity;
        verticalInput = Input.GetAxis("Vertical");
        isBreaking = Input.GetKey(KeyCode.Space);
        float handbrake = Convert.ToSingle(isBreaking);

        // Drift kontrolü için gerekli hesaplamalar
        var cturnAngleY = transform.eulerAngles.y;
        diff = Mathf.DeltaAngle(cturnAngleY, prev);
        turnAngleY -= diff;
        prev = cturnAngleY;

        // Drift için steer kontrolü
        steerControl -= (Time.deltaTime * horizontalInput * driftSensitivity);
        steerControl = Mathf.Clamp(steerControl, -turnAngleY - 60f, -turnAngleY + 60f);
        
        // Araç hareketi
        Move(horizontalInput, verticalInput, verticalInput, handbrake);
    }

    private void GearChanging()
    {
        float f = Mathf.Abs(CurrentSpeed / MaxSpeed);
        float upgearlimit = (1 / (float)NoOfGears) * (m_GearNum + 1);
        float downgearlimit = (1 / (float)NoOfGears) * m_GearNum;

        if (m_GearNum > 0 && f < downgearlimit)
        {
            StartCoroutine(BackFire());

            m_GearNum--;

        }

        if (f > upgearlimit && (m_GearNum < (NoOfGears - 1)))
        {

            m_GearNum++;
            if (last_gear != m_GearNum)
            {
                StartCoroutine(BackFire());
                StartCoroutine(MotorDisengage());
                last_gear = m_GearNum;
            }

        }
    }
    float prevTorque;
    IEnumerator BackFire()
    {
        backfire = true;
        yield return new WaitForSeconds(0.1f);
        backfire = false;
        yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
        backfire = true;
        yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
        backfire = false;

    }
    IEnumerator MotorDisengage()
    {
        yield return new WaitForSeconds(0.1f);
        prevTorque = m_CurrentTorque;
        m_CurrentTorque = 0;
        yield return new WaitForSeconds(0.3f);
        m_CurrentTorque = prevTorque;
    }


    // simple function to add a curved bias towards 1 for a value in the 0-1 range
    private static float CurveFactor(float factor)
    {
        return 1 - (1 - factor) * (1 - factor);
    }


    // unclamped version of Lerp, to allow value to exceed the from-to range
    private static float ULerp(float from, float to, float value)
    {
        return (1.0f - value) * from + value * to;
    }


    private void CalculateGearFactor()
    {
        float f = (1 / (float)NoOfGears);
        // gear factor is a normalised representation of the current speed within the current gear's range of speeds.
        // We smooth towards the 'target' gear factor, so that revs don't instantly snap up or down when changing gear.
        var targetGearFactor = Mathf.InverseLerp(f * m_GearNum, f * (m_GearNum + 1), Mathf.Abs(CurrentSpeed / MaxSpeed));
        m_GearFactor = Mathf.Lerp(m_GearFactor, targetGearFactor, Time.deltaTime * 5f);

    }

    // Add this method to trigger handbrake functionality
    public void ActivateHandbrake()
    {
        // Set handbrake to true
        isBreaking = true;

        // Start a coroutine to release the handbrake after a delay
        StartCoroutine(ReleaseHandbrakeAfterDelay(3.0f));
    }

    // Coroutine to release the handbrake
    private IEnumerator ReleaseHandbrakeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Release the handbrake
        isBreaking = false;
    }
    private void CalculateRevs()
    {
        // calculate engine revs (for display / sound)
        // (this is done in retrospect - revs are not used in force/power calculations)
        CalculateGearFactor();
        var gearNumFactor = m_GearNum / (float)NoOfGears;
        var revsRangeMin = ULerp(0f, m_RevRangeBoundary, CurveFactor(gearNumFactor));
        var revsRangeMax = ULerp(m_RevRangeBoundary, 1f, gearNumFactor);
        Revs = ULerp(revsRangeMin, revsRangeMax, m_GearFactor);
    }


    public void Move(float steering, float accel, float footbrake, float handbrake)
    {
        for (int i = 0; i < 4; i++)
        {
            Quaternion quat;
            Vector3 position;
            m_WheelColliders[i].GetWorldPose(out position, out quat);
            m_WheelMeshes[i].transform.position = position;
            m_WheelMeshes[i].transform.localRotation = quat;

        }

        //clamp input values
        steering = Mathf.Clamp(steering, -1, 1);
        AccelInput = accel = Mathf.Clamp(accel, 0, 1);
        BrakeInput = footbrake = -1 * Mathf.Clamp(footbrake, -1, 0);
        handbrake = Mathf.Clamp(handbrake, 0, 1);

        //Drift Physics Controls
        float speed = m_Rigidbody.velocity.magnitude;
        m_SteerAngle = steering * m_MaximumSteerAngle;
        m_RearSteerAngle = steering * DriftAttackAngle;

        var cturnAngleY = transform.eulerAngles.y; //takes the euler angles of the vehicle
        diff = Mathf.DeltaAngle(cturnAngleY, prev); //Calculates the delta angle (difference) every frame (0.00034 or -0.00054)
        turnAngleY -= diff; //Adds/subtracts that value and turns it into a linear value
        prev = cturnAngleY; //the current frame data (now old) goes into previous variable

        steerControl = Mathf.Clamp(steerControl, -turnAngleY - 60f, -turnAngleY + 60f);

        switch (m_DriftType)
        {
            case DriftType.Automatic:
            auto = true;
                speedoffset = speed - SpeedThreshold;
                if (speed < SpeedThreshold)
                    steerControl = -turnAngleY;
                break;
            case DriftType.Assisted_Manual:
                if (isBreaking && speed > SpeedThreshold)
                {
                    speedoffset += Time.deltaTime * 3;
                    if(speedoffset>TractionThreshold/2)
                    m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x * 0.993f, m_Rigidbody.velocity.y, m_Rigidbody.velocity.z * 0.993f);
                }
                if (speedoffset > TractionThreshold)
                    speedoffset = TractionThreshold;
                if (speedoffset > 0 && (steering != Mathf.Clamp(steering,-0.1f,0.1f)) && speed > SpeedThreshold)
                    speedoffset += Time.deltaTime * 0.2f;
                else if (speedoffset > 0 && !isBreaking)
                {
                    speedoffset -= Time.deltaTime * 2f;
                }
                if (speedoffset < 0)
                {
                    speedoffset = 0;
                }
                if (!isBreaking && speedoffset == 0)
                    steerControl = -turnAngleY;
                if (speed < SpeedThreshold && isBreaking)
                    m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x * 0.99f, m_Rigidbody.velocity.y, m_Rigidbody.velocity.z * 0.99f);



                break;
            case DriftType.Manual_NoAssits:
                SpeedThreshold = 100;
                if (isBreaking)
                frictionVal-=Time.deltaTime*3;
                else
                frictionVal+=Time.deltaTime;
                frictionVal = Mathf.Clamp(frictionVal,0.3f,1);


                    WheelFrictionCurve wfc;
                    wfc = m_WheelColliders[2].sidewaysFriction;
                    wfc.extremumValue = frictionVal;
                    m_WheelColliders[2].sidewaysFriction = wfc;
                    m_WheelColliders[3].sidewaysFriction = wfc;



                break;


        }





        if (CounterSteerOffset != 0)
        {
            m_WheelColliders[0].steerAngle = Mathf.Lerp(m_SteerAngle, -turnAngleY - steerControl, (speedoffset / TractionThreshold) * (Mathf.Clamp(Mathf.Abs(steering), CounterSteerOffset, 1) - CounterSteerOffset)); //Normal - m_SteerAngle
            m_WheelColliders[1].steerAngle = Mathf.Lerp(m_SteerAngle, -turnAngleY - steerControl, (speedoffset / TractionThreshold) * (Mathf.Clamp(Mathf.Abs(steering), CounterSteerOffset, 1) - CounterSteerOffset)); //Normal - m_SteerAngle
            m_WheelColliders[2].steerAngle = Mathf.Lerp(0, -m_RearSteerAngle, (speedoffset / TractionThreshold) * (Mathf.Clamp(Mathf.Abs(steering), CounterSteerOffset, 1) - CounterSteerOffset));
            m_WheelColliders[3].steerAngle = Mathf.Lerp(0, -m_RearSteerAngle, (speedoffset / TractionThreshold) * (Mathf.Clamp(Mathf.Abs(steering), CounterSteerOffset, 1) - CounterSteerOffset));
        }
        else
        {
            m_WheelColliders[0].steerAngle = Mathf.Lerp(m_SteerAngle, -turnAngleY - steerControl, (speedoffset / TractionThreshold)); //Normal - m_SteerAngle
            m_WheelColliders[1].steerAngle = Mathf.Lerp(m_SteerAngle, -turnAngleY - steerControl, (speedoffset / TractionThreshold)); //Normal - m_SteerAngle
            m_WheelColliders[2].steerAngle = Mathf.Lerp(0, -m_RearSteerAngle, (speedoffset / TractionThreshold));
            m_WheelColliders[3].steerAngle = Mathf.Lerp(0, -m_RearSteerAngle, (speedoffset / TractionThreshold));
        }

        WheelFrictionCurve fc;
        fc = m_WheelColliders[2].sidewaysFriction;
        fc.extremumSlip = 1 - FrictionCurve.Evaluate(speed / SpeedThreshold);
        m_WheelColliders[2].sidewaysFriction = fc;
        m_WheelColliders[3].sidewaysFriction = fc;


        SteerHelper();
        ApplyDrive(accel, footbrake);
        CapSpeed();

        //Set the handbrake.
        //Assuming that wheels 2 and 3 are the rear wheels.
        if (handbrake > 0f)
        {
            var hbTorque = handbrake * m_MaxHandbrakeTorque;
            m_WheelColliders[2].brakeTorque = hbTorque;
            m_WheelColliders[3].brakeTorque = hbTorque;

        }


        CalculateRevs();
        GearChanging();

        AddDownForce();
        TractionControl();
    }

    private void SteerHelper()
    {
        for (int i = 0; i < 4; i++)
        {
            WheelHit wheelhit;
            m_WheelColliders[i].GetGroundHit(out wheelhit);
            if (wheelhit.normal == Vector3.zero)
                return; // wheels arent on the ground so dont realign the rigidbody velocity
        }

        // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
        if (Mathf.Abs(m_OldRotation - transform.eulerAngles.y) < 10f)
        {
            var turnadjust = (transform.eulerAngles.y - m_OldRotation) * m_SteerHelper;
            Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
            m_Rigidbody.velocity = velRotation * m_Rigidbody.velocity;
        }
        m_OldRotation = transform.eulerAngles.y;
    }


    // this is used to add more grip in relation to speed
    private void AddDownForce()
    {
        m_WheelColliders[0].attachedRigidbody.AddForce(-transform.up * m_Downforce *
                                                     m_WheelColliders[0].attachedRigidbody.velocity.magnitude);
    }

    // crude traction control that reduces the power to wheel if the car is wheel spinning too much
    private void TractionControl()
    {
        WheelHit wheelHit;
        switch (m_CarDriveType)
        {
            case CarDriveType.FourWheelDrive:
                // loop through all wheels
                for (int i = 0; i < 4; i++)
                {
                    m_WheelColliders[i].GetGroundHit(out wheelHit);

                    AdjustTorque(wheelHit.forwardSlip);
                }
                break;

            case CarDriveType.RearWheelDrive:
                m_WheelColliders[2].GetGroundHit(out wheelHit);
                AdjustTorque(wheelHit.forwardSlip);

                m_WheelColliders[3].GetGroundHit(out wheelHit);
                AdjustTorque(wheelHit.forwardSlip);
                break;

        }
    }


    private void AdjustTorque(float forwardSlip)
    {
        if (forwardSlip >= m_SlipLimit && m_CurrentTorque >= 0)
        {
            m_CurrentTorque -= 10 * m_TractionControl;
        }
        else
        {
            m_CurrentTorque += 10 * m_TractionControl;
            if (m_CurrentTorque > m_MaxTorque)
            {
                m_CurrentTorque = m_MaxTorque;
            }
        }
    }

    private void CapSpeed()
    {
        float speed = m_Rigidbody.velocity.magnitude;
        //m_MaximumSteerAngle =20f + (25f/((speed/4f) + 1f));
        switch (m_SpeedType)
        {
            case SpeedType.MPH:

                speed *= 2.23693629f;
                if (speed > m_Topspeed)
                    m_Rigidbody.velocity = (m_Topspeed / 2.23693629f) * m_Rigidbody.velocity.normalized;
                break;

            case SpeedType.KPH:
                speed *= 3.6f;
                if (speed > m_Topspeed)
                    m_Rigidbody.velocity = (m_Topspeed / 3.6f) * m_Rigidbody.velocity.normalized;
                break;
        }
    }


    private void ApplyDrive(float accel, float footbrake)
    {

        float thrustTorque;
        switch (m_CarDriveType)
        {
            case CarDriveType.FourWheelDrive:
                thrustTorque = accel * (m_CurrentTorque / 4f);
                for (int i = 0; i < 4; i++)
                {
                    m_WheelColliders[i].motorTorque = thrustTorque;
                }
                break;

            case CarDriveType.RearWheelDrive:
                thrustTorque = accel * (m_CurrentTorque / 2f);
                m_WheelColliders[2].motorTorque = m_WheelColliders[3].motorTorque = thrustTorque;
                break;

        }

        for (int i = 0; i < 4; i++)
        {
            if (CurrentSpeed > 10 && Vector3.Angle(transform.forward, m_Rigidbody.velocity) < 100f)
            {
                m_WheelColliders[i].brakeTorque = m_BrakeTorque * footbrake;
                if (verticalInput < 0)
                    m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x * 0.9995f, m_Rigidbody.velocity.y, m_Rigidbody.velocity.z * 0.9995f);

            }
            else if (footbrake > 0)
            {
                m_WheelColliders[i].brakeTorque = 0f;
                m_WheelColliders[i].motorTorque = -m_ReverseTorque * footbrake;
            }
        }
    }
}


