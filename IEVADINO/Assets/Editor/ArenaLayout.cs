using UnityEngine;

// Disposition de l'arène (45 x 45), partagée par les deux builders : les coffres sont posés sur les zones.
public static class ArenaLayout
{
    public const float HalfSize = 22.5f;
    public const float PainterCorner = 18.0f;

    public struct ZoneSpot
    {
        public string ColorName;
        public Vector3 Center;
        public float RadiusX;
        public float RadiusZ;

        public ZoneSpot(string colorName, Vector3 center, float radiusX, float radiusZ)
        {
            ColorName = colorName;
            Center = center;
            RadiusX = radiusX;
            RadiusZ = radiusZ;
        }
    }

    public struct ObstacleSpot
    {
        public string Name;
        public Vector3 Position;
        public Vector3 Size;
        public float Yaw;

        public ObstacleSpot(string name, Vector3 position, Vector3 size, float yaw)
        {
            Name = name;
            Position = position;
            Size = size;
            Yaw = yaw;
        }
    }

    public static readonly ZoneSpot[] Zones =
    {
        new ZoneSpot("Rouge", new Vector3(-12.0f, 0.0f, 9.0f), 6.0f, 4.5f),
        new ZoneSpot("Rouge", new Vector3(3.0f, 0.0f, -7.5f), 5.25f, 3.75f),
        new ZoneSpot("Bleu", new Vector3(1.5f, 0.0f, 11.25f), 4.5f, 3.75f),
        new ZoneSpot("Bleu", new Vector3(16.5f, 0.0f, -9.0f), 4.5f, 5.25f),
        new ZoneSpot("Jaune", new Vector3(13.5f, 0.0f, 12.0f), 6.0f, 3.75f),
        new ZoneSpot("Jaune", new Vector3(-9.0f, 0.0f, 0.0f), 4.5f, 3.0f),
        new ZoneSpot("Vert", new Vector3(9.0f, 0.0f, 1.5f), 5.25f, 4.5f),
        new ZoneSpot("Vert", new Vector3(-12.0f, 0.0f, -9.0f), 6.75f, 5.25f),
    };

    // Dans les espaces libres entre les zones de couleur et les PNJ des coins.
    public static readonly ObstacleSpot[] Obstacles =
    {
        new ObstacleSpot("Rocher_Centre", new Vector3(-0.45f, 0.75f, 1.95f), new Vector3(3.0f, 1.5f, 3.0f), 15.0f),
        new ObstacleSpot("Rocher_Ouest", new Vector3(-19.5f, 0.6f, 0.75f), new Vector3(2.25f, 1.2f, 3.75f), -10.0f),
        new ObstacleSpot("Rocher_Est", new Vector3(19.5f, 0.6f, 2.25f), new Vector3(2.25f, 1.2f, 3.75f), 10.0f),
        new ObstacleSpot("Muret_Nord", new Vector3(0.0f, 0.6f, 16.9f), new Vector3(18.0f, 1.2f, 0.5f), 0.0f),
        new ObstacleSpot("Muret_Sud", new Vector3(0.0f, 0.6f, -16.9f), new Vector3(18.0f, 1.2f, 0.5f), 0.0f),
    };

    // Refuge hors de l'arène, relié au mur sud par une passerelle.
    public static readonly Vector3 SpawnCenter = new Vector3(0.0f, 0.0f, -29.0f);
    public static readonly Vector2 SpawnSize = new Vector2(12.0f, 6.0f);
    public const float BridgeWidth = 3.0f;

    public static readonly Vector3[] SurvivorSpawns =
    {
        new Vector3(-4.0f, 0.0f, -29.0f),
        new Vector3(-2.0f, 0.0f, -29.0f),
        new Vector3(0.0f, 0.0f, -29.0f),
        new Vector3(2.0f, 0.0f, -29.0f),
        new Vector3(4.0f, 0.0f, -29.0f),
    };

    public static readonly Vector3 DinoSpawn = new Vector3(0.0f, 0.0f, 12.0f);
}
