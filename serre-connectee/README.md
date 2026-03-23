# Serre connectée - Cosmos - README

## Log - Qualite de Developpement

Branche de développement : merginbranch

### RENOMMAGE - Uniformisation du projet en Anglais, en renommant les classes suivantes :

Marie : \
CaseScript -> CellScript \
ControlCarreScript -> GridControlScript \
ControlCaseScript -> CellControlScript \
Plante -> Plant \
AchatScript -> PurchaseScript \
CaseBoutique -> ShopCell \
ControlAchatVenteScript -> PurchaseSaleControlScript \
PrixScript -> PriceScript \
QuantiteScript -> QuantityScript \
VenteScript -> SaleScript \
CaseInventaire -> InventoryCell \
GestionInterface -> SaveManager \
ControlDateScript -> DateControlScript \
DatationPanneauScript -> DatePanelScript

William : \
MenuCoffre -> ChestMenu \
PanneauAmelioration -> UpgradePanel \
Capteurs -> Sensors \
Graines -> Seeds \
PanneauScript --> PanelScript \
ParametresSelectionScript --> ParametersSelectionScript \
SerreInterfaceScript --> GlassHouseInterfaceScript \
VueInventaireScript --> InventoryViewScript

Raphaël : \
BoutonProgrammerScript -> ProgramScriptButton \
OptionButtonLampes -> LampsOptionButton \
OrdinateurInterface -> ComputerInterface \
PanneauProgrammationLampes -> LampProgrammingPanel \
PanneauProgrammationPots -> PotsProgrammingPanel \
PanneauProgrammationThermostat -> ThermostatProgrammingPanel \
PanneauProgrammationVolets -> ShuttersProgrammingPanel \
BoutonLivreScript -> BookScriptButton \
EncyclopedieInterface -> EncyclopediaInterface \
EncyclopedieScript -> EncyclopediaScript \
GuidesScript -> GuidesScript \
MaterielScript -> MaterialScript \
SlotsGrandesCases -> LargeCellsSlots

Mathis : \
CaseInventaire->InventoryCell \
PanneauInventaireScript->InventoryPanelScript \
InventaireScript->InventoryScript \
InventaireInterface->InventoryInterface \
BoutonClose->ButtonClose \
CaseSerreInterfaceScript->PotInventoryCellScript

Une seconde étape, de renommage des fichiers avec le nouveau nom des classes, a été nécessaire via l'éditeur Godot
afin de ne pas perdre les liens utilisés par les scènes du logiciel pour retrouver les fichiers.

### COMMENTAIRES - Retrait des commentaires non nécessaires et ajout

Raphaël : \
Documentation de ses scripts

Marie : \
Retrait de la majorité des commentaires de ses scripts de Boutique et traduction en anglais

### DEFENSIF - Programmation Défensive

Mathis : \
Déplacement des fonctions InitWeather et InitSaveInteract de Global vers SaveChoiceMenu et mise en privé. \
Déplacement des variables CompressedTexture de Global vers World.

Marie : \
Dans GridControlScript changement de type et mise en readonly du tableau constant de noms des cases de la grille.
Dans Plant mise en readonly du tableau listant les stades de plantes.

Raphaël : \
Dans UpgradePanel vérification de la valeur des paramètres de fonctions \
-la chaîne fait-elle bien partie des matériels du jeu \
-l'indice est-il bien positif et inférieur à la longueur de l'objet

William : \
**Dans InteractionComponents, ajout d'un default dans le switch case assignant le signal nécessaire** \

- le parent name correspond à aucun ? Throw new Exception

### DEBOGGAGE

