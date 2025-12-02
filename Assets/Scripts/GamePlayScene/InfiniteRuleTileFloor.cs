using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// RuleTile(또는 일반 Tile)을 이용해
/// 플레이어 주변의 타일맵을 계속 채워 넣어
/// 끝없이 이어지는 바닥처럼 보이게 만드는 스크립트.
///
/// 사용 방법:
/// 1. Grid 아래 Tilemap 오브젝트를 만든다.
/// 2. 이 스크립트를 Tilemap이 아니라 '빈 GameObject' 또는 Tilemap 객체에 붙인다.
/// 3. tilemap 필드에 해당 Tilemap을 넣고, floorTile 필드에 RuleTile(또는 TileBase)을 할당한다.
/// 4. radius에 플레이어 주변 몇 칸까지 채울지(타일 기준)를 설정한다.
/// </summary>
public class InfiniteRuleTileFloor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;   // 플레이어 Transform
    [SerializeField] private Tilemap tilemap;    // 바닥 타일맵
    [SerializeField] private TileBase floorTile; // 보통 RuleTile

    [Header("Area Settings")]
    [Tooltip("플레이어를 중심으로 몇 타일 반경까지 채울지 (타일 단위)")]
    [SerializeField] private int radius = 20;

    [Tooltip("타일 다시 그릴 때, 범위 밖의 타일을 전부 지울지 여부")]
    [SerializeField] private bool clearOutside = false;

    // 마지막으로 기준으로 삼았던 셀 좌표
    private Vector3Int lastCenterCell;

    private void Awake()
    {
        // 타겟 자동 탐색 (Player 컴포넌트 기준)
        if (target == null)
        {
            Player player = FindObjectOfType<Player>();
            if (player != null)
                target = player.transform;
        }

        // tilemap을 안 넣어놨으면, 내 자식 중에서 찾아보기
        if (tilemap == null)
            tilemap = GetComponentInChildren<Tilemap>();

        if (tilemap == null)
        {
            Debug.LogError("InfiniteRuleTileFloor: Tilemap이 할당되지 않았습니다.");
            enabled = false;
        }

        if (floorTile == null)
        {
            Debug.LogError("InfiniteRuleTileFloor: Floor Tile(보통 RuleTile)이 할당되지 않았습니다.");
            enabled = false;
        }
    }

    private void Start()
    {
        if (enabled)
            UpdateTiles(force: true); // 처음 한 번 강제 채우기
    }

    private void LateUpdate()
    {
        if (!enabled || target == null)
            return;

        UpdateTiles(force: false);
    }

    private void UpdateTiles(bool force)
    {
        // 플레이어 현재 위치 기준 셀 좌표
        Vector3Int centerCell = tilemap.WorldToCell(target.position);

        // 플레이어가 셀 단위로 이동했을 때만 갱신
        if (!force && centerCell == lastCenterCell)
            return;

        lastCenterCell = centerCell;

        // 1) 범위 밖 타일을 삭제하고 싶으면 전부 클리어
        if (clearOutside)
        {
            tilemap.ClearAllTiles();
        }

        // 2) 중심 주변 radius 범위만큼 타일 채우기
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                Vector3Int pos = new Vector3Int(centerCell.x + x, centerCell.y + y, 0);

                // clearOutside = false 인 경우,
                // 이미 타일이 있는 칸은 그냥 둬서 불필요한 SetTile 호출 줄이기
                if (!clearOutside && tilemap.HasTile(pos))
                    continue;

                tilemap.SetTile(pos, floorTile);
            }
        }
    }

#if UNITY_EDITOR
    // 에디터에서 radius 시각화 (선택)
    private void OnDrawGizmosSelected()
    {
        if (tilemap == null || target == null) return;

        Gizmos.color = Color.yellow;
        Vector3Int centerCell = tilemap.WorldToCell(target.position);
        Vector3 centerWorld = tilemap.CellToWorld(centerCell) + tilemap.cellSize * 0.5f;

        float sizeX = tilemap.cellSize.x * (radius * 2 + 1);
        float sizeY = tilemap.cellSize.y * (radius * 2 + 1);

        Gizmos.DrawWireCube(centerWorld, new Vector3(sizeX, sizeY, 0.1f));
    }
#endif
}
