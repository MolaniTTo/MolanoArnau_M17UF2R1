# **CognomsNom_M17UF2R1: Roguelike Project**

## **Descripció del Joc**
El projecte és un videojoc **roguelike** 2D, on el jugador explora nivells generats proceduralment, s'enfronta a enemics i supera desafiaments per avançar al següent nivell.

El joc incorpora mecàniques clàssiques de roguelike, com ara moviment tàctic, inventari amb armes diverses, enemics amb comportaments únics i una experiència rejugable gràcies al disseny procedural dels nivells.

## **Característiques Principals**
- **Moviment del jugador:**
  - Desplaçament en vuit direccions (N, S, E, W, NE, NW, SE, SW) amb animacions per a cada direcció.
  - Animacions:
    - **Idle**: el jugador respira o es manté quiet.
    - **Movement**: el jugador camina.
    - **Hurt**: animació quan el jugador rep mal.
    - **Death**: animació de mort abans del Game Over.

- **Enemics:**
  - **Enemic Torreta:** fixa al lloc, dispara projectils dirigits al jugador.
  - **Enemic Bomba:** segueix el jugador i explota en contacte.

- **Armes:**
  - **Sniper:** tret ràpid de llarga distància.
  - **Llançagranades:** tret parabòlic amb explosió repulsiva.
  - **Llançaflames:** raig continu de partícules que fa dany progressiu.
  - **Melee:** atac de curta distància amb empenta.

- **Nivells:**
  - Mapes generats proceduralment amb zones transitables i no transitables.
  - Nivells amb portes que s'obren per complir certs esdeveniments (matar enemics, trobar claus...).
  - Scrolling de fons dinàmic amb efecte de parallaxi.

- **So i música:**
  - Música en bucle.
  - Sons específics per esdeveniments: canvi de gravetat, mort, canvi d'escena, etc.

- **Interfície:**
  - Sistema de diàlegs amb opcions que afecten el camí del jugador.
  - Inventari per equipar armes, gestionat amb Scriptable Objects.
  - HUD amb vida, munició i informació essencial del jugador.
  - Barra de vida visible sobre els enemics.

## **Controls del Joc**
| **Acció**            | **Tecla**     |
|-----------------------|---------------|
| Moviment             | W, A, S, D    |
| Atacar               | Botó esquerre del ratolí |
| Canviar arma         | Tecles numèriques 1-4 |
| Pausar el joc        | Esc           |
| Interactuar          | E             |

## **Instruccions d'Instal·lació**
1. Descarregar o clonar el repositori:
   ```bash
   git clone https://gitlab.com/CognomsNom_M17UF2R1.git
