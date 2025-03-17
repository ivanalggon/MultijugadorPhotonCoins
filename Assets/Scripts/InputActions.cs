using UnityEngine;
using Photon.Pun;

public class InputActions : MonoBehaviourPunCallbacks, IPunObservable
{
    private InputSystem _inputSystem;
    private Animator _animator;

    [Header("Player Movement")]
    [SerializeField]
    private float speed = 5f;

    private Vector3 networkPosition; // Para recibir la posici�n desde la red
    private float networkInputX; // Para sincronizar animaciones

    private float inputX; // Para el movimiento local

    private void Awake()
    {
        _inputSystem = new InputSystem();
        _animator = GetComponent<Animator>();
    }

    private void OnEnable() { _inputSystem.Enable(); }
    private void OnDisable() { _inputSystem.Disable(); }

    private void Update()
    {
        if (photonView.IsMine)
        {
            // Recibir input
            inputX = _inputSystem.Player.Move.ReadValue<Vector2>().x;

            // Mover al jugador local
            Vector2 movement = new Vector2(inputX, 0) * (speed * Time.deltaTime);
            transform.Translate(movement, Space.World);

            // Actualizar animaciones para el jugador local
            _animator.SetFloat("InputX", inputX);
        }
        else
        {
            // Interpolaci�n suave para la posici�n recibida desde la red
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10);

            // Sincronizar la animaci�n en el jugador remoto
            _animator.SetFloat("InputX", networkInputX);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) // Si este es el jugador local, enviamos datos
        {
            // Enviar la posici�n como Vector3, independientemente de si es 2D o 3D
            stream.SendNext(transform.position); // Sincronizar posici�n
            stream.SendNext(inputX); // Enviar el valor de InputX para la animaci�n
        }
        else // Si es otro jugador, recibimos datos
        {
            // Recibir la posici�n como Vector3
            networkPosition = (Vector3)stream.ReceiveNext(); // Recibir posici�n

            // Recibir el valor de InputX para sincronizar animaciones
            networkInputX = (float)stream.ReceiveNext(); // Recibir el valor de InputX
        }
    }
}
