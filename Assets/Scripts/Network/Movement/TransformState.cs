using Unity.Netcode;
using UnityEngine;

//Tutorial:
//https://www.youtube.com/watch?v=leL6MdkJEaE

public class TransformState : INetworkSerializable {

    public int tickState;
    public Vector3 positionState;
    public Quaternion rotationState;
    public bool hasStartedMovingState;

    public void NetworkSerialize<Transform>(BufferSerializer<Transform> serializer) where Transform : IReaderWriter {

        if (serializer.IsReader) {

            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out tickState);
            reader.ReadValueSafe(out positionState);
            reader.ReadValueSafe(out rotationState);
            reader.ReadValueSafe(out hasStartedMovingState);

        } else {

            var writer = serializer.GetFastBufferWriter();
            writer.WriteValueSafe(tickState);
            writer.WriteValueSafe(positionState);
            writer.WriteValueSafe(rotationState);
            writer.WriteValueSafe(hasStartedMovingState);

        }
    }

}
