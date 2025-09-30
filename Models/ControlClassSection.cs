namespace SMS.Models;

public class ControlClassSection
{
    public int ControlClassId { get; set; }
    public ControlClass ControlClass { get; set; }

    public int SectionId { get; set; }
    public Section Section { get; set; }
}