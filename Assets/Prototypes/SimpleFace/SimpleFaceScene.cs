using System.Collections.Generic;
using UnityEngine;
using toio;
using toio.MathUtils;
using static System.Math;

public class SimpleFaceScene : MonoBehaviour {
    CubeManager cm;
    List<Vector> targets = new List<Vector>();
    int N = 3; // number of neighbors
    float elapsedTime = 0.0f;
    float intervalTime = 2.0f;

    async void Start() {
        cm = new CubeManager();
        await cm.MultiConnect(4);

        // initialize targets
        targets.Add(new Vector(250, 250));
        for (int i = 0; i < N; i++) {
            targets.Add(new Vector(250, 250) + Vector.fromRadMag(2 * PI / N * i, 150));
        }
        foreach (var t in targets) {
            Debug.Log(string.Format("{0} {1}", t.x, t.y));
        }
    }

    int phase = 0;
    int count = 0;
    void Update() {
        if (phase == 0) {
            if (cm.synced) {
                bool allReached = true;
                for (int i = 0; i < 4; i++) {
                    var mv = cm.navigators[i].Navi2Target(targets[i]).Exec();
                    allReached &= mv.reached;
                }
                if (allReached) phase = 1;
            }
        } else if (phase == 1) {
            elapsedTime += Time.deltaTime;
            if (elapsedTime > intervalTime) {
                if (cm.synced) {
                    // var mv = cm.handles[0].Rotate2Target(1, 1).Exec();
                    var mv = cm.handles[0].Rotate2Target(targets[count + 1].x, targets[count + 1].y).Exec();
                    if (mv.reached) {
                        count++;
                        if (count == N) count = 0;
                        elapsedTime = 0.0f;
                    }
                }

            }
        }
    }
}
