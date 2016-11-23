namespace Linkedin.Models
{
    interface IScoreCalculator
    {
        int calculateScore(ScorableData Scorabledata);
    }
}
