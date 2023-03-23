using System;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerNetwork : NetworkBehaviour
{
    private NetworkVariable<MyData> _randomData = new(
        new()
        {
            randomInt = 48,
            someString = "hello"
        }, writePerm: NetworkVariableWritePermission.Owner);

    public struct MyData : INetworkSerializable
    {
        public int randomInt;
        public FixedString128Bytes someString;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref randomInt);
            serializer.SerializeValue(ref someString);
        }
    }

    public override void OnNetworkSpawn()
    {
        _randomData.OnValueChanged += ((prevValue, newValue) =>
        {
            var output = String.Format("OwnerClientId: {0}, int: {1}; text: {2}", OwnerClientId, newValue.randomInt,
                newValue.someString);
            Debug.Log(output);
        });
    }

    private void Update()
    {
        if (!IsOwner) return;


        Vector3 moveDir = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W)) moveDir.y = +1f;
        if (Input.GetKey(KeyCode.S)) moveDir.y = -1f;
        if (Input.GetKey(KeyCode.A)) moveDir.x = -1f;
        if (Input.GetKey(KeyCode.D)) moveDir.x = +1f;

        float moveSpeed = 3f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.K))
        {
            var newNumber = Random.Range(0, 100);
            _randomData.Value = new()
            {
                randomInt = newNumber,
                someString = "bye"
            };
        }
    }
}