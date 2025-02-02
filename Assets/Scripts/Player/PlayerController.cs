
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("For Testing")]
    [SerializeField] bool _EnableGun;
    [SerializeField] bool _OverrideCharacter;
    [SerializeField] Character _Character;
    [SerializeField] PlayerStats _Stats;
    [Space(10)]
    [SerializeField] float _CoyoteTime;
    [SerializeField][Range(0.0001f, 0.1f)] float _MoveThreshold;
    [Space(5)]
    [SerializeField] float _Gravity;
    [SerializeField] float _MaxGravity;
    [Space(5)]
    [SerializeField][Range(1, 20)] int _MaxIteration;
    [SerializeField][Range(0, 1)] float _GroundDistance;
    [SerializeField][Range(0, 1)] float _CellingDistance;
    [SerializeField][Range(0, 1)] float _WallDistance;
    [Space(5)]
    [SerializeField][Range(0, 90)] float _GravitySlideAngle;
    [SerializeField][Range(0, 90)] float _SurfaceSlideAngle;
    [Space(5)]
    [SerializeField] LayerMask _GroundLayers;
    [Space(5)]
    [SerializeField] CapsuleCollider2D _Collider;
    [SerializeField] Animator _Animator;
    [SerializeField] Transform _Visuals;
    [SerializeField] ReverseGun _Gun;
    [SerializeField] CharacterCreator _CharacterCreater;
    [SerializeField] PlayerData _PlayerData;

    // Input variables
    Vector2 _MoveInput;

    Rigidbody2D _Rb;
    Rigidbody2D.SlideMovement _SlideData;

    ContactFilter2D _GroundFilter;
    RaycastHit2D[] _HitResults = new RaycastHit2D[2];

    bool _IsGrounded;
    bool _IsJumping;
    bool _IsMoving;

    bool _IsHeadCollide;
    bool _IsWallCollide;
    bool _CanJump;
    bool IsCoyote => !_IsJumping && Time.time - _LastGroundTime < _CoyoteTime;

    float _LastGroundTime;
    int _JumpCount;

    int _XDirection = 1;
    Vector2 _Velocity = Vector2.zero;

    private void Awake()
    {
        _Rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _Gun.Enabled = _EnableGun;
        SetData();

        // set camera follow target to this object
        CameraManager.Instance.FollowTarget = transform;
    }

    private void OnEnable()
    {
        GameEvents.Input.OnPlayerMove += OnMove;        // Subscribe to Move Input
        GameEvents.Input.OnPlayerJump += OnJump;        // Subscribe to Jump Input
    }

    private void OnDisable()
    {
        GameEvents.Input.OnPlayerMove -= OnMove;        // Unsubscribe from Move Input
        GameEvents.Input.OnPlayerJump -= OnJump;        // Unsubscribe from Jump Input
    }

    private void Update()
    {
        CheckCollision();                   // check head and foot collisions
        UpdateJump();                       // update jump data
        SetAnimations();                    // set animation states
        UpdateMovement(Time.deltaTime);     // update movement velocities
    }

    private void FixedUpdate()
    {
        Move(Time.fixedDeltaTime);  // apply movement
    }

    /// <summary>
    /// Set default Slide data
    /// </summary>
    private void SetData()
    {
        // inititalize variables
        _XDirection = 1;
        _Velocity = Vector2.zero;

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

        if (_OverrideCharacter)
            _Stats = _PlayerData.Get(_Character);
        else
            // set player stats and create character visuals
            _Stats = GameManager.CurrentScene switch
            {
                Scenes.MainMenu => _PlayerData.Get(Character.Grandpa),
                Scenes.Level1 => _PlayerData.Get(Character.Grandpa),
                Scenes.Level2 => _PlayerData.Get(Character.Daddy),
                Scenes.Level3 => _PlayerData.Get(Character.Kid),
                _ => throw new System.NotImplementedException()
            };

        _CharacterCreater.Create(_Stats.sprites);

        _Visuals.localScale = Vector3.one * _Stats.scale;
        _Collider.size *= _Stats.scale;
        _Collider.offset = _Collider.transform.localPosition * _Stats.scale - _Collider.transform.localPosition;
    }

    private void SetYVelocity(float deltaTime)
    {
        // check for head collision
        if (_IsHeadCollide && _Velocity.y > 0)
        {
            _Velocity.y = 0;
            return;
        }

        // update gravity
        _Velocity.y = (_IsGrounded ? 0 : _Velocity.y) - _Gravity * deltaTime;
        _Velocity.y = Mathf.Clamp(_Velocity.y, -_MaxGravity, _MaxGravity);
    }

    private void SetXVelocity(float deltaTime)
    {
        // check if jumping and collides 
        if (!_IsGrounded && _IsWallCollide)
        {
            _Velocity.x = -_Velocity.x * 0.3f;
            return;
        }

        // set speed, direction and acceleration
        _XDirection = _IsMoving ? (_MoveInput.x > 0 ? 1 : -1) : _XDirection;
        float speed = (_IsMoving ? _Stats.speed : 0) * _XDirection;
        float accel = _IsMoving ? _Stats.acceleration : _Stats.deceleration;

        // set velocity
        _Velocity.x = Mathf.Abs(_Velocity.x - speed) > _MoveThreshold ?
            Mathf.Lerp(_Velocity.x, speed, deltaTime * accel) : speed;

        // swap visuals if needed
        if (_Velocity.x != 0)
            _Visuals.localScale = new Vector3(_Velocity.x < 0 ? -1 : 1, 1, 1) * _Stats.scale;
    }

    private void UpdateMovement(float deltaTime)
    {
        _IsMoving = _MoveInput.x != 0;
        SetYVelocity(deltaTime);    // Set vertical velocity
        SetXVelocity(deltaTime);    // Set horizontal velocity
    }

    private void Move(float deltaTime)
    {
        // set ground snap distance to prevent snapping when jump
        _SlideData.surfaceAnchor = new Vector2(0, _IsJumping ? 0 : -_GroundDistance);

        // update position
        _Rb.Slide(_Velocity, deltaTime, _SlideData);

    }

    void UpdateJump()
    {
        // reset jumping
        _IsJumping = _IsJumping && !_IsGrounded;

        // reset jump count on grounded
        if (_IsGrounded && _JumpCount > 0)
            _JumpCount = 0;

        _CanJump = _IsGrounded || (_IsJumping && _JumpCount < _Stats.maxJumpCount) || IsCoyote;
    }

    void SetJump()
    {
        if (!_CanJump)
            return;

        _Velocity.y = _Stats.jumpForce;
        _IsJumping = true;
        _JumpCount++;
    }

    void CheckCollision()
    {
        bool wasGrounded = _IsGrounded;

        _IsGrounded = _Velocity.y <= 0 && _Collider.Cast(Vector2.down, _GroundFilter, _HitResults, _GroundDistance) > 0;     // foot collision
        _IsHeadCollide = _Velocity.y > 0 && _Collider.Cast(Vector2.up, _GroundFilter, _HitResults, _CellingDistance) > 0;    // head collision
        _IsWallCollide = _Collider.Cast(Vector2.right * _XDirection, _GroundFilter, _HitResults, _WallDistance) > 0;         // wall collision

        if (wasGrounded && !_IsGrounded)
            _LastGroundTime = Time.time;
    }

    void SetAnimations()
    {
        if (_Animator == null)
            return;

        _Animator.SetBool("Move", _IsMoving);
        _Animator.SetBool("Grounded", _IsGrounded);
    }

    void OnJump(bool _val)
    {
        if (_val) SetJump();
    }

    void OnMove(Vector2 _dir) => _MoveInput = _dir; // input direction

    private void OnDrawGizmos()
    {
        //debug head collision ray
        if (_Collider != null)
        {
            float hBody = _Collider.size.y / 2;

            Gizmos.color = Color.red;
            Gizmos.DrawRay(_Collider.transform.position + Vector3.up * hBody, Vector2.up * _CellingDistance);
            Gizmos.DrawRay(_Collider.transform.position + Vector3.down * hBody, Vector2.down * _GroundDistance);
            Gizmos.DrawRay(_Collider.transform.position + (_Collider.size.x * _XDirection * Vector3.right / 2),
                _WallDistance * _XDirection * Vector2.right);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Gun"))
        {
            if (!_Gun.Enabled)
                _Gun.Enabled = true;
            Destroy(collision.gameObject);
        }
    }
}
