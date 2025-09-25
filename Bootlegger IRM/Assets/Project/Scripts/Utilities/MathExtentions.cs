using UnityEngine;

namespace Bootlegger
{
    public static class MathExtentions
    {
        public static Vector3 FlattenVector(this ref Vector3 vector) => new Vector3(vector.x, 0, vector.z);
    }
}
