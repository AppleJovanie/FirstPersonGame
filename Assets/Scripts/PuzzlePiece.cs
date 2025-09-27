using UnityEngine;

public class PuzzlePiece : MonoBehaviour, IInteractable
{
    // --- ADD THIS LINE ---
    [Tooltip("A unique ID for this piece (e.g., 'Puzzle_Piece_1'). MUST BE UNIQUE.")]
    public string pieceId;

    private FourPiecePuzzleController puzzleController;

    void Start()
    {
        puzzleController = FindObjectOfType<FourPiecePuzzleController>();

        // --- ADD THIS CHECK ---
        if (string.IsNullOrEmpty(pieceId))
        {
            Debug.LogError($"PuzzlePiece '{gameObject.name}' is missing a unique ID!");
        }
    }

    public void Interact()
    {
        if (puzzleController != null)
        {
            // --- MODIFY THIS LINE ---
            // Tell the controller WHICH piece was collected.
            puzzleController.CollectPiece(pieceId);
        }
        Destroy(gameObject);
    }
}