Marie : \
**Objectif**: Apporter une couche graphique à la popup de tutoriel\
**Histoire**: J'ai réalisé un élément graphique en forme d'enveloppe ouverte avec une feuille posée par dessus. L'objectif était de donner ce style à la popup de tutoriel, en affichant la croix et le texte au niveau de la feuille au centre. Il existe une node "Popup Panel" dans Godot, c'est ce qui était utilisé pour la popup de tutoriel.\
**Tentative n°1**: Cette node possède un "Thème" qui permet de la styliser. En tentant d'attribuer la lettre comme style de fond de la Popup Panel, j'ai réalisé qu'il n'y avait aucun moyen d'excentrer la lettre pour que la croix de fermeture de la popup soit au niveau du papier de la lettre et non en haut à droite de toute l'image. \
**Tentative n°2**: J'ai alors changé de technique. J'ai englobé la Popup Panel et une node de texture "Texture2D" dans un même conteneur qui allait désormais servir de popup. J'ai rendu la plupart du style de la Popup Panel transparent afin que la Texture2D serve de fond, étant donné que je pouvais la placer là où je voulais dans son conteneur. \
**Problème** : En testant le tutoriel, j'ai découvert que la popup dont il ne restait plus que le style de la croix, du titre, et du texte, s'affichait correctement par-dessus les autres interfaces du jeu. Par contre, la Texture2D qui servait de fond s'affichait derrière les autres interfaces.\
**Hypothèse n°1**: C'est un problème de z-index, un attribut qui permet de définir quel élément graphique de Godot doit s'afficher par-dessus les autres. J'ai alors augmenté le z-index de la Texture2D, puis carrément du conteneur lui-même, sans succès. Cette hypothèse n'a pas abouti.\
**Hypothèse n°2**: C'est un problème d'autoload, c'est-à-dire que le script "PopupManager" qui gérait les Popups était chargé dés le lancement du jeu ce qui affichait forcément les Popups au-dessus des autres interfaces. J'ai tenté de le retirer de l'autoload, le constat n'a pas changé. \
**Hypothèse n°3**: C'est un problème interne à la manière dont Godot gère les nodes "Popup Panel". Nous sommes partis lire des documentations et des réponses sur la question. \
**Fix**: Le manque cruel de documentation sur les Popup Panel, et des messages sur le forum de Godot Engine nous ont indiqués que les nodes de type Popup étaient en réalité en expérimentation dans Godot. Ces nodes étant encore assez primitives et semblant utiliser une caractéristique non-documentée pour apparaître par-dessus tout affichage, elles ne permettaient pas de réaliser des stylisations complexes, en dehors de ce qui était prévu par leur attribut "thème". J'ai donc abandonné l'élément graphique de la lettre et ai réalisé une stylisation du tutoriel simple à partir de ce que le thème permettait de faire. \
**Commit**: merginbranch - 4bd2c8710ca0629ab3c8cdacd0ad5bacb195cb13 - "restore et texturing du tutoriel" \
\
William :\
**Zone à débugger** : \
Transition entre les différentes scènes (3D ou 2D) \
**Problème** : \
Lors d'une interaction avec un objet qui revoit vers un menu, ou bien lorsque l'on quitte ce menu, une transition faites à la main (fondu au noir) ce lance. Le problème est que pendant que la transition se fait, le joueur à suffisamment de temps pour soit appuyer plusieurs fois sur un bouton (par exemple dans le menu 2D de la météo, lui permettant de passer plusieurs jours), soit de bouger la caméra pour viser un autre objet (qu'il soit possible d'interagir avec à la base (lui permettant par exemple de chargée les données d'un pot en ayant cliqué sur un autre) ou non) \
**Hypothèse** : \
1 - Désactiver la transition (class ActionTransition), rendant donc les changements de scènes instantanés. Cela nécessite changer tout les appels à la fonction ChangeToScene de ActionTransition par la fonction ChangeSceneToFile de Godot. \
2 - Désactiver les inputs du joueurs pendant la transition, pour les réactiver ensuite. Une méthode est disponible pour cela (SetProcessInput, qui prend un booléan en paramètre), qu'on appelerait donc au début et à la fin de l'appel à la fonction ChangeToScene de ActionTransition. \
**Conclusion** : \
1 - La désactivation des transitions règlent le bug en question, mais rend la transition entre toutes les scènes/interfaces extrêmement "brutale" pour l'utilisateur. Ce choix n'est pas donc pas le plus adapté.
2 - La méthode SetProcessInput fonctionne pour désactiver les inputs du joueurs (mouvement de caméra, déplacement) dans le monde 3D, mais la méthode ne fonctionne pas de la même manière pour une interface 2D. Cette option est donc intéressante pour les scènes en 3D, mais cela nécessite de trouver un équivalent pour celles en 2D.
**Commit** : \
Bug non résolu à l'heure actuelle, donc aucun commit

