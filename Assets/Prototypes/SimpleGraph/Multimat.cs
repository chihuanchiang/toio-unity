using UnityEngine;
using toio;
using toio.MathUtils;
using static toio.MathUtils.Utils;

namespace toio.Multimat
{
    public class HandleMats : CubeHandle
    {
        public HandleMats(Cube _cube) : base(_cube)
        {}

        public int matX = 0;
        public int matY = 0;
        protected float lastX=250;
        protected float lastY=250;
        protected float margin = 50;
#if (UNITY_EDITOR || UNITY_STANDALONE)
        protected float leftX = 45, rightX = 455, topY = 45, bottomY = 455;
#elif (UNITY_IOS || UNITY_ANDROID || UNITY_WEBGL)
        protected float leftX = 98, rightX = 402, topY = 142, bottomY = 358;
#endif

        public override void Update()
        {
            var frm = Time.frameCount;
            if (frm == updateLastFrm) return;
            updateLastFrm = frm;

            var rawX = cube.x;
            var rawY = cube.y;

            // update matX, matY
            if (lastX > rightX - margin && rawX < leftX + margin)
                matX += 1;
            else if (rawX > rightX - margin && lastX < leftX + margin)
                matX -= 1;

            if (lastY > bottomY - margin && rawY < topY + margin)
                matY += 1;
            else if (rawY > bottomY - margin && lastY < topY + margin)
                matY -= 1;

            if (matX < 0) matX = 0;
            if (matY < 0) matY = 0;

            lastX = rawX; lastY = rawY;

            // update x, y
            x = rawX + matX * (rightX - leftX);
            y = rawY + matY * (bottomY - topY);
            deg = Deg(cube.angle);
            UpdateProperty();

            Predict();
        }
    }
}