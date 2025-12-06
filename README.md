# Gespensterjäger - Ghost Hunter VR

Ein VR-Geisterjagd-Spiel mit zweihändiger Bogen-Interaktion für Meta Quest entwickelt mit Unity.

## Projekt-Informationen

- **Unity Version**: 6000.2.4f1
- **XR Interaction Toolkit**: Version 3.2.1
- **Platform**: Meta Quest (Android)
- **VR Framework**: XR Interaction Toolkit

## Features

### Zwei-Hand Bogen-System

Das Projekt enthält ein vollständiges zweihändiges Bogen-Interaktionssystem:

- **TwoHandGrabInteractable**: Ermöglicht das Greifen mit beiden Händen
- **BowTwoHandController**: Hauptsteuerung für Bogen-Mechanik
- **BowString**: Visuelle Darstellung und Physik der Bogensehne
- **Arrow**: Pfeil-Physik mit Kollisionserkennung und Schadensystem
- **BowPoseProvider**: Hand-Posen für realistisches Bogen-Halten
- **HapticsHelper**: Utility für haptisches Feedback

### Ghost AI

- Geister mit KI-Verhalten
- Patrouille und Verfolgung des Spielers
- Schaden am Spieler bei Kontakt

## Setup-Anleitung

### 1. Unity Installation

