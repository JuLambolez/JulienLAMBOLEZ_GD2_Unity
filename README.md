Julien Lambolez GD2
Projet Roll A Ball

Pour mon jeu, j'ai voulu revoir le système de gameplay principal. Je suis parti sur une mécanique où le joueur se déplace de case en case dans le niveau, avec un nombre de coups limités. Le but est d'arriver jusqu'à la zone verte (le goal) pour réussir le niveau et ensuite passer au suivant. Dans chaque niveaux se trouvent des collectibles, qui permettent de gagner des points ou d'en perdre. Le but pour le joueur est donc de calculer ses déplacements pour optimiser au maximum les points qu'il peut gagner avant d'arriver au goal. Une fois arrivé au dernier niveau, une nouvelle mécanique s'ajoute : des clés à récupérer. En effet, dans ce niveau une porte bloque le goal, le joueur doit donc récupérer des clés dans le niveau pour dévérouiller la porte et ainsi finir le niveau, ce qui rajoute de la réfléxion dans ses mouvements.

Ce que j'ai réussi à implémenter :
- Un menu principal qui permet de lancer une partie ou de quitter le jeu
- Un système déplacement de case en case
- Un système de gestion de coups
- Une gestion du niveau en grille
- Un système de création de niveau automatique grâce à un grille en texte dans l'inspector (pour éviter de devoir ajouter chaque composant moi même)
- Un premier niveau simple qui contient des Target_Hard qui retirent des points, des Targets_Soft qui ajoutent des points, un goal qui permet de remporter le niveau
- Un écran de victoire sur lequel on peut lancer le prochain niveau ou retourner au menu principal
- Un écran de défaite sur lequel on peut recommencer le niveau ou retourner au menu principal
- Un gestion de score
- Un deuxième niveau plus compliqué que le premier (zone plus grande, plus de collectibles)
- Une musique d'ambiance
- Un troisième niveau qui rajoute une logique de porte à dévérouiller grâce à des clés trouvés dans le niveau pour atteindre le goal
- Un écran de fin de partie lorsqu'on réussit le dernier niveau qui indique le score final du joueur, et permet de retourner au menu principal
