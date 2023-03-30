using Unity.Netcode;
using UnityEngine;

//Tutorial:
//https://www.youtube.com/watch?v=leL6MdkJEaE

public class TransformState : INetworkSerializable {

    public int tick;
    public Vector3 position;
    public Quaternion rotation;
    public bool hasStartedMoving;

    public void NetworkSerialize<Transform>(BufferSerializer<Transform> serializer) where Transform : IReaderWriter {

        if (serializer.IsReader) {

            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out tick);
            reader.ReadValueSafe(out position);
            reader.ReadValueSafe(out rotation);
            reader.ReadValueSafe(out hasStartedMoving);

        } else {

            var writer = serializer.GetFastBufferWriter();
            writer.WriteValueSafe(tick);
            writer.WriteValueSafe(position);
            writer.WriteValueSafe(rotation);
            writer.WriteValueSafe(hasStartedMoving);

        }
    }

}
