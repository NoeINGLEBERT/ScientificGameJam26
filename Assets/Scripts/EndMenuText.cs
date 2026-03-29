using UnityEngine;
using TMPro;

public class EndMenuText : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float PeachQuality;
    public int PeachNumber;
    private float TotalScore;
    public TextMeshProUGUI EndText;
    public TextMeshProUGUI ScoreText;
    void Start()
    {
        ScoreText.text = " Score : " + TotalScore;

        if (TotalScore < 1)
        {
            EndText.text = "Peu de fruits et de qualité moyenne\r\nDommage, s’occuper d’un arbre n’est pas évident entre gérer la ressource en eau tout au long de l’année et répondre aux évènements. Essaye encore et tu pourras devenir un formidable arboriculteur!";
        }
        if (TotalScore < 2)
        {
            EndText.text = "Peu de fruits et de bonne qualité\r\nBravo tes fruits sont de bonne qualité même si tu n’as pas beaucoup produit, tes décision ont été en faveur d’une stratégie de concentration des sucres dans les fruits au détriment de leur quantité, la prochaine fois essaye de sauver plus de fruits!";
        }

        if (TotalScore < 3)
        {
            EndText.text = "Beaucoup de fruits et de qualité moyenne\r\nBravo tu as suffisamment bien gérer ta ressource en eau pour produire des pêches mais tes décisions et ta gestion n’ont pas été en faveur de la qualité de la production, persévère tu es sur le bon chemin";
        }
        if (TotalScore < 4)
        {
            EndText.text = "Beaucoup de fruits et de bonne qualité!\r\nFélicitations : tu as su gérer ta ressource en eau et faire des bons compromis dans ta gestion des événements, tu es prêt pour devenir un véritable arboriculteur";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
