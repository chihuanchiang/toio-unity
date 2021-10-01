using UnityEngine;
using toio;
// using toio.Navigation;
// using toio.MathUtils;
// using static toio.MathUtils.Utils;

public class SimpleMoveScene : MonoBehaviour {
    CubeManager cm;
    async void Start() {
        cm = new CubeManager();
        await cm.MultiConnect(4);

        cm.navigators[0].ClearOther();
    }

    int phase = 0;
    void Update() {
        if (cm.synced) {
            if (phase == 0) {
                var mv = cm.navigators[0].Navi2Target(150, 150).Exec();
                if (mv.reached) phase = 1;
            } else if (phase == 1) {
                var mv = cm.navigators[0].Navi2Target(350, 150).Exec();
                if (mv.reached) phase = 2;
            } else if (phase == 2) {
                var mv = cm.navigators[0].Navi2Target(350, 350).Exec();
                if (mv.reached) phase = 3;
            } else if (phase == 3) {
                var mv = cm.navigators[0].Navi2Target(150, 350).Exec();
                if (mv.reached) phase = 0;
            } else {
                Debug.Log("Invalid phase");
            }
        }
    }
}
