using UnityEngine;

namespace CodeBase.StaticData
{
    [CreateAssetMenu(fileName = "MonsterData", menuName = "Static Data/Monster")]
    public class MonsterStaticData : ScriptableObject
    {
        public MonsterTypeId MonsterTypeId;

        [Range(1, 100)] public int HP;
        [Range(1f, 30f)] public float Damage;
        [Range(.5f, 1f)] public float EffectiveDistance = 1f;
        [Range(.5f, 1f)] public float Cleavage = .5f;
        [Range(2f, 8f)] public float MoveSpeed = 3f;
        [Range(.5f, 4f)] public float AttackCooldown = 3f;

        public GameObject Prefab;
    }
}