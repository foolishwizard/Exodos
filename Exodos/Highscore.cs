using System.Collections.Generic;

namespace ExodosGame {
    public class Highscore {
        public List<long> scores;

        private const short max_entries = 5;

        private long newScore;

        public bool AddScore(long newScore) {
            this.newScore = newScore;
            scores.Sort();
            if (newScore <= scores[0]) return false;
            if (scores.Contains(newScore)) return false;
            scores.Add(newScore);
            scores.Sort();
            if (scores.Count > max_entries) {
                scores.RemoveAt(0);
            }
            return true;
        }

        public long GetTop() {
            if (scores.Count > 0) return scores[scores.Count - 1];
            return 0;
        }

        public long GetNew() {
            return newScore;
        }
    }
}