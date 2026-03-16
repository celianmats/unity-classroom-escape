# 🏫 Classroom Escape

Un jeu d'**Escape Game en réalité virtuelle / first-person** développé sous Unity. Le joueur se retrouve enfermé dans une salle de classe et doit résoudre une série de puzzles dans un ordre précis pour s'échapper.

---

## 🎮 Gameplay & Progression

Le jeu suit une progression linéaire et verrouillée. Chaque étape doit être complétée dans l'ordre :

```
0. Allumer les lumières (Optionnel)
       ↓
1. Allumer le projecteur (télécommande)
       ↓
2. Disposer les chaises dans le bon ordre (pattern secret)
       ↓
3. Interagir avec la carte du Groenland → fait glisser un livre
       ↓
4. Ramasser la clé (cachée derrière le livre)
       ↓
5. Ouvrir la porte → Victoire !
```

---

## 🕹️ Contrôles

| Action | Touche |
|---|---|
| Se déplacer | `ZQSD` |
| Regarder | Souris |
| Interagir | `Clic gauche` |
| Courir | `Shift` |
| S'accroupir | `Control` |

---

## 📜 Scripts

### `PlayerInteraction.cs`
Script central d'interaction. Gère tous les raycasts depuis la caméra du joueur et dispatche les actions selon le **tag** de l'objet regardé.

| Tag | Action |
|---|---|
| `SchoolChair` | Ouvre/ferme la chaise (ChairToggle) |
| `Drawer` | Ouvre/ferme le tiroir (DrawerToggle) |
| `RemoteController` | Allume/éteint le projecteur |
| `LightSwitch` | Allume/éteint les lumières directionnelles |
| `Groenland` | Fait glisser le livre (si pattern chaises réussi) |
| `Key` | Ramasse la clé (si livre glissé) |
| `Door` | Déclenche la victoire (si clé ramassée) |

---

### `ChairToggle.cs`
Gère l'animation d'ouverture/fermeture d'une chaise (translation sur l'axe X). Notifie le `ChairPatternManager` à chaque changement d'état.

**Tags requis :** `SchoolChair`

---

### `ChairPatternManager.cs`
Singleton qui valide si les chaises sont dans la configuration attendue. Quand le bon pattern est formé ET que le projecteur a été allumé au moins une fois, joue le son de réussite avec un délai et un cooldown.

**Variables importantes :**
- `public static bool patternReussiUneFois` — `true` dès que le son de réussite a été joué
- `public List<ChairStateData> patternAttendu` — Liste des chaises et de leur état attendu (ouvert/fermé)

---

### `DrawerToggle.cs`
Gère l'animation d'un tiroir (translation sur l'axe Y). Joue un son différent à l'ouverture et à la fermeture.

**Tags requis :** `Drawer`

---

### `RemoteController.cs`
Gère l'état ON/OFF du projecteur de la salle.
- Affiche deux images successives sur un écran (MeshRenderer + texture) avec des fondus.
- Joue un son d'allumage, un son d'extinction, et un son en boucle (ventilateur) qui s'atténue progressivement à l'extinction.
- Contrôle une lumière liée au projecteur.
- Expose `IsTransitioning` pour bloquer l'interaction pendant le chargement de l'image.

**Tags requis :** `RemoteController`

**Variables importantes :**
- `public static bool hasBeenTurnedOnAtLeastOnce` — requis pour valider le pattern des chaises

---

### `LightSwitchToggle.cs`
Interrupteur physique qui allume/éteint des `Directional Lights`. Effectue une rotation de l'interrupteur lors du basculement. La lumière de proximité du joueur (Point Light) fait l'inverse : elle s'éteint quand les lumières s'allument.

**Tags requis :** `LightSwitch`

---

### `BookSlide.cs`
Fait glisser un livre de `-0.5` sur l'axe Z pendant une durée configurable, puis laisse la physique (Rigidbody) prendre le relais.

**Variables importantes :**
- `public static bool bookSlideActive` — utilisé pour débloquer le ramassage de la clé

---

### `KeyPickup.cs`
Ramassage de la clé. Désactive l'objet 3D et active l'icône UI dans l'inventaire. Joue un son via `AudioSource.PlayClipAtPoint()` pour éviter les conflits avec `SetActive(false)`.

**Tags requis :** `Key`

**Variables importantes :**
- `public static bool keyCollected` — utilisé pour débloquer l'interaction avec la porte

---

### `VictoryScreen.cs`
Affiche le panneau de fin de jeu, joue un son de victoire, libère le curseur et met le jeu en pause. Expose une méthode `Rejouer()` à brancher sur un bouton UI.

---

### `FlashlightFollow.cs`
*(Optionnel)* Fait suivre à une lampe torche la rotation de la caméra principale, si la torche ne peut pas être placée directement enfant de la caméra.

---

## 🗂️ Asset tiers utilisés

- **First Person Controller Pro** — *ElmanGameDevTools* (mouvement joueur, sons de pas)

---

## 👤 Auteur

Célian Mats et Dalil Hammachi