1. Installiere Unity Hub von [unity.com](https://unity.com)
2. Installiere Unity Version **6000.2.4f1** über den Unity Hub
3. Stelle sicher, dass das Android Build Module installiert ist

### 2. Git LFS Setup

Dieses Projekt verwendet Git LFS (Large File Storage) für binäre Assets wie Unity-Szenen, Prefabs und Modelle.

#### Git LFS Installation

**Windows:**
```bash
# Mit Chocolatey
choco install git-lfs

# Oder Download von: https://git-lfs.github.com/
```

**macOS:**
```bash
brew install git-lfs
```

**Linux:**
```bash
sudo apt-get install git-lfs
```

#### Git LFS Aktivierung

Nach der Installation von Git LFS, aktiviere es in deinem Repository:

```bash
# Git LFS einmalig global installieren
git lfs install

# Repository klonen
git clone https://github.com/schneiderxenia-del/Gespensterj-ger.git
cd Gespensterj-ger

# LFS-Dateien herunterladen (falls nicht automatisch)
git lfs pull
```

#### Nachträgliche LFS-Konfiguration

Falls du das Repository bereits ohne LFS geklont hast:

```bash
# In das Repository-Verzeichnis wechseln
cd Gespensterj-ger

# Git LFS für dieses Repository aktivieren
git lfs install

# Bestehende LFS-Dateien tracken
git lfs track "*.unity"
git lfs track "*.prefab"
git lfs track "*.fbx"
git lfs track "*.png"
git lfs track "*.jpg"
git lfs track "*.mat"
git lfs track "*.anim"

# Alle LFS-Dateien herunterladen
git lfs pull
```

### 3. Projekt in Unity öffnen

1. Öffne Unity Hub
2. Klicke auf "Add" und wähle das geklonte Repository-Verzeichnis
3. Stelle sicher, dass Unity Version 6000.2.4f1 ausgewählt ist
4. Öffne das Projekt

### 4. XR Interaction Toolkit Setup

Das XR Interaction Toolkit sollte bereits im Projekt konfiguriert sein. Falls nicht:

1. Öffne den Package Manager (Window > Package Manager)
2. Suche nach "XR Interaction Toolkit"
3. Installiere Version 3.2.1
4. Importiere die Samples für XR Device Simulator (optional für Desktop-Testing)

### 5. Meta Quest Setup

#### Entwickleroptionen aktivieren

1. Verbinde die Meta Quest mit der Meta Quest Mobile App
2. Aktiviere den Entwicklermodus in den Einstellungen
3. Verbinde das Headset per USB mit dem Computer

#### Build Settings

1. Öffne File > Build Settings
2. Wähle Android als Platform
3. Klicke auf "Switch Platform"
4. Füge die BowTwoHandDemo-Szene hinzu
5. Konfiguriere Player Settings:
   - Company Name
   - Product Name
   - Minimum API Level: Android 10.0 (API Level 29)

## Bogen-System Testen

### In Unity Editor (mit XR Device Simulator)

1. Öffne die Szene: `Assets/Scenes/BowTwoHandDemo.unity`
2. Aktiviere den XR Device Simulator über Window > XR > Device Simulator
3. Starte die Szene
4. Verwende Maus und Tastatur zur Simulation:
   - Rechtsklick gedrückt halten + Mausbewegung: Kopfbewegung
   - Strg + Rechtsklick: Hand-Steuerung
   - G: Greifen (Grip)
   - T: Trigger

### Auf Meta Quest

1. Baue das Projekt (File > Build And Run)
2. Setze das Headset auf
3. Teste die Bogen-Interaktion:
   - Greife den Bogen mit einer Hand (Grip-Button)
   - Greife die Sehne mit der anderen Hand
   - Ziehe zurück und lasse los zum Schießen

## Projekt-Struktur

```
Assets/
├── Scenes/
│   ├── Main Scene.unity          # Hauptszene
│   └── BowTwoHandDemo.unity      # Demo-Szene für Bogen-System
├── Scripts/
│   ├── Bow/                      # Bogen-System Scripts
│   │   ├── TwoHandGrabInteractable.cs
│   │   ├── BowTwoHandController.cs
│   │   ├── BowString.cs
│   │   ├── Arrow.cs
│   │   ├── BowPoseProvider.cs
│   │   └── HapticsHelper.cs
│   ├── GhostAI.cs
│   ├── GameManagerVR.cs
│   └── ...
├── Prefabs/
│   ├── Bow/                      # Bogen-Prefabs
│   │   ├── BowRoot.prefab
│   │   └── Arrow.prefab
│   └── Ghost.prefab
├── Models/                        # 3D-Modelle (falls vorhanden)
├── Materials/
└── ...
```

## Bogen-Prefabs

### BowRoot Prefab

Das Haupt-Bogen-Prefab enthält:
- Bogen-Modell (oder Platzhalter)
- TwoHandGrabInteractable-Komponente
- BowTwoHandController-Komponente
- BowString-Komponente mit LineRenderer
- BowPoseProvider (optional)
- Collider für Interaktion

### Arrow Prefab

Das Pfeil-Prefab enthält:
- Pfeil-Modell (oder Platzhalter)
- Arrow-Komponente
- Rigidbody
- Collider
- TrailRenderer (optional)

## Modelle und Assets

### Verwendung vorhandener Modelle

Falls im Repository bereits 3D-Modelle vorhanden sind:
- Modelle sollten unter `Assets/Models/` oder ähnlichen Verzeichnissen liegen
- Ersetze die Platzhalter in den Prefabs durch die echten Modelle
- Weise die entsprechenden Materials zu

### Platzhalter ersetzen

Falls die Prefabs Platzhalter-GameObjects (Primitives) verwenden:

1. Öffne das BowRoot.prefab
2. Ersetze die Platzhalter-Geometrie (z.B. Cube/Cylinder) durch:
   - Bogen-Modell (importiert aus .fbx/.blend)
   - Sehnen-Visualisierung
3. Passe die Komponenten-Referenzen an
4. Speichere das Prefab

Für das Arrow.prefab:
1. Ersetze Platzhalter durch Pfeil-Modell
2. Positioniere arrowTip-Transform an der Pfeilspitze
3. Passe Collider an die Geometrie an

## Bekannte Einschränkungen

- **Ballistik-Feintuning**: Die Pfeil-Physik kann noch optimiert werden
- **IK-Integration**: Hand-IK für realistischere Hand-Positionen noch nicht implementiert
- **Hand-Poses**: Vollständige Hand-Pose-Integration für XR Hands noch ausstehend
- **Modelle**: Eventuell Platzhalter-Geometrie - echte Modelle müssen ersetzt werden
- **Audio**: Noch keine Audio-Effekte für Bogen-Spannen und Pfeil-Schuss

## Weiterentwicklung

### Geplante Features

- [ ] Inverse Kinematics (IK) für realistische Hand-Positionen
- [ ] Vollständige Hand-Tracking-Integration
- [ ] Audio-System für Bogen und Pfeile
- [ ] Erweiterte Pfeil-Typen (Feuer, Eis, etc.)
- [ ] Ziel-System mit Scoring
- [ ] Multiplayer-Support

### Ballistik-Optimierung

Die Pfeil-Physik kann über folgende Parameter in der Arrow-Komponente angepasst werden:
- `arrowMass`: Masse des Pfeils
- `drag`: Luftwiderstand
- `gravityMultiplier`: Schwerkraft-Einfluss

In BowTwoHandController:
- `maxArrowVelocity`: Maximale Geschwindigkeit
- `velocityMultiplier`: Geschwindigkeits-Multiplikator
- `minDrawStrength`: Mindest-Zugstärke für Schuss

## Troubleshooting

### XR Interaction Toolkit Fehler

Wenn Fehler bzgl. XR ITK auftreten:
1. Prüfe ob Version 3.2.1 installiert ist
2. Reimportiere das Package
3. Lösche Library-Ordner und lass Unity neu kompilieren

### Git LFS Probleme

**Problem**: Dateien werden als Text-Pointer angezeigt
```bash
# Lösung: LFS-Dateien neu herunterladen
git lfs fetch --all
git lfs pull
```

**Problem**: LFS-Quota überschritten
- Kontaktiere Repository-Administrator
- Oder: Nutze `git lfs prune` um alte Versionen zu löschen

### Build-Fehler Android

Falls Build-Fehler auftreten:
1. Prüfe Android SDK/NDK Installation
2. Stelle sicher, dass JDK installiert ist
3. Aktualisiere Android Build Tools in Unity Hub

## Lizenz

[Lizenz hier einfügen]

## Kontakt

[Kontaktinformationen hier einfügen]

## Credits

- Unity XR Interaction Toolkit
- [Weitere Credits]
