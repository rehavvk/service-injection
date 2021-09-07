#if ODIN_INSPECTOR
using Sirenix.Serialization;
using UnityEngine;

namespace Rehawk.ServiceInjection
{
    public abstract class SerializedMonoBootstrapper : MonoBootstrapper, ISerializationCallbackReceiver, ISupportsPrefabSerialization
    {
        [SerializeField]
        [HideInInspector]
        private SerializationData serializationData;

        SerializationData ISupportsPrefabSerialization.SerializationData
        {
            get { return this.serializationData; }
            set { this.serializationData = value; }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            UnitySerializationUtility.DeserializeUnityObject(this, ref this.serializationData);
            this.OnAfterDeserialize();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            this.OnBeforeSerialize();
            UnitySerializationUtility.SerializeUnityObject(this, ref this.serializationData);
        }

        /// <summary>Invoked after deserialization has taken place.</summary>
        protected virtual void OnAfterDeserialize() {}

        /// <summary>Invoked before serialization has taken place.</summary>
        protected virtual void OnBeforeSerialize() {}
        
#if UNITY_EDITOR
        [Sirenix.OdinInspector.HideInTables]
        [Sirenix.OdinInspector.OnInspectorGUI]
        [Sirenix.OdinInspector.PropertyOrder(-2.147484E+09f)]
        private void InternalOnInspectorGUI() => Sirenix.OdinInspector.EditorOnlyModeConfigUtility.InternalOnInspectorGUI(this);
#endif
    }
}
#endif