using UnityEngine;


// Définit les différents types de cases qui peuvent exister dans la grille
public enum TileType
{
    Empty,   // Case vide (en dehors de la grille)
    Floor,   // Sol praticable (le joueur peut marcher dessus)
    Wall,    // Mur (bloque le passage du joueur)
    Goal,    // Case objectif (la case à atteindre pour gagner)
    Player   // Position du joueur (utilisée pour marquer où est le joueur)
}
