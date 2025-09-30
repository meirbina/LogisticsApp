namespace SMS.Models.ViewModels;

public class StateVM
{
    public IEnumerable<State> StateList { get; set; }
    public State NewState { get; set; }
}