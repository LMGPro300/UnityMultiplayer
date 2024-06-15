using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/*
 * Program name: PlayerPrediction.cs
 * Author: Elvin Shen (but not really at the same time)
 * What the program does: Client prediction with server authoritive movement
 * CREDITS: https://www.youtube.com/watch?v=-lGsuCEWkM0
 */

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


    //NetworkTimer just counts ticks for target tickrate
    //Client state buffer is a ciculating array of what the client thinks they are right now
    //Client input buffer is a ciculating array of the last inputs of the client
    //Server state buffer is a circulating array of where the server thinks the client is right now
    //Server input queue is a queue of inputs last sent by the client, or input needed to rewinded for syncing
    //Reconcil timer is a timer that prevent client and server ping-ponging due to reconcilation
    public void Awake(){
        Debug.Log("a timer was activated");
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

    //update the server clock and the reconcil timer
    public void Update(){
        networkTimer.Update(Time.deltaTime);
        reconcilTimer.Tick(Time.deltaTime);
    }

    //physics should be in fixed update
    public void FixedUpdate(){
        /*
        if (teleportInput == 1){
            transform.position += transform.forward * 10f;
            teleportInput = 0;
            //Debug.Break();
        }
        */
        //if a server tick has gone by, tick the server and client
        while (networkTimer.ShouldTick()){
            Debug.Log("everything got ticked");
            HandleClientTick();
            HandleServerTick();
        }
    }

    //A container that holds all the last inputs by the client
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

    //A container that holds player relative information 
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

    //Server ticks
    public void HandleServerTick(){
        //Only the server can tick the server (duh)
        if (!IsServer) return;
        int bufferIndex = -1;
        InputPayload inputPayload = default;
        //grab all of the last client inputs and render the movements on the server
        while (serverInputQueue.Count > 0){
            inputPayload = serverInputQueue.Dequeue();

            bufferIndex = inputPayload.tick % v_bufferSize;
            //int previousBufferIndex = bufferIndex - 1;
            //if (previousBufferIndex < 0) previousBufferIndex = v_bufferSize - 1;
            Debug.Log("Hello Elvin, this code is running haha");
            //Store each state of the player
            StatePayload statePayload = ProcessMovement(inputPayload);
            serverStateBuffer.Add(statePayload, bufferIndex);
        }
        if (bufferIndex == -1) return;
        //Tell all the client the current state of server
        SendToClientRpc(serverStateBuffer.Get(bufferIndex));
    }

    [ClientRpc]
    public void SendToClientRpc(StatePayload statePayload){
        //Recieve from server telling them where they should be
        serverCube.transform.position = statePayload.position;
        if (!IsOwner) return;
        //Store the last state the server sent to clients
        this.lastServerState2 = this.lastServerState;
        this.lastServerState = statePayload; 
    }

    public void HandleClientTick(){
        //Tick the client
        if (!IsClient || !IsOwner) return;
        //Debug.Log("server ticked by " + OwnerClientId);

        int currentTick = networkTimer.currentTick;
        int bufferIndex = currentTick % v_bufferSize;

        //Get the current inputs 
        InputPayload inputPayload = new InputPayload(){
            tick = currentTick,
            inputVector = playerMovement.getWishDir(),
            jumpInput = playerMovement.getJumpInput(),
            position = transform.position
        };

        //store the input and send it to the server
        clientInputBuffer.Add(inputPayload, bufferIndex);
        SendToServerRpc(inputPayload);

        //calculate the position for the client for "responsive movement"
        StatePayload statePayload = ProcessMovement(inputPayload);

        clientStateBuffer.Add(statePayload, bufferIndex);
        //Check up with the server and compare
        HandleServerReconciliation();
    }

    //Check if a reconcil is nessary
    public bool ShouldReconcil(){
        bool isNewServerState = !lastServerState.Equals(default); //if the last state updated is the latest state
        bool isLastStateUndefinedOrDifferent = lastProcessedState.Equals(default) || !lastProcessedState.Equals(lastServerState); //if the last state was different
        return isNewServerState && isLastStateUndefinedOrDifferent && reconcilTimer.IsFinished();
    }

    //Handle the reconilation
    public void HandleServerReconciliation(){
        if (!ShouldReconcil()) return;

        float positionError;
        int bufferIndex;
        //StatePayload rewindState = default;
        //Get the last appropriate state to rewind to
        bufferIndex = lastServerState.tick % v_bufferSize;
        if (bufferIndex - 1 < 0) return;


        //StatePayload rewindState = IsHost ? serverStateBuffer.Get(bufferIndex-1) : lastServerState;
        
        //positionError = Vector3.Distance(rewindState.position, clientStateBuffer.Get(bufferIndex).position);
        //Debug.Log("Diff " + positionError);
        //Get both states of the server and the client
        StatePayload rewindState = IsHost ? serverStateBuffer.Get(bufferIndex - 1) : lastServerState; // Host RPCs execute immediately, so we can use the last server state
        StatePayload clientState = IsHost ? clientStateBuffer.Get(bufferIndex) : clientStateBuffer.Get(bufferIndex-1);
        positionError = Vector3.Distance(rewindState.position, clientState.position);
        //Debug.Log(positionError + "diff");
        
        //If there is a major difference in positions, the client will be adjusted
        if (positionError > reconcilThreshold){
            ReconcileState(rewindState);
            Debug.Log("should reconcil");
            reconcilTimer.Start();
        }

        lastProcessedState = rewindState;
    }

    public void ReconcileState(StatePayload rewindState){
        //Set the client position to the correct server position
        transform.position = rewindState.position;
        transform.rotation = rewindState.rotation;
        rb.velocity = rewindState.velocity;

        if (!rewindState.Equals(lastServerState)) return;
        clientStateBuffer.Add(rewindState, rewindState.tick % v_bufferSize);

        int tickToReplay = lastServerState.tick;        
        //Recalculate all future inputs to match the new adjusted position
        while (tickToReplay < networkTimer.currentTick){
            int bufferIndex = tickToReplay % v_bufferSize;
            StatePayload statePayload = ProcessMovement(clientInputBuffer.Get(bufferIndex));
            clientStateBuffer.Add(statePayload, bufferIndex);
            tickToReplay++;
        }
    }
    
    //Queue a client input to the server
    [ServerRpc]
    public void SendToServerRpc(InputPayload inputPayload){
        clientCube.transform.position = inputPayload.position;
        serverInputQueue.Enqueue(inputPayload);
    }

    //Simulate the physics of the movement on the server
    public StatePayload ProcessMovement(InputPayload input){
        playerMovement.Move(input.inputVector, input.jumpInput);
        return new StatePayload(){
            tick = input.tick,
            position = transform.position,
            rotation = transform.rotation,
            velocity = rb.velocity
        };
    }


    public void SetNewThresHold(float thres){
        reconcilThreshold = thres;
    }

    /*
    public void RecieveTeleportInput(float teleportInput){
        this.teleportInput = teleportInput; 
    }
    */
}
