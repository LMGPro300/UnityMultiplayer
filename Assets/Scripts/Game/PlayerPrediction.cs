using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerPrediction : NetworkBehaviour
{
    private Vector3 wishDirection = Vector3.zero;

    public NetworkTimer networkTimer;
    private const float v_serverTickRate = 60f;
    private const int v_bufferSize = 1024;

    //Client buffers
    private CircularBuffer<StatePayload> clientStateBuffer;
    private CircularBuffer<InputPayload> clientInputBuffer;
    private StatePayload lastServerState;
    private StatePayload lastServerState2;
    private StatePayload lastProcessedState;

    //Server buffers
    private CircularBuffer<StatePayload> serverStateBuffer;
    private Queue<InputPayload> serverInputQueue;

    //Client movement
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] float reconciliationCooldownTime = 1f;
    [SerializeField] private float reconcilThreshold = 1f;
    [SerializeField] private GameObject serverCube;
    [SerializeField] private GameObject clientCube;

    //private float teleportInput;

    private CountdownTimer reconcilTimer;



    public void Awake(){
        networkTimer = new NetworkTimer(v_serverTickRate);
        clientStateBuffer = new CircularBuffer<StatePayload>(v_bufferSize);
        clientInputBuffer = new CircularBuffer<InputPayload>(v_bufferSize);

        serverStateBuffer = new CircularBuffer<StatePayload>(v_bufferSize);
        serverInputQueue = new Queue<InputPayload>();

        reconcilTimer = new CountdownTimer(reconciliationCooldownTime);
    }

    public override void OnNetworkSpawn(){
        Debug.Log(IsServer + " is server");
        Debug.Log(IsClient + " is cleint");
        Debug.Log(IsOwner + " is owner");
    }

    public void Update(){
        networkTimer.Update(Time.deltaTime);
        reconcilTimer.Tick(Time.deltaTime);
    }

    public void FixedUpdate(){
        /*
        if (teleportInput == 1){
            transform.position += transform.forward * 10f;
            teleportInput = 0;
            //Debug.Break();
        }
        */

        while (networkTimer.ShouldTick()){
            HandleClientTick();
            HandleServerTick();
        }
    }

    public struct InputPayload : INetworkSerializable{
        public int tick;
        public Vector3 inputVector;
        public Vector3 position;
        public float jumpInput;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter{
            serializer.SerializeValue(ref tick);
            serializer.SerializeValue(ref inputVector);
            serializer.SerializeValue(ref position);
            serializer.SerializeValue(ref jumpInput);
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
        if (!IsServer) return;
        int bufferIndex = -1;
        InputPayload inputPayload = default;
        while (serverInputQueue.Count > 0){
            inputPayload = serverInputQueue.Dequeue();

            bufferIndex = inputPayload.tick % v_bufferSize;
            //int previousBufferIndex = bufferIndex - 1;
            //if (previousBufferIndex < 0) previousBufferIndex = v_bufferSize - 1;
            Debug.Log("Hello Elvin, this code is running haha");
            StatePayload statePayload = ProcessMovement(inputPayload);
            serverStateBuffer.Add(statePayload, bufferIndex);
        }
        if (bufferIndex == -1) return;
        SendToClientRpc(serverStateBuffer.Get(bufferIndex));
    }

    [ClientRpc]
    public void SendToClientRpc(StatePayload statePayload){
        serverCube.transform.position = statePayload.position;
        if (!IsOwner) return;
        this.lastServerState2 = this.lastServerState;
        this.lastServerState = statePayload; 
    }

    public StatePayload SimulateMovement(InputPayload inputPayload){
        Physics.simulationMode = SimulationMode.Script;

        playerMovement.Move(inputPayload.inputVector, inputPayload.jumpInput);

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
        if (!IsClient || !IsOwner) return;
        //Debug.Log("server ticked by " + OwnerClientId);

        int currentTick = networkTimer.currentTick;
        int bufferIndex = currentTick % v_bufferSize;

        InputPayload inputPayload = new InputPayload(){
            tick = currentTick,
            inputVector = playerMovement.getWishDir(),
            jumpInput = playerMovement.getJumpInput(),
            position = transform.position
        };

        clientInputBuffer.Add(inputPayload, bufferIndex);
        SendToServerRpc(inputPayload);

        StatePayload statePayload = ProcessMovement(inputPayload);

        clientStateBuffer.Add(statePayload, bufferIndex);

        HandleServerReconciliation();
    }

    public bool ShouldReconcil(){
        bool isNewServerState = !lastServerState.Equals(default);
        bool isLastStateUndefinedOrDifferent = lastProcessedState.Equals(default) || !lastProcessedState.Equals(lastServerState);
        return isNewServerState && isLastStateUndefinedOrDifferent && reconcilTimer.IsFinished();
    }

    public void HandleServerReconciliation(){
        if (!ShouldReconcil()) return;

        float positionError;
        int bufferIndex;
        //StatePayload rewindState = default;

        bufferIndex = lastServerState.tick % v_bufferSize;
        if (bufferIndex - 1 < 0) return;


        //StatePayload rewindState = IsHost ? serverStateBuffer.Get(bufferIndex-1) : lastServerState;
        
        //positionError = Vector3.Distance(rewindState.position, clientStateBuffer.Get(bufferIndex).position);
        //Debug.Log("Diff " + positionError);

        StatePayload rewindState = IsHost ? serverStateBuffer.Get(bufferIndex - 1) : lastServerState; // Host RPCs execute immediately, so we can use the last server state
        StatePayload clientState = IsHost ? clientStateBuffer.Get(bufferIndex) : clientStateBuffer.Get(bufferIndex-1);
        positionError = Vector3.Distance(rewindState.position, clientState.position);
        //Debug.Log(positionError + "diff");
        
        if (positionError > reconcilThreshold){
            ReconcileState(rewindState);
            Debug.Log("should reconcil");
            reconcilTimer.Start();
        }

        lastProcessedState = rewindState;
    }

    public void ReconcileState(StatePayload rewindState){
        transform.position = rewindState.position;
        transform.rotation = rewindState.rotation;
        rb.velocity = rewindState.velocity;

        if (!rewindState.Equals(lastServerState)) return;
        clientStateBuffer.Add(rewindState, rewindState.tick % v_bufferSize);

        int tickToReplay = lastServerState.tick;

        while (tickToReplay < networkTimer.currentTick){
            int bufferIndex = tickToReplay % v_bufferSize;
            StatePayload statePayload = ProcessMovement(clientInputBuffer.Get(bufferIndex));
            clientStateBuffer.Add(statePayload, bufferIndex);
            tickToReplay++;
        }
    }
    

    [ServerRpc]
    public void SendToServerRpc(InputPayload inputPayload){
        clientCube.transform.position = inputPayload.position;
        serverInputQueue.Enqueue(inputPayload);
    }

    public StatePayload ProcessMovement(InputPayload input){
        playerMovement.Move(input.inputVector, input.jumpInput);
        return new StatePayload(){
            tick = input.tick,
            position = transform.position,
            rotation = transform.rotation,
            velocity = rb.velocity
        };
    }

    /*
    public void RecieveTeleportInput(float teleportInput){
        this.teleportInput = teleportInput; 
    }
    */
}
