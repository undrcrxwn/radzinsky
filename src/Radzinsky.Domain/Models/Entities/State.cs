using System.ComponentModel.DataAnnotations;

namespace Radzinsky.Domain.Models.Entities;

public class State
{
    [Key] public string Key { get; set; }

    private State() { }

    public State(string key) => Key = key;
}