**Raphaël :** :\
**Le programme** \
Dans le menu des options, une fonctionnalité qui permet de changer la résolution du jeu.\
La résolution souhaitée par le joueur est stockée dans un fichier de sauvegarde afin que quand il relance le jeu, la fenêtre s’affiche avec la bonne résolution.\
**Le problème** \
Au relancement du programme, la fenêtre ne s’affichait pas avec le bon paramètre de résolution. Pourtant, en regardant le fichier de sauvegarde, il y avait les bonnes données (celles que l’utilisateur a choisi). De plus, le programme ne générait aucune erreur et le problème apparaissait à chaque lancement du programme. \
**L’histoire** \
J’ai vérifié à maintes reprise le code de mon fichier, notamment ces points-là :\
1 Vérification de la fonction qui permet de passer d’une résolution à une autre.\
2 Vérification de l’appel de cette fonction dans le \_Ready(). C’est une fonction qui est automatiquement appelé au lancement.\
3 Vérification du fichier de sauvegarde pour voir si le programme écrivait les bonnes données à l’intérieur.\
Ces points là me semblaient valides donc je ne comprenais pas pourquoi ça ne fonctionnait pas.\
Après plusieurs dizaines de minutes à investiguer, j’ai pu trouver le problème.\
Le problème était que le menu des options avait sa propre scène, indépendante du reste de l’application. Au lancement du programme, la scène n’était pas chargée et comme mon fichier qui s’occupait de la résolution était attaché à cette scène, la fonction \_Ready() qui était à l’intérieur n’était pas appelé.\
La solution pour résoudre ce problème était plutôt simple. Il suffisait que dans le fichier global (c’est un fichier qui n’est rattaché à aucune scène et qui est appelé au lancement du jeu), j’instancie ma scène et que j’appelle manuellement la fonction \_Ready().\
**Temps pour réparer** \
Quelques heures pour réussir à identifier le problème et le corriger.\
**Leçon apprise** \
Ne pas se focaliser sur un fichier précis mais étendre sa vision, car pour ma part tous le code de mon fichier était correct.\

### L'IA POUR CODER

Marie : \
IA : Copilot en ligne
Query : "Peux-tu traduire en anglais les commentaires de ce code : ". Réalisé sur 5 scripts différents.
Succès : 100%

Mathis : \
_Objectif_ :
Le but du test était de remplacer toutes les occurrences de `System.Generic.List` dans un fichier C# par `Godot.Collections.Array`, en utilisant l'assistance de différentes IA : Copilot et ChatGPT.\
_Méthodologie_ :
5 essais ont été réalisés pour atteindre cet objectif :

1. **Copier-coller du code dans Copilot** :
   - Résultat : Le fichier était trop grand pour être traité.
2. **Envoi du fichier C# à Copilot** :
   - Résultat : Copilot ne peut pas lire les fichiers C# directement.
3. **Copier-coller dans ChatGPT** :
   - Résultat : Le code était également trop long pour être traité.
4. **Envoi du fichier à ChatGPT** :
   - Résultat : ChatGPT a renvoyé un fichier beaucoup trop court et incorrect.
5. **Demande à ChatGPT de compléter et renvoyer tout le fichier** :
   - Résultat : ChatGPT a renvoyé exactement le même fichier qu'à l'essai précédent. \

William : \
_**Objectif**_ : Générer un modèle 3d plus réaliste à partir d'un modèle déjà existant.\
_**IA Utilisé**_ : GitHub Copilot \
_**Essais réalisés**_ : 2 essais \
_**Gain de temps**_ : Aucun, l'IA a d'abord renvoyé un glb non ouvrable sur `Blender`, puis après précision de ma requête, elle a dit ne pas pouvoir manipuler de .glb mais peut expliquer point par point comment rendre le modèle plus réaliste \
_**Etapes de la réalisation**_ : \

1. **Envoie du fichier glb d'exemple à copilot et explication de la démarche attendu** :
   - Résultat : Copilot me dit qu'il va créer un dossier et me demande ou le mettre
2. **Envoi du chemin ou placer le dossier à copilot** :
   - Résultat : Copilot génère le dossier et m'explique les démarches suivies
3. **Vérification du .glb envoyé par Copilot** :
   - Résultat : Le fichier est corrompu, et inexploitable dans Blender
4. **Demande à Copilot de générer le .glb à partir du fichier exemple donné en appliquant les étapes énnoncées** :
   - Résultat : Copilot répond ne pas pouvoir manipuler les fichiers glb, et renvoie les étapes

