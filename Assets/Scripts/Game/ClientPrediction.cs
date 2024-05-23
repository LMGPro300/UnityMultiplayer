using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ClientPrediction : NetworkBehaviour
{
    //Server Settings
    public NetworkTimer networkTimer;
    private const float v_serverTickRate = 60f;
    private const int v_bufferSize = 1024;

    //Client buffers
    private CircularBuffer<StatePayload> clientStateBuffer;
    private CircularBuffer<InputPayload> clientInputBuffer;
    private StatePayload lastServerState;
    private StatePayload lastProcessedState;

    //Server buffers
    private CircularBuffer<StatePayload> serverStateBuffer;
    private Queue<InputPayload> serverInputQueue;

    //Client movement
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Rigidbody rb;

    public void Awake(){
        networkTimer = new NetworkTimer(v_serverTickRate);
        clientStateBuffer = new CircularBuffer<StatePayload>(v_bufferSize);
        clientInputBuffer = new CircularBuffer<InputPayload>(v_bufferSize);

        serverStateBuffer = new CircularBuffer<StatePayload>(v_bufferSize);
        serverInputQueue = new Queue<InputPayload>();
    }

    public void Update(){
        networkTimer.Update(Time.deltaTime);
    }

    public struct InputPayload : INetworkSerializable{
        public int tick;
        public Vector3 inputVector;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter{
            serializer.SerializeValue(ref tick);
            serializer.SerializeValue(ref inputVector);
        }
    }

    public struct StatePayload : INetworkSerializable{
        public int tick;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 velocity;
        //public Vector3 angularVelocity;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter{
            serializer.SerializeValue(ref tick);
            serializer.SerializeValue(ref position);
            serializer.SerializeValue(ref rotation);
            serializer.SerializeValue(ref velocity);
            //serializer.SerializeValue(ref angularVelocity);
        }
    }

    public void HandleServerTick(){
        int bufferIndex = -1;
        while (serverInputQueue.Count > 0){
            InputPayload inputPayload = serverInputQueue.Dequeue();

            bufferIndex = inputPayload.tick % v_bufferSize;
            StatePayload statePayload = SimulateMovement(inputPayload);
            serverStateBuffer.Add(statePayload, bufferIndex);
        }
        if (bufferIndex == -1) return;
        SendToClientRpc(serverStateBuffer.Get(bufferIndex));
    }

    [ClientRpc]
    public void SendToClientRpc(StatePayload statePayload){
        if (!IsOwner) return;
        lastServerState = statePayload;
    }

    public StatePayload SimulateMovement(InputPayload inputPayload){
        Physics.simulationMode = SimulationMode.Script;

        playerMovement.Move();

        Physics.Simulate(Time.fixedDeltaTime);
        Physics.simulationMode = SimulationMode.FixedUpdate;

        return new StatePayload(){
            tick = inputPayload.tick,
            position = transform.position,
            rotation = transform.rotation,
            velocity = rb.velocity
        };
    }

    public void HandleClientTick(){
        if (!IsClient) return;

        int currentTick = networkTimer.currentTick;
        int bufferIndex = currentTick % v_bufferSize;

        InputPayload inputPayload = new InputPayload(){
            tick = currentTick,
            inputVector = playerMovement.wishDirection
        };

        clientInputBuffer.Add(inputPayload, bufferIndex);
        SendToServerRpc(inputPayload);

        StatePayload statePayload = ProcessMovement(inputPayload);
        clientStateBuffer.Add(statePayload, bufferIndex);

        //HandleServerReconciliation();
    }

    [ServerRpc]
    public void SendToServerRpc(InputPayload inputPayload){
        serverInputQueue.Enqueue(inputPayload);
    }

    public StatePayload ProcessMovement(InputPayload input){
        playerMovement.Move();
        return new StatePayload(){
            tick = input.tick,
            position = transform.position,
            rotation = transform.rotation,
            velocity = rb.velocity
        };
    }
}
