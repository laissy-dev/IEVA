# IEVA

![](IEVA.png)

6,Kenan&Alexandre,"2011: ""Extinction""",Matcher,Surpasser,5,16,21

- 5. The World is Alive Even stationary objects have some animations.
- 16. Herding cats The player controls five or more characters.
- 21. Forrest Gump: The character only has the "Yes" option in all dialogues with NPCs.

## Le jeu

Cinq survivants doivent récupérer l'antidote gardé par un dinosaure. L'arène est divisée en zones de couleur.
Le joueur contrôle un survivant à la fois et passe de l'un à l'autre avec `Tab`.
En parlant aux PNJ des coins (une seule réponse possible : « Oui »), le survivant prend leur couleur.
Si sa couleur correspond à la zone où il se trouve, il est caché du dinosaure pendant 20 s.
Le dinosaure patrouille, puis poursuit les survivants visibles. **Victoire** : ramasser l'antidote. **Défaite** : les 5 survivants sont attrapés.

Utilisation des scripts du TP d'intro (`PlayerController`, `SingleAgentController`, `ChomperBehaviour`, `OpenLevel`, `PlatformSpawner`)

## Avancement

**Lundi 12 octobre** : étapes 0, 1 et 2 du mail de Yann terminées.

- [x] Projet Unity commun `IEVADINO` (Unity 6, URP) + `.gitignore`
- [x] Player Controller : `PlayerController.cs` du TP, sur le prefab du bonhomme vert du TP
- [x] PNJ dinosaure : cylindre + NavMeshAgent + `DinoController.cs` (repris de `SingleAgentController`), NavMeshSurface sur le sol
- [x] Ramasser un objet pour gagner : `Antidote.cs` (15/10)
- [x] Récupérer le RGB d'un material : `ColorSource.cs` (13/10)
- [x] Interaction (touche + collider) : `ColorInteraction.cs`, touche E (13/10)
- [x] Appliquer la couleur au personnage : `PlayerColor.cs` (13/10)
- [x] Placer les PNJ / objets qui donnent les couleurs : 4 PNJ peintres (13/10) + 8 zones de couleur (17/10)
- [x] Script caché (20 s) : `PlayerHiding.cs` (17/10)
- [ ] Animator / machine à états du dinosaure (fait : Chomper + anim marche/idle `DinoAnimation.cs` ; reste : patrouille / poursuite)
- [ ] Scripts perdre et gagner (fait : gagner ; reste : perdre)



## Diagramme de Gantt

```mermaid
gantt
    title Projet IEVA : Extinction (12 → 23 octobre)
    dateFormat YYYY-MM-DD
    axisFormat %d/%m

    section Mise en place (12-13/10)
    Projet Unity commun, .gitignore, GitHub      :done, a0, 2026-10-12, 1d
    Arène vide (sol + NavMeshSurface)            :done, a1, 2026-10-12, 1d
    README + Gantt                               :done, a11, 2026-10-13, 1d

    section Semaine 1 : cœur du jeu
    PlayerController (repris du TP)              :done, a2, 2026-10-12, 1d
    Dinosaure NavMeshAgent qui suit le joueur    :done, a3, 2026-10-12, 1d
    Script récupération RGB du material          :done, a5, 2026-10-13, 1d
    Interaction (touche E + collider trigger)    :done, a6, 2026-10-13, 1d
    Appliquer la couleur au personnage           :done, a7, 2026-10-13, 1d
    4 PNJ peintres                               :done, a8, 2026-10-13, 1d
    Chomper (modèle + anim marche/idle)          :done, a12, 2026-10-13, 1d
    Merge des 2 parties                          :done, a13, 2026-10-15, 1d
    Ramasser l'antidote → victoire               :done, a4, 2026-10-15, 1d
    Zones de couleur                             :done, a14, 2026-10-17, 1d
    Script camouflage (zone + 20 s)              :done, a9, 2026-10-17, 1d
    Prototype jouable                            :milestone, m1, 2026-10-17, 0d
    Premier build .exe Windows (reporté)         :crit, a10, 2026-10-20, 1d

    section Semaine 2 : gameplay complet
    Animator dino (Patrouille / Poursuite)       :b1, 2026-10-19, 1d
    Switch entre les 5 survivants (Tab)          :b2, 2026-10-19, 1d
    Survivant attrapé + défaite (0 survivant)    :b3, 2026-10-20, 1d
    Menu principal + écrans victoire / défaite   :b4, 2026-10-20, 1d
    Dialogue « Oui » des PNJ (Forrest Gump)      :b5, 2026-10-21, 1d
    Animations d'ambiance (World is Alive)       :b6, 2026-10-21, 1d
    Sons + éclairage statique / dynamique        :b7, 2026-10-21, 1d
    Playtest + équilibrage                       :b8, 2026-10-22, 1d

    section Rendu
    Build final .exe                             :c1, 2026-10-22, 1d
    README (choix de gameplay et techniques)     :c2, 2026-10-22, 1d
    Slides + répétition présentation (10 min)    :c3, 2026-10-22, 1d
    Rendu (vendredi)                             :milestone, m2, 2026-10-23, 0d
```
