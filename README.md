# Gespensterj-ger - Zwei-Hand-Bogeninteraktion

Dieses Repository enthält eine vollständige Implementierung einer Zwei-Hand-Bogeninteraktion für Unity VR mit dem XR Interaction Toolkit.

## Anforderungen

- **Unity Version**: 6000.2.4f1 oder höher
- **XR Interaction Toolkit**: 3.2.1
- **Target Platform**: Meta Quest, PC VR (OpenXR)

## Setup-Anleitung

### 1. Projekt-Setup

1. Öffnen Sie das Projekt in Unity 6000.2.4f1
2. Stellen Sie sicher, dass die folgenden Packages installiert sind:
   - XR Interaction Toolkit (3.2.1)
   - XR Plugin Management
   - OpenXR Plugin (oder Oculus XR Plugin für Quest)

### 2. XR Interaction Toolkit Installation

Falls noch nicht installiert:

1. Öffnen Sie den Package Manager (`Window > Package Manager`)
2. Wählen Sie "Packages: Unity Registry"
3. Suchen Sie nach "XR Interaction Toolkit"
4. Installieren Sie Version 3.2.1
5. Importieren Sie die Samples (empfohlen):
   - "Starter Assets"
   - "Hands Interaction Demo" (für Hand-Tracking)

### 3. Bogen-Komponenten Einrichten

#### 3.1. Bogen GameObject Erstellen

1. Erstellen Sie ein neues leeres GameObject in Ihrer Szene: `BowRoot`
2. Position: Dort, wo der Bogen erscheinen soll
3. Fügen Sie ein 3D-Modell für den Bogen als Child hinzu (z.B. ein Cylinder oder importiertes Modell)

#### 3.2. Bogen-Controller Hinzufügen

1. Wählen Sie das `BowRoot` GameObject
2. Fügen Sie folgende Komponenten hinzu:
   - **BowTwoHandController** (Assets/Scripts/Bow/BowTwoHandController.cs)
   - **Rigidbody**:
     - Mass: 0.5
     - Drag: 1
     - Angular Drag: 0.5
     - Use Gravity: false (wird vom XR Grab gehandhabt)
     - Is Kinematic: false
   - **Collider** (z.B. Box Collider für den Griff-Bereich)

3. Konfigurieren Sie den **BowTwoHandController**:
   - **Max Draw Length**: 0.5 (kann angepasst werden)
   - **Arrow Force Multiplier**: 20
   - **Min Draw To Fire**: 0.3

#### 3.3. Bow String Einrichten

1. Erstellen Sie ein Child GameObject unter `BowRoot`: `BowString`
2. Fügen Sie die **BowString** Komponente hinzu
3. Erstellen Sie drei Child GameObjects für die String-Attachment-Points:
   - `TopAttach` - Position: (0, 0.5, 0)
   - `BottomAttach` - Position: (0, -0.5, 0)
   - `CenterPoint` - Position: (0, 0, 0)
4. Verknüpfen Sie diese im BowString Component
5. Die BowString-Komponente erstellt automatisch einen LineRenderer

#### 3.4. Arrow Prefab Erstellen

1. Erstellen Sie ein neues GameObject: `Arrow`
2. Fügen Sie ein 3D-Modell hinzu (z.B. Cylinder mit Scale: 0.01, 0.01, 0.3)
3. Fügen Sie Komponenten hinzu:
   - **Arrow** (Assets/Scripts/Bow/Arrow.cs)
   - **Rigidbody**:
     - Mass: 0.05
     - Drag: 0.01
     - Angular Drag: 0.05
     - Use Gravity: true
   - **Collider** (Capsule Collider empfohlen)
4. Speichern Sie als Prefab: `Assets/Prefabs/Arrow.prefab`
5. Verknüpfen Sie den Arrow Prefab im BowTwoHandController

#### 3.5. XR Interaction Setup

1. Stellen Sie sicher, dass Ihre Szene ein **XR Interaction Manager** hat
2. Der Bogen benötigt **Interaction Layer** Konfiguration:
   - Setzen Sie den Bow auf einen Layer (z.B. "Interactable")
   - Stellen Sie sicher, dass Ihre XR Direct/Ray Interactors diesen Layer sehen können

### 4. Händigkeit-Konfiguration

Die Bogen-Interaktion unterstützt beide Händigkeiten:

- **Rechtshänder**: Linke Hand greift den Bogen, rechte Hand zieht die Sehne
- **Linkshänder**: Rechte Hand greift den Bogen, linke Hand zieht die Sehne

Die Implementierung erkennt automatisch, welche Hand zuerst greift (primäre Hand) und welche Hand dann die Sehne zieht (sekundäre Hand).

### 5. Optionale Komponenten

#### 5.1. Pose Provider

Fügen Sie **BowPoseProvider** hinzu für custom Hand-Posen:
- Grip Pose: Pose der Bogen-haltenden Hand
- Draw Pose: Pose der ziehenden Hand