Raphaël : \
**IA utilisée**: Github Copilot \
**Tâche demandée**: ‘Rajoute de la programmation défensive à mon code (par exemple vérifier que les paramètres des fonctions ne sont pas nuls, générer des exceptions, …)’\
**Résultats** : ‘code trop long’\
**Deuxième essai avec une seule fonction** : fait ce que j’ai voulu et marche bien\
**Gain de temps** : fais gagner du temps à condition que le code ne soit pas trop long

**Conclusion**\
Les limites des IA testées ont empêché de réaliser la tâche demandée sur un fichier volumineux. D'autres outils ou approches doivent être envisagés pour atteindre cet objectif.

### CONCEPTS - Les bonnes pratiques utilisées durant le développement

Marie:\
Je n'ai pas utilisé de design pattern ou d'architecture particulière.\
Ce qui s'en rapproche le plus sont deux principes que j'ai utilisé dans mes codes :\
**1 - Déléguer les tâches aux sous-objets.**\
 Par exemple, pour la boutique, j'ai un script principal PurchaseSaleControlScript qui charge la boutique avec ses données des articles en vente et les données de l'inventaire.
Ensuite, deux autres scripts PurchaseScript et SaleScript vont venir agir sur ce script principal pour gérer l'achat ou la vente d'objets, tout en controllant les valeurs entrées par l'utilisateur.
Toutefois, ce ne sont pas ces scripts qui détectent les appuis utilisateurs, ce sont encore des scripts inférieurs appelés PriceScript,QuantityScript et ShopCell.\
**2 - Séparer les données de l'affichage.**\
 Par exemple, dans les scripts qui gèrent le Calendrier, j'ai un premier script DatePanelScript qui s'occupe de l'affichage du calendrier et de détecter les appuis utilisateurs.
J'ai également un second script DateControlScript qui possède et met à jour les données de la météo, de la date, et l'évolution des plantes devant cette nouvelle journée, et indiquer à la vue de se recharger.
Etant donné la quantité d'éléments graphiques chargés dynamiquement dans l'interface du Calendrier, ne pas faire cette division aurait surchargé le script.

Raphaël:\
**1 - Architecture Globale**\
Nous avons utilisé le moteur de jeu Godot qui implémente sa propre architecture. Cette architecture repose sur une hiérarchie de noeuds, où chaque scène est composée de noeuds organisés en arbre. Chaque noeud a un rôle spécifique (affichage, interaction, logique, etc.), ce qui favorise la modularité. Cette architecture permet aussi de séparer les responsabilités entre les différentes parties du projet, chaque scène correspondant ainsi à une section distincte de l'application.\
**2 - Exemple**\
L'environnement 3D est une scène composée de divers noeuds représentant par exemple les objets de la serre tels que les pots ou encore le coffre. Cette scène est totalement indépendante des autres parties du jeu comme les interfaces 2D, qui ont chacune leur propre scène.\
Si on prend pour exemple l'interface 2D d'améliorations de la serre, on y retrouve des noeuds qui représentent les boutons, les labels textes et les autres éléments présents dans la scène.\

## Description

Cosmos est un jeu sérieux en 3D réalisé sur Godot. Dans ce projet, le joueur peut se déplacer, acheter, vendre et stoquer des graines. Il peut les faire pousser, en programmant l'arrosage automatique, la température et la luminosité. Il y a un système de sauvegarde et de configuration des touches, ainsi que de la documentation en jeu pour comprendre son fonctionnement.

Ce projet a été réalisé dans le cadre de la deuxième année de BUT informatique en 2024-205.

## Installation et Utilisation

Se référer au document ManuelUtilisationInstallation présent dans la liste des documents rendus à la fin du projet, dans le cadre du BUT.

## Support

Contactez l'un des membres de l'équipe de développement.

## Améliorations

Voici les pistes d'amélioration du projet :

- Correction des bugs de l'interface d'amélioration de la serre
- Complexifier la pousse avec des facteurs comme l'azote et l'oxygénation
- Ajouter des modèles 3D de stores et de lampes qui changent d'état selon la programmation

## Equipe de Developpement

Projet réalisé par :

- Raphaël BRENA
- Marie CAVALLA
- William RIBEIRO
- Mathis RODIER

Le développement de ce projet est clos.
