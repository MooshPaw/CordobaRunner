using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

// Herramientas de blockout para CordobaRunner. Todo lo que crea queda bajo un
// unico GameObject raiz por escena ("Generado (Claude)") para poder borrarlo y
// rehacerlo sin tocar lo que ya tenias armado a mano (Platforms, Coins, Birds,
// Checkpoint_02, etc. en Nivel 1 y 3 no se tocan, solo se les agregan hijos
// nuevos cuando el menu dice "agregar").
//
// IMPORTANTE: corre cada accion con la escena correspondiente (Level1/Level2/
// Level3) abierta y activa - trabaja sobre la escena que tengas abierta.
public static class CordobaBlockoutTools
{
    private const string GeneratedRootName = "Generado (Claude)";
    private const string PrefabsPath = "Assets/Prefabs/";

    // ------------------------------------------------------------------
    // NIVEL 2 - EL RIO: la escena esta practicamente vacia (solo camara, luz,
    // Terrain y managers), asi que esto arma el camino completo desde cero.
    // ------------------------------------------------------------------
    [MenuItem("Cordoba Runner/Nivel 2 - Generar El Rio (desde cero)")]
    public static void GenerateLevel2()
    {
        GameObject terrainGo = GameObject.Find("Terrain");
        Vector3 terrainPos = terrainGo != null ? terrainGo.transform.position : new Vector3(428.44f, 441.71f, 31.8f);

        // Mismo offset que usa Nivel 1 entre su Terrain y su Platforms (~12.7 unidades
        // mas abajo), para que el camino nuevo quede a una altura razonable respecto
        // al terreno existente. Es una primera aproximacion - ajusta en el Editor si
        // el camino queda flotando o enterrado.
        GameObject platformsRoot = new GameObject("Platforms");
        platformsRoot.transform.position = terrainPos + new Vector3(0f, -12.7f, 0f);
        Undo.RegisterCreatedObjectUndo(platformsRoot, "Generar Nivel 2");

        Vector3 cubeScale = new Vector3(2.3316f, 0.19236f, 1f);

        // Bloque A - orillas, sin peligro todavia
        GameObject c1 = CreatePlatform(platformsRoot.transform, "Cube_R1", new Vector3(-1.34f, 10.756f, -26.3f), cubeScale);
        CreatePlatform(platformsRoot.transform, "Cube_R2", new Vector3(2.05f, 10.756f, -23.85f), cubeScale);
        CreatePlatform(platformsRoot.transform, "Cube_R3", new Vector3(-1.34f, 10.756f, -21.3f), cubeScale);
        CreatePlatform(platformsRoot.transform, "Cube_R4", new Vector3(2.05f, 10.756f, -18.9f), cubeScale);

        AddMovingPlatform(platformsRoot.transform, "Planchon",
            new[] { new Vector3(-1.34f, 10.756f, -16.5f), new Vector3(-1.34f, 10.756f, -12.5f) },
            cubeScale, bobAmplitude: 0.15f, speed: 1.5f);

        // Bloque B - los canales: esto es lo que hace que el nivel no sea una linea recta
        CreatePlatform(platformsRoot.transform, "Cube_R5", new Vector3(2.05f, 11.306f, -9.46f), cubeScale);
        AddCurrentZone(platformsRoot.transform, "Canal1", new Vector3(2.05f, 11.8f, -7.2f), new Vector3(3f, 1.5f, 3f), Vector3.right, 5f);
        CreatePlatform(platformsRoot.transform, "Cube_R6", new Vector3(5f, 11.306f, -5f), cubeScale);
        AddCurrentZone(platformsRoot.transform, "Canal2", new Vector3(5f, 11.8f, -2.7f), new Vector3(3f, 1.5f, 3f), Vector3.left, 5f);
        CreatePlatform(platformsRoot.transform, "Cube_R7", new Vector3(2.05f, 11.306f, -0.05f), cubeScale);
        CreatePlatform(platformsRoot.transform, "Cube_R8", new Vector3(-1.34f, 11.306f, 2.28f), cubeScale);

        // Bloque C - redes de pesca + checkpoint
        AddDamageZone(platformsRoot.transform, "Red", new Vector3(0.3f, 12.3f, 4.5f), new Vector3(3f, 0.6f, 0.6f));
        CreatePlatform(platformsRoot.transform, "Cube_R9", new Vector3(2.05f, 11.946f, 4.74f), cubeScale);
        CreatePlatform(platformsRoot.transform, "Cube_R10", new Vector3(-1.34f, 11.946f, 7.29f), cubeScale);
        InstantiatePrefab(PrefabsPath + "Checkpoint_01.prefab", platformsRoot.transform.TransformPoint(new Vector3(-1.34f, 12.4f, 7.29f)), platformsRoot.transform, "Checkpoint_Rio");

        // Bloque D - cierre y salida
        CreatePlatform(platformsRoot.transform, "Cube_R11", new Vector3(2.05f, 11.946f, 9.69f), cubeScale);
        CreatePlatform(platformsRoot.transform, "Cube_R12", new Vector3(-1.34f, 11.946f, 12.14f), cubeScale);
        CreateLevelExit(platformsRoot.transform, new Vector3(-1.34f, 12.4f, 14.5f), new Vector3(3f, 1f, 3f), "Level3");

        // Red de seguridad: si te caes del camino, KillZone te respawnea igual que
        // caer al rio. Cubre todo el largo del nivel por debajo del camino.
        GameObject killZone = InstantiatePrefab(PrefabsPath + "KillZone.prefab",
            platformsRoot.transform.TransformPoint(new Vector3(0.5f, 5f, -8f)), platformsRoot.transform, "KillZone_Rio");
        if (killZone != null) killZone.transform.localScale = new Vector3(15f, 3f, 45f);

        // Reusa el grupo "Coins" que ya existe en la escena si esta, si no crea uno.
        Transform coinsGroup = FindOrCreateGroup("Coins");
        AddCoinAt(coinsGroup, c1.transform.position + Vector3.up);
        AddCoinAt(coinsGroup, platformsRoot.transform.TransformPoint(new Vector3(2.05f, 11.9f, -18.9f)));
        AddCoinAt(coinsGroup, platformsRoot.transform.TransformPoint(new Vector3(2.05f, 12.4f, -0.05f)));
        AddCoinAt(coinsGroup, platformsRoot.transform.TransformPoint(new Vector3(-1.34f, 13.0f, 12.14f)));

        FinishAndSave("Nivel 2 (El Rio) generado. Revisa alturas/gaps con Play antes de darlo por bueno.");
    }

