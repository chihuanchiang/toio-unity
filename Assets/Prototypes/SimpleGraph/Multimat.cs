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
        public override void Update()
        {
            var frm = Time.frameCount;
            if (frm == updateLastFrm) return;
            updateLastFrm = frm;

            var rawX = cube.x;
            var rawY = cube.y;

            // update matX, matY
            if (lastX > 455 - 50 && rawX < 45 + 50)
                matX += 1;
            else if (rawX > 455 - 50 && lastX < 45 + 50)
                matX -= 1;

            if (lastY > 455 - 50 && rawY < 45 + 50)
                matY += 1;
            else if (rawY > 455 - 50 && lastY < 45 + 50)
                matY -= 1;

            if (matX < 0) matX = 0;
            if (matY < 0) matY = 0;

            lastX = rawX; lastY = rawY;

            // update x, y
            x = rawX + matX*411;
            y = rawY + matY*411;
            deg = Deg(cube.angle);
            UpdateProperty();

            Predict();
        }
    }
}