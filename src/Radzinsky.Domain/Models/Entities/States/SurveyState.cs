namespace Radzinsky.Domain.Models.Entities.States;

public class SurveyState : State
{
    public int? MatrixCellId { get; set; }
    public int? Rating { get; set; }

    public SurveyState(string key) : base(key) { }
}