    // ------------------------------------------------------------------
    // NIVEL 1 - LA TIERRA: ya tiene el camino de 16 plataformas armado a mano.
    // Esto SOLO agrega hazards entre plataformas existentes, llena los grupos
    // Birds/Coins que estan vacios, y agrega la salida que falta - no toca
    // ninguna Cube existente.
    // ------------------------------------------------------------------
    [MenuItem("Cordoba Runner/Nivel 1 - Agregar hazards, vida y salida")]
    public static void EnhanceLevel1()
    {
        GameObject platforms = GameObject.Find("Platforms");
        if (platforms == null)
        {
            Debug.LogError("[CordobaBlockout] No encontre el GameObject 'Platforms' en esta escena. ¿Esta abierta Level1?");
            return;
        }

        Transform p = platforms.transform;

        AddDamageZone(p, "Barro_1", new Vector3(0.5f, 10.9f, -17.8f), new Vector3(2.5f, 0.4f, 1.5f));
        AddDamageZone(p, "Barro_2", new Vector3(0.5f, 11.0f, -8.4f), new Vector3(2.5f, 0.4f, 1.5f));

        GameObject killTile = InstantiatePrefab(PrefabsPath + "KillZone.prefab", p.TransformPoint(new Vector3(0.5f, 11.5f, -1.2f)), p, "KillZone_Corral");
        if (killTile != null) killTile.transform.localScale = new Vector3(1.5f, 0.4f, 1.2f);

        Transform coinsGroup = FindOrCreateGroup("Coins");
        AddCoinAt(coinsGroup, p.TransformPoint(new Vector3(-1.34f, 11.4f, -23.85f)));
        AddCoinAt(coinsGroup, p.TransformPoint(new Vector3(2.3f, 11.4f, -9.46f)));
        AddCoinAt(coinsGroup, p.TransformPoint(new Vector3(2.3f, 12.0f, -0.05f)));
        AddCoinAt(coinsGroup, p.TransformPoint(new Vector3(2.3f, 12.6f, 9.69f)));

        Transform birdsGroup = FindOrCreateGroup("Birds");
        InstantiatePrefabAtWorld(PrefabsPath + "Bird1.prefab", p.TransformPoint(new Vector3(3.5f, 13.5f, -14.4f)), birdsGroup, "Bird1_A");
        InstantiatePrefabAtWorld(PrefabsPath + "Bird1.prefab", p.TransformPoint(new Vector3(-3f, 13.5f, 2.28f)), birdsGroup, "Bird1_B");

        // La ultima Cube(16) esta en local (2.30, 11.946, 9.69). Sigo la misma
        // direccion para la salida.
        CreateLevelExit(p, new Vector3(2.3f, 11.946f, 13.0f), new Vector3(3f, 1f, 3f), "Level2");

        Debug.LogWarning("[CordobaBlockout] Ojo: Checkpoint_02 esta en world z=65, pero el camino de Cubes termina en world z~45 " +
            "(20 unidades antes). Puse la salida justo despues de la ultima Cube en vez de en el checkpoint - si el nivel " +
            "debia seguir hasta alla, dime y lo extiendo.");

        FinishAndSave("Nivel 1 (La Tierra): hazards, monedas, pajaros y salida agregados. No toque las Cubes existentes.");
    }

