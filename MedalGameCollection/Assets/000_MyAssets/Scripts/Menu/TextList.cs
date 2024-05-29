using System;
using UnityEngine;

namespace Menu
{
    [Serializable]
    public class TextList
    {
        public enum Stage
        {
            None,
            DropBalls,
            JanKen,
            Pusher01,
            SmartBall01,
            Todo5,
        }

        public Stage stage;
        public string stageNameTextLine1;
        public string stageNameTextLine2;
        public string stageIntroductionTextLine1;
        public string stageIntroductionTextLine2;
        public string stageIntroductionTextLine3;

        public TextList(Stage stage, string stageNameTextLine1, string stageNameTextLine2,
            string stageIntroductionTextLine1, string stageIntroductionTextLine2, string stageIntroductionTextLine3)
        {
            this.stage = stage;
            this.stageNameTextLine1 = stageNameTextLine1;
            this.stageNameTextLine2 = stageNameTextLine2;
            this.stageIntroductionTextLine1 = stageIntroductionTextLine1;
            this.stageIntroductionTextLine2 = stageIntroductionTextLine2;
            this.stageIntroductionTextLine3 = stageIntroductionTextLine3;
        }
    }
}
