# Modell-Platzhalter für Bogen-System

## Status

Die aktuellen Bow- und Arrow-Prefabs verwenden **Platzhalter-Geometrie** (Unity Primitives wie Cubes, Cylinders und Spheres).

## Erforderliche 3D-Modelle

### Für BowRoot.prefab

Das BowRoot-Prefab benötigt ein vollständiges Bogen-Modell mit folgenden Komponenten:

1. **Bogenkörper** (Bow Body)
   - Aktuell: Cube (0.05 x 0.6 x 0.05)
   - Benötigt: 3D-Modell eines Bogens (z.B. .fbx oder .blend)
   - Empfohlene Größe: ~0.6-0.8m hoch, ~0.05-0.1m breit

2. **Sehnen-Befestigungspunkte**
   - Oberer Punkt: TopStringPoint (y: +0.25)
   - Unterer Punkt: BottomStringPoint (y: -0.25)
   - Diese Punkte sollten an den Enden des Bogen-Modells positioniert werden

### Für Arrow.prefab

Das Arrow-Prefab benötigt ein Pfeil-Modell:

1. **Pfeilschaft** (Arrow Shaft)
   - Aktuell: Cylinder (0.01 x 0.01 x 0.5)
   - Benötigt: 3D-Modell eines Pfeilschafts
   - Länge: ~0.5m

2. **Pfeilspitze** (Arrow Tip)
   - Aktuell: Sphere (0.01 x 0.01 x 0.05)
   - Benötigt: 3D-Modell einer Pfeilspitze
   - Position: Am vorderen Ende des Schafts (z: +0.5)

3. **Befiederung** (optional)
   - Kann am hinteren Ende hinzugefügt werden

## Modelle aus dem Repository verwenden

Falls bereits Bogen- oder Pfeil-Modelle im Repository vorhanden sind:

1. Suche nach Modellen in:
   - `Assets/Models/`
   - `Assets/Cartoon_Weapon_Pack/`
   - Andere Asset-Ordner

2. Importierte .fbx/.blend Dateien können verwendet werden

## Modelle ersetzen - Anleitung

### BowRoot.prefab anpassen

1. Öffne `Assets/Prefabs/Bow/BowRoot.prefab` in Unity
2. Wähle das "BowBody" GameObject
3. Lösche die MeshFilter und MeshRenderer Komponenten
4. Ziehe das Bogen-Modell als Child unter BowBody
5. Passe die Position und Rotation an
6. Stelle sicher, dass die Collider-Größe zum Modell passt
7. Überprüfe die TopStringPoint und BottomStringPoint Positionen

### Arrow.prefab anpassen

1. Öffne `Assets/Prefabs/Bow/Arrow.prefab` in Unity
2. Ersetze "ArrowShaft" mit dem Pfeil-Modell
3. Ersetze "ArrowTip" mit der Pfeilspitzen-Geometrie
4. **Wichtig**: Setze die `arrowTip` Referenz in der Arrow-Komponente auf die Spitze
5. Passe den CapsuleCollider an die neue Geometrie an
6. Teste die Pfeil-Physik

## Empfohlene Asset-Quellen

Falls keine Modelle vorhanden sind:

- **Unity Asset Store**: Suche nach "Bow", "Arrow", "Medieval Weapons"
- **Sketchfab**: CC-lizenzierte Modelle
- **Blender**: Eigene Modelle erstellen
- Cartoon_Weapon_Pack (falls bereits im Projekt)

## Materialien und Texturen

Vergiss nicht, auch passende Materialien zuzuweisen:

- Holz-Material für Bogenkörper
- Metall-Material für Pfeilspitze
- String-Material für die Sehne (bereits vorhanden via LineRenderer)

## Testing nach Modell-Austausch

Nach dem Austausch der Modelle:

1. Öffne die `BowTwoHandDemo` Szene
2. Teste das Greifen des Bogens
3. Teste das Spannen der Sehne
4. Teste das Schießen von Pfeilen
5. Überprüfe Collider und Physik
6. Passe Parameter in den Komponenten an (falls nötig)

## Hinweis

Die aktuelle Implementation funktioniert auch mit den Platzhaltern für Testing und Entwicklung. Der Austausch ist primär für visuelle Verbesserungen.