#### 5.2. Haptic Feedback

Das **HapticsHelper**-Utility wird automatisch verwendet, wenn die Haptics im BowTwoHandController aktiviert sind:
- **Enable Haptics**: true
- **Draw Haptic Intensity**: 0.3
- **Release Haptic Duration**: 0.2

## Verwendung

### Bogen Ziehen und Schießen

1. **Greifen**: Greifen Sie den Bogen mit einer Hand (Grip-Button)
2. **Ziehen**: Greifen Sie mit der anderen Hand die Sehne (automatisch erkannt im Bereich der Secondary Attach Position)
3. **Zielen**: Bewegen Sie die ziehende Hand zurück, um die Sehne zu spannen
4. **Schießen**: Lassen Sie die ziehende Hand los (Release Grip), um den Pfeil abzuschießen

### Parameter Tuning

#### Draw Length

- **Max Draw Length**: Bestimmt, wie weit die Sehne zurückgezogen werden kann
- Empfohlen: 0.3 - 0.6m je nach Bogen-Größe
- Testen Sie verschiedene Werte für das beste Gefühl

#### Arrow Speed

- **Arrow Force Multiplier**: Bestimmt die Pfeilgeschwindigkeit
- Formel: `velocity = drawAmount * forceMultiplier`
- Empfohlen: 15-25 für realistische Geschwindigkeiten
- Höhere Werte = schnellere Pfeile

#### Haptics

- **Draw Haptic Intensity**: Feedback während des Ziehens (0.2-0.5 empfohlen)
- **Release Haptic Duration**: Feedback beim Loslassen (0.1-0.3 empfohlen)

## Bekannte Einschränkungen

1. **Keine Scene-Binaries**: Dieses Repository enthält nur Code-Dateien. Sie müssen die Szene und Prefabs manuell einrichten.
2. **3D-Modelle**: Keine Bogen- oder Pfeil-Modelle enthalten. Verwenden Sie Primitive oder importieren Sie eigene Assets.
3. **Hand-Animation**: Die BowPoseProvider-Komponente ist ein Framework. Vollständige Hand-Animationen müssen separat eingerichtet werden.
4. **Kollisionserkennung**: Die Arrow-Komponente verwendet standard Unity Physics. Für komplexe Kollisionen ggf. erweitern.

## Fehlerbehebung

### Bogen lässt sich nicht greifen

- Überprüfen Sie die Interaction Layers
- Stellen Sie sicher, dass der Collider korrekt konfiguriert ist
- Prüfen Sie, ob XRGrabInteractable aktiviert ist (BowTwoHandController erbt davon)

### Zweite Hand wird nicht erkannt

- Überprüfen Sie die Min/Max Hand Distance Einstellungen
- Stellen Sie sicher, dass beide Controller tracked werden
- Prüfen Sie die Secondary Attach Transform Position

### Pfeil fliegt nicht

- Überprüfen Sie, dass der Arrow Prefab zugewiesen ist
- Stellen Sie sicher, dass Min Draw To Fire < 1.0 ist
- Prüfen Sie die Arrow Rigidbody-Konfiguration

### Keine Haptics

- Überprüfen Sie, dass "Enable Haptics" aktiviert ist
- Stellen Sie sicher, dass XRBaseController an den Interactors vorhanden ist
- Testen Sie mit verschiedenen Intensitätswerten

## Weiterführende Entwicklung

### Empfohlene Erweiterungen

1. **Arrow Pooling**: Implementieren Sie Object Pooling für bessere Performance
2. **Verschiedene Pfeil-Typen**: Feuer, Eis, explosive Pfeile
3. **Bogen-Upgrades**: Verschiedene Bögen mit unterschiedlichen Stats
4. **Ziel-System**: Quests und Achievements für Bogenschießen
5. **Multiplayer**: Netzwerk-Synchronisation der Pfeile

### Integration mit bestehendem Code

Die Bow-Scripts verwenden den Namespace `GhostHunter.Bow` und sind so designed, dass sie leicht in bestehende Projekte integriert werden können:

- **IDamageable Interface**: Implementieren Sie dies auf Ihren Enemy-Klassen
- **HapticsHelper**: Kann für andere Interaktionen wiederverwendet werden
- **TwoHandGrabInteractable**: Basis für andere Zwei-Hand-Objekte (z.B. Zweihandschwerter)

## Lokales ZIP-Bundle erstellen

Ein optionales Shell-Script `make_zip.sh` ist vorhanden, um ein lokales ZIP des Projekts zu erstellen:

```bash
chmod +x make_zip.sh
./make_zip.sh
```

Dies erstellt eine `Gespensterjaeger_Bow.zip` Datei mit allen relevanten Projekt-Dateien (ohne Library, Temp, etc.).

## Lizenz

Siehe LICENSE-Datei im Repository.

## Support

Bei Fragen oder Problemen öffnen Sie bitte ein Issue im GitHub Repository.

---

**Viel Erfolg beim Gespensterjagen! 🏹👻**