    [MenuItem("Cordoba Runner/Borrar lo agregado por Claude en esta escena")]
    public static void ClearGenerated()
    {
        GameObject existing = GameObject.Find(GeneratedRootName);
        if (existing != null) Undo.DestroyObjectImmediate(existing);

        Debug.Log("[CordobaBlockout] Nota: esto solo borra el root 'Generado (Claude)' de Nivel 2. " +
            "Las adiciones a Nivel 1 (hazards, monedas, pajaros, salida) se agregaron sueltas dentro de grupos " +
            "existentes (Platforms/Coins/Birds) para no duplicar esos grupos, asi que hay que borrarlas a mano " +
            "si no te gustaron - todas tienen nombres que empiezan con Barro_, KillZone_, Coin_, Bird1_ o LevelExit.");

        FinishAndSave("Listo.");
    }

    // ------------------------------------------------------------------
    // Infraestructura compartida
    // ------------------------------------------------------------------

    private static void FinishAndSave(string message)
    {
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        Debug.Log($"[CordobaBlockout] {message} Guarda la escena (Ctrl+S).");
    }

    private static Transform FindOrCreateGroup(string name)
    {
        GameObject existing = GameObject.Find(name);
        if (existing != null) return existing.transform;

        GameObject go = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(go, "Generar Blockout");
        return go.transform;
    }

