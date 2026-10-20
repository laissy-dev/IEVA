using UnityEngine;

// Positions des zones de couleur, partagées par les deux builders : les coffres sont posés dessus.
public static class ArenaLayout
{
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

    public static readonly ZoneSpot[] Zones =
    {
        new ZoneSpot("Rouge", new Vector3(-8.0f, 0.0f, 6.0f), 4.0f, 3.0f),
        new ZoneSpot("Rouge", new Vector3(2.0f, 0.0f, -5.0f), 3.5f, 2.5f),
        new ZoneSpot("Bleu", new Vector3(1.0f, 0.0f, 7.5f), 3.0f, 2.5f),
        new ZoneSpot("Bleu", new Vector3(11.0f, 0.0f, -6.0f), 3.0f, 3.5f),
        new ZoneSpot("Jaune", new Vector3(9.0f, 0.0f, 8.0f), 4.0f, 2.5f),
        new ZoneSpot("Jaune", new Vector3(-6.0f, 0.0f, 0.0f), 3.0f, 2.0f),
        new ZoneSpot("Vert", new Vector3(6.0f, 0.0f, 1.0f), 3.5f, 3.0f),
        new ZoneSpot("Vert", new Vector3(-8.0f, 0.0f, -6.0f), 4.5f, 3.5f),
    };
}
