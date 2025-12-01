using UnityEngine;

public class EnemyAttackState : State<EnemyBase>
{
    private float skill1Timer;
    private float skill2Timer;

    // 서로 간 간격을 둬서 사용하기 위한 락 타이머
    private float skill1LockTimer; // 스킬2 사용 후 1초 동안 스킬1 금지
    private float skill2LockTimer; // 스킬1 사용 후 1초 동안 스킬2 금지

    // stopDistance를 중심으로 한 작은 여유 범위
    private const float distanceEpsilon = 0.1f;

    // 서로 다른 스킬 사이에 강제 대기 시간
    private const float crossSkillDelay = 1f;

    public override void Enter(EnemyBase owner)
    {
        skill1Timer    = 0f;
        skill2Timer    = 0f;
        skill1LockTimer = 0f;
        skill2LockTimer = 0f;

        if (owner.Animator != null)
            owner.Animator.SetTrigger("Idle");
    }

    public override void Update(EnemyBase owner)
    {
        if (owner.Target == null)
        {
            owner.StateMachine.ChangeState(owner.States[StateType.Idle]);
            return;
        }

        Vector2 pos       = owner.transform.position;
        Vector2 targetPos = owner.Target.position;
        float   dist      = Vector2.Distance(pos, targetPos);

        // 너무 멀어지면 다시 Move 상태로 나가서 추격
        if (dist > owner.ChaseRange)
        {
            owner.StateMachine.ChangeState(owner.States[StateType.Move]);
            return;
        }

        Vector2 dirToPlayer = (targetPos - pos).normalized;

        // ===== 1) 거리 유지 (카이팅 / 링 유지) =====
        if (owner.Rigidbody2D != null)
        {
            if (dist > owner.StopDistance + distanceEpsilon)
            {
                // 너무 멀다 → 플레이어 쪽으로 접근
                owner.Rigidbody2D.linearVelocity = dirToPlayer * owner.MoveSpeed;
            }
            else if (dist < owner.StopDistance - distanceEpsilon)
            {
                // 너무 가깝다 → 플레이어 반대 방향으로 후퇴
                owner.Rigidbody2D.linearVelocity = -dirToPlayer * owner.MoveSpeed;
            }
            else
            {
                // 적당한 거리 → 멈추고 스킬 사용
                owner.Rigidbody2D.linearVelocity = Vector2.zero;
            }
        }

        // ===== 2) 쿨타임 & 락 타이머 처리 =====
        float dt = Time.deltaTime;
        skill1Timer    -= dt;
        skill2Timer    -= dt;
        skill1LockTimer -= dt;
        skill2LockTimer -= dt;

        // 0 밑으로 내려가면 강제로 0으로 클램프
        if (skill1Timer < 0f)    skill1Timer = 0f;
        if (skill2Timer < 0f)    skill2Timer = 0f;
        if (skill1LockTimer < 0f) skill1LockTimer = 0f;
        if (skill2LockTimer < 0f) skill2LockTimer = 0f;

        // ===== 3) 사용 가능 여부 판정 =====
        bool canUseSkill1 =
            skill1Timer <= 0f &&              // 쿨타임 끝
            skill1LockTimer <= 0f &&          // 다른 스킬(2번) 쓰고 난 후 딜레이도 끝
            dist <= owner.Skill1Range;        // 사거리 안

        bool canUseSkill2 =
            skill2Timer <= 0f &&
            skill2LockTimer <= 0f &&
            dist <= owner.Skill2Range;

        // 둘 다 못 쓰면 그냥 거리 유지만
        if (!canUseSkill1 && !canUseSkill2)
            return;

        // ===== 4) 스킬 사용 로직 =====
        // 한 프레임에 한 개만 사용.
        // 둘 다 가능한 경우, 취향대로 우선순위 설정 (예시: 2번이 더 멀리/위험한 스킬)
        if (canUseSkill1 && canUseSkill2)
        {
            // 예시: 플레이어가 좀 더 멀리 있다면 사거리 긴 스킬2 우선,
            // 아니면 스킬1 우선.
            if (owner.Skill2Range > owner.Skill1Range && dist > owner.Skill1Range)
            {
                // 스킬2 사용
                owner.Skill2();
                skill2Timer    = owner.Skill2Cooldown;
                skill1LockTimer = crossSkillDelay; // 2번 쓰고 나서 1초 동안 1번 막기
            }
            else
            {
                // 스킬1 사용
                owner.Skill1();
                skill1Timer    = owner.Skill1Cooldown;
                skill2LockTimer = crossSkillDelay; // 1번 쓰고 나서 1초 동안 2번 막기
            }

            return; // 한 프레임에 두 번 못 쓰게 여기서 종료
        }

        if (canUseSkill1)
        {
            owner.Skill1();
            skill1Timer    = owner.Skill1Cooldown;
            skill2LockTimer = crossSkillDelay; // 1번 후 2번 잠깐 봉인
        }
        else if (canUseSkill2)
        {
            owner.Skill2();
            skill2Timer    = owner.Skill2Cooldown;
            skill1LockTimer = crossSkillDelay; // 2번 후 1번 잠깐 봉인
        }
    }

    public override void Exit(EnemyBase owner)
    {
        if (owner.Animator != null)
            owner.Animator.ResetTrigger("Idle");

        if (owner.Rigidbody2D != null)
            owner.Rigidbody2D.linearVelocity = Vector2.zero;
    }
}
