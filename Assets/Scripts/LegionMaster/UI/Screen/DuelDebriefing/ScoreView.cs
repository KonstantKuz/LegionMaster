using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LegionMaster.UI.Screen.DuelDebriefing
{
    public class ScoreView : MonoBehaviour
    {
        private List<ScoreItemView> _scoreViews;

        private List<ScoreItemView> ScoreViews => _scoreViews ??= GetComponentsInChildren<ScoreItemView>().ToList();

        public void Init(int score, bool isWinner)
        {
            for (int i = 0; i < ScoreViews.Count; i++)
            {
                bool isLastItem = i + 1 == score;
                ScoreViews[i].Init(i < score, isWinner && isLastItem);
            }
        }
    }
}