    private static GameObject CreatePlatform(Transform parent, string name, Vector3 localPos, Vector3 localScale)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPos;
        go.transform.localScale = localScale;
        Undo.RegisterCreatedObjectUndo(go, "Generar Blockout");
        return go;
    }

    private static Transform CreateWaypoint(Transform parent, string name, Vector3 worldPosition)
    {
        GameObject go = new GameObject(name);
        go.transform.position = worldPosition;
        Undo.RegisterCreatedObjectUndo(go, "Generar Blockout");
        return go.transform;
    }

    private static GameObject InstantiatePrefab(string assetPath, Vector3 worldPosition, Transform parent, string nameOverride)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
        if (prefab == null)
        {
            Debug.LogWarning($"[CordobaBlockout] No encontre el prefab en '{assetPath}'.");
            return null;
        }

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        instance.transform.SetParent(parent, true);
        instance.transform.position = worldPosition;
        instance.name = nameOverride;

        Undo.RegisterCreatedObjectUndo(instance, "Generar Blockout");
        return instance;
    }

    private static GameObject InstantiatePrefabAtWorld(string assetPath, Vector3 worldPosition, Transform parent, string nameOverride)
    {
        return InstantiatePrefab(assetPath, worldPosition, parent, nameOverride);
    }

    private static void AddCoinAt(Transform parent, Vector3 worldPosition)
    {
        InstantiatePrefab(PrefabsPath + "Coin.prefab", worldPosition, parent, $"Coin_{Random.Range(1000, 9999)}");
    }

    private static void AddDamageZone(Transform parent, string name, Vector3 localCenter, Vector3 size)
    {
        GameObject go = InstantiatePrefab(PrefabsPath + "Barro.prefab", parent.TransformPoint(localCenter), parent, name);
        if (go != null) go.transform.localScale = size;
    }

    private static void AddMovingPlatform(Transform parent, string name, Vector3[] localWaypoints, Vector3 size, float bobAmplitude, float speed)
    {
        Vector3[] worldWaypoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; i++) worldWaypoints[i] = parent.TransformPoint(localWaypoints[i]);

        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.position = worldWaypoints[0];
        go.transform.localScale = size;
        Undo.RegisterCreatedObjectUndo(go, "Generar Blockout");

        var topTrigger = go.AddComponent<BoxCollider>();
        topTrigger.isTrigger = true;
        topTrigger.size = new Vector3(1f, 0.15f, 1f);
        topTrigger.center = new Vector3(0f, 0.5f + 0.075f, 0f);

        Transform waypointsRoot = new GameObject($"{name}_Waypoints").transform;
        Undo.RegisterCreatedObjectUndo(waypointsRoot.gameObject, "Generar Blockout");

        var wps = new Transform[worldWaypoints.Length];
        for (int i = 0; i < worldWaypoints.Length; i++) wps[i] = CreateWaypoint(waypointsRoot, $"WP{i}", worldWaypoints[i]);

        var platform = go.AddComponent<MovingPlatform>();
        platform.waypoints = wps;
        platform.speed = speed;
        platform.pingPong = true;
        platform.waitAtEnds = 0.5f;
        platform.bobAmplitude = bobAmplitude;
        platform.bobSpeed = 1f;
    }

    private static void AddCurrentZone(Transform parent, string name, Vector3 localCenter, Vector3 size, Vector3 pushDirection, float pushStrength)
    {
        GameObject go = new GameObject(name);
        go.transform.position = parent.TransformPoint(localCenter);
        Undo.RegisterCreatedObjectUndo(go, "Generar Blockout");

        var box = go.AddComponent<BoxCollider>();
        box.isTrigger = true;
        box.size = size;

        var zone = go.AddComponent<CurrentZone>();
        zone.pushDirection = pushDirection;
        zone.pushStrength = pushStrength;
        zone.horizontalOnly = true;
    }

    private static void CreateLevelExit(Transform parent, Vector3 localCenter, Vector3 size, string nextScene)
    {
        GameObject go = new GameObject("LevelExit");
        go.transform.position = parent.TransformPoint(localCenter);
        Undo.RegisterCreatedObjectUndo(go, "Generar Blockout");

        var box = go.AddComponent<BoxCollider>();
        box.isTrigger = true;
        box.size = size;

        var exit = go.AddComponent<LevelExit>();

        GameObject camPosGo = new GameObject("LevelExit_CamPos");
        camPosGo.transform.position = go.transform.position + new Vector3(0f, 3f, -4f);
        camPosGo.transform.LookAt(go.transform.position);
        Undo.RegisterCreatedObjectUndo(camPosGo, "Generar Blockout");

        exit.levelToLoad = nextScene;
        exit.camPos = camPosGo.transform;

        InstantiatePrefab(PrefabsPath + "Final.prefab", go.transform.position, null, $"{go.name}_Final");
    }
}
