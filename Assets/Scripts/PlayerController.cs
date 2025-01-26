
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerStats _Stats;
    [Space(5)]
    [SerializeField] bool _AirControl;
    [SerializeField] float _CoyoteTime;
    [SerializeField][Range(0.0001f, 0.1f)] float _MoveThreshold;
    [Space(5)]
    [SerializeField] float _Gravity;
    [SerializeField] float _MaxGravity;
    [Space(5)]
    [SerializeField][Range(1, 20)] int _MaxIteration;
    [SerializeField][Range(0, 1)] float _GroundDistance;
    [SerializeField][Range(0, 1)] float _CellingDistance;
    [Space(5)]
    [SerializeField][Range(0, 90)] float _GravitySlideAngle;
    [SerializeField][Range(0, 90)] float _SurfaceSlideAngle;
    [Space(5)]
    [SerializeField] LayerMask _GroundLayers;
    [Space(5)]
    [SerializeField] CapsuleCollider2D _Collider;
    [SerializeField] Animator _Animator;
    [SerializeField] Transform _Visuals;

    // Input variables
    Vector2 _Direction;

    Rigidbody2D _Rb;
    Rigidbody2D.SlideMovement _SlideData;
    Rigidbody2D.SlideResults _SlideResults;

    ContactFilter2D _GroundFilter;
    RaycastHit2D[] _HitResults = new RaycastHit2D[5];

    bool _IsGrounded;
    bool _IsJumping;
    bool _IsSprint;
    bool _IsMoving;

    bool _IsHeadCollide;
    bool _CanJump;
    bool IsCoyote => !_IsJumping && Time.time - _LastGroundTime < _CoyoteTime;

    float _LastGroundTime;
    int _JumpCount;

    int _XDirection;
    float _XVelocity;
    float _YVelocity;

    private void Awake()
    {
        _Rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        OnAirControlChanged();
        SetData();

        // set camera follow target to this object
        CameraManager.Instance.FollowTarget = transform;
    }

    private void OnEnable()
    {
        GameEvents.UI.OnAirControlChanged += OnAirControlChanged;   //**for testing only
        GameEvents.Input.OnPlayerMove += OnMove;          // Subscrive to Move Input
        GameEvents.Input.OnPlayerJump += OnJump;          // Subscrive to Jump Input
        GameEvents.Input.OnPlayerSprint += OnSprint;      // Subscrive to Sprint Input
    }

    private void OnDisable()
    {
        GameEvents.UI.OnAirControlChanged -= OnAirControlChanged;   //**for testing only
        GameEvents.Input.OnPlayerMove -= OnMove;        // Unsubscrive from Move Input
        GameEvents.Input.OnPlayerJump -= OnJump;        // Unsubscrive from Jump Input
        GameEvents.Input.OnPlayerSprint -= OnSprint;    // Subscrive to Sprint Input
    }

    private void Update()
    {
        CheckCollision();                   // check head and foot collisions
        SetJump();                          // update jump data
        SetAnimations();                    // set animation states
        UpdateMovement(Time.deltaTime);     // update movement velocities
    }

    private void FixedUpdate()
    {
        Move(Time.fixedDeltaTime);  // apply movement
    }

    // **for testing only
    private void OnAirControlChanged()
    {
        _AirControl = Settings.s_AirControlEnabled; // **for testing only
    }

    /// <summary>
    /// Set default Slide data
    /// </summary>
    private void SetData()
    {
        // inititalize variables
        _XDirection = 0;
        _XVelocity = 0;
        _YVelocity = 0;

        _LastGroundTime = Time.time;

        // Set ground filter for foot and head collision check
        _GroundFilter.useLayerMask = true;
        _GroundFilter.layerMask = _GroundLayers;

        // set slide data for movement collisions
        _SlideData.maxIterations = _MaxIteration;
        _SlideData.gravitySlipAngle = _GravitySlideAngle;
        _SlideData.surfaceSlideAngle = _SurfaceSlideAngle;

        _SlideData.gravity = Vector2.down;
        _SlideData.surfaceUp = Vector2.up;

        _SlideData.SetLayerMask(_GroundLayers);
        _SlideData.useSimulationMove = true;
    }

    private void SetYVelocity(float deltaTime)
    {
        // check for head collision
        if (_IsHeadCollide && _YVelocity > 0)
        {
            _YVelocity = 0;
            return;
        }

        // update gravity
        _YVelocity = Mathf.Clamp((_IsGrounded ? 0 : _YVelocity) - _Gravity * deltaTime, -_MaxGravity, _MaxGravity);
    }

    private void SetXVelocity(float deltaTime)
    {
        // check if jumping and collides 
        if (_IsJumping)
        {
            // bounce back from collision to prevent wall stuck
            if (_SlideResults.slideHit)
            {
                _XVelocity = -_XVelocity * 0.3f;
                return;
            }

            // return if no air controll allowed
            if (!_AirControl)
                return;
        }

        // set horizontal velocity
        _XDirection = _IsMoving ? Mathf.CeilToInt(_Direction.x) : _XDirection;

        // set speed and acceleration
        float speed, accel;
        speed = _IsMoving ? (_IsSprint ? _Stats.sprintSpeed : _Stats.walkSpeed) : 0;
        speed = _XDirection * speed;
        accel = _IsMoving ? _Stats.acceleration : _Stats.deceleration;

        // set velocity
        _XVelocity = Mathf.Abs(_XVelocity - speed) > _MoveThreshold ?
            Mathf.Lerp(_XVelocity, speed, deltaTime * accel) : speed;

        // swap visuals if needed
        if (_XVelocity != 0)
            _Visuals.localScale = new Vector3(_XVelocity < 0 ? -1 : 1, 1, 1);
    }

    private void UpdateMovement(float deltaTime)
    {
        _IsMoving = _Direction.x != 0;
        SetYVelocity(deltaTime);    // Set vertical velocity
        SetXVelocity(deltaTime);    // Set horizontal velocity

        // set ground snap distance to prevent snapping when jump
        _SlideData.surfaceAnchor = new Vector2(0, _IsJumping ? 0 : -_GroundDistance);
    }

    private void Move(float deltaTime)
    {
        // update position
        Vector2 vel = new(_XVelocity, _YVelocity);
        //_Rb.MovePosition(transform.position + (Vector3)vel * deltaTime);
        _SlideResults = _Rb.Slide(vel, deltaTime, _SlideData);
    }

    void SetJump()
    {
        // reset jumping
        _IsJumping = _IsJumping && !_IsGrounded;

        // reset jump count on grounded
        if (_IsGrounded && _JumpCount > 0)
            _JumpCount = 0;

        _CanJump = _IsGrounded || (_IsJumping && _JumpCount < _Stats.maxJumpCount) || IsCoyote;
    }

    void CheckCollision()
    {
        bool wasGrounded = _IsGrounded;


        _IsGrounded = _YVelocity <= 0 && _Collider.Cast(Vector2.down, _GroundFilter, _HitResults, _GroundDistance) > 0; // foot collision
        _IsHeadCollide = _YVelocity > 0 && _Collider.Cast(Vector2.up, _GroundFilter, _HitResults, _CellingDistance) > 0;    // head collision


        if (wasGrounded && !_IsGrounded)
            _LastGroundTime = Time.time;
    }

    void SetAnimations()
    {
        if (_Animator == null)
            return;

        _Animator.SetBool("Move", _IsMoving);
        _Animator.SetBool("Sprint", _IsSprint);
        _Animator.SetBool("Grounded", _IsGrounded);
    }

    void OnJump(bool _val)
    {
        if (_val && _CanJump)
        {
            _YVelocity = _Stats.jumpForce;
            _IsJumping = true;
            _JumpCount++;
        }
    }

    void OnMove(Vector2 _dir)
    {
        _Direction = _dir; // input direction
    }

    void OnSprint(bool _val)
    {
        _IsSprint = _val;
    }

    void OnCrouch(bool _val)
    {

    }

    private void OnDrawGizmos()
    {
        //debug head collision ray
        if (_Collider != null)
        {
            float hBody = _Collider.size.y / 2;

            Gizmos.color = Color.red;
            Gizmos.DrawRay(_Collider.transform.position + Vector3.up * hBody, Vector2.up * _CellingDistance);
            Gizmos.DrawRay(_Collider.transform.position + Vector3.down * hBody, Vector2.down * _GroundDistance);
        }
    }
}
