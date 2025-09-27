using UnityEngine;
using TMPro;
using System.Collections.Generic; // Required for using Lists

public class FourPiecePuzzleController : MonoBehaviour
{
    [Header("Puzzle Settings")]
    [TextArea(5, 10)]
    public string clueToShow;
    private const int RequiredPieces = 4;

    [Header("UI References")]
    public GameObject cluePanel;
    public TextMeshProUGUI clueText;

    // We now store a list of the IDs of collected pieces.
    private List<string> collectedPieceIds = new List<string>();

    void Start()
    {
        if (cluePanel != null)
        {
            cluePanel.SetActive(false);
        }
    }

    /// <summary>
    /// This is called by the loading system to restore the puzzle's state.
    /// </summary>
    public void LoadPuzzleState(List<string> loadedIds)
    {
        if (loadedIds != null)
        {
            collectedPieceIds = loadedIds;
            Debug.Log($"Puzzle progress loaded. Player has {collectedPieceIds.Count}/{RequiredPieces} pieces.");
        }
    }

    /// <summary>
    /// This is called by each PuzzlePiece when it is collected.
    /// </summary>
    public void CollectPiece(string id)
    {
        // Only add the piece if it hasn't been collected before.
        if (!collectedPieceIds.Contains(id))
        {
            collectedPieceIds.Add(id);
            Debug.Log($"Piece '{id}' collected! Player now has {collectedPieceIds.Count}/{RequiredPieces} pieces.");

            // New Log 1: Check if we are checking for completion.
            Debug.Log("Checking for puzzle completion...");

            if (collectedPieceIds.Count >= RequiredPieces)
            {
                // New Log 2: Confirm that the puzzle is considered complete.
                Debug.Log("<color=green>PUZZLE COMPLETE! Calling ShowClue().</color>");
                ShowClue();
            }
        }
    }

    // This method returns the current list of collected IDs for saving.
    public List<string> GetCollectedPieceIds()
    {
        return collectedPieceIds;
    }

    private void ShowClue()
    {
        // New Log 3: Check if the UI references are valid.
        if (cluePanel == null || clueText == null)
        {
            Debug.LogError("ShowClue FAILED: The 'Clue Panel' or 'Clue Text' reference is missing in the Inspector!");
            return; // Stop here if references are missing.
        }

        Debug.Log("SUCCESS: Clue Panel and Clue Text references are valid. Showing clue.");

        // This is your existing code to show the panel.
        clueText.text = clueToShow;
        cluePanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseCluePanel()
    {
        // This method remains the same
        if (cluePanel != null)
        {
            cluePanel.SetActive(false);
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    public bool IsPuzzleComplete()
    {
        return collectedPieceIds.Count >= RequiredPieces;
    }

}