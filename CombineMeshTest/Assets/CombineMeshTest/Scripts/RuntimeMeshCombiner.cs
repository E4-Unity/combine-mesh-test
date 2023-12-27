using UnityEngine;

public class RuntimeMeshCombiner : MonoBehaviour
{
    /* 상수 */
    const string CombinedMeshName = "Combined Mesh (Runtime)";

    /* 컴포넌트 */
    GameObject m_CombinedMeshGameObject;
    MeshFilter m_CombinedMeshFilter;
    MeshRenderer m_CombinedMeshRenderer;

    /* 필드 */
    [Header("설정")]
    [Tooltip("동일한 머터리얼을 사용하는 메시만 등록하세요")]
    [SerializeField] MeshFilter[] m_TargetMeshFilters;

    bool m_IsCombined;

    /* MonoBehaviour */
    void Awake()
    {
        // 중복 호출 방지
        if (m_IsCombined) Destroy(this);
        m_IsCombined = true;

        // 유효성 검사
        if(m_TargetMeshFilters.Length == 0) Destroy(this);

        // 결합 메시 할당을 위한 게임 오브젝트 생성
        m_CombinedMeshGameObject = new GameObject(CombinedMeshName)
        {
            transform =
            {
                parent = transform
            }
        };

        // 컴포넌트 부착
        m_CombinedMeshFilter = m_CombinedMeshGameObject.AddComponent<MeshFilter>();
        m_CombinedMeshRenderer = m_CombinedMeshGameObject.AddComponent<MeshRenderer>();

        // 머터리얼 복사
        m_CombinedMeshRenderer.sharedMaterial = m_TargetMeshFilters[0].GetComponent<MeshRenderer>().sharedMaterial;

        // 메시 결합 및 할당
        m_CombinedMeshFilter.sharedMesh = GetCombinedMesh();

        // 불필요한 메시 비활성화
        foreach (var target in m_TargetMeshFilters)
        {
            target.gameObject.SetActive(false);
        }

        // 컴포넌트 제거
        Destroy(this);
    }

    /* 메서드 */
    Mesh GetCombinedMesh()
    {
        // Combine Instance 정보 수집
        CombineInstance[] combineInstances = new CombineInstance[m_TargetMeshFilters.Length];
        for(int i = 0; i < m_TargetMeshFilters.Length; i++)
        {
            combineInstances[i].mesh = m_TargetMeshFilters[i].sharedMesh;
            combineInstances[i].transform = m_TargetMeshFilters[i].transform.localToWorldMatrix;
        }

        // 메시 결합
        var combinedMesh = new Mesh
        {
            name = CombinedMeshName
        };
        combinedMesh.CombineMeshes(combineInstances);

        // 결합된 메시 반환
        return combinedMesh;
    